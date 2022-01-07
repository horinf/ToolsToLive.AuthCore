using ToolsToLive.AuthCore.Interfaces.Model;

namespace ToolsToLive.AuthCore.Model
{
    public class AuthResult<T> where T : IAuthCoreUser
    {
        public AuthResult(AuthResultType result)
        {
            Result = result;
        }

        /// <summary>
        /// Indicates status of authentication (success, reason why it fails, or just failed if something is wrong)
        /// </summary>
        public AuthResultType Result { get; private set; }

        /// <summary>
        /// User, always provided when auth is success
        /// </summary>
        public T User { get; set; }

        public string AccessToken { get; set; }

        public RefreshToken RefreshToken { get; set; }

        public bool IsSuccess => Result == AuthResultType.Success;
    }
}
