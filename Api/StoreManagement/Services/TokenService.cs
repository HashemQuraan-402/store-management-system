using Microsoft.IdentityModel.Tokens;
using StoreManagement.Models.Entities;

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace StoreManagement.Services
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration configuration;

        public TokenService(IConfiguration configuration) 
        {
            this.configuration = configuration;
        }

        public (string Token, DateTime Expiration) GenerateToken( User user)
        {
            var jwtSection = configuration.GetSection("Jwt");
            var key = jwtSection["Key"]!;// ! means the values is not null
            var issuer = jwtSection["Issuer"];
            var audience = jwtSection["Audience"];
            var expiryMinutes = int.Parse(jwtSection["ExpiryMinutes"] ?? "120");

            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new(ClaimTypes.Name , user.UserName),
                new("FullName",user.UserFullName),
                 // Role claim drives [Authorize(Roles = "Manager")] / "Employee" on controllers.
                new(ClaimTypes.Role, user.UserType.ToString())
            };


            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var credentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.UtcNow.AddMinutes(expiryMinutes);

            var token = new JwtSecurityToken(
                    issuer:issuer,
                    audience:audience,
                    claims:claims,
                    expires:expires,
                    signingCredentials:credentials
                );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
            return (tokenString, expires);

        }
    }
    
}
