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
        public string Issuer { get; set; } = IssuerDefault;
        public static string IssuerDefault { get; } = "AuthCoreIssuer";

        /// <summary>
        /// The one for which the token should be created (consumer).
        /// Any valid string.
        /// </summary>
        public string Audience { get; set; } = AudienceDefault;
        public static string AudienceDefault { get; } = "AuthCoreAudience";

        /// <summary>
        /// Token lifetime.
        /// </summary>
        public TimeSpan TokenLifetime { get; set; } = TokenLifetimeDefault;
        public static TimeSpan TokenLifetimeDefault { get; } = TimeSpan.FromMinutes(40);

        /// <summary>
        /// Refresh token lifetime.
        /// </summary>
        public TimeSpan RefreshTokenLifeTime { get; set; } = RefreshTokenLifeTimeDefault;
        public static TimeSpan RefreshTokenLifeTimeDefault { get; } = TimeSpan.FromDays(7);

        /// <summary>
        /// How many attempts can be failed before user is locked (for LockoutPeriod)
        /// </summary>
        public int MaxAccessFailedCount { get; set; } = MaxAccessFailedCountDefault;
        public static int MaxAccessFailedCountDefault { get; } = 12;

        /// <summary>
        /// User lockout period if MaxAccessFailedCount is reached
        /// </summary>
        public TimeSpan LockoutPeriod { get; set; } = LockoutPeriodDefault;
        public static TimeSpan LockoutPeriodDefault { get; } = TimeSpan.FromSeconds(15);

        /// <summary>
        /// Whether to use the user Id as an additional salt to the password (recommended)
        /// </summary>
        public bool UseUserIdSalt { get; set; } = UseUserIdSaltDefault;
        public static bool UseUserIdSaltDefault { get; } = true;

        /// <summary>
        /// Additional salt that is being added to password
        /// Be careful - if you change it, old passwords will no longer be valid
        /// </summary>
        public string ExtraSalt { get; set; } = ExtraSaltDefault;
        public static string ExtraSaltDefault { get; } = "";

        // Cookies

        /// <summary>
        /// Wheter cookies is used to stroe token along the way (token will be returned as response to signIn request anyway)
        /// </summary>
        public bool AddTokenToCookie { get; set; } = AddTokenToCookieDefault;
        public static bool AddTokenToCookieDefault { get; } = true;

        public string AuthCookieName { get; set; } = AuthCookieNameDefault;
        public static string AuthCookieNameDefault { get; } = "AuthCoreCookie";

        public string DeviceIdCookieName { get; set; } = DeviceIdCookieNameDefault;
        public static string DeviceIdCookieNameDefault { get; } = "AuthCoreDeviceIdCookie";

        public bool CookieSecure { get; set; } = CookieSecureDefault;
        public static bool CookieSecureDefault { get; } = true;

        public SameSiteMode CookieSameSiteMode { get; set; } = CookieSameSiteModeDefault;
        public static SameSiteMode CookieSameSiteModeDefault { get; } = SameSiteMode.Strict;

        /// <summary>
        /// The name of the domain for which the cookie will be set (eg 'mydomain.com')
        /// Required (should be set in appsettings)
        /// </summary>
        public string CookieDomain { get; set; }

        /// <summary>
        /// Path for auth cookie (eg 'api/auth')
        /// '/' by default, but it's strongly recommended to set something like 'api/auth'
        /// </summary>
        public string DeviceIdCookiePath { get; set; } = DeviceIdCookiePathDefault;
        public static string DeviceIdCookiePathDefault { get; } = "/";

        /// <summary>
        /// Path for auth cookie (eg 'api/auth')
        /// '/' by default
        /// </summary>
        public string AuthCookiePath { get; set; } = AuthCookiePathDefault;
        public static string AuthCookiePathDefault { get; } = "/";



        //public TimeSpan EmailConfirmCodeLifeTime { get; set; }

        //public TimeSpan ChangePasswordCodeLifeTime { get; set; }

        //public string OauthRedirectUrl { get; set; }
    }
}
