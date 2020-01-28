using System;
using System.Threading.Tasks;

namespace ToolsToLive.AuthCore.Interfaces
{
    public interface IUserStore
    {
        Task<IUser> GetUserByUserName(string userName);
        Task<IUser> GetUserByEmail(string email);
        Task<IUser> GetUserById(string id);

        Task<IAuthToken> GetRefreshToken(string userName);
        Task SaveRefreshToken(string userName, string refreshToken, DateTime issueDate, DateTime expireDate);
        Task UpdateRefreshToken(string userName, string refreshToken);
        Task DeleteRefreshToken(string userName);
        Task UpdateLastActivity(string userId);
    }
}
