using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ToolsToLive.AuthCore.Interfaces.Helpers;
using ToolsToLive.AuthCore.Interfaces.IdentityServices;
using ToolsToLive.AuthCore.Model;

namespace ToolsToLive.AuthCore.Helpers
{
    public class AuthCookiesHelper : IAuthCookiesHelper
    {
        private readonly IIdentityValidationService _identityValidationService;
        private readonly IOptions<AuthOptions> _authOptions;
        private readonly ILogger<AuthCookiesHelper> _logger;

        public AuthCookiesHelper(
            IIdentityValidationService identityValidationService,
            IOptions<AuthOptions> authOptions,
            ILogger<AuthCookiesHelper> logger)
        {
            _identityValidationService = identityValidationService;
            _authOptions = authOptions;
            _logger = logger;
        }

        public string GetDeviceIdFromCookie(IRequestCookieCollection requestCookies)
        {
            if (requestCookies.ContainsKey(_authOptions.Value.DeviceIdCookieName))
            {
                return requestCookies[_authOptions.Value.DeviceIdCookieName];
            }
            return null;
        }

        public void SetDeviceIdCookie(IResponseCookies responseCookies, string deviceId, DateTime expire)
        {
            responseCookies.Append(_authOptions.Value.DeviceIdCookieName, deviceId, new CookieOptions()
            {
                Domain = _authOptions.Value.CookieDomain,
                Path = _authOptions.Value.DeviceIdCookiePath,
                HttpOnly = true,
                Secure = _authOptions.Value.CookieSecure,
                SameSite = _authOptions.Value.CookieSameSiteMode,
                Expires = expire,
                MaxAge = expire - DateTime.UtcNow,
            });
        }

        public void ClearDeviceIdCookie(IResponseCookies responseCookies)
        {
            responseCookies.Append(_authOptions.Value.DeviceIdCookieName, "", new CookieOptions()
            {
                Domain = _authOptions.Value.CookieDomain,
                Path = _authOptions.Value.DeviceIdCookiePath,
                HttpOnly = true,
                Secure = _authOptions.Value.CookieSecure,
                SameSite = _authOptions.Value.CookieSameSiteMode,
                Expires = DateTime.UtcNow.AddYears(-2),
                MaxAge = TimeSpan.Zero,
            });
        }

        public ClaimsPrincipal GetAuthCookie(IRequestCookieCollection requestCookies)
        {
            if (requestCookies.ContainsKey(_authOptions.Value.AuthCookieName))
            {
                var token = requestCookies[_authOptions.Value.AuthCookieName];
                try
                {
                    ClaimsPrincipal principal = _identityValidationService.GetPrincipalFromToken(token);
                    if (!principal.HasClaim(AuthCoreConstants.TokenTransportClaim, TokenTransport.Cookie.ToString()))
                    {
                        return null;
                    }
                    return principal;
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Unable to get ClaimsPrincipal from token (from cookies)");
                    return null;
                }
            }
            return null;
        }

        public void SetAuthCookie(IResponseCookies responseCookies, string token, DateTime expire)
        {
            responseCookies.Append(_authOptions.Value.AuthCookieName, token, new CookieOptions()
            {
                Domain = _authOptions.Value.CookieDomain,
                Path = _authOptions.Value.AuthCookiePath,
                HttpOnly = true,
                Secure = _authOptions.Value.CookieSecure,
                SameSite = _authOptions.Value.CookieSameSiteMode,
                Expires = expire,
                MaxAge = expire - DateTime.UtcNow,
            });
        }

        public void ClearAuthCookie(IResponseCookies responseCookies)
        {
            responseCookies.Append(_authOptions.Value.AuthCookieName, "", new CookieOptions()
            {
                Domain = _authOptions.Value.CookieDomain,
                Path = _authOptions.Value.AuthCookiePath,
                HttpOnly = true,
                Secure = _authOptions.Value.CookieSecure,
                SameSite = _authOptions.Value.CookieSameSiteMode,
                Expires = DateTime.UtcNow.AddYears(-2),
                MaxAge = TimeSpan.Zero,
            });
        }
    }
}
