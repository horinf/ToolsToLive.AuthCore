using System;
using ToolsToLive.AuthCore.Interfaces.Model;

namespace ToolsToLive.AuthCore.Model
{
    public class AuthToken : IAuthToken
    {
        public string UserId { get; set; }
        public string Token { get; set; }
        public DateTime IssueDate { get; set; }
        public DateTime ExpireDate { get; set; }
    }
}
