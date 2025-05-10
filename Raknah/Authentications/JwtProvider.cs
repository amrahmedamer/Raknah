using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Raknah.Authentications;

public class JwtProvider : IJwtProvider
{
    public (string token, int expiresIn) GenerateJwtTokenAsync(ApplicationUser user)
    {
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id),
            new Claim(JwtRegisteredClaimNames.Email, user.Email!),
            new Claim(JwtRegisteredClaimNames.GivenName, user.FullName),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())

        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("R2gMz5peB4b6C4du9q8ZvHh6UL1Z8q3K"));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expireIn = 15;
        var token = new JwtSecurityToken(
            issuer: "Raknah",
            audience: "Users",
            claims: claims,
            expires: DateTime.Now.AddMinutes(expireIn),
            signingCredentials: creds
        );

        return (new JwtSecurityTokenHandler().WriteToken(token), expireIn * 60);
    }

    public string? ValidateToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var symmetricKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("R2gMz5peB4b6C4du9q8ZvHh6UL1Z8q3K"));

        try
        {
            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = symmetricKey,
                ValidateIssuer = false,
                ValidateAudience = false,
                ClockSkew = TimeSpan.Zero
            }, out SecurityToken validatedToken);

            var jwtToken = (JwtSecurityToken)validatedToken;
            return jwtToken.Claims.First(x => x.Type == JwtRegisteredClaimNames.Sub).Value;
        }
        catch
        {
            return null;
        }
    }
}
