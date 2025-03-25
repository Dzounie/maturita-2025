using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Eshop.Api.Entities;

namespace Eshop.Api.Utils
{
    public static class JwtTokenGenerator //převzáno z chatgpt + pár mých úprav
    {
        public static string GenerateToken(string secretKey, string issuer, string audience, User user)
        {
            // Vytvoření tokenu
            var tokenHandler = new JwtSecurityTokenHandler(); // Třída pro vytvoření tokenu
            var key = Encoding.UTF8.GetBytes(secretKey!);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim("UserId", user.Id.ToString()) // Custom claim
            }),
                Expires = DateTime.UtcNow.AddHours(1), // Platnost tokenu
                Issuer = issuer,
                Audience = audience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
        
    }
}