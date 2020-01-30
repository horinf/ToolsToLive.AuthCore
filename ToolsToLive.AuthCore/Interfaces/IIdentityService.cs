using System.Security.Claims;

namespace ToolsToLive.AuthCore.Interfaces
{
    public interface IIdentityService
    {
        ClaimsIdentity GetIdentity(IUser user);

        ClaimsPrincipal GetPrincipalFromToken(string token);
    }
}
