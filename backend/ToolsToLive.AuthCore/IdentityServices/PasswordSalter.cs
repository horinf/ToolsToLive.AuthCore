using System;
using Microsoft.Extensions.Options;
using ToolsToLive.AuthCore.Interfaces.IdentityServices;
using ToolsToLive.AuthCore.Interfaces.Model;
using ToolsToLive.AuthCore.Model;

namespace ToolsToLive.AuthCore.IdentityServices
{
    public class PasswordSalter : IPasswordSalter
    {
        private readonly IOptions<AuthOptions> _options;

        public PasswordSalter(
            IOptions<AuthOptions> options
            )
        {
            _options = options;
        }

       
        /// <inheritdoc />
        public string SaltPassword(IAuthCoreUser user, string password)
        {
            switch (user.PasswordVersion)
            {
                case 1:
                    password = _options.Value.ExtraSalt + password;
                    password = _options.Value.UseUserIdSalt == true ? password + user.Id : password;

                    return password;
                default:
                    throw new NotImplementedException($"Password version {user.PasswordVersion} not supported by PasswordSalter");
            }
        }
    }
}
