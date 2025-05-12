using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ModelContextProtocol.Contracts;
using ModelContextProtocol.Server;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using YnabMcpServer.Config;
using YnabMcpServer.Models.MCP;
using YnabMcpServer.Tools;

namespace YnabMcpServer;

public class YnabMcpServer
{
    private readonly ILogger<YnabMcpServer> _logger;
    private readonly YnabConfig _ynabConfig;
    private readonly McpServerConfig _mcpConfig;
    private readonly BudgetTools _budgetTools;
    private readonly AccountTools _accountTools;
    private readonly TransactionTools _transactionTools;
    private readonly CategoryTools _categoryTools;
    private McpServer? _server;

    public YnabMcpServer(
        ILogger<YnabMcpServer> logger,
        IOptions<YnabConfig> ynabConfig,
        IOptions<McpServerConfig> mcpConfig,
        BudgetTools budgetTools,
        AccountTools accountTools,
        TransactionTools transactionTools,
        CategoryTools categoryTools)
    {
        _logger = logger;
        _ynabConfig = ynabConfig.Value;
        _mcpConfig = mcpConfig.Value;
        _budgetTools = budgetTools;
        _accountTools = accountTools;
        _transactionTools = transactionTools;
        _categoryTools = categoryTools;
    }

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
                Tools = CreateToolDefinitions(),
                Resources = new Dictionary<string, ResourceDefinition>(),
            }
        };

        // Initialize server
        _server = new McpServer(serverDefinition);

        // Register tool handlers
        RegisterToolHandlers(_server);

        // Set up transport
        var transport = GetTransport(_mcpConfig.TransportType);

        // Connect server to transport
        await _server.ConnectAsync(transport);

        _logger.LogInformation("YNAB MCP Server initialized successfully");
    }

    private Dictionary<string, ToolDefinition> CreateToolDefinitions()
    {
        return new Dictionary<string, ToolDefinition>
        {
            // Budget Tools
            {
                "get-budgets",
                new ToolDefinition
                {
                    Description = "List all budgets with summary information",
                    InputSchema = JsonSerializer.SerializeToDocument(new
                    {
                        type = "object",
                        properties = new
                        {
                            includeAccounts = new { type = "boolean", description = "Whether to include the list of accounts" }
                        }
                    })
                }
            },
            {
                "get-budget-details",
                new ToolDefinition
                {
                    Description = "Get detailed information about a specific budget",
                    InputSchema = JsonSerializer.SerializeToDocument(new
                    {
                        type = "object",
                        properties = new
                        {
                            budgetId = new
                            {
                                type = "string",
                                description = "The ID of the budget. Use 'last-used' for the last used budget or 'default' for the default budget."
                            }
                        },
                        required = new[] { "budgetId" }
                    })
                }
            },
            {
                "get-budget-settings",
                new ToolDefinition
                {
                    Description = "Get settings for a specific budget",
                    InputSchema = JsonSerializer.SerializeToDocument(new
                    {
                        type = "object",
                        properties = new
                        {
                            budgetId = new
                            {
                                type = "string",
                                description = "The ID of the budget. Use 'last-used' for the last used budget or 'default' for the default budget."
                            }
                        },
                        required = new[] { "budgetId" }
                    })
                }
            },
            
            // Account Tools
            {
                "get-accounts",
                new ToolDefinition
                {
                    Description = "List all accounts for a budget",
                    InputSchema = JsonSerializer.SerializeToDocument(new
                    {
                        type = "object",
                        properties = new
                        {
                            budgetId = new
                            {
                                type = "string",
                                description = "The ID of the budget. Use 'last-used' for the last used budget or 'default' for the default budget."
                            }
                        },
                        required = new[] { "budgetId" }
                    })
                }
            },
            {
                "get-account-details",
                new ToolDefinition
                {
                    Description = "Get detailed information about a specific account",
                    InputSchema = JsonSerializer.SerializeToDocument(new
                    {
                        type = "object",
                        properties = new
                        {
                            budgetId = new
                            {
                                type = "string",
                                description = "The ID of the budget. Use 'last-used' for the last used budget or 'default' for the default budget."
                            },
                            accountId = new
                            {
                                type = "string",
                                description = "The ID of the account."
                            }
                        },
                        required = new[] { "budgetId", "accountId" }
                    })
                }
            },
            
            // Transaction Tools
            {
                "get-transactions",
                new ToolDefinition
                {
                    Description = "List transactions for a budget with optional filtering",
                    InputSchema = JsonSerializer.SerializeToDocument(new
                    {
                        type = "object",
                        properties = new
                        {
                            budgetId = new
                            {
                                type = "string",
                                description = "The ID of the budget. Use 'last-used' for the last used budget or 'default' for the default budget."
                            },
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
                        },
                        required = new[] { "budgetId" }
                    })
                }
            },
            
            // Category Tools
            {
    "get-categories",
                new ToolDefinition
                {
                    Description = "List all categories for a budget grouped by category group",
                    InputSchema = JsonSerializer.SerializeToDocument(new
                    {
                        type = "object",
                        properties = new
                        {
                            budgetId = new
                            {
                                type = "string",
                                description = "The ID of the budget. Use 'last-used' for the last used budget or 'default' for the default budget."
                            }
                        },
                        required = new[] { "budgetId" }
                    })
                }
            },
            {
    "get-category-details",
                new ToolDefinition
                {
                    Description = "Get detailed information about a specific category",
                    InputSchema = JsonSerializer.SerializeToDocument(new
                    {
                        type = "object",
                        properties = new
                        {
                            budgetId = new
                            {
                                type = "string",
                                description = "The ID of the budget. Use 'last-used' for the last used budget or 'default' for the default budget."
                            },
                            categoryId = new
                            {
                                type = "string",
                                description = "The ID of the category."
                            }
                        },
                        required = new[] { "budgetId", "categoryId" }
                    })
                }
            }
        };
    }
    
    private void RegisterToolHandlers(McpServer server)
{
    // Register Budget Tools
    server.RegisterToolHandler("get-budgets", async (toolRequest, cancellationToken) =>
    {
        var request = JsonSerializer.Deserialize<GetBudgetsRequest>(toolRequest.Parameters) ?? new GetBudgetsRequest();
        return await _budgetTools.GetBudgetsAsync(request);
    });

    server.RegisterToolHandler("get-budget-details", async (toolRequest, cancellationToken) =>
    {
        var request = JsonSerializer.Deserialize<GetBudgetDetailsRequest>(toolRequest.Parameters) ?? new GetBudgetDetailsRequest();
        return await _budgetTools.GetBudgetDetailsAsync(request);
    });

    server.RegisterToolHandler("get-budget-settings", async (toolRequest, cancellationToken) =>
    {
        var request = JsonSerializer.Deserialize<GetBudgetSettingsRequest>(toolRequest.Parameters) ?? new GetBudgetSettingsRequest();
        return await _budgetTools.GetBudgetSettingsAsync(request);
    });

    // Register Account Tools
    server.RegisterToolHandler("get-accounts", async (toolRequest, cancellationToken) =>
    {
        var request = JsonSerializer.Deserialize<GetAccountsRequest>(toolRequest.Parameters) ?? new GetAccountsRequest();
        return await _accountTools.GetAccountsAsync(request);
    });

    server.RegisterToolHandler("get-account-details", async (toolRequest, cancellationToken) =>
    {
        var request = JsonSerializer.Deserialize<GetAccountDetailsRequest>(toolRequest.Parameters) ?? new GetAccountDetailsRequest();
        return await _accountTools.GetAccountDetailsAsync(request);
    });

    // Register Transaction Tools
    server.RegisterToolHandler("get-transactions", async (toolRequest, cancellationToken) =>
    {
        var request = JsonSerializer.Deserialize<GetTransactionsRequest>(toolRequest.Parameters) ?? new GetTransactionsRequest();
        return await _transactionTools.GetTransactionsAsync(request);
    });

    // Register Category Tools
    server.RegisterToolHandler("get-categories", async (toolRequest, cancellationToken) =>
    {
        var request = JsonSerializer.Deserialize<GetCategoriesRequest>(toolRequest.Parameters) ?? new GetCategoriesRequest();
        return await _categoryTools.GetCategoriesAsync(request);
    });

    server.RegisterToolHandler("get-category-details", async (toolRequest, cancellationToken) =>
    {
        var request = JsonSerializer.Deserialize<GetCategoryDetailsRequest>(toolRequest.Parameters) ?? new GetCategoryDetailsRequest();
        return await _categoryTools.GetCategoryDetailsAsync(request);
    });

    _logger.LogInformation("Tool handlers registered successfully");
}

private IServerTransport GetTransport(string transportType)
{
    return transportType.ToLowerInvariant() switch
    {
        "stdio" => new StdioServerTransport(),
        _ => throw new ArgumentException($"Unsupported transport type: {transportType}")
    };
}
}
