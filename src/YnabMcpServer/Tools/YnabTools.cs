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
            {
                result.AppendLine($"• {budget.Name}");
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

            var result = new StringBuilder();
            result.AppendLine($"Budget: {budget.Name}");
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

    [McpServerTool, Description("List all categories in a budget")]
    public static async Task<string> ListCategories(
        IClient client,
        [Description("The ID of the budget. Use 'last-used' for the most recently used budget.")] string budgetId)
    {
        try
        {
            var response = await client.GetCategoriesAsync(budgetId, null);
            var categoryGroups = response.Data.Category_groups;

            if (categoryGroups == null || categoryGroups.Count == 0)
                return "No categories found in this budget.";

            var result = new StringBuilder($"Categories in Budget:\n\n");

            foreach (var group in categoryGroups)
            {
                result.AppendLine($"## {group.Name}");

                if (group.Categories != null && group.Categories.Count > 0)
                {
                    foreach (var category in group.Categories)
                    {
                        var balance = category.Balance / 1000.0m; // Convert milliunits to actual currency
                        var budgeted = category.Budgeted / 1000.0m;
                        var activity = category.Activity / 1000.0m;

                        result.AppendLine($"• {category.Name}");
                        result.AppendLine($"  ID: {category.Id}");
                        result.AppendLine($"  Budgeted: {budgeted:C}");
                        result.AppendLine($"  Activity: {activity:C}");
                        result.AppendLine($"  Balance: {balance:C}");
                        result.AppendLine();
                    }
                }
                else
                {
                    result.AppendLine("  No categories in this group.\n");
                }
            }

            return result.ToString();
        }
        catch (Exception ex)
        {
            return $"Error retrieving categories: {ex.Message}";
        }
    }

    [McpServerTool, Description("Get details for a specific category")]
    public static async Task<string> GetCategoryDetails(
        IClient client,
        [Description("The ID of the budget. Use 'last-used' for the most recently used budget.")] string budgetId,
        [Description("The ID of the category.")] string categoryId)
    {
        try
        {
            var response = await client.GetCategoryByIdAsync(budgetId, categoryId);
            var category = response.Data.Category;

            var result = new StringBuilder();

            var balance = category.Balance / 1000.0m;
            var budgeted = category.Budgeted / 1000.0m;
            var activity = category.Activity / 1000.0m;

            result.AppendLine($"Category: {category.Name}");
            result.AppendLine($"Group: {category.Category_group_name}");
            result.AppendLine($"Budgeted: {budgeted:C}");
            result.AppendLine($"Activity: {activity:C}");
            result.AppendLine($"Balance: {balance:C}");

            if (!string.IsNullOrEmpty(category.Note))
            {
                result.AppendLine($"Note: {category.Note}");
            }

            return result.ToString();
        }
        catch (Exception ex)
        {
            return $"Error retrieving category details: {ex.Message}";
        }
    }
    [McpServerTool, Description("List account transactions")]
    public static async Task<string> ListAccountTransactions(
        IClient client,
        [Description("The ID of the budget. Use 'last-used' for the most recently used budget.")] string budgetId,
        [Description("The ID of the account.")] string accountId,
        [Description("Optional date to filter transactions on or after this date (ISO format: YYYY-MM-DD).")] string? sinceDate = null)
    {
        try
        {
            DateTimeOffset? sinceDateParsed = null;
            if (!string.IsNullOrEmpty(sinceDate))
            {
                if (DateTimeOffset.TryParse(sinceDate, out var parsedDate))
                {
                    sinceDateParsed = parsedDate;
                }
                else
                {
                    return $"Error: Invalid date format. Please use ISO format (YYYY-MM-DD).";
                }
            }

            var response = await client.GetTransactionsByAccountAsync(budgetId, accountId, sinceDateParsed, null, null);
            var transactions = response.Data.Transactions;

            if (transactions == null || transactions.Count == 0)
                return "No transactions found for this account.";

            var result = new StringBuilder($"Transactions for Account:\n\n");

            // Sort transactions by date (newest first)
            var sortedTransactions = transactions.OrderByDescending(t => t.Date).ToList();

            foreach (var transaction in sortedTransactions)
            {
                var amount = transaction.Amount / 1000.0m; // Convert milliunits to actual currency
                string payeeName = transaction.Payee_name ?? "Transfer";
                string categoryName = transaction.Category_name ?? "Uncategorized";

                result.AppendLine($"• {transaction.Date:d} - {payeeName}");
                result.AppendLine($"  Amount: {amount:C}");
                result.AppendLine($"  Category: {categoryName}");
                if (!string.IsNullOrEmpty(transaction.Memo))
                {
                    result.AppendLine($"  Memo: {transaction.Memo}");
                }
                result.AppendLine($"  Cleared: {transaction.Cleared}");
                result.AppendLine();
            }

            return result.ToString();
        }
        catch (Exception ex)
        {
            return $"Error retrieving account transactions: {ex.Message}";
        }
    }
    [McpServerTool, Description("List accounts in a budget")]
    public static async Task<string> ListAccounts(
        IClient client,
        [Description("The ID of the budget. Use 'last-used' for the most recently used budget.")] string budgetId)
    {
        try
        {
            var response = await client.GetAccountsAsync(budgetId, null);
            var accounts = response.Data.Accounts;

            if (accounts == null || accounts.Count == 0)
                return "No accounts found in this budget.";

            var result = new StringBuilder($"Accounts in Budget:\n\n");

            // Group accounts by type
            var accountsByType = accounts
                .GroupBy(a => a.Type)
                .OrderBy(g => g.Key);

            foreach (var group in accountsByType)
            {
                result.AppendLine($"## {group.Key} Accounts");

                foreach (var account in group.OrderByDescending(a => a.Balance))
                {
                    var balance = account.Balance / 1000.0m; // Convert milliunits to actual currency
                    result.AppendLine($"• {account.Name}");
                    result.AppendLine($"  ID: {account.Id}");
                    result.AppendLine($"  Balance: {balance:C}");
                    result.AppendLine($"  Status: {(account.Closed ? "Closed" : "Open")}");
                    if (!string.IsNullOrEmpty(account.Note))
                    {
                        result.AppendLine($"  Note: {account.Note}");
                    }
                    result.AppendLine();
                }
            }

            return result.ToString();
        }
        catch (Exception ex)
        {
            return $"Error retrieving accounts: {ex.Message}";
        }
    }

    [McpServerTool, Description("Get budget months")]
    public static async Task<string> GetBudgetMonths(
        IClient client,
        [Description("The ID of the budget. Use 'last-used' for the most recently used budget.")] string budgetId)
    {
        try
        {
            var response = await client.GetBudgetMonthsAsync(budgetId, null);
            var months = response.Data.Months;

            if (months == null || months.Count == 0)
                return "No budget months found for this budget.";

            var result = new StringBuilder($"Budget Months:\n\n");

            // Sort months by date (newest first)
            var sortedMonths = months.OrderByDescending(m => m.Month).ToList();

            foreach (var month in sortedMonths)
            {
                var income = month.Income / 1000.0m; // Convert milliunits to actual currency
                var budgeted = month.Budgeted / 1000.0m;
                var activity = month.Activity / 1000.0m;
                var toBebudgeted = month.To_be_budgeted / 1000.0m;

                result.AppendLine($"• {month.Month:MMMM yyyy}");
                result.AppendLine($"  Income: {income:C}");
                result.AppendLine($"  Budgeted: {budgeted:C}");
                result.AppendLine($"  Activity: {activity:C}");
                result.AppendLine($"  To Be Budgeted: {toBebudgeted:C}");
                result.AppendLine();
            }

            return result.ToString();
        }
        catch (Exception ex)
        {
            return $"Error retrieving budget months: {ex.Message}";
        }
    }

    [McpServerTool, Description("Search for transactions")]
    public static async Task<string> SearchTransactions(
        IClient client,
        [Description("The ID of the budget. Use 'last-used' for the most recently used budget.")] string budgetId,
        [Description("Optional search term to filter by (payee, category, memo).")] string? searchTerm = null,
        [Description("Optional date to filter transactions on or after this date (ISO format: YYYY-MM-DD).")] string? sinceDate = null,
        [Description("Optional type to filter by ('uncategorized' or 'unapproved').")] string? type = null,
        [Description("Maximum number of transactions to return (default: 20).")] int limit = 20)
    {
        try
        {
            DateTimeOffset? sinceDateParsed = null;
            if (!string.IsNullOrEmpty(sinceDate))
            {
                if (DateTimeOffset.TryParse(sinceDate, out var parsedDate))
                {
                    sinceDateParsed = parsedDate;
                }
                else
                {
                    return $"Error: Invalid date format. Please use ISO format (YYYY-MM-DD).";
                }
            }
            Generated.YnabApi.Type? typeParsed = null;
            if (!string.IsNullOrEmpty(type))
            {
                if (type.ToLowerInvariant() == "uncategorized")
                    typeParsed = Generated.YnabApi.Type.Uncategorized;
                else if (type.ToLowerInvariant() == "unapproved")
                    typeParsed = Generated.YnabApi.Type.Unapproved;
                else
                    return $"Error: Invalid transaction type. Use 'uncategorized' or 'unapproved'.";
            }

            var response = await client.GetTransactionsAsync(budgetId, sinceDateParsed, typeParsed, null);
            var transactions = response.Data.Transactions;

            if (transactions == null || transactions.Count == 0)
                return "No transactions found.";

            // Filter transactions if search term is provided
            if (!string.IsNullOrEmpty(searchTerm))
            {
                var search = searchTerm.ToLowerInvariant();
                transactions = transactions
                    .Where(t =>
                        (t.Payee_name != null && t.Payee_name.ToLowerInvariant().Contains(search)) ||
                        (t.Category_name != null && t.Category_name.ToLowerInvariant().Contains(search)) ||
                        (t.Memo != null && t.Memo.ToLowerInvariant().Contains(search)))
                    .ToList();

                if (transactions.Count == 0)
                    return $"No transactions found matching search term: '{searchTerm}'";
            }

            // Sort transactions by date (newest first) and limit the results
            var sortedTransactions = transactions
                .OrderByDescending(t => t.Date)
                .Take(limit)
                .ToList();

            var result = new StringBuilder($"Transactions{(!string.IsNullOrEmpty(searchTerm) ? $" matching '{searchTerm}'" : "")}:\n\n");

            foreach (var transaction in sortedTransactions)
            {
                var amount = transaction.Amount / 1000.0m; // Convert milliunits to actual currency
                string payeeName = transaction.Payee_name ?? "Transfer";
                string categoryName = transaction.Category_name ?? "Uncategorized";

                result.AppendLine($"• {transaction.Date:d} - {payeeName}");
                result.AppendLine($"  Amount: {amount:C}");
                result.AppendLine($"  Category: {categoryName}");
                if (!string.IsNullOrEmpty(transaction.Memo))
                {
                    result.AppendLine($"  Memo: {transaction.Memo}");
                }
                result.AppendLine($"  Account: {transaction.Account_name}");
                result.AppendLine();
            }

            if (transactions.Count > limit)
            {
                result.AppendLine($"Showing {limit} of {transactions.Count} transactions.");
            }

            return result.ToString();
        }
        catch (Exception ex)
        {
            return $"Error searching transactions: {ex.Message}";
        }
    }

    [McpServerTool, Description("List payees in a budget")]
    public static async Task<string> ListPayees(
        IClient client,
        [Description("The ID of the budget. Use 'last-used' for the most recently used budget.")] string budgetId)
    {
        try
        {
            var response = await client.GetPayeesAsync(budgetId, null);
            var payees = response.Data.Payees;

            if (payees == null || payees.Count == 0)
                return "No payees found in this budget.";

            var result = new StringBuilder($"Payees in Budget:\n\n");

            // Sort payees alphabetically
            var sortedPayees = payees
                .OrderBy(p => p.Name)
                .ToList();

            foreach (var payee in sortedPayees)
            {
                result.AppendLine($"• {payee.Name}");
                result.AppendLine($"  ID: {payee.Id}");
                if (payee.Deleted)
                {
                    result.AppendLine($"  Status: Deleted");
                }

                result.AppendLine();
            }

            return result.ToString();
        }
        catch (Exception ex)
        {
            return $"Error retrieving payees: {ex.Message}";
        }
    }

    [McpServerTool, Description("Get recent activity summary")]
    public static async Task<string> GetRecentActivitySummary(
        IClient client,
        [Description("The ID of the budget. Use 'last-used' for the most recently used budget.")] string budgetId,
        [Description("Number of days to include in the summary (default: 30).")] int days = 30)
    {
        try
        {
            // Calculate the date for filtering transactions
            var sinceDate = DateTimeOffset.Now.AddDays(-days);

            // Get all transactions since the specified date
            var transactionsResponse = await client.GetTransactionsAsync(budgetId, sinceDate, null, null);
            var transactions = transactionsResponse.Data.Transactions;

            if (transactions == null || transactions.Count == 0)
                return $"No transactions found in the past {days} days.";

            var result = new StringBuilder($"Activity Summary for Past {days} Days:\n\n");

            // Calculate total inflow and outflow
            decimal totalInflow = 0;
            decimal totalOutflow = 0;

            foreach (var transaction in transactions)
            {
                if (transaction.Amount > 0)
                    totalInflow += transaction.Amount;
                else
                    totalOutflow += transaction.Amount;
            }

            // Convert from milliunits to actual currency
            totalInflow /= 1000.0m;
            totalOutflow /= 1000.0m;
            var netActivity = totalInflow + totalOutflow; // Outflow is already negative

            result.AppendLine($"Total Inflow: {totalInflow:C}");
            result.AppendLine($"Total Outflow: {totalOutflow:C}");
            result.AppendLine($"Net Activity: {netActivity:C}");
            result.AppendLine();

            // Get top 5 categories by spending
            var categorySpending = transactions
                .Where(t => t.Amount < 0 && t.Category_id != null)
                .GroupBy(t => new { Id = t.Category_id, Name = t.Category_name })
                .Select(g => new
                {
                    Category = g.Key.Name ?? "Uncategorized",
                    Amount = g.Sum(t => t.Amount) / -1000.0m // Make positive for display
                })
                .OrderByDescending(x => x.Amount)
                .Take(5)
                .ToList();

            if (categorySpending.Any())
            {
                result.AppendLine("Top Categories by Spending:");
                foreach (var category in categorySpending)
                {
                    result.AppendLine($"• {category.Category}: {category.Amount:C}");
                }
                result.AppendLine();
            }

            // Get transaction count summary
            int incomeCount = transactions.Count(t => t.Amount > 0);
            int expenseCount = transactions.Count(t => t.Amount < 0);

            result.AppendLine($"Transaction Count: {transactions.Count} total");
            result.AppendLine($"• Income Transactions: {incomeCount}");
            result.AppendLine($"• Expense Transactions: {expenseCount}");

            return result.ToString();
        }
        catch (Exception ex)
        {
            return $"Error retrieving activity summary: {ex.Message}";
        }
    }

    [McpServerTool, Description("Get current month budget snapshot")]
    public static async Task<string> GetCurrentMonthSnapshot(
        IClient client,
        [Description("The ID of the budget. Use 'last-used' for the most recently used budget.")] string budgetId)
    {
        try
        {
            // Get the current month in ISO format (YYYY-MM-01)
            string currentMonth = DateTime.Now.ToString("yyyy-MM-01");

            // Get budget details for the current month
            var monthResponse = await client.GetBudgetMonthAsync(budgetId, DateTimeOffset.Parse(currentMonth));
            var month = monthResponse.Data.Month;

            if (month == null)
                return "Could not retrieve current month data.";

            var result = new StringBuilder($"Budget Snapshot for {month.Month:MMMM yyyy}:\n\n");

            // Convert from milliunits to actual currency
            decimal income = month.Income / 1000.0m;
            decimal budgeted = month.Budgeted / 1000.0m;
            decimal activity = month.Activity / 1000.0m;
            decimal toBeBudgeted = month.To_be_budgeted / 1000.0m;
            decimal ageOfMoney = month.Age_of_money ?? 0;

            result.AppendLine($"Income: {income:C}");
            result.AppendLine($"Budgeted: {budgeted:C}");
            result.AppendLine($"Activity: {activity:C}");
            result.AppendLine($"To Be Budgeted: {toBeBudgeted:C}");

            if (ageOfMoney > 0)
                result.AppendLine($"Age of Money: {ageOfMoney} days");

            result.AppendLine();

            // Add category group breakdown if available
            if (month.Categories != null && month.Categories.Count > 0)
            {
                // Group categories by their group
                var categoryGroups = month.Categories
                    .GroupBy(c => c.Category_group_name)
                    .OrderBy(g => g.Key);

                result.AppendLine("Category Group Summary:");

                foreach (var group in categoryGroups)
                {
                    var groupBudgeted = group.Sum(c => c.Budgeted) / 1000.0m;
                    var groupActivity = group.Sum(c => c.Activity) / 1000.0m;

                    result.AppendLine($"• {group.Key}");
                    result.AppendLine($"  Budgeted: {groupBudgeted:C}");
                    result.AppendLine($"  Activity: {groupActivity:C}");
                    result.AppendLine();
                }
            }

            return result.ToString();
        }
        catch (Exception ex)
        {
            return $"Error retrieving current month snapshot: {ex.Message}";
        }
    }

    [McpServerTool, Description("Get income versus expense summary")]
    public static async Task<string> GetIncomeVsExpenseSummary(
        IClient client,
        [Description("The ID of the budget. Use 'last-used' for the most recently used budget.")] string budgetId,
        [Description("Number of months to include in the summary (default: 3).")] int months = 3)
    {
        try
        {
            if (months <= 0 || months > 12)
                return "Error: Months must be between 1 and 12.";

            // Get all budget months
            var monthsResponse = await client.GetBudgetMonthsAsync(budgetId, null);
            var budgetMonths = monthsResponse.Data.Months;

            if (budgetMonths == null || budgetMonths.Count == 0)
                return "No budget months found.";

            // Sort by date (newest first) and take the requested number of months
            var recentMonths = budgetMonths
                .OrderByDescending(m => m.Month)
                .Take(months)
                .OrderBy(m => m.Month) // Now sort chronologically for display
                .ToList();

            if (recentMonths.Count == 0)
                return "No recent budget months found.";

            var result = new StringBuilder($"Income vs. Expense Summary ({months} Month{(months > 1 ? "s" : "")}):\n\n");

            // Table header
            result.AppendLine("Month      | Income    | Expenses  | Net");
            result.AppendLine("-----------|-----------|-----------|----------");

            decimal totalIncome = 0;
            decimal totalExpenses = 0;

            foreach (var month in recentMonths)
            {
                decimal income = month.Income / 1000.0m;
                decimal activity = month.Activity / 1000.0m;
                decimal expenses = -activity; // Activity is negative for expenses
                decimal net = income - expenses;

                totalIncome += income;
                totalExpenses += expenses;

                result.AppendLine($"{month.Month:MMM yyyy} | {income,9:C0} | {expenses,9:C0} | {net,9:C0}");
            }

            // Add totals
            decimal totalNet = totalIncome - totalExpenses;
            result.AppendLine("-----------|-----------|-----------|----------");
            result.AppendLine($"Total      | {totalIncome,9:C0} | {totalExpenses,9:C0} | {totalNet,9:C0}");
            result.AppendLine();

            // Add monthly averages
            decimal avgIncome = totalIncome / recentMonths.Count;
            decimal avgExpenses = totalExpenses / recentMonths.Count;
            decimal avgNet = totalNet / recentMonths.Count;

            result.AppendLine($"Monthly Average Income: {avgIncome:C}");
            result.AppendLine($"Monthly Average Expenses: {avgExpenses:C}");
            result.AppendLine($"Monthly Average Net: {avgNet:C}");

            return result.ToString();
        }
        catch (Exception ex)
        {
            return $"Error retrieving income vs expense summary: {ex.Message}";
        }
    }
}
