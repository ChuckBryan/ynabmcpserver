using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ModelContextProtocol.Contracts;
using ModelContextProtocol.Server;
using YnabMcpServer.Config;
using YnabMcpServer.Factories;
using YnabMcpServer.Tools;

namespace YnabMcpServer;

/// <summary>
/// Manages the MCP server lifecycle and configuration
/// </summary>
public class ServerManager
{
    private readonly ILogger<ServerManager> _logger;
    private readonly McpServerConfig _mcpConfig;
    private readonly ToolHandlerRegistry _toolRegistry;
    private readonly TransportFactory _transportFactory;
    private McpServer? _server;

    public ServerManager(
        ILogger<ServerManager> logger,
        IOptions<McpServerConfig> mcpConfig,
        ToolHandlerRegistry toolRegistry,
        TransportFactory transportFactory)
    {
        _logger = logger;
        _mcpConfig = mcpConfig.Value;
        _toolRegistry = toolRegistry;
        _transportFactory = transportFactory;
    }

    /// <summary>
    /// Initializes the MCP server with all required components
    /// </summary>
    public async Task InitializeAsync()
    {
        _logger.LogInformation("Initializing YNAB MCP Server...");

        // Create server definition
        var serverDefinition = new ServerDefinition
        {
            Name = _mcpConfig.ServerName,
            Version = _mcpConfig.ServerVersion,
            Capabilities = new ServerCapabilities
            {
                Tools = _toolRegistry.CreateToolDefinitions(),
                Resources = new Dictionary<string, ResourceDefinition>(),
            }
        };

        // Initialize server
        _server = new McpServer(serverDefinition);

        // Register tool handlers
        _toolRegistry.RegisterAllHandlers(_server);

        // Set up transport
        var transport = _transportFactory.CreateTransport(_mcpConfig.TransportType);

        // Connect server to transport
        await _server.ConnectAsync(transport);

        _logger.LogInformation("YNAB MCP Server initialized successfully");
    }
}
}
