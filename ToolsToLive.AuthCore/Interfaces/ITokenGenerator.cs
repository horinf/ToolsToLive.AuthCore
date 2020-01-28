using System;
using System.Security.Claims;

namespace ToolsToLive.AuthCore.Interfaces
{
    public interface ITokenGenerator
    {
        string GenerateToken(ClaimsIdentity identity, DateTime notBefore, DateTime expires);
    }
}
