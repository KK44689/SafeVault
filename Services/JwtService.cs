using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

public class JwtService : IJwtService
{
    private string _secretKey = ""; 
    private string _issuer = ""; 
    private string _audience = ""; 

    private const int TokenExpiryMinutes = 30;

    public JwtService(IConfiguration configuration)
    {
        _secretKey = configuration["JwtSetting:Key"] ?? throw new ArgumentNullException("JwtSetting:Key", "JWT secret key is not configured.");
        _issuer = configuration["JwtSetting:Issuer"] ?? throw new ArgumentNullException("JwtSetting:Issuer", "JWT issuer is not configured.");
        _audience = configuration["JwtSetting:Audience"] ?? throw new ArgumentNullException("JwtSetting:Audience", "JWT audience is not configured.");
    }

    public string GenerateToken(string username, string role)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var tokenDescriptor = new JwtSecurityToken(
            issuer: _issuer,
            audience: _audience,
            claims: new[] { new Claim(ClaimTypes.Name, username), new Claim(ClaimTypes.Role, role) },
            expires: DateTime.UtcNow.AddMinutes(TokenExpiryMinutes),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
    }
}