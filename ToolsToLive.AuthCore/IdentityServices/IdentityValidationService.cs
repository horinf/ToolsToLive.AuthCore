using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using ToolsToLive.AuthCore.Interfaces.IdentityServices;
using ToolsToLive.AuthCore.Model;

namespace ToolsToLive.AuthCore.IdentityServices
{
    public class IdentityValidationService : IIdentityValidationService
    {
        private readonly ISigningCredentialsProvider _signingCredentialsProvider;
        private readonly IOptions<AuthOptions> _options;

        public IdentityValidationService(
            ISigningCredentialsProvider signingCredentialsProvider,
            IOptions<AuthOptions> options)
        {
            _signingCredentialsProvider = signingCredentialsProvider;
            _options = options;
        }

        public ClaimsPrincipal GetPrincipalFromToken(string token)
        {
            var authOptions = _options.Value;

            //var paths = new List<string>();

            //List<X509SecurityKey> signingKeys = paths
            //    .Where(x => !string.IsNullOrWhiteSpace(x))
            //    .Select(x => new X509SecurityKey(new System.Security.Cryptography.X509Certificates.X509Certificate2(Path.Combine("appPath", x))))
            //    .ToList();

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = true,
                ValidAudience = authOptions.Audience,
                ValidateIssuer = true,
                ValidIssuer = authOptions.Issuer,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = _signingCredentialsProvider.GetSecurityKey(),
                ValidateLifetime = true,
                RequireExpirationTime = true,
                RequireSignedTokens = true,
                //ClockSkew = TimeSpan.FromSeconds(300),
                //IssuerSigningKeys = signingKeys, // for seamless key changes
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);

            var jwtSecurityToken = securityToken as JwtSecurityToken;
            if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("Invalid token");
            }

            return principal;
        }
    }
}
