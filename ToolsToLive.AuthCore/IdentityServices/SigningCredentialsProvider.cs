using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using ToolsToLive.AuthCore.Interfaces.IdentityServices;
using ToolsToLive.AuthCore.Model;

namespace ToolsToLive.AuthCore.IdentityServices
{
    public class SigningCredentialsProvider : ISigningCredentialsProvider
    {
        private readonly IOptions<AuthOptions> _options;

        public SigningCredentialsProvider(IOptions<AuthOptions> options)
        {
            _options = options;
        }

        public SigningCredentials GetSigningCredentials()
        {
            var securityKey = GetSecurityKey(_options.Value.Key);

            return new SigningCredentials(
                securityKey,
                SecurityAlgorithms.HmacSha256);
        }

        SecurityKey ISigningCredentialsProvider.GetSecurityKey()
        {
            return GetSecurityKey(_options.Value.Key);
        }

        public static SecurityKey GetSecurityKey(string key)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.Unicode.GetBytes(key));
            return securityKey;
        }
    }
}
