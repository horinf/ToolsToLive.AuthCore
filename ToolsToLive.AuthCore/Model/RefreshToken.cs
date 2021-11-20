using ToolsToLive.AuthCore.Interfaces.Model;

namespace ToolsToLive.AuthCore.Model
{
    public class RefreshToken : AuthToken, IRefreshToken
    {
        public string PreviousToken { get; set; }
    }
}
