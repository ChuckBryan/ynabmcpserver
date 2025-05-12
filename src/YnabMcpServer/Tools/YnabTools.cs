using ModelContextProtocol.Server;
using System.ComponentModel;
using YnabMcpServer.Generated.YnabApi;
using YnabMcpServer.Configuration;
using YnabMcpServer.Services;
using System.Text;

namespace YnabMcpServer.Tools;

[McpServerToolType]
public static class YnabTools
{
    [McpServerTool, Description("Get user information from YNAB")]
    public static async Task<string> GetUserInfo(IClient client)
    {
        try
        {
            // Attempt to fetch user information from the YNAB API
            var userResponse = await client.GetUserAsync();
            return $"User ID: {userResponse.Data.User.Id}";
        }
        catch (Exception ex)
        {
            // In case of an error, return a friendly message
            return $"Unable to retrieve user information: {ex.Message}";
        }
    }

    [McpServerTool, Description("List all budgets in your YNAB account")]
    public static async Task<string> ListBudgets(IClient client)
    {
        try
        {
            var budgetsResponse = await client.GetBudgetsAsync(true);
            var budgets = budgetsResponse.Data.Budgets;

            if (budgets.Count == 0)
                return "No budgets found in your YNAB account.";

            var result = new StringBuilder("Your YNAB Budgets:\n\n");

            foreach (var budget in budgets)
            {                result.AppendLine($"• {budget.Name}");
                result.AppendLine($"  ID: {budget.Id}");
                result.AppendLine($"  Last modified: {budget.Last_modified_on.ToLocalTime():g}");
                if (budget.Accounts != null && budget.Accounts.Count > 0)
                {
                    result.AppendLine($"  Accounts: {budget.Accounts.Count}");
                }
                result.AppendLine();
            }

            return result.ToString();
        }
        catch (Exception ex)
        {
            return $"Error retrieving budgets: {ex.Message}";
        }
    }

    [McpServerTool, Description("Get details for a specific budget")]
    public static async Task<string> GetBudgetDetails(
        IClient client,
        [Description("The ID of the budget to retrieve. Use 'last-used' for the most recently used budget.")] string budgetId)
    {
        try
        {
            var budgetResponse = await client.GetBudgetByIdAsync(budgetId, null);
            var budget = budgetResponse.Data.Budget;

            var result = new StringBuilder();            result.AppendLine($"Budget: {budget.Name}");
            result.AppendLine($"Currency: {budget.Currency_format?.Iso_code}");
            result.AppendLine($"First Month: {budget.First_month}");
            result.AppendLine($"Last Month: {budget.Last_month}");
            result.AppendLine();

            // Add account summary
            if (budget.Accounts != null && budget.Accounts.Count > 0)
            {
                result.AppendLine("Accounts:");
                foreach (var account in budget.Accounts)
                {
                    var balance = account.Balance / 1000.0m; // Convert milliunits to units
                    result.AppendLine($"• {account.Name}: {balance:C} ({(account.Closed ? "Closed" : "Open")})");
                }
                result.AppendLine();
            }

            return result.ToString();
        }
        catch (Exception ex)
        {
            return $"Error retrieving budget details: {ex.Message}";
        }
    }
}
