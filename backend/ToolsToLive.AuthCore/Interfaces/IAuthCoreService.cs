using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using ToolsToLive.AuthCore.Interfaces.Model;
using ToolsToLive.AuthCore.Model;

namespace ToolsToLive.AuthCore.Interfaces
{
    public interface IAuthCoreService<T> where T : IAuthCoreUser
    {
        /// <summary>
        /// Gets ClaimsPrincipal by access token
        /// </summary>
        /// <param name="token"></param>
        /// <returns>ClaimsPrincipal</returns>
        ClaimsPrincipal GetPrincipalByToken(string token);

        /// <summary>
        /// SignIn (generate tokens) without checking password
        /// </summary>
        /// <returns>Auth result with token, refresh token and user</returns>
        Task<AuthResult<T>> SignIn(string userId, string deviceId, IResponseCookies responseCookies);

        /// <summary>
        /// Validate password (hashes passed password and compares result to password hash in database)
        /// And generate access token and refresh token
        /// </summary>
        /// <param name="userNameOrEmail">User name or user email (if user not found by user name, auth service will try to find user by email (emails should be unique for each user))</param>
        /// <param name="password">Password to validate (not hashed, plain password that user typed in login field)</param>
        /// <param name="deviceId">Device fingerprint</param>
        /// <returns>Auth result with token, refresh token and user</returns>
        Task<AuthResult<T>> SignIn(string userNameOrEmail, string password, string deviceId, IResponseCookies responseCookies);

        Task SignOut(string userId, string deviceId, IRequestCookieCollection requestCookies, IResponseCookies responseCookies);

        Task SignOutFromEverywhere(string userId, IResponseCookies responseCookies);

        /// <summary>
        /// Validates refresh token (compares to refresh token in db) and creates new token
        /// Before call this method you should validate token (user must be authorized for creating new token)
        /// </summary>
        /// <returns>Auth result with token, refresh token and user</returns>
        Task<AuthResult<T>> RefreshToken(string userId, string deviceId, string providedRefreshToken, IRequestCookieCollection requestCookies, IResponseCookies responseCookies);
    }
}
