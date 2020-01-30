using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
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

        public IAuthToken GenerateToken<TUser>(TUser user) where TUser: IUser
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

            return new AuthToken
            {
                Token = encodedJwt,
                IssueDate = now,
                ExpireDate = expires,
                UserName = user.UserName,
            };
        }

        public IAuthToken GenerateRefreshToken()
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
