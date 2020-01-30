using System.Security.Claims;
using System.Threading.Tasks;
using ToolsToLive.AuthCore.Interfaces;
using ToolsToLive.AuthCore.Model;

namespace ToolsToLive.AuthCore
{
    public class AuthenticationService<T> : IAuthenticationService<T> where T : IUser
    {
        public ClaimsPrincipal Auth(string token)
        {
            throw new System.NotImplementedException();
        }

        public Task<AuthResult<T>> CheckPasswordAndGenerateToken(string userNameOrEmail, string password)
        {
            throw new System.NotImplementedException();
        }

        public Task<AuthResult<T>> RefreshToken(string token, string refreshToken)
        {
            var principal = _authenticationCore.GetPrincipalFromToken(token, _authOptions);
            return principal;
        }
    }
}
