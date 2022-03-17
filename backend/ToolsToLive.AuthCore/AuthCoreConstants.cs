namespace ToolsToLive.AuthCore
{
    public static class AuthCoreConstants
    {
        /// <summary>
        /// Claim name for identify "transport" for token (cookies or header)
        /// </summary>
        public const string TokenTransportClaim = "AuthCore_TokenTransport";

        public const string RoleClaim = "AuthCore_Role";

        public const string UserIdClaim = "AuthCore_UserId";

        public const string UserNameClaim = "AuthCore_UserName";

    }
}
