using System.Threading.Tasks;
using ToolsToLive.AuthCore.Interfaces.Model;

namespace ToolsToLive.AuthCore.Interfaces
{
    public interface IIdentityService
    {
        /// <summary>
        /// Generates new token.
        /// </summary>
        /// <param name="user">User.</param>
        /// <param name="sessionId">Session Id.</param>
        /// <returns>Token</returns>
        Task<IAuthToken> GenerateToken(IUser user, string sessionId);

        /// <summary>
        /// Generates refresh token.
        /// </summary>
        /// <param name="user">User.</param>
        /// <param name="sessionId">Session Id.</param>
        /// <returns>Refresh token.</returns>
        Task<IAuthToken> GenerateRefreshToken(IUser user, string sessionId);
    }
}
