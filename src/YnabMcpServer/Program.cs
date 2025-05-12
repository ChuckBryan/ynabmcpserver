using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using YnabMcpServer.Config;
using YnabMcpServer.Extensions;

namespace YnabMcpServer;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = Host.CreateEmptyApplicationBuilder(settings: null);

        // Add configuration
        builder.Configuration
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddEnvironmentVariables();

        // Configure services
        builder.Services.Configure<YnabConfig>(builder.Configuration.GetSection("YnabConfig"));
        builder.Services.Configure<McpServerConfig>(builder.Configuration.GetSection("McpServerConfig"));

        // Add logging
        builder.Services.AddLogging(configure =>
        {
            configure.AddConsole();
            configure.SetMinimumLevel(LogLevel.Information);
        });

        // Register all YNAB MCP server services
        builder.Services.AddYnabMcpServer();

        var app = builder.Build();

        try
        {
            // Generate the YNAB API client first
            var codeGenerator = app.Services.GetRequiredService<CodeGenerator>();
            await codeGenerator.GenerateYnabClientAsync();

            // Initialize and run the server
            var serverManager = app.Services.GetRequiredService<ServerManager>();
            await serverManager.InitializeAsync();

            // Run the host
            await app.RunAsync();
        }
        catch (Exception ex)
        {
            var logger = app.Services.GetRequiredService<ILogger<Program>>();
            logger.LogError(ex, "An error occurred while starting the server");
            throw;
        }
    }
}
