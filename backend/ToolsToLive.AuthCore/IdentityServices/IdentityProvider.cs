using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using ToolsToLive.AuthCore.Interfaces.IdentityServices;
using ToolsToLive.AuthCore.Interfaces.Model;

namespace ToolsToLive.AuthCore.IdentityServices
{
    public class IdentityProvider : IIdentityProvider
    {
        //public ClaimsIdentity GetIdentity(IEnumerable<Claim> claims)
        //{
        //    var claimsIdentity = new ClaimsIdentity(
        //        claims,
        //        "Token",
        //        ClaimsIdentity.DefaultNameClaimType,
        //        ClaimsIdentity.DefaultRoleClaimType);

        //    return claimsIdentity;
        //}

        public List<Claim> GetClaimsForUser(IAuthCoreUser user)
        {
            var claims = new List<Claim>
            {
                new Claim(AuthCoreConstants.UserNameClaim, user.UserName),
                new Claim(AuthCoreConstants.UserIdClaim, user.Id),
            };

            if (user.Roles != null)
            {
                foreach (var item in user.Roles)
                {
                    claims.Add(new Claim(AuthCoreConstants.RoleClaim, item));
                }
            }

            if (user.Claims != null && user.Claims.Any())
            {
                claims.AddRange(user.Claims);
            }

            return claims;
        }
    }
}
