using System.Security.Claims;

namespace ToolsToLive.AuthCore.Interfaces.IdentityServices
{
    public interface IIdentityValidationService
    {
        /// <summary>
        /// Gets Principal from token
        /// </summary>
        ClaimsPrincipal GetPrincipalFromToken(string token);
    }
}
