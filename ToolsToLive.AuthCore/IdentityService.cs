using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using ToolsToLive.AuthCore.Interfaces;

namespace ToolsToLive.AuthCore
{
    public class IdentityService : IIdentityService
    {
        private readonly IOptions<AuthOptions> _options;

        public IdentityService(IOptions<AuthOptions> options)
        {
            _options = options;
        }

        public ClaimsIdentity GetIdentity(IUser user)
        {
            List<Claim> claims = GetClaimsForUser(user);

            ClaimsIdentity claimsIdentity = new ClaimsIdentity(
                claims,
                "Token",
                ClaimsIdentity.DefaultNameClaimType,
                ClaimsIdentity.DefaultRoleClaimType);

            return claimsIdentity;
        }

        public ClaimsPrincipal GetPrincipalFromToken(string token)
        {
            var authOptions = _options.Value;

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = true,
                ValidAudience = authOptions.Audience,
                ValidateIssuer = true,
                ValidIssuer = authOptions.Issuer,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = GetSecurityKey(),
                ValidateLifetime = true,
                RequireExpirationTime = true,
                RequireSignedTokens = false,
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken securityToken;
            ClaimsPrincipal principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);

            var jwtSecurityToken = securityToken as JwtSecurityToken;
            if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("Invalid token");
            }

            return principal;
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

        private SymmetricSecurityKey GetSecurityKey()
        {
            return new SymmetricSecurityKey(Encoding.Unicode.GetBytes(_options.Value.Key));
        }
    }
}
