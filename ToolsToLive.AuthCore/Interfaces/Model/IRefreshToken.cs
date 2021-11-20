namespace ToolsToLive.AuthCore.Interfaces.Model
{
    public interface IRefreshToken : IAuthToken
    {
        string PreviousToken { get; set; }
    }
}
