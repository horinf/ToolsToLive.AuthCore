using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
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

        public AuthCookiesHelper(
            IIdentityValidationService identityValidationService,
            IOptions<AuthOptions> authOptions)
        {
            _identityValidationService = identityValidationService;
            _authOptions = authOptions;
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
                Path = _authOptions.Value.CookiePath,
                HttpOnly = true,
                Secure = _authOptions.Value.CookieSecure,
                SameSite = _authOptions.Value.CookieSameSiteMode,
                Expires = expire,
                MaxAge = expire - DateTime.Now,
            });
        }

        public void ClearDeviceIdCookie(IResponseCookies responseCookies)
        {
            responseCookies.Append(_authOptions.Value.DeviceIdCookieName, "", new CookieOptions()
            {
                Domain = _authOptions.Value.CookieDomain,
                Path = _authOptions.Value.CookiePath,
                HttpOnly = true,
                Secure = _authOptions.Value.CookieSecure,
                SameSite = _authOptions.Value.CookieSameSiteMode,
                Expires = DateTime.Now.AddYears(-2),
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
                catch (Exception)
                {
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
                Path = "/",
                HttpOnly = true,
                Secure = _authOptions.Value.CookieSecure,
                SameSite = _authOptions.Value.CookieSameSiteMode,
                Expires = expire,
                MaxAge = expire - DateTime.Now,
            });
        }

        public void ClearAuthCookie(IResponseCookies responseCookies)
        {
            responseCookies.Append(_authOptions.Value.AuthCookieName, "", new CookieOptions()
            {
                Domain = _authOptions.Value.CookieDomain,
                Path = "/",
                HttpOnly = true,
                Secure = _authOptions.Value.CookieSecure,
                SameSite = _authOptions.Value.CookieSameSiteMode,
                Expires = DateTime.Now.AddYears(-2),
                MaxAge = TimeSpan.Zero,
            });
        }
    }
}
