using System.Collections.Generic;
using System.Security.Claims;
using ToolsToLive.AuthCore.Interfaces.Model;

namespace ToolsToLive.AuthCore.Interfaces.IdentityServices
{
    public interface IIdentityProvider
    {
        List<Claim> GetClaimsForUser(IAuthCoreUser user);
    }
}
