using ModelContextProtocol.Server;
using System.ComponentModel;
using YnabMcpServer.Generated.YnabApi;

namespace YnabMcpServer.Tools;

[McpServerToolType]
public static class YnabTools
{
    // This is a placeholder for YNAB tools
    // We will implement actual tools after generating the client code    [McpServerTool, Description("Get user information from YNAB")]
    public static async Task<string> GetUserInfo(
        IClient client)
    {
        try
        {
            // Attempt to fetch user information from the YNAB API
            var userResponse = await client.GetUserAsync();
            return $"User ID: {userResponse.Data.User.Id}\n" +
                   $"Username: {userResponse.Data.User.Id}";
        }
        catch (Exception ex)
        {
            // In case of an error, return a friendly message
            return $"Unable to retrieve user information: {ex.Message}";
        }
    }
}
