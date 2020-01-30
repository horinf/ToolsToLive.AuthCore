using System.Security.Claims;

namespace ToolsToLive.AuthCore.Interfaces
{
    public interface ITokenHandler
    {
        ClaimsPrincipal GetPrincipalFromToken(string token, bool validateLifeTime = true);
    }
}
