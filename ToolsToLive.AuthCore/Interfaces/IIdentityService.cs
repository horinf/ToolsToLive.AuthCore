using System.Security.Claims;
using System.Threading.Tasks;
using ToolsToLive.AuthCore.Interfaces.Model;

namespace ToolsToLive.AuthCore.Interfaces
{
    public interface IIdentityService
    {
        //ClaimsIdentity GetIdentity(IUser user);

        ClaimsPrincipal GetPrincipalFromToken(string token);

        Task<IAuthToken> GenerateToken<TUser>(TUser user) where TUser : IUser;
        Task<IAuthToken> GenerateRefreshToken(IUser user);
    }
}
