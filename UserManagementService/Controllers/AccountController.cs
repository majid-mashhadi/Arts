using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using UserManagementService.Models.Account;
using MassTransit;
using MongoDB.Driver;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authorization;
using UserManagementService.Configuration;
using System.Net;
using UserManagementService.Repositories;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860
namespace UserManagement.Controllers
{

    [ApiController]
    [ApiVersion("1.0")] // Specify the version for this controller
    [Route("api/v{version:apiVersion}/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly ApplicationJwtHandler _jwtHandler;
        private readonly JwtService jwtService;
        //private readonly IPublishEndpoint publishEndpoint;
        private readonly IBus bus;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly IEmailRepository emailService;

        private const string AmazonSellerRole = "AmazonSeller";
        public AccountController(UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IConfiguration configuration,
            ApplicationJwtHandler jwtHandler,
            JwtService jwtService,
            IBus bus,
            RoleManager<IdentityRole> roleManager,
            IEmailRepository emailService
            //IPublishEndpoint publishEndpoint
            )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _jwtHandler = jwtHandler;
            this.jwtService = jwtService;
            //this.publishEndpoint = publishEndpoint;
            this.bus = bus;
            this.roleManager = roleManager;
            this.emailService = emailService;
        }

        [HttpPost("signin")]
        public async Task<IActionResult> SignInAsync([FromBody] SigninRequest request)
        {
            request.ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            // Find the user by their email or username
            var user = await _userManager.FindByEmailAsync(request.Email!)
                ?? await _userManager.FindByNameAsync(request.Email!);

            if (user == null)
            {
                return NotFound("Invalid Email and,or Password");
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password!, true);
            if (result.Succeeded)
            {
                if (!user.EmailConfirmed)
                {
                    return BadRequest("Email has not been confirmed yet.");
                }
                var token = await GenerateJwtTokenAsync(user);
                return Ok(new { token });
            }
            else
            {
                if (result.IsLockedOut)
                {
                    await emailService.SendLockoutNotificationAsync(user);
                    return NoContent();
                }
                return NotFound("Invalid Email and,or Password");
            }

        }

        [HttpPost("signup")]
        public async Task<IActionResult> SignUp(SignUpRequest model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    Email = model.Email!,
                    UserName = model.Email!,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Created = DateTime.UtcNow,
                };

                var result = await _userManager.CreateAsync(user, model.Password!);
                if (result.Succeeded)
                {
                    try
                    {
                        await _userManager.AddToRoleAsync(user, AmazonSellerRole);
                        await SendEmailConfirmationAsync(user);
                        return Ok("User registered successfully. Please check your Email to verify your account.");
                    }
                    catch (Exception)
                    {
                    }
                    return BadRequest();
                }
                return BadRequest(result.Errors);
            }
            return BadRequest(ModelState);
        }

        private async Task<bool> SendEmailConfirmationAsync(ApplicationUser user)
        {
            await _userManager.RemoveAuthenticationTokenAsync(user, "M2Store", "EmailConfirmatioToken");
            var new_token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            new_token = WebUtility.UrlEncode(new_token);
            var result = await _userManager.SetAuthenticationTokenAsync(user, "M2Store", "EmailConfirmatioToken", new_token);
            if (!result.Succeeded)
            {
                return false;
            }
            await emailService.SendEmailConfirmationEmailAsync(user, new_token!);
            return true;
        }



        //private async Task<bool> SendEmailConfirmationAsync(ApplicationUser user)
        //{
        //    await _userManager.RemoveAuthenticationTokenAsync(user, "M2Store", "EmailConfirmatioToken");
        //    var new_token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        //    new_token = WebUtility.UrlEncode(new_token);
        //    var result = await _userManager.SetAuthenticationTokenAsync(user, "M2Store", "EmailConfirmatioToken", new_token);
        //    if (!result.Succeeded)
        //    {
        //        return false;
        //    }
        //    await SendEmailConfirmationEmail(user, new_token!);
        //    return true;
        //}

        //private async Task SendEmailConfirmationEmail(ApplicationUser user, string Token)
        //{
        //    var message = new EmailConfirmationDto(
        //        (Enum.GetName(typeof(DtoTypes), DtoTypes.EmailConfirmation)!).ToLower(),
        //        new EmailConfirmationDtoData(user.FirstName!, user.LastName!, user.Email!, Token)
        //    );
        //    var endpoint = await bus.GetSendEndpoint(new Uri("queue:account-service"));
        //    using (var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(10)))
        //    {
        //        await endpoint.Send(message, cancellationTokenSource.Token);
        //    }
        //}


        [HttpPost]
        [Route("EmailConfirmation")]
        public async Task<IActionResult> EmailConfirmationAsync([FromBody] EmailConfirmationRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user != null)
            {
                var request_token = WebUtility.UrlEncode(request.Token);
                var token = await _userManager.GetAuthenticationTokenAsync(user, "M2Store", "EmailConfirmatioToken");
                if (token == null || token != request_token)
                {
                    return BadRequest("Something went wrong, please try again later.");
                }

                var result = await _userManager.ConfirmEmailAsync(user, request.Token);
                if (result.Succeeded)
                {
                    await _userManager.RemoveAuthenticationTokenAsync(user, "M2Store", "EmailConfirmatioToken");
                    user.EmailConfirmed = true;
                    await _userManager.UpdateAsync(user);
                    return Ok();
                }
            }
            return BadRequest("Something went wrong, please try again later.");
        }


        [HttpPost("RefreshToken")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenModel model)
        {
            bool isRefreshTokenValid = ValidateRefreshToken(model.Token!, out var userId);

            if (!isRefreshTokenValid)
            {
                return BadRequest();
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return BadRequest();
            }
            var token = await GenerateJwtTokenAsync(user!);
            return Ok(new { token });
        }

        private bool ValidateRefreshToken(string refreshToken, out string userId)
        {
            userId = _jwtHandler.ValidateRefreshToken(refreshToken);
            return !string.IsNullOrEmpty(userId);
        }
        private async Task<string> GenerateJwtTokenAsync(ApplicationUser user)
        {
            //var jwtService = new JwtService("M2Store", "https://localhost:3000", "1234567890123456");
            var token = await jwtService.GenerateTokenAsync(user);
            return token;
        }

        [HttpPost("forgot-password")]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user != null)
            {
                if (!user.EmailConfirmed)
                {
                    await SendEmailConfirmationAsync(user);
                    return BadRequest("Email has not been confirmed yet.");
                }

                if (await _userManager.GetAuthenticationTokenAsync(user, "M2Store", "ResetPasswordToken") == null)
                {
                    var new_token = await _userManager.GeneratePasswordResetTokenAsync(user);
                    var result = await _userManager.SetAuthenticationTokenAsync(user, "M2Store", "ResetPasswordToken", new_token);
                    if (!result.Succeeded)
                    {
                        return NoContent();
                    }
                }

                string? token = await _userManager.GetAuthenticationTokenAsync(user, "M2Store", "ResetPasswordToken");
                await emailService.SendForgotPasswordEmailAsync(user, token!);
                return Ok(new { token });
            }
            return NoContent();
        }

        [HttpPost]
        [Route("resetpassword")]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordModel request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user != null)
            {
                var result = await _userManager.ResetPasswordAsync(user, request.Token, request.Password);
                if (result.Succeeded)
                {
                    await _userManager.RemoveAuthenticationTokenAsync(user, "M2Store", "ResetPasswordToken");
                    return Ok();
                }
                return BadRequest(result.Errors.Select(x => x.Description).ToList());
            }
            return BadRequest();
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("LoginProviders")]
        public async Task<IActionResult> GetLoginProviders()
        {
            var externalSchemes = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            var LoginProviders = externalSchemes.Select(s =>
                new ExternalLogin
                {
                    DisplayName = s.DisplayName,
                    Name = s.Name,
                }
            ).ToList();
            return Ok(LoginProviders);
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("ExternalLogin")]
        public async Task<IActionResult> ExternalLogin(string Provider, string returnUrl) // [FromBody] ExternalLoginRequest request)
        {
            string state = Guid.NewGuid().ToString();
            HttpContext.Session.SetString("AuthState", state);
            var redirectUrl = Url.ActionWithPort("ExternalLoginCallback", "Account", new { version = "1" });
            var full_state = $"{state}:{Provider}:{Convert.ToBase64String(Encoding.UTF8.GetBytes(returnUrl))}";
            return Ok(new
            {
                ClientId = _configuration[$"Authentication:{Provider}:ClientId"],
                RedirectUri = redirectUrl,
                State = full_state,
                Provider
            });
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("ExternalLoginCallback")]
        public async Task<IActionResult> ExternalLoginCallback(string state, string code, string scope, string authuser, string prompt, string remoteError = null)
        {
            string[] parts = state.Split(":");
            var received_state = parts[0];
            var Provider = parts[1];
            var base64EncodedBytes = Convert.FromBase64String(parts[2]);
            var return_url = Encoding.UTF8.GetString(base64EncodedBytes);
            try
            {
                if (Provider == "Google")
                {
                    return await GoogleCallback(received_state, return_url, code, scope, authuser, prompt, remoteError);
                }
                else
                {
                    return BadRequest($"Invalid Provider: {Provider}");
                }
            }
            catch (Exception ex)
            {
                return BadRequest("Something went wrong, please try again later.");
            }
        }


        private async Task<IActionResult> GoogleCallback(string state, string return_url, string code, string scope, string authuser, string prompt, string remoteError)
        {
            //if ( state != storedState)
            //{
            //    var s = "invalid";
            //    //return BadRequest("Invalid State");
            //}

            if (remoteError != null)
            {

            }

            // Exchange the authorization code for an access token
            var tokenRequestContent = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("code", code),
                new KeyValuePair<string, string>("client_id", _configuration["Authentication:Google:ClientId"]!),
                new KeyValuePair<string, string>("client_secret", _configuration["Authentication:Google:ClientSecret"]!),
                new KeyValuePair<string, string>("redirect_uri", Url.ActionWithPort("ExternalLoginCallback", "Account", new { version = "1" })),
                new KeyValuePair<string, string>("grant_type", "authorization_code")
            });
            var httpClient = new HttpClient();
            var tokenResponse = await httpClient.PostAsync("https://oauth2.googleapis.com/token", tokenRequestContent);
            var tokenResponseContent = await tokenResponse.Content.ReadAsStringAsync();
            var tokenResponseData = JsonConvert.DeserializeObject<dynamic>(tokenResponseContent);
            var accessToken = tokenResponseData!.access_token!;
           
            // Use the access token to retrieve user information from Google
            var userInfoResponse = await httpClient.GetAsync($"https://www.googleapis.com/oauth2/v2/userinfo?access_token={accessToken}");
            var userInfoResponseContent = await userInfoResponse.Content.ReadAsStringAsync();
            var info = JsonConvert.DeserializeObject<dynamic>(userInfoResponseContent);


            var email = info!.email?.ToString(); //  "Majid1347@gmail.com"; //  info.Principal.FindFirstValue(ClaimTypes.Email);

            if (email == null)
            {
                return BadRequest();
            }
            ApplicationUser user = await _userManager.FindByEmailAsync(email);

            if (user == null)// User does not exist, create a new user
            {
                user = new ApplicationUser
                {
                    UserName = email,
                    Email = email,
                    FirstName = info.given_name?.ToString() ?? email,
                    LastName = info.family_name?.ToString() ?? email,
                    Created = DateTime.UtcNow,
                    EmailConfirmed = info.verified_email ?? false,
                };
                var result = await _userManager.CreateAsync(user);
                if (!result.Succeeded)
                {
                    return BadRequest("Failed to create user");
                }
                
                await _userManager.AddToRoleAsync(user, AmazonSellerRole);
            }
            if (!user.EmailConfirmed)
            {
                await SendEmailConfirmationAsync(user);
                return Ok("User registered successfully. Please check your Email to verify your account.");
            }
            else
            {
                var claims = new List<Claim>() {
                    new Claim(ClaimTypes.Email, email)
                };
                await _signInManager.SignInWithClaimsAsync(user, isPersistent: false, claims);
                var token = await GenerateJwtTokenAsync(user!);
                var querySign = return_url.Contains('?') ? '&' : '?';
                return Redirect($"{return_url}{querySign}jwt_token={token}");
            }
        }
    }
}