using Flownix.Backend.Application.Interfaces;
using Flownix.Backend.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Flownix.Backend.Infrastructure.Integration.Authentication
{
    public class JwtService : IJwtService
    {
        private readonly IConfiguration _configuration;
        private readonly TokenValidationParameters _tokenValidationParameters;

        private static byte[] GetJwtKeyBytes(IConfiguration configuration)
        {
            var key = configuration["Jwt:Key"];
            if (string.IsNullOrEmpty(key))
                throw new InvalidOperationException("JWT key configuration value 'Jwt:Key' is missing or empty.");

            return Encoding.UTF8.GetBytes(key);
        }

        public JwtService(IConfiguration configuration)
        {
            _configuration = configuration;

            _tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateIssuerSigningKey = true,
                ValidateLifetime = true,

                IssuerSigningKey = new SymmetricSecurityKey(GetJwtKeyBytes(_configuration)),
                ValidIssuer = _configuration["Jwt:Issuer"],
                ValidAudience = _configuration["Jwt:Audience"]
            };
        }

        public string GenerateAccessToken(User user)
        {
            var securityKey = new SymmetricSecurityKey(GetJwtKeyBytes(_configuration));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
                new Claim(ClaimTypes.Role, user.Role.ToString())
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(
                    Convert.ToDouble(_configuration["Jwt:ExpiryInMinutes"])
                ),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string GenerateRefreshToken()
        {
            return Guid.NewGuid().ToString();
        }

        public ClaimsPrincipal? GetPrincipalFromExpiredToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            try
            {
                var principal = tokenHandler.ValidateToken(
                    token,
                    _tokenValidationParameters,
                    out SecurityToken securityToken
                );

                if (securityToken is not JwtSecurityToken jwtSecurityToken ||
                    !jwtSecurityToken.Header.Alg.Equals(
                        SecurityAlgorithms.HmacSha256,
                        StringComparison.InvariantCultureIgnoreCase))
                {
                    return null;
                }

                return principal;
            }
            catch
            {
                return null;
            }
        }

        public bool ValidateToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            try
            {
                tokenHandler.ValidateToken(token, _tokenValidationParameters, out _);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public string GetUserIdFromToken(string token)
        {
            var principal = GetPrincipalFromExpiredToken(token);
            return principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
        }
    }
}
