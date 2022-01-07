using System;
using System.Collections.Generic;
using System.Security.Claims;
using ToolsToLive.AuthCore.Interfaces.Model;
using ToolsToLive.AuthCore.Model;

namespace ToolsToLive.AuthCore.Interfaces.IdentityServices
{
    public interface IIdentityService
    {
        /// <summary>
        /// Generates new token.
        /// </summary>
        /// <returns>Token</returns>
        AuthToken GenerateAuthToken(IAuthCoreUser user);

        /// <summary>
        /// Generates token for cookies
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        AuthToken GenerateAuthTokenForCookie(IAuthCoreUser user);

        //(string, DateTime, DateTime) GenerateJwtToken(IEnumerable<Claim> claims, TimeSpan tokenLifeTime);

        /// <summary>
        /// Generates refresh token.
        /// </summary>
        /// <returns>Refresh token.</returns>
        RefreshToken GenerateRefreshToken(IAuthCoreUser user);
    }
}
