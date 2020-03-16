using System.Security.Cryptography;
using System.Text;
using ToolsToLive.AuthCore.Interfaces;

namespace ToolsToLive.AuthCore
{
    public class PasswordHasher : IPasswordHasher
    {

        /// <summary>
        /// Hashes password. It is a good idea to add salt to password.
        /// </summary>
        /// <param name="password"></param>
        public string HashPassword(string password)
        {
            var sha = new SHA512Managed();
            byte[] hash = sha.ComputeHash(Encoding.Unicode.GetBytes(password));
            return FromByteToHex(hash);
        }

        /// <summary>
        /// Verifies password. If you added salt when setting a password, do not forget to add it to the password provided.
        /// </summary>
        /// <param name="hashedPassword"></param>
        /// <param name="providedPassword"></param>
        public bool VerifyHashedPassword(string hashedPassword, string providedPassword)
        {
            return HashPassword(providedPassword) == hashedPassword;
        }

        private static string FromByteToHex(byte[] bytes)
        {
            var sb = new StringBuilder();
            foreach (byte b in bytes)
            {
                sb.Append(b.ToString("x2"));
            }

            return sb.ToString();
        }
    }
}
