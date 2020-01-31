using System.Threading.Tasks;

namespace ToolsToLive.AuthCore.Interfaces
{
    public interface ITokenGenerator
    {
        Task<IAuthToken> GenerateToken<TUser>(TUser user) where TUser : IUser;
        Task<IAuthToken> GenerateRefreshToken(IUser user);
    }
}
