using System;
using Microsoft.AspNetCore.Http;

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

        public int MaxAccessFailedCount { get; set; }

        public TimeSpan LockoutPeriod { get; set; }

        // Cookies

        public string AuthCookieName { get; set; }

        public string DeviceIdCookieName { get; set; }

        public string CookieDomain { get; set; }

        public string CookiePath { get; set; }

        public bool CookieSecure { get; set; }

        public SameSiteMode CookieSameSiteMode { get; set; }


        //public TimeSpan EmailConfirmCodeLifeTime { get; set; }

        //public TimeSpan ChangePasswordCodeLifeTime { get; set; }

        //public string OauthRedirectUrl { get; set; }

        //public string ExtraSalt { get; set; }
    }
}
