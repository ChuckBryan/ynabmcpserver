using Microsoft.Extensions.DependencyInjection;
using YnabMcpServer.Factories;
using YnabMcpServer.Services;
using YnabMcpServer.Tools;

namespace YnabMcpServer.Extensions;

/// <summary>
/// Extension methods for service collection to register MCP server components
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds all required services for the YNAB MCP server
    /// </summary>
    public static IServiceCollection AddYnabMcpServer(this IServiceCollection services)
    {
        return services
            .AddYnabApiServices()
            .AddYnabTools()
            .AddMcpInfrastructure();
    }

    /// <summary>
    /// Adds YNAB API-related services
    /// </summary>
    public static IServiceCollection AddYnabApiServices(this IServiceCollection services)
    {
        services.AddHttpClient();
        services.AddSingleton<YnabApiClientService>();
        services.AddTransient<CodeGenerator>();

        return services;
    }

    /// <summary>
    /// Adds YNAB tool implementations
    /// </summary>
    public static IServiceCollection AddYnabTools(this IServiceCollection services)
    {
        services.AddSingleton<BudgetTools>();
        services.AddSingleton<AccountTools>();
        services.AddSingleton<TransactionTools>();
        services.AddSingleton<CategoryTools>();
        services.AddSingleton<ToolHandlerRegistry>();

        return services;
    }

    /// <summary>
    /// Adds MCP infrastructure components
    /// </summary>
    public static IServiceCollection AddMcpInfrastructure(this IServiceCollection services)
    {
        services.AddSingleton<TransportFactory>();
        services.AddSingleton<ServerManager>();

        return services;
    }
}
