namespace ToolsToLive.AuthCore.Model
{
    public enum AuthResultType
    {
        Fault,
        Success,
        UserNotFound,
        PasswordWrong,
        RefreshTokenWrong,
        LockedOut,
    }
}
