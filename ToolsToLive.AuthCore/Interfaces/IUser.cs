using System.Collections.Generic;
using System.Security.Claims;

namespace ToolsToLive.AuthCore.Interfaces
{
    public interface IUser
    {
        string Id { get; }
        string UserName { get; }
        string PasswordHash { get; set; }
        IRole Role { get; }
        List<Claim> Claims { get; }
    }
}
