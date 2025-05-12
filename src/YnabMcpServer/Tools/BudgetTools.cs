using Microsoft.Extensions.Logging;
using ModelContextProtocol.Contracts;
using System.Text.Json;
using YnabMcpServer.Generated.YnabApi;
using YnabMcpServer.Models.MCP;
using YnabMcpServer.Services;

namespace YnabMcpServer.Tools;

/// <summary>
/// MCP tools for YNAB budget management.
/// </summary>
public class BudgetTools
{
    private readonly ILogger<BudgetTools> _logger;
    private readonly YnabApiClientService _ynabClientService;

    public BudgetTools(ILogger<BudgetTools> logger, YnabApiClientService ynabClientService)
    {
        _logger = logger;
        _ynabClientService = ynabClientService;
    }

    /// <summary>
    /// Gets the list of budgets.
    /// </summary>
    /// <param name="request">The request parameters.</param>
    /// <returns>A list of budgets.</returns>
    public async Task<ToolResponse> GetBudgetsAsync(GetBudgetsRequest request)
    {
        try
        {
            _logger.LogInformation("Getting budgets with includeAccounts={IncludeAccounts}", request.IncludeAccounts);

            var client = _ynabClientService.CreateAuthenticatedClient();
            var response = await client.GetBudgetsAsync(request.IncludeAccounts);

            return ToolResponses.CreateJsonResponse(response.Data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting budgets");
            return ToolResponses.CreateErrorResponse(ex.Message);
        }
    }

    /// <summary>
    /// Gets the details of a specific budget.
    /// </summary>
    /// <param name="request">The request parameters.</param>
    /// <returns>The budget details.</returns>
    public async Task<ToolResponse> GetBudgetDetailsAsync(GetBudgetDetailsRequest request)
    {
        try
        {
            _logger.LogInformation("Getting budget details for budgetId={BudgetId}", request.BudgetId);

            var client = _ynabClientService.CreateAuthenticatedClient();
            var response = await client.GetBudgetByIdAsync(request.BudgetId, null);

            return ToolResponses.CreateJsonResponse(response.Data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting budget details for budgetId={BudgetId}", request.BudgetId);
            return ToolResponses.CreateErrorResponse(ex.Message);
        }
    }

    /// <summary>
    /// Gets the settings of a specific budget.
    /// </summary>
    /// <param name="request">The request parameters.</param>
    /// <returns>The budget settings.</returns>
    public async Task<ToolResponse> GetBudgetSettingsAsync(GetBudgetSettingsRequest request)
    {
        try
        {
            _logger.LogInformation("Getting budget settings for budgetId={BudgetId}", request.BudgetId);

            var client = _ynabClientService.CreateAuthenticatedClient();
            var response = await client.GetBudgetSettingsByIdAsync(request.BudgetId);

            return ToolResponses.CreateJsonResponse(response.Data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting budget settings for budgetId={BudgetId}", request.BudgetId);
            return ToolResponses.CreateErrorResponse(ex.Message);
        }
    }
}
