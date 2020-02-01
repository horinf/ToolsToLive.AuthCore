//using System.Security.Claims;
using System.Threading.Tasks;
using ToolsToLive.AuthCore.Interfaces.Model;
using ToolsToLive.AuthCore.Model;

namespace ToolsToLive.AuthCore.Interfaces
{
    public interface IAuthCoreService<T> where T : IUser
    {
        //ClaimsPrincipal Auth(string token);
        Task<AuthResult<T>> CheckPasswordAndGenerateToken(string userNameOrEmail, string password);
        Task<AuthResult<T>> RefreshToken(string token, string refreshToken);
    }
}
