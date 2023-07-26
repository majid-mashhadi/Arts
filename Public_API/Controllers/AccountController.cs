using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Public_API.Models.Account;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860
namespace Public_API.Controllers
{

    [AllowAnonymous]
    [ApiController]
    [ApiVersion("1.0")] // Specify the version for this controller
    [Route("api/v{version:apiVersion}/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly ApplicationJwtHandler _jwtHandler;

        public AccountController(UserManager<ApplicationUser> userManager,
            //ApplicationDbContext dbContext,
            SignInManager<ApplicationUser> signInManager,
            IConfiguration configuration,
            ApplicationJwtHandler jwtHandler)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _jwtHandler = jwtHandler;
        }


        [HttpPost("signin")]
        public async Task<IActionResult> SignInAsync([FromBody] SigninRequest request)
        {

            // Find the user by their email or username
            var user = await _userManager.FindByEmailAsync(request.Email!) 
                ?? await _userManager.FindByNameAsync(request.Email!);

            if (user == null)
            {
                return NotFound("Invalid Email and,or Password");
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password!, lockoutOnFailure: false);
            if (result.Succeeded)
            {
                var token = GenerateJwtToken(user);
                return Ok(new { token } );
            }
            else
            {
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
                    Email = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName
                    // Add other properties as needed
                };


                // Hash the password using PasswordHasher
                var passwordHasher = new PasswordHasher<ApplicationUser>();
                user.PasswordHash = passwordHasher.HashPassword(user, model.Password);

                var result = await _userManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    // User registration successful
                    return Ok("User registered successfully.");
                }
                else
                {
                    return BadRequest(result.Errors);
                }
            }

            return BadRequest(ModelState);
        }

        [HttpPost("SignInWithGoogle")]
        public IActionResult SignInWithGoogle(string provider, string returnUrl)
        {
            var props = new AuthenticationProperties
            {
                RedirectUri = returnUrl,
                Items =
                    {
                        { "scheme", provider }
                    }
            };

            return Challenge(props, "Google");
        }

        //[HttpPost]
        //public IActionResult SignInWithGoogleCallback(string returnUrl)
        //{
        //    // Handle the user authentication here
        //    // ...

        //    return Redirect(returnUrl); // Redirect to the original URL after authentication
        //}

        [HttpPost("RefreshToken")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenModel model)
        {
            bool isRefreshTokenValid = ValidateRefreshToken(model.RefreshToken!, out var userId);

            if (!isRefreshTokenValid)
            {
                return BadRequest();
            }

            var user = await _userManager.FindByIdAsync(userId);
            if ( user == null)
            {
                return BadRequest();
            }
            // Generate a new access token
            var token = GenerateJwtToken(user!);

            // Return the new access token
            return Ok(new { token });
        }


        private bool ValidateRefreshToken(string refreshToken, out string userId)
        {
            userId = _jwtHandler.ValidateRefreshToken(refreshToken);
            return !string.IsNullOrEmpty(userId);
        }

        private string GenerateJwtToken(ApplicationUser user)
        {
            var subject = _jwtHandler.GetClaimsSubject(user);
            return _jwtHandler.GenerateJwtToken(subject);
        }

        private string GenerateJwtToken3(ApplicationUser user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWTSettings:SecretKey"]!));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.Email, user.Email!),
                new Claim(ClaimTypes.Name, user.FirstName!),
                new Claim(ClaimTypes.GivenName, user.LastName!),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["JWTSettings:Issuer"],
                audience: _configuration["JWTSettings:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2), // Convert.ToDouble(_configuration["JWTSettings:ExpiryInMinutes"])),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}

