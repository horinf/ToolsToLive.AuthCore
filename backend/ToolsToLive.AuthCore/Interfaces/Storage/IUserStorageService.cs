using System;
using System.Threading.Tasks;
using ToolsToLive.AuthCore.Interfaces.Model;

namespace ToolsToLive.AuthCore.Interfaces.Storage
{
    public interface IUserStorageService<TUser> where TUser : IAuthCoreUser
    {
        /// <summary>
        /// Gets user from db by user id
        /// </summary>
        /// <param name="userId">User id.</param>
        /// <returns>User, or null if user does not exists</returns>
        Task<TUser> GetUserById(string userId);

        /// <summary>
        /// Gets user from db by user name
        /// </summary>
        /// <param name="userName">User name</param>
        /// <returns>User, or null if user does not exists</returns>
        Task<TUser> GetUserByUserNameOrEmail(string userNameOrEmail);

        /// <summary>
        /// Updates user last activity (is being called when new token is generated (user logged in or refresh his token))
        /// </summary>
        Task UpdateLastActivity(string userId);

        Task UpdateLockoutData(string userId, int accessFailedCount, DateTime? lockoutEndDate);
    }
}
