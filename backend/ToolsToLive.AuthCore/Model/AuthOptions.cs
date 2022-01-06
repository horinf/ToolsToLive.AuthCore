using System;
using Microsoft.AspNetCore.Http;

namespace ToolsToLive.AuthCore.Model
{
    public class AuthOptions
    {
        /// <summary>
        /// Strong key to sign token. (The longer string is, the stronger key -- 256 chars should be Ok).
        /// Required (should be set in appsettings)
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// The one that creates the token (identity server).
        /// Any valid string.
        /// </summary>
        public string Issuer { get; set; } = DefaultIssuer;
        public static string DefaultIssuer { get; set; } = "AuthCoreIssuer";

        /// <summary>
        /// The one for which the token should be created (consumer).
        /// Any valid string.
        /// </summary>
        public string Audience { get; set; } = DefaultAudience;
        public static string DefaultAudience { get; set; } = "AuthCoreAudience";

        /// <summary>
        /// Token lifetime.
        /// </summary>
        public TimeSpan TokenLifetime { get; set; } = DefaultTokenLifetime;
        public static TimeSpan DefaultTokenLifetime { get; set; } = TimeSpan.FromMinutes(40);

        /// <summary>
        /// Refresh token lifetime.
        /// </summary>
        public TimeSpan RefreshTokenLifeTime { get; set; } = DefaultRefreshTokenLifeTime;
        public static TimeSpan DefaultRefreshTokenLifeTime { get; set; } = TimeSpan.FromDays(7);

        public int MaxAccessFailedCount { get; set; } = DefaultMaxAccessFailedCount;
        public static int DefaultMaxAccessFailedCount { get; set; } = 12;

        public TimeSpan LockoutPeriod { get; set; } = DefaultLockoutPeriod;
        public static TimeSpan DefaultLockoutPeriod { get; set; } = TimeSpan.FromSeconds(15);

        // Cookies

        public bool AddTokenToCookie { get; set; } = DefaultAddTokenToCookie;
        public static bool DefaultAddTokenToCookie { get; set; } = true;

        public string AuthCookieName { get; set; } = DefaultAuthCookieName;
        public static string DefaultAuthCookieName { get; set; } = "AuthCoreCookie";

        public string DeviceIdCookieName { get; set; } = DefaultDeviceIdCookieName;
        public static string DefaultDeviceIdCookieName { get; set; } = "AuthCoreDeviceIdCookie";

        public bool CookieSecure { get; set; } = DefaultCookieSecure;
        public static bool DefaultCookieSecure { get; set; } = true;

        public SameSiteMode CookieSameSiteMode { get; set; } = DefaultCookieSameSiteMode;
        public static SameSiteMode DefaultCookieSameSiteMode { get; set; } = SameSiteMode.Strict;

        /// <summary>
        /// The name of the domain for which the cookie will be set (eg 'mydomain.com')
        /// Required (should be set in appsettings)
        /// </summary>
        public string CookieDomain { get; set; }

        /// <summary>
        /// Path for auth cookie (eg 'api/auth')
        /// '/' by default, but it's strongly recommended to set something like 'api/auth'
        /// </summary>
        public string CookiePath { get; set; } = DefaultCookiePath;
        public static string DefaultCookiePath { get; set; } = "/";




        //public TimeSpan EmailConfirmCodeLifeTime { get; set; }

        //public TimeSpan ChangePasswordCodeLifeTime { get; set; }

        //public string OauthRedirectUrl { get; set; }

        //public string ExtraSalt { get; set; }
    }
}
