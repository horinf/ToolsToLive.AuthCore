using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;
using ToolsToLive.AuthCore.Interfaces.IdentityServices;

namespace ToolsToLive.AuthCore.IdentityServices
{
    public class CodeGenerator : ICodeGenerator
    {
        public string GenerateCode(int length = 32)
        {
            var randomNumber = new byte[length];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                var tokenString = Base64UrlEncoder.Encode(randomNumber);
                return tokenString;
            }
        }
    }
}
