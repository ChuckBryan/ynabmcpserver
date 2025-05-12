using Microsoft.Extensions.Logging;
using ModelContextProtocol.Contracts;
using YnabMcpServer.Generated.YnabApi;
using YnabMcpServer.Models.MCP;
using YnabMcpServer.Services;

namespace YnabMcpServer.Tools;

/// <summary>
/// MCP tools for YNAB category management.
/// </summary>
public class CategoryTools
{
    private readonly ILogger<CategoryTools> _logger;
    private readonly YnabApiClientService _ynabClientService;

    public CategoryTools(ILogger<CategoryTools> logger, YnabApiClientService ynabClientService)
    {
        _logger = logger;
        _ynabClientService = ynabClientService;
    }

    /// <summary>
    /// Gets a list of all categories for a budget.
    /// </summary>
    /// <param name="request">The request parameters.</param>
    /// <returns>A list of categories organized by category groups.</returns>
    public async Task<ToolResponse> GetCategoriesAsync(GetCategoriesRequest request)
    {
        try
        {
            _logger.LogInformation("Getting categories for budgetId={BudgetId}", request.BudgetId);

            var client = _ynabClientService.CreateAuthenticatedClient();
            var response = await client.GetCategoriesAsync(request.BudgetId, null);

            return ToolResponses.CreateJsonResponse(response.Data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting categories for budgetId={BudgetId}", request.BudgetId);
            return ToolResponses.CreateErrorResponse(ex.Message);
        }
    }

    /// <summary>
    /// Gets details of a specific category.
    /// </summary>
    /// <param name="request">The request parameters.</param>
    /// <returns>The category details.</returns>
    public async Task<ToolResponse> GetCategoryDetailsAsync(GetCategoryDetailsRequest request)
    {
        try
        {
            _logger.LogInformation("Getting category details for budgetId={BudgetId}, categoryId={CategoryId}",
                request.BudgetId, request.CategoryId);

            var client = _ynabClientService.CreateAuthenticatedClient();
            var response = await client.GetCategoryByIdAsync(request.BudgetId, request.CategoryId);

            return ToolResponses.CreateJsonResponse(response.Data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting category details for budgetId={BudgetId}, categoryId={CategoryId}",
                request.BudgetId, request.CategoryId);
            return ToolResponses.CreateErrorResponse(ex.Message);
        }
    }
}
