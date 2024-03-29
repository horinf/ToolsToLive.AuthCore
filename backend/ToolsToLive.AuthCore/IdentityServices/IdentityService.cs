using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.Extensions.Options;
using ToolsToLive.AuthCore.Interfaces.IdentityServices;
using ToolsToLive.AuthCore.Interfaces.Model;
using ToolsToLive.AuthCore.Model;

namespace ToolsToLive.AuthCore.IdentityServices
{
    public class IdentityService : IIdentityService
    {
        private readonly ISigningCredentialsProvider _signingCredentialsProvider;
        private readonly IIdentityProvider _identityProvider;
        private readonly ICodeGenerator _tokenGenerator;
        private readonly IOptions<AuthOptions> _authOptions;

        public IdentityService(
            ISigningCredentialsProvider signingCredentialsProvider,
            IIdentityProvider identityProvider,
            ICodeGenerator tokenGenerator,
            IOptions<AuthOptions> authOptions)
        {
            _signingCredentialsProvider = signingCredentialsProvider;
            _identityProvider = identityProvider;
            _tokenGenerator = tokenGenerator;
            _authOptions = authOptions;
        }

        public AuthToken GenerateAuthToken(IAuthCoreUser user)
        {
            var claims = _identityProvider.GetClaimsForUser(user);
            claims.Add(new Claim(AuthCoreConstants.TokenTransportClaim, TokenTransport.Header.ToString()));
            (string encodedJwt, DateTime expires) = GenerateJwtToken(claims, _authOptions.Value.TokenLifetime);

            var token = new AuthToken
            {
                Token = encodedJwt,
                ExpireDate = expires,
            };

            return token;
        }

        public AuthToken GenerateAuthTokenForCookie(IAuthCoreUser user)
        {
            var claims = _identityProvider.GetClaimsForUser(user);
            claims.Add(new Claim(AuthCoreConstants.TokenTransportClaim, TokenTransport.Cookie.ToString())); // will be checked when parsing
            (string encodedJwt, DateTime expires) = GenerateJwtToken(claims, _authOptions.Value.TokenLifetime);

            var token = new AuthToken
            {
                Token = encodedJwt,
                ExpireDate = expires,
            };

            return token;
        }

        public RefreshToken GenerateRefreshToken(IAuthCoreUser user)
        {
            var tokenString = _tokenGenerator.GenerateCode();

            var now = DateTime.UtcNow;
            var expires = now.Add(_authOptions.Value.RefreshTokenLifeTime);

            var refreshToken = new RefreshToken
            {
                Token = tokenString,
                IssueDate = now,
                ExpireDate = expires,
                UserId = user.Id,
            };

            return refreshToken;
        }

        private (string, DateTime) GenerateJwtToken(IEnumerable<Claim> claims, TimeSpan tokenLifeTime)
        {
            if (tokenLifeTime == TimeSpan.Zero)
            {
                throw new ArgumentOutOfRangeException(nameof(tokenLifeTime), "Token lifetime is not set. Please set at least a few seconds if you use ServerToServer auth, or at least a few minutes if you use client authentication");
            }

            var now = DateTime.UtcNow;
            var expires = now.Add(tokenLifeTime);

            var jwt = new JwtSecurityToken(
                issuer: _authOptions.Value.Issuer,
                audience: _authOptions.Value.Audience,
                claims: claims,
                notBefore: now,
                expires: expires,
                signingCredentials: _signingCredentialsProvider.GetSigningCredentials()
                );

            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);
            return (encodedJwt, expires);
        }
    }
}
