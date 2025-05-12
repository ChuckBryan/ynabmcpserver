using ModelContextProtocol.Contracts;

namespace YnabMcpServer.Models.MCP;

/// <summary>
/// Helper class for creating MCP tool responses.
/// </summary>
public static class ToolResponses
{
    /// <summary>
    /// Creates a successful text response.
    /// </summary>
    /// <param name="text">The text content.</param>
    /// <returns>A tool response with text content.</returns>
    public static ToolResponse CreateTextResponse(string text)
    {
        return new ToolResponse
        {
            Content = new List<ToolResponseContent>
            {
                new ToolResponseContent
                {
                    Type = "text",
                    Text = text
                }
            }
        };
    }

    /// <summary>
    /// Creates a successful JSON response.
    /// </summary>
    /// <param name="data">The data to serialize as JSON.</param>
    /// <returns>A tool response with JSON content.</returns>
    public static ToolResponse CreateJsonResponse(object data)
    {
        return new ToolResponse
        {
            Content = new List<ToolResponseContent>
            {
                new ToolResponseContent
                {
                    Type = "text",
                    Text = System.Text.Json.JsonSerializer.Serialize(data, new System.Text.Json.JsonSerializerOptions
                    {
                        WriteIndented = true
                    })
                }
            }
        };
    }

    /// <summary>
    /// Creates an error response.
    /// </summary>
    /// <param name="errorMessage">The error message.</param>
    /// <returns>A tool response with error content.</returns>
    public static ToolResponse CreateErrorResponse(string errorMessage)
    {
        return new ToolResponse
        {
            Content = new List<ToolResponseContent>
            {
                new ToolResponseContent
                {
                    Type = "text",
                    Text = $"Error: {errorMessage}"
                }
            }
        };
    }
}
