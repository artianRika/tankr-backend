using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using TankR.Data.Models.Identity;
using TankR.Repos.Interfaces;

namespace TankR.Auth;

public class TokenService
{
    private string? _issuer = Environment.GetEnvironmentVariable("JWT_ISSUER");
    private string? _audience = Environment.GetEnvironmentVariable("JWT_AUDIENCE");
    
    public readonly UserManager<ApplicationUser> _userManager;
    public const int ExpirationMinues = 30;

    public TokenService(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<string> CreateToken(ApplicationUser user)
    {
        var expiration = DateTime.UtcNow.AddMinutes(ExpirationMinues);
        var roles =  await _userManager.GetRolesAsync(user);
        
        var token = CreateJwtToken(
            CreateClaims(user, roles),
            CreateSigningCredentials(),
            expiration
        );
        var tokenHandler = new JwtSecurityTokenHandler();
        return tokenHandler.WriteToken(token);
    }

    private JwtSecurityToken CreateJwtToken(List<Claim> claims, SigningCredentials credentials, DateTime expiration) =>
        new JwtSecurityToken(
            issuer: _issuer,
            audience: _audience,
            expires: expiration,
            claims: claims,
            signingCredentials: credentials
        );

    private List<Claim> CreateClaims(ApplicationUser user, IList<string> roles)
    {
        try
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName!),
            };

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }
            
            return claims;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    private SigningCredentials CreateSigningCredentials()
    {
        return new SigningCredentials(
            new SymmetricSecurityKey(
                System.Text.Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("JWT_KEY"))),
            SecurityAlgorithms.HmacSha256);
    }
}