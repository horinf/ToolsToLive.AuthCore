using System;

namespace ToolsToLive.AuthCore.Interfaces.Model
{
    public interface IAuthToken
    {
        string UserId { get; set; }
        string Token { get; set; }
        DateTime IssueDate { get; set; }
        DateTime ExpireDate { get; set; }
    }
}
