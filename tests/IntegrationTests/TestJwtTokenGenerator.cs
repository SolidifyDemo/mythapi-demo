using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace IntegrationTests;

public static class TestJwtTokenGenerator
{
    private const string JwtKey = "ThisIsASecretKeyForDevelopmentPurposesOnly-ChangeInProduction-MustBeAtLeast32Characters";
    private const string JwtIssuer = "MythApi";
    private const string JwtAudience = "MythApiUsers";

    public static string GenerateToken(string username, string role, int expirationMinutes = 60)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtKey));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.Name, username),
            new Claim(ClaimTypes.Role, role),
            new Claim(JwtRegisteredClaimNames.Sub, username),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer: JwtIssuer,
            audience: JwtAudience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expirationMinutes),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public static string GenerateAdminToken(string username = "admin") => GenerateToken(username, "Admin");
    
    public static string GenerateUserToken(string username = "user") => GenerateToken(username, "User");
}
