using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using ToolsToLive.AuthCore.Interfaces;
using ToolsToLive.AuthCore.Model;

namespace ToolsToLive.AuthCore
{
    public class TokenGenerator : ITokenGenerator
    {
        private readonly IOptions<AuthOptions> _options;
        private readonly IIdentityService _identityService;

        public TokenGenerator(
            IOptions<AuthOptions> options,
            IIdentityService identityService
            )
        {
            _options = options;
            _identityService = identityService;
        }

        public Task<IAuthToken> GenerateToken<TUser>(TUser user) where TUser: IUser
        {
            var authOptions = _options.Value;

            DateTime now = DateTime.UtcNow;
            DateTime expires = now.Add(_options.Value.TokenLifetime);

            ClaimsIdentity identity = _identityService.GetIdentity(user);

            var jwt = new JwtSecurityToken(
                issuer: authOptions.Issuer,
                audience: authOptions.Audience,
                claims: identity.Claims,
                notBefore: now,
                expires: expires,
                signingCredentials: GetSigningCredentials()
                );

            string encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            IAuthToken token = new AuthToken
            {
                Token = encodedJwt,
                IssueDate = now,
                ExpireDate = expires,
                UserName = user.UserName,
            };

            return Task.FromResult(token);
        }

        public Task<IAuthToken> GenerateRefreshToken(IUser user)
        {
            var randomNumber = new byte[32];
            string tokenString;
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                tokenString = Convert.ToBase64String(randomNumber);
            }

            var now = DateTime.UtcNow;
            var expires = now.Add(_options.Value.RefreshTokenLifeTime);

            IAuthToken refreshToken = new AuthToken
            {
                Token = tokenString,
                IssueDate = now,
                ExpireDate = expires,
                UserName = user.UserName,
            };

            return Task.FromResult(refreshToken);
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
