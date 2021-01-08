namespace ToolsToLive.AuthCore.Model
{
    public enum AuthResultType
    {
        Fault,
        UserNotFound,
        PasswordWrong,
        UserNotConfirmed,
        //RefreshTokenExpired,
        RefreshTokenWrong,
        Success
    }
}
