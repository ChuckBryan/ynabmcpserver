using Microsoft.Extensions.Logging;
using ModelContextProtocol.Contracts;
using System.Text.Json;
using YnabMcpServer.Generated.YnabApi;
using YnabMcpServer.Models.MCP;
using YnabMcpServer.Services;

namespace YnabMcpServer.Tools;

/// <summary>
/// MCP tools for YNAB transaction management.
/// </summary>
public class TransactionTools
{
    private readonly ILogger<TransactionTools> _logger;
    private readonly YnabApiClientService _ynabClientService;

    public TransactionTools(ILogger<TransactionTools> logger, YnabApiClientService ynabClientService)
    {
        _logger = logger;
        _ynabClientService = ynabClientService;
    }

    /// <summary>
    /// Gets a list of transactions for a budget.
    /// </summary>
    /// <param name="request">The request parameters.</param>
    /// <returns>A list of transactions.</returns>
    public async Task<ToolResponse> GetTransactionsAsync(GetTransactionsRequest request)
    {
        try
        {
            _logger.LogInformation("Getting transactions for budgetId={BudgetId}, sinceDate={SinceDate}, type={Type}",
                request.BudgetId, request.SinceDate, request.Type);
            
            var client = _ynabClientService.CreateAuthenticatedClient();
            var response = await client.GetTransactionsAsync(
                request.BudgetId, 
                request.SinceDate?.ToString("yyyy-MM-dd"), 
                request.Type,
                null);
            
            return ToolResponses.CreateJsonResponse(response.Data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting transactions for budgetId={BudgetId}", request.BudgetId);
            return ToolResponses.CreateErrorResponse(ex.Message);
        }
    }

    /// <summary>
    /// Creates a new transaction.
    /// </summary>
    /// <param name="request">The request parameters.</param>
    /// <returns>The created transaction.</returns>
    public async Task<ToolResponse> CreateTransactionAsync(CreateTransactionRequest request)
    {
        try
        {
            _logger.LogInformation("Creating transaction for budgetId={BudgetId}", request.BudgetId);
            
            var client = _ynabClientService.CreateAuthenticatedClient();
            
            // Convert the transaction dictionary to the appropriate SaveTransaction object
            // This will need to be handled based on the generated client's requirements
            
            // For now, return an error since we'll need to implement this later
            return ToolResponses.CreateErrorResponse("Transaction creation is not yet implemented");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating transaction for budgetId={BudgetId}", request.BudgetId);
            return ToolResponses.CreateErrorResponse(ex.Message);
        }
    }

    /// <summary>
    /// Updates an existing transaction.
    /// </summary>
    /// <param name="request">The request parameters.</param>
    /// <returns>The updated transaction.</returns>
    public async Task<ToolResponse> UpdateTransactionAsync(UpdateTransactionRequest request)
    {
        try
        {
            _logger.LogInformation("Updating transaction for budgetId={BudgetId}, transactionId={TransactionId}",
                request.BudgetId, request.TransactionId);
            
            var client = _ynabClientService.CreateAuthenticatedClient();
            
            // Convert the transaction dictionary to the appropriate SaveTransaction object
            // This will need to be handled based on the generated client's requirements
            
            // For now, return an error since we'll need to implement this later
            return ToolResponses.CreateErrorResponse("Transaction update is not yet implemented");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating transaction for budgetId={BudgetId}, transactionId={TransactionId}",
                request.BudgetId, request.TransactionId);
            return ToolResponses.CreateErrorResponse(ex.Message);
        }
    }
}
