using Microsoft.Extensions.Logging;
using ModelContextProtocol.Transport;
using YnabMcpServer.Transport;

namespace YnabMcpServer.Factories;

/// <summary>
/// Factory for creating MCP server transports
/// </summary>
public class TransportFactory
{
    private readonly ILogger<TransportFactory> _logger;

    public TransportFactory(ILogger<TransportFactory> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Creates a server transport based on the specified type
    /// </summary>
    public IServerTransport CreateTransport(string transportType)
    {
        _logger.LogInformation($"Creating {transportType} transport");

        return transportType.ToLowerInvariant() switch
        {
            "stdio" => new StdioServerTransport(),
            _ => throw new ArgumentException($"Unsupported transport type: {transportType}")
        };
    }
}
