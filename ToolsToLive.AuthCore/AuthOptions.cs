using System;

namespace ToolsToLive.AuthCore
{
    public class AuthOptions
    {
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public string Key { get; set; }
        public TimeSpan TokenLifetime { get; set; }
        public TimeSpan RefreshTokenLifeTime { get; set; }
    }
}
