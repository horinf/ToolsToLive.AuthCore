namespace ToolsToLive.AuthCore.Interfaces.IdentityServices
{
    /// <summary>
    /// Service to hash password and check if password is correct (does it match to its hash)
    /// </summary>
    public interface IPasswordHasher
    {
        /// <summary>
        /// Hashes password. It is a good idea to add salt to password.
        /// </summary>
        /// <returns>Hash of password</returns>
        string HashPassword(string password);

        /// <summary>
        /// Check if password is correct (does it match to its hash)
        /// If you added salt when setting a password, do not forget to add it to the password provided.
        /// </summary>
        /// <returns>True if password matchs to hash</returns>
        bool VerifyPassword(string hashedPassword, string providedPassword);
    }
}
