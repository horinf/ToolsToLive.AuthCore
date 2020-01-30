using System;
using System.Security.Claims;

namespace ToolsToLive.AuthCore.Interfaces
{
    public interface ITokenGenerator
    {
        IAuthToken GenerateToken<TUser>(TUser user) where TUser : IUser;
        IAuthToken GenerateRefreshToken();
    }
}
