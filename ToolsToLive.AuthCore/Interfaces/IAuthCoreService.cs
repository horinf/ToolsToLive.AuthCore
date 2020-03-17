//using System.Security.Claims;
using System.Threading.Tasks;
using ToolsToLive.AuthCore.Interfaces.Model;
using ToolsToLive.AuthCore.Model;

namespace ToolsToLive.AuthCore.Interfaces
{
    public interface IAuthCoreService<T> where T : IUser
    {
        /// Auth, not in use
        //ClaimsPrincipal Auth(string token);

        /// <summary>
        /// Validate password (hashes passed password and compares result to password hash in database).
        /// </summary>
        /// <param name="userNameOrEmail">User name or user email (if user not found by user name, auth service will try to find user by email (emails should be unique for each user)).</param>
        /// <param name="password">Password to validate (not hashed, plain password that user typed in login field).</param>
        /// <returns>Auth result with token, refresh token and user.</returns>
        Task<AuthResult<T>> SignIn(string userNameOrEmail, string password);

        /// <summary>
        /// Validates refresh token (compares to refresh token in db) and creates new token.
        /// Before call this method you should validate token (user must be authorized for creating new token).
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="sessionId"></param>
        /// <param name="refreshToken"></param>
        /// <returns>Auth result with token, refresh token and user.</returns>
        Task<AuthResult<T>> RefreshToken(string userName, string sessionId, string refreshToken);
    }
}
