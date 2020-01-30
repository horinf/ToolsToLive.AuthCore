using System;
using System.Threading.Tasks;

namespace ToolsToLive.AuthCore.Interfaces
{
    public interface IUserStore<TUser> where TUser : IUser
    {
        Task<TUser> GetUserByUserName(string userName);
        Task<TUser> GetUserByEmail(string email);
        Task<TUser> GetUserById(string id);

        Task<IAuthToken> GetRefreshToken(string userName);
        Task SaveRefreshToken(IAuthToken authToken);
        Task UpdateRefreshToken(string userName, string refreshToken);
        Task DeleteRefreshToken(string userName);
        Task UpdateLastActivity(string userId);
    }
}
