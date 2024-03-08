using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using UserManagementService.JwtFeatures;
using UserManagementService.Models.Account;

public class JwtService
{
    private readonly JWTSettings? jwtSettings;
    private readonly UserManager<ApplicationUser> _userManager;
    public JwtService(IConfiguration configuration, UserManager<ApplicationUser> userManager)
    {
        this.jwtSettings = configuration.GetSection(nameof(JWTSettings)).Get<JWTSettings>();
        _userManager = userManager;
    }

    public async Task<string> GenerateTokenAsync(ApplicationUser user )
    {
        var tokenHandler = new JwtSecurityTokenHandler();

        var key = Encoding.UTF8.GetBytes(jwtSettings!.SecretKey!);
        var secret = new SymmetricSecurityKey(key);
        var credentials =  new SigningCredentials(secret, SecurityAlgorithms.HmacSha256);
        var Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId!.ToString()),
                new Claim(ClaimTypes.Email, user.Email!),
                new Claim(ClaimTypes.Name, user.FirstName!),
                new Claim(ClaimTypes.Surname, user.LastName!),
            });
        var roles = await _userManager.GetRolesAsync(user);
        if (roles != null )
        {
            var claimedRoles = string.Join(",", roles);
            Subject.AddClaim(new Claim(
                ClaimTypes.Role,
                claimedRoles
             ));
        };
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Issuer = jwtSettings.Issuer!,// _issuer,
            Audience = jwtSettings.Audience, // _audience,
            Subject = Subject,
            Expires = DateTime.UtcNow.AddMinutes(jwtSettings.ExpiryInMinutes), // Token expiration time
            SigningCredentials = credentials
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}
