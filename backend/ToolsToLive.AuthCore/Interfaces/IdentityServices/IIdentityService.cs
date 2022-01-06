using System;
using System.Collections.Generic;
using System.Security.Claims;
using ToolsToLive.AuthCore.Interfaces.Model;

namespace ToolsToLive.AuthCore.Interfaces.IdentityServices
{
    public interface IIdentityService
    {
        /// <summary>
        /// Generates new token.
        /// </summary>
        /// <returns>Token</returns>
        IAuthToken GenerateAuthToken(IUser user);

        /// <summary>
        /// Generates token for cookies
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        IAuthToken GenerateAuthTokenForCookie(IUser user);

        //(string, DateTime, DateTime) GenerateJwtToken(IEnumerable<Claim> claims, TimeSpan tokenLifeTime);

        /// <summary>
        /// Generates refresh token.
        /// </summary>
        /// <returns>Refresh token.</returns>
        IAuthToken GenerateRefreshToken(IUser user);
    }
}
