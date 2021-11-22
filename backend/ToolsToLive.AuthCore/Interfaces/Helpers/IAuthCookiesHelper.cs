using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace ToolsToLive.AuthCore.Interfaces.Helpers
{
    public interface IAuthCookiesHelper
    {
        string GetDeviceIdFromCookie(IRequestCookieCollection requestCookies);

        void SetDeviceIdCookie(IResponseCookies responseCookies, string deviceId, DateTime expire);

        void ClearDeviceIdCookie(IResponseCookies responseCookies);


        void SetAuthCookie(IResponseCookies responseCookies, string token, DateTime expire);

        ClaimsPrincipal GetAuthCookie(IRequestCookieCollection requestCookies);

        void ClearAuthCookie(IResponseCookies responseCookies);

    }
}
