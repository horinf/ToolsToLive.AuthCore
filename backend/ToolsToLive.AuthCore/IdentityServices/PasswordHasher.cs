using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using ToolsToLive.AuthCore.Interfaces.IdentityServices;

namespace ToolsToLive.AuthCore.IdentityServices
{
    public class PasswordHasher : IPasswordHasher
    {
        public PasswordHasher()
        {
        }

        /// <summary>
        /// Hashes password. It is a good idea to add salt to password.
        /// </summary>
        /// <returns>Hash of password</returns>
        public string HashPassword(string password)
        {
            var sha = new SHA512Managed();
            var hash = sha.ComputeHash(Encoding.Unicode.GetBytes(password));
            return FromByteToHex(hash);
        }

        /// <summary>
        /// Verifies password. If you added salt when setting a password, do not forget to add it to the password provided.
        /// </summary>
        /// <returns>True if password matchs to hash</returns>
        public bool VerifyPassword(string hashedPassword, string providedPassword)
        {
            var hash = HashPassword(providedPassword).ToLowerInvariant();
            return hash.Equals(hashedPassword.ToLowerInvariant());
        }

        private static string FromByteToHex(byte[] bytes)
        {
            var sb = new StringBuilder();
            foreach (var b in bytes)
            {
                sb.Append(b.ToString("x2", CultureInfo.InvariantCulture).ToUpperInvariant());
            }

            return sb.ToString();
        }
    }
}
