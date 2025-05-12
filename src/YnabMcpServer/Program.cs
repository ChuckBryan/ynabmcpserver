using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using YnabMcpServer.Config;
using YnabMcpServer.Services;
using YnabMcpServer.Tools;

namespace YnabMcpServer;

public class Program
{
    public static async Task Main(string[] args)
    {
        // Set up configuration
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddEnvironmentVariables()
            .Build();

        // Set up dependency injection
        var serviceProvider = new ServiceCollection()
            .AddLogging(configure =>
            {
                configure.AddConsole();
                configure.SetMinimumLevel(LogLevel.Information);
            })
            .Configure<YnabConfig>(configuration.GetSection("YnabConfig"))
            .Configure<McpServerConfig>(configuration.GetSection("McpServerConfig"))
            
            // Register services
            .AddTransient<CodeGenerator>()
            .AddHttpClient()
            .AddSingleton<YnabApiClientService>()
            
            // Register tools
            .AddSingleton<BudgetTools>()
            .AddSingleton<AccountTools>()
            .AddSingleton<TransactionTools>()
            .AddSingleton<CategoryTools>()
            
            // Register server
            .AddSingleton<YnabMcpServer>()
            .BuildServiceProvider();

        // Get logger
        var logger = serviceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogInformation("Starting YNAB MCP Server...");

        try
        {
            // Generate the YNAB API client
            var codeGenerator = serviceProvider.GetRequiredService<CodeGenerator>();
            await codeGenerator.GenerateYnabClientAsync();

            // Get server instance
            var server = serviceProvider.GetRequiredService<YnabMcpServer>();
            
            // Initialize server
            await server.InitializeAsync();
            
            // Keep application running
            logger.LogInformation("Server is running. Press Ctrl+C to exit.");
            await Task.Delay(Timeout.InfiniteTimeSpan);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while starting the server");
        }
    }
}
