using ModelContextProtocol.Server;
using System.ComponentModel;
using YnabMcpServer.Generated.YnabApi;

namespace YnabMcpServer.Tools;

[McpServerToolType]
public static class YnabTools
{
    // This is a placeholder for YNAB tools
    // We will implement actual tools after generating the client code

    [McpServerTool, Description("Get user information from YNAB")]
    public static async Task<string> GetUserInfo(
        IClient client)
    {
        // This is a placeholder implementation that will be completed
        // after we generate the client code
        return "User information will be available once the client is generated.";
    }
}
