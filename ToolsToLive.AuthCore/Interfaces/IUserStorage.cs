using System.Threading.Tasks;
using ToolsToLive.AuthCore.Interfaces.Model;

namespace ToolsToLive.AuthCore.Interfaces
{
    public interface IUserStorage<TUser> where TUser : IUser
    {
        /// <summary>
        /// Gets user from db by user name.
        /// </summary>
        /// <param name="userName">User name.</param>
        /// <returns>User.</returns>
        Task<TUser> GetUserByUserName(string userName);

        /// <summary>
        /// Gets user from db by user e-mail address (assuming that emails must be unique to each user).
        /// </summary>
        /// <param name="email">Email address.</param>
        /// <returns>User.</returns>
        Task<TUser> GetUserByEmail(string email);

        /// <summary>
        /// Gets refresh token from db by user name.
        /// </summary>
        /// <param name="userName">User name.</param>
        /// <returns>Auth token.</returns>
        Task<IAuthToken> GetRefreshToken(string userName);

        /// <summary>
        /// Saves new refresh token to database (if already exists should be overwritten).
        /// </summary>
        /// <param name="authToken">Auth token interface.</param>
        Task SaveNewRefreshToken(IAuthToken authToken);

        /// <summary>
        /// Updates refresh token that already exists.
        /// </summary>
        /// <param name="userName">User name.</param>
        /// <param name="refreshToken">Refresh token to store (only token, created date and other information should be left as is).</param>
        Task UpdateRefreshToken(string userName, string refreshToken);

        /// <summary>
        /// Updates user last activity (is being called when new token is generated (user logged in or refresh his token)).
        /// </summary>
        Task UpdateLastActivity(string userId);
    }
}
