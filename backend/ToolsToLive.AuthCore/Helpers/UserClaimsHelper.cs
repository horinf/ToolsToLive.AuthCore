using System.Linq;
using System.Security.Claims;

namespace ToolsToLive.AuthCore.Helpers
{
    public static class UserClaimsHelper
    {
        public static string GetUserId(this ClaimsPrincipal user)
        {
            return GetClaimByClaimType(user, AuthCoreConstants.UserIdClaim);
        }

        public static string GetUserName(this ClaimsPrincipal user)
        {
            return GetClaimByClaimType(user, AuthCoreConstants.UserNameClaim);
        }

        public static string GetClaimByClaimType(this ClaimsPrincipal user, string claimType)
        {
            return user.Claims.FirstOrDefault(x => x.Type == claimType)?.Value;
        }
    }
}
