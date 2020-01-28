using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using ToolsToLive.AuthCore.Interfaces;

namespace ToolsToLive.AuthCore
{
    public class IdentityService
    {
        public ClaimsIdentity GetIdentity(IUser person)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, person.UserName),
            };
            foreach (var item in person.Roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, item.Id));
            }

            if (person.Claims != null && person.Claims.Any())
            {
                claims.AddRange(person.Claims);
            }

            ClaimsIdentity claimsIdentity =
                new ClaimsIdentity(claims, "Token", ClaimsIdentity.DefaultNameClaimType,
                    ClaimsIdentity.DefaultRoleClaimType);
            return claimsIdentity;
        }
    }
}
