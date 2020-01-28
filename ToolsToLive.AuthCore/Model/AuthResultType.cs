﻿namespace ToolsToLive.AuthCore.Model
{
    public enum AuthResultType
    {
        Fault,
        UserNotFound,
        PasswordIsWrong,
        RefreshTokenExpired,
        Success
    }
}
