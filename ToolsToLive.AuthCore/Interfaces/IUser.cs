using System.Collections.Generic;
using System.Security.Claims;

namespace ToolsToLive.AuthCore.Interfaces
{
    public interface IUser
    {
        string Id { get; }
        string UserName { get; }
        string PasswordHash { get; set; }
        List<IRole> Roles { get; }
        List<Claim> Claims { get; }
    }
}
