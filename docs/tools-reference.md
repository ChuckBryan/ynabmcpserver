# YNAB MCP Tools Reference

This document provides detailed information about all the MCP tools implemented in the YNAB MCP Server. These tools allow AI assistants to interact with your YNAB data in various ways.

## User Information

### GetUserInfo

Gets basic information about the authenticated YNAB user.

**Parameters:** None

**Returns:** User ID from YNAB

**Example Usage:**

```
Please get my YNAB user information.
```

## Budget Tools

### ListBudgets

Lists all budgets in the user's YNAB account with basic details.

**Parameters:** None

**Returns:** List of budgets with their IDs, names, and modification dates

**Example Usage:**

```
Show me all my YNAB budgets.
```

### GetBudgetDetails

Fetches detailed information about a specific budget.

**Parameters:**

- `budgetId` (string): The ID of the budget to retrieve. Use 'last-used' for the most recently used budget.

**Returns:** Detailed budget information including currency format, date format, accounts summary, etc.

**Example Usage:**

```
Get details for my most recently used budget.
```

### GetBudgetMonths

Retrieves a list of all months in a specified budget with their summary information.

**Parameters:**

- `budgetId` (string): The ID of the budget. Use 'last-used' for the most recently used budget.

**Returns:** List of budget months with income, expense, and category data

**Example Usage:**

```
Show me all the months in my current budget.
```

### GetCurrentMonthSnapshot

Provides a snapshot of the current month's budget status.

**Parameters:**

- `budgetId` (string): The ID of the budget. Use 'last-used' for the most recently used budget.

**Returns:** Current month budget information with category spending, available amounts, and overall status

**Example Usage:**

```
What's the status of my budget for this month?
```

## Category Tools

### ListCategories

Lists all categories in a budget, grouped by category groups.

**Parameters:**

- `budgetId` (string): The ID of the budget. Use 'last-used' for the most recently used budget.

**Returns:** All categories organized by category groups

**Example Usage:**

```
List all categories in my budget.
```

### GetCategoryDetails

Gets detailed information about a specific category.

**Parameters:**

- `budgetId` (string): The ID of the budget. Use 'last-used' for the most recently used budget.
- `categoryId` (string): The ID of the category.

**Returns:** Detailed information about the specified category including budgeted amount, activity, and balance

**Example Usage:**

```
Tell me about my Groceries category.
```

## Transaction Tools

### SearchTransactions

Searches for transactions with various filters.

**Parameters:**

- `budgetId` (string): The ID of the budget. Use 'last-used' for the most recently used budget.
- `searchTerm` (string, optional): Search term to filter by (payee, category, memo).
- `sinceDate` (string, optional): Date to filter transactions on or after this date (ISO format: YYYY-MM-DD).
- `type` (string, optional): Type to filter by ('uncategorized' or 'unapproved').
- `limit` (integer, optional): Maximum number of transactions to return (default: 20).

**Returns:** List of transactions matching the specified criteria

**Example Usage:**

```
Find all transactions with Starbucks in the last month.
```

### ListAccountTransactions

Lists transactions for a specific account.

**Parameters:**

- `budgetId` (string): The ID of the budget. Use 'last-used' for the most recently used budget.
- `accountId` (string): The ID of the account.
- `sinceDate` (string, optional): Date to filter transactions on or after this date (ISO format: YYYY-MM-DD).

**Returns:** List of transactions for the specified account

**Example Usage:**

```
Show me all transactions in my checking account for the past week.
```

## Account Tools

### ListAccounts

Lists all accounts in a budget.

**Parameters:**

- `budgetId` (string): The ID of the budget. Use 'last-used' for the most recently used budget.

**Returns:** All accounts in the budget with their balances and status

**Example Usage:**

```
List all accounts in my budget.
```

## Payee Tools

### ListPayees

Lists all payees in a budget.

**Parameters:**

- `budgetId` (string): The ID of the budget. Use 'last-used' for the most recently used budget.

**Returns:** All payees in the budget

**Example Usage:**

```
Show me all payees in my budget.
```

## Analysis Tools

### GetRecentActivitySummary

Provides a summary of recent financial activity.

**Parameters:**

- `budgetId` (string): The ID of the budget. Use 'last-used' for the most recently used budget.
- `days` (integer, optional): Number of days to include in the summary (default: 30).

**Returns:** Summary of recent transactions, spending by category, and account changes

**Example Usage:**

```
What has my financial activity been like in the last 30 days?
```

### GetIncomeVsExpenseSummary

Compares income to expenses over a specified period.

**Parameters:**

- `budgetId` (string): The ID of the budget. Use 'last-used' for the most recently used budget.
- `months` (integer, optional): Number of months to include in the summary (default: 3).

**Returns:** Monthly comparison of income versus expenses with trends and totals

**Example Usage:**

```
How does my income compare to my expenses over the last 3 months?
```

## Best Practices for Using These Tools

When working with the YNAB MCP Server through an AI assistant, consider these tips:

1. **Start with overview tools** like `ListBudgets` and `GetCurrentMonthSnapshot` to get a general understanding of the data.

2. **Use specific IDs** when possible. If you don't know the IDs, use tools like `ListCategories` or `ListAccounts` first.

3. **Limit date ranges** for large queries. If you're looking for transactions over a long period, consider narrowing the time frame.

4. **Combine tools** for deeper analysis. For example, use `GetIncomeVsExpenseSummary` followed by `SearchTransactions` to investigate specific spending patterns.

5. **Ask for summaries** rather than raw data when possible, especially for large datasets.

## Security Considerations

- The MCP server runs locally on your machine
- Your YNAB API token is never shared with external services
- AI assistants can only access your data when the MCP server is running
- Consider terminating the server when you're done using it
