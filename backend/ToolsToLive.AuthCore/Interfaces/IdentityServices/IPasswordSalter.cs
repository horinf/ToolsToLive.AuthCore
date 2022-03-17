using ToolsToLive.AuthCore.Interfaces.Model;

namespace ToolsToLive.AuthCore.Interfaces.IdentityServices
{
    /// <summary>
    /// Service to salt password
    /// </summary>
    public interface IPasswordSalter
    {
        /// <summary>
        /// Salt password
        /// </summary>
        /// <returns>Salted password</returns>
        string SaltPassword(IAuthCoreUser user, string password);
    }
}
