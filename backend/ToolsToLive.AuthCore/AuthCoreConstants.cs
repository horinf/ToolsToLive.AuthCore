namespace ToolsToLive.AuthCore
{
    public static class AuthCoreConstants
    {
        /// <summary>
        /// Claim name for identify "transport" for token (cookies or header)
        /// </summary>
        public const string TokenTransportClaim = "AuthCoreTokenTransport";

        public const string RoleClaim = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role";

        public const string UserIdClaim = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier";

        public const string UserNameClaim = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name";

    }
}
