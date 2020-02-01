using ToolsToLive.AuthCore.Interfaces.Model;

namespace ToolsToLive.AuthCore.Model
{
    public class AuthResult<T> where T : IUser
    {
        public AuthResult(AuthResultType result)
        {
            Result = result;
        }

        public AuthResultType Result { get; private set; }

        public T User { get; set; }

        public IAuthToken Token { get; set; }

        public IAuthToken RefreshToken { get; set; }

        public bool IsSuccess => Result == AuthResultType.Success;
    }
}
