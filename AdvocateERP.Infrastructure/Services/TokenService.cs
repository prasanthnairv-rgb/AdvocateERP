using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using AdvocateERP.Core.Entities;
using AdvocateERP.Application.Interfaces.Services;
using Microsoft.Extensions.Configuration;

namespace AdvocateERP.Infrastructure.Services;

public class TokenService : ITokenService
{
    private readonly SymmetricSecurityKey _key;

    public TokenService(IConfiguration config)
    {
        // FIX: You must convert the string to a Byte Array using Encoding
        var jwtKey = config["Jwt:Key"] ?? throw new Exception("Jwt:Key not found");
        _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
    }

    public string CreateToken(ApplicationUser user)
    {
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.NameId, user.Id), // Should work now!
            new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName!),
            new Claim("tenantId", user.TenantId.ToString())
        };

        var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.Now.AddDays(7),
            SigningCredentials = creds
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }
}