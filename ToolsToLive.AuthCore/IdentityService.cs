using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using ToolsToLive.AuthCore.Interfaces;
using ToolsToLive.AuthCore.Interfaces.Model;
using ToolsToLive.AuthCore.Model;

namespace ToolsToLive.AuthCore
{
    public class IdentityService : IIdentityService
    {
        private readonly IOptions<AuthOptions> _options;

        public IdentityService(IOptions<AuthOptions> options)
        {
            _options = options;
        }

        public Task<IAuthToken> GenerateToken(IUser user)
        {
            if(_options.Value.TokenLifetime == TimeSpan.Zero)
            {
                throw new Exception("Tonet lifetime is not set. Please set at least a few seconds if you use server to server aut, or at least a few minutes if you use client authentication");
            }

            DateTime now = DateTime.UtcNow;
            DateTime expires = now.Add(_options.Value.TokenLifetime);

            ClaimsIdentity identity = GetIdentity(user);

            var jwt = new JwtSecurityToken(
                issuer: _options.Value.Issuer,
                audience: _options.Value.Audience,
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

        private ClaimsIdentity GetIdentity(IUser user)
        {
            List<Claim> claims = GetClaimsForUser(user);

            ClaimsIdentity claimsIdentity = new ClaimsIdentity(
                claims,
                "Token",
                ClaimsIdentity.DefaultNameClaimType,
                ClaimsIdentity.DefaultRoleClaimType);

            return claimsIdentity;
        }

        private static List<Claim> GetClaimsForUser(IUser user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, user.UserName),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
            };

            foreach (var item in user.Roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, item.Id));
            }

            if (user.Claims != null && user.Claims.Any())
            {
                claims.AddRange(user.Claims);
            }

            return claims;
        }

        private SigningCredentials GetSigningCredentials()
        {
            var simmetricKey = new SymmetricSecurityKey(Encoding.Unicode.GetBytes(_options.Value.Key));

            return new SigningCredentials(
                simmetricKey,
                SecurityAlgorithms.HmacSha256);
        }

        //// not in use. Can be used to auth users.

        //public ClaimsPrincipal GetPrincipalFromToken(string token)
        //{
        //    var authOptions = _options.Value;

        //    var tokenValidationParameters = new TokenValidationParameters
        //    {
        //        ValidateAudience = true,
        //        ValidAudience = authOptions.Audience,
        //        ValidateIssuer = true,
        //        ValidIssuer = authOptions.Issuer,
        //        ValidateIssuerSigningKey = true,
        //        IssuerSigningKey = GetSecurityKey(),
        //        ValidateLifetime = true,
        //        RequireExpirationTime = true,
        //        RequireSignedTokens = false,
        //    };

        //    var tokenHandler = new JwtSecurityTokenHandler();
        //    SecurityToken securityToken;
        //    ClaimsPrincipal principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);

        //    var jwtSecurityToken = securityToken as JwtSecurityToken;
        //    if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
        //    {
        //        throw new SecurityTokenException("Invalid token");
        //    }

        //    return principal;
        //}
    }
}
