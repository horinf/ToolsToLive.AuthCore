namespace ToolsToLive.AuthCore.Model
{
    public enum AuthResultType
    {
        Fault,
        UserNotFound,
        PasswordIsWrong,
        NotConfirmed,
        //RefreshTokenExpired,
        RefreshTokenWrong,
        Success
    }
}
