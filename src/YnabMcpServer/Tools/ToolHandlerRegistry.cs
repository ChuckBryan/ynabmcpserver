using Microsoft.Extensions.Logging;
using ModelContextProtocol.Contracts;
using ModelContextProtocol.Server;
using System.Text.Json;
using YnabMcpServer.Factories;
using YnabMcpServer.Models.MCP;

namespace YnabMcpServer.Tools;

/// <summary>
/// Manages tool definitions and handler registration for the MCP server
/// </summary>
public class ToolHandlerRegistry
{
    private readonly ILogger<ToolHandlerRegistry> _logger;
    private readonly BudgetTools _budgetTools;
    private readonly AccountTools _accountTools;
    private readonly TransactionTools _transactionTools;
    private readonly CategoryTools _categoryTools;

    public ToolHandlerRegistry(
        ILogger<ToolHandlerRegistry> logger,
        BudgetTools budgetTools,
        AccountTools accountTools,
        TransactionTools transactionTools,
        CategoryTools categoryTools)
    {
        _logger = logger;
        _budgetTools = budgetTools;
        _accountTools = accountTools;
        _transactionTools = transactionTools;
        _categoryTools = categoryTools;
    }

    /// <summary>
    /// Creates all tool definitions for the MCP server
    /// </summary>
    public Dictionary<string, ToolDefinition> CreateToolDefinitions()
    {
        _logger.LogInformation("Creating tool definitions");

        return new Dictionary<string, ToolDefinition>
        {
            // Budget Tools
            {
                "get-budgets",
                ToolDefinitionFactory.CreateBudgetTool(
                    description: "List all budgets with summary information",
                    requiresBudgetId: false,
                    additionalProperties: new {
                        includeAccounts = new { type = "boolean", description = "Whether to include the list of accounts" }
                    })
            },
            {
                "get-budget-details",
                ToolDefinitionFactory.CreateBudgetTool(
                    description: "Get detailed information about a specific budget")
            },
            {
                "get-budget-settings",
                ToolDefinitionFactory.CreateBudgetTool(
                    description: "Get settings for a specific budget")
            },
            
            // Account Tools
            {
                "get-accounts",
                ToolDefinitionFactory.CreateBudgetTool(
                    description: "List all accounts for a budget")
            },
            {
                "get-account-details",
                ToolDefinitionFactory.CreateEntityTool(
                    description: "Get detailed information about a specific account",
                    entityName: "account",
                    entityIdDescription: "The ID of the account."
                )
            },
            
            // Transaction Tools
            {
                "get-transactions",
                ToolDefinitionFactory.CreateBudgetTool(
                    description: "List transactions for a budget with optional filtering",
                    additionalProperties: new
                    {
                        sinceDate = new
                        {
                            type = "string",
                            format = "date",
                            description = "If specified, only transactions on or after this date will be included (in ISO format YYYY-MM-DD)."
                        },
                        type = new
                        {
                            type = "string",
                            enum = new[] { "uncategorized", "unapproved" },
                            description = "If specified, only transactions of the specified type will be included."
                        }
                    }
                )
            },
            
            // Category Tools
            {
    "get-categories", 
                ToolDefinitionFactory.CreateBudgetTool(
                    description: "List all categories for a budget grouped by category group")
            },
            {
    "get-category-details",
                ToolDefinitionFactory.CreateEntityTool(
                    description: "Get detailed information about a specific category",
                    entityName: "category",
                    entityIdDescription: "The ID of the category."
                )
            }
        };
    }
    
    /// <summary>
    /// Registers all tool handlers with the MCP server
    /// </summary>
    public void RegisterAllHandlers(McpServer server)
{
    _logger.LogInformation("Registering tool handlers");

    // Register Budget Tools
    RegisterGenericHandler<GetBudgetsRequest>(server, "get-budgets", _budgetTools.GetBudgetsAsync);
    RegisterGenericHandler<GetBudgetDetailsRequest>(server, "get-budget-details", _budgetTools.GetBudgetDetailsAsync);
    RegisterGenericHandler<GetBudgetSettingsRequest>(server, "get-budget-settings", _budgetTools.GetBudgetSettingsAsync);

    // Register Account Tools
    RegisterGenericHandler<GetAccountsRequest>(server, "get-accounts", _accountTools.GetAccountsAsync);
    RegisterGenericHandler<GetAccountDetailsRequest>(server, "get-account-details", _accountTools.GetAccountDetailsAsync);

    // Register Transaction Tools
    RegisterGenericHandler<GetTransactionsRequest>(server, "get-transactions", _transactionTools.GetTransactionsAsync);

    // Register Category Tools
    RegisterGenericHandler<GetCategoriesRequest>(server, "get-categories", _categoryTools.GetCategoriesAsync);
    RegisterGenericHandler<GetCategoryDetailsRequest>(server, "get-category-details", _categoryTools.GetCategoryDetailsAsync);

    _logger.LogInformation("Tool handlers registered successfully");
}

/// <summary>
/// Registers a generic handler for a tool
/// </summary>
private void RegisterGenericHandler<TRequest>(
    McpServer server,
    string toolName,
    Func<TRequest, Task<ToolResponse>> handlerMethod)
    where TRequest : new()
{
    server.RegisterToolHandler(toolName, async (toolRequest, cancellationToken) =>
    {
        var request = JsonSerializer.Deserialize<TRequest>(toolRequest.Parameters) ?? new TRequest();
        return await handlerMethod(request);
    });
}
}
