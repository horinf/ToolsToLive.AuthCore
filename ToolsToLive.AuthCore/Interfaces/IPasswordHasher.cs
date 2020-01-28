namespace ToolsToLive.AuthCore.Interfaces
{
    /// <summary>
    /// Service to hash password and check if password is correct (does it match to its hash)
    /// </summary>
    public interface IPasswordHasher
    {
        /// <summary>
        /// Hash password
        /// </summary>
        /// <param name="password">Password</param>
        /// <returns>Hash of password</returns>
        string HashPassword(string password);

        /// <summary>
        /// Check if password is correct (does it match to its hash)
        /// </summary>
        /// <param name="hashedPassword">Hash of password</param>
        /// <param name="providedPassword">Plain (not hashed) password</param>
        /// <returns>True if password matchs to hash</returns>
        bool VerifyHashedPassword(string hashedPassword, string providedPassword);
    }
}
