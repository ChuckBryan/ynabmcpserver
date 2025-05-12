using Microsoft.Extensions.Logging;
using ModelContextProtocol.Contracts;
using YnabMcpServer.Generated.YnabApi;
using YnabMcpServer.Models.MCP;
using YnabMcpServer.Services;

namespace YnabMcpServer.Tools;

/// <summary>
/// MCP tools for YNAB account management.
/// </summary>
public class AccountTools
{
    private readonly ILogger<AccountTools> _logger;
    private readonly YnabApiClientService _ynabClientService;

    public AccountTools(ILogger<AccountTools> logger, YnabApiClientService ynabClientService)
    {
        _logger = logger;
        _ynabClientService = ynabClientService;
    }

    /// <summary>
    /// Gets a list of all accounts for a budget.
    /// </summary>
    /// <param name="request">The request parameters.</param>
    /// <returns>A list of accounts.</returns>
    public async Task<ToolResponse> GetAccountsAsync(GetAccountsRequest request)
    {
        try
        {
            _logger.LogInformation("Getting accounts for budgetId={BudgetId}", request.BudgetId);
            
            var client = _ynabClientService.CreateAuthenticatedClient();
            var response = await client.GetAccountsAsync(request.BudgetId, null);
            
            return ToolResponses.CreateJsonResponse(response.Data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting accounts for budgetId={BudgetId}", request.BudgetId);
            return ToolResponses.CreateErrorResponse(ex.Message);
        }
    }

    /// <summary>
    /// Gets details of a specific account.
    /// </summary>
    /// <param name="request">The request parameters.</param>
    /// <returns>The account details.</returns>
    public async Task<ToolResponse> GetAccountDetailsAsync(GetAccountDetailsRequest request)
    {
        try
        {
            _logger.LogInformation("Getting account details for budgetId={BudgetId}, accountId={AccountId}", 
                request.BudgetId, request.AccountId);
            
            var client = _ynabClientService.CreateAuthenticatedClient();
            var response = await client.GetAccountByIdAsync(request.BudgetId, Guid.Parse(request.AccountId));
            
            return ToolResponses.CreateJsonResponse(response.Data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting account details for budgetId={BudgetId}, accountId={AccountId}", 
                request.BudgetId, request.AccountId);
            return ToolResponses.CreateErrorResponse(ex.Message);
        }
    }
}
