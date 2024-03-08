using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using UserManagementService.Models.Account;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

public class ApplicationJwtHandler
{
    private readonly IConfiguration _configuration;
    private readonly IConfigurationSection _jwtSettings;
    private readonly string Issuer;
    private readonly string Audience;
    private readonly string ExpireyInMinutes;
    private readonly byte[] SecretKey;

    public ApplicationJwtHandler(IConfiguration configuration)
    {
        _configuration = configuration;
        _jwtSettings = _configuration.GetSection("JWTSettings");
        Issuer = _jwtSettings["Issuer"]!;
        Audience = _jwtSettings["Audience"]!;
        ExpireyInMinutes = _jwtSettings["expiryInMinutes"]!;
        SecretKey = GetKey();
    }

    private byte[] GetKey()
    {
        var key = Encoding.UTF8.GetBytes(_jwtSettings["SecretKey"]!);
        return key;
    }

    public SigningCredentials GetSigningCredentials()
    {
        var secret = new SymmetricSecurityKey(SecretKey);
        // SecurityAlgorithms.HmacSha256Signature
        return new SigningCredentials(secret, SecurityAlgorithms.HmacSha256);
    }

    public ClaimsIdentity GetClaimsSubject(ApplicationUser user)
    {
        var Subject = new ClaimsIdentity(new[]
             {
                new Claim(ClaimTypes.Email, user.Email!),
                new Claim(ClaimTypes.Name, user.FirstName!),
                new Claim(ClaimTypes.Surname, user.LastName!),
                new Claim(ClaimTypes.NameIdentifier, user.UserId!.ToString()),
            }
        );
        return Subject;
    }

    public string GenerateJwtToken(ClaimsIdentity Subject)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var signingCredentials = GetSigningCredentials();

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Issuer = Issuer,
            Audience = Audience,
            Subject = Subject,
            NotBefore = DateTime.UtcNow,
            Expires = DateTime.UtcNow.AddMinutes(Convert.ToInt32(ExpireyInMinutes)),
            SigningCredentials = signingCredentials,
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

        return tokenString;
    }


    public string ValidateRefreshToken(string refreshToken)
    {
        try
        {
            var _secretKey = GetKey();
            var tokenHandler = new JwtSecurityTokenHandler();

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = Issuer,
                ValidAudience = Audience,
                IssuerSigningKey = new SymmetricSecurityKey(_secretKey),
                ValidateLifetime = false, // Disable lifetime validation for the refresh token
            };

            // Validate the refresh token
            SecurityToken validatedToken;
            var principal = tokenHandler.ValidateToken(refreshToken, validationParameters, out validatedToken);

            // Extract user ID from the refresh token if it's stored in the claim
            var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier);
            return userIdClaim?.Value ?? "";
        }
        catch (Exception)
        {
            // Token validation failed (e.g., invalid signature, expired token, etc.)
        }

        // Return empty if the refresh token is not valid or expired
        return "";


    }
}
