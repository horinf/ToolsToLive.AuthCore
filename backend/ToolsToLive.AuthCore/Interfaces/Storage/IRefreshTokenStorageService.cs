using System.Collections.Generic;
using System.Threading.Tasks;
using ToolsToLive.AuthCore.Model;

namespace ToolsToLive.AuthCore.Interfaces.Storage
{
    public interface IRefreshTokenStorageService
    {
        /// <summary>
        /// Gets refresh token from db by user name
        /// </summary>
        /// <returns>Auth token</returns>
        Task<RefreshToken> GetRefreshToken(string userId, string deviceId);

        /// <summary>
        /// Gets refresh token from db by user name
        /// </summary>
        /// <returns>Auth token</returns>
        Task<List<RefreshToken>> GetRefreshTokens(string userId);

        /// <summary>
        /// Saves new refresh token to database (if already exists should be overwritten)
        /// </summary>
        Task SaveNewRefreshToken(RefreshToken authToken, string deviceId);

        /// <summary>
        /// Updates refresh token that already exists
        /// </summary>
        Task UpdateRefreshToken(string userId, string deviceId, RefreshToken refreshToken);

        /// <summary>
        /// Deletes refresh token from Db (e.g. when logout)
        /// </summary>
        Task DeleteRefreshToken(string userId, string deviceId);

        /// <summary>
        /// Deletes all refresh tokens from Db
        /// </summary>
        Task DeleteRefreshTokens(string userId);
    }
}
