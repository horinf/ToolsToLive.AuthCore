using ToolsToLive.AuthCore.Interfaces;

namespace ToolsToLive.AuthCore.Model
{
    public class AuthResult<T> where T : IUser
    {
        public AuthResult(AuthResultType type)
        {
            AuthResultType = type;
        }

        public AuthResultType AuthResultType { get; private set; }

        public T User { get; set; }

        public IAuthToken Token { get; set; }

        public IAuthToken RefreshToken { get; set; }

        public bool IsSuccess => AuthResultType == AuthResultType.Success;
    }
}
