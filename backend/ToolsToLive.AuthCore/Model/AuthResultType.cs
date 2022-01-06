namespace ToolsToLive.AuthCore.Model
{
    public enum AuthResultType
    {
        Failed = 1,
        Success = 2,
        UserNotFound = 3,
        PasswordWrong = 4,
        RefreshTokenWrong = 5,
        LockedOut = 6,
    }
}
