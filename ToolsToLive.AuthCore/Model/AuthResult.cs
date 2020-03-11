using ToolsToLive.AuthCore.Interfaces.Model;

namespace ToolsToLive.AuthCore.Model
{
    public class AuthResult<T> where T : IUser
    {
        public AuthResult(AuthResultType result)
        {
            Result = result;
        }

        /// <summary>
        /// Indicates status of authentication (success, reason why fail, or just fault if something wrong)
        /// </summary>
        public AuthResultType Result { get; private set; }

        /// <summary>
        /// User, always provided when auth is success
        /// </summary>
        public T User { get; set; }

        public IAuthToken Token { get; set; }

        public IAuthToken RefreshToken { get; set; }

        public bool IsSuccess => Result == AuthResultType.Success;
    }
}
