using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using ToolsToLive.AuthCore.Interfaces;

namespace ToolsToLive.AuthCore
{
    public class TokenGenerator : ITokenGenerator
    {
        private readonly IOptions<AuthOptions> _options;

        public TokenGenerator(
            IOptions<AuthOptions> options)
        {
            _options = options;
        }

        public string GenerateToken(ClaimsIdentity identity, DateTime notBefore, DateTime expires)
        {
            var authOptions = _options.Value;

            var jwt = new JwtSecurityToken(
                issuer: authOptions.Issuer,
                audience: authOptions.Audience,
                claims: identity.Claims,
                notBefore: notBefore,
                expires: expires,
                signingCredentials: GetSigningCredentials()
                );

            string encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            return encodedJwt;
        }

        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }

        private SigningCredentials GetSigningCredentials()
        {
            var sKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_options.Value.Key));

            return new SigningCredentials(
                sKey,
                SecurityAlgorithms.HmacSha256);
        }
    }
}
