using System;

namespace ToolsToLive.AuthCore.Model
{
    public class AuthOptions
    {
        /// <summary>
        /// Someone who creates token (identity server).
        /// Any valid string.
        /// </summary>
        public string Issuer { get; set; }

        /// <summary>
        /// Someone for whome token should be created (consumer).
        /// Any valid string.
        /// </summary>
        public string Audience { get; set; }

        /// <summary>
        /// Strong key to sign token. (The longer string is, the stronger key -- 256 chars should be Ok).
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// Token lifetime.
        /// </summary>
        public TimeSpan TokenLifetime { get; set; }

        /// <summary>
        /// Refresh token lifetime.
        /// </summary>
        public TimeSpan RefreshTokenLifeTime { get; set; }
    }
}
