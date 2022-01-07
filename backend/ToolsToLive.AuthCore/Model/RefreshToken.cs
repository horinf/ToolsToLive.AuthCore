using System;
using Newtonsoft.Json;

namespace ToolsToLive.AuthCore.Model
{
    public class RefreshToken
    {
        public string UserId { get; set; }
        public string Token { get; set; }
        public DateTime IssueDate { get; set; }
        public DateTime ExpireDate { get; set; }

        [JsonIgnore]
        public string PreviousToken { get; set; }
    }
}
