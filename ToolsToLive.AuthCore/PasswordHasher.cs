using System.Security.Cryptography;
using System.Text;
using ToolsToLive.AuthCore.Interfaces;

namespace ToolsToLive.AuthCore
{
    public class PasswordHasher : IPasswordHasher
    {
        public string HashPassword(string password)
        {
            var sha = new SHA512Managed();
            byte[] hash = sha.ComputeHash(Encoding.Unicode.GetBytes(password));
            return FromByteToHex(hash);
        }

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
