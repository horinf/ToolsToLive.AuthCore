using Microsoft.IdentityModel.Tokens;

namespace ToolsToLive.AuthCore.Interfaces.IdentityServices
{
    public interface ISigningCredentialsProvider
    {
        SecurityKey GetSecurityKey();

        SigningCredentials GetSigningCredentials();
    }
}
