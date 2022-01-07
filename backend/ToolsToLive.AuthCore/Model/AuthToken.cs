using System;

namespace ToolsToLive.AuthCore.Model
{
    public class AuthToken
    {
        public string Token { get; set; }
        public DateTime ExpireDate { get; set; }
    }
}
