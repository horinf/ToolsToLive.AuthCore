namespace ToolsToLive.AuthCore.Interfaces.IdentityServices
{
    public interface ICodeGenerator
    {
        string GenerateCode(int length = 32);
    }
}
