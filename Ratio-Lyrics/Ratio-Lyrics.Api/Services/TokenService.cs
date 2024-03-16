using Microsoft.IdentityModel.Tokens;
using Ratio_Lyrics.Api.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Ratio_Lyrics.Api.Services
{
    public class TokenService : ITokenService
    {
        private ILogger<TokenService> _logger;
        private readonly IConfiguration _configuration;        

        public TokenService(ILogger<TokenService> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        public TokenResponse CreateToken(string userName, string password)
        {
            var expiration = DateTime.UtcNow.AddMinutes(DemoConstant.JWTExpirationMinutes);
            var token = CreateJwtToken(
                CreateClaims(userName),
                CreateSigningCredentials(),
                expiration
            );
            var tokenHandler = new JwtSecurityTokenHandler();
            
            _logger.LogInformation($"JWT Token created. Expire time: {token.ValidTo}");

            var result = new TokenResponse
            {
                Token = tokenHandler.WriteToken(token),
                Expiration = token.ValidTo
            };
            return result;
        }

        private JwtSecurityToken CreateJwtToken(List<Claim> claims, SigningCredentials credentials,
            DateTime expiration) =>
            new(
                _configuration["Jwt:Issuer"],
                _configuration["Jwt:Audience"],
                claims,
                expires: expiration,
                signingCredentials: credentials
            );

        private List<Claim> CreateClaims(string userName)
        {
            var jwtSub = _configuration["Jwt:Subject"];

            try
            {
                var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, jwtSub),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString()),                
                new Claim("UserName", userName),                
            };                

                return claims;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Can't create token claim: {ex}");
                throw;
            }
        }

        private SigningCredentials CreateSigningCredentials()
        {
            var symmetricSecurityKey = _configuration["Jwt:Key"];

            return new SigningCredentials(
                new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(symmetricSecurityKey)
                ),
                SecurityAlgorithms.HmacSha256
            );
        }        
    }
}
