using Microsoft.IdentityModel.Tokens;

namespace ToolsToLive.AuthCore.Interfaces
{
    public interface ICredentialsGeterator
    {
        SigningCredentials GetSigningCredentials();
    }
}
