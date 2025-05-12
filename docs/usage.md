# Using YNAB MCP Server with Claude

This guide shows practical examples of using the YNAB MCP Server with Claude for Desktop to analyze your financial data.

## Setup Process

1. **Start the YNAB MCP Server**:

   ```bash
   dotnet run --project src/YnabMcpServer/YnabMcpServer.csproj
   ```

2. **Connect Claude for Desktop**:
   - Open Claude for Desktop
   - Navigate to Settings > Tools > Add Local Tool Server
   - Select the YNAB MCP Server
   - When prompted, enter your YNAB API token

## Example Questions

Here are some examples of questions you can ask Claude once the YNAB MCP Server is connected:

### Basic Budget Information

- "What budgets do I have in YNAB?"
- "Show me details about my main budget"
- "What categories are in my budget?"
- "List all the accounts in my budget"

### Spending Analysis

- "How much did I spend at restaurants last month?"
- "What's my average grocery spending over the last 3 months?"
- "Show me my top 5 expense categories this year"
- "Find all transactions over $100 in the past month"

### Account Management

- "What's my current checking account balance?"
- "How much money do I have across all accounts?"
- "Show me recent transactions from my credit card"
- "List all transfers between accounts"

### Budget Health

- "Am I over budget in any categories this month?"
- "What's my income versus expenses trend over the past 6 months?"
- "How much have I saved this year compared to last year?"
- "Show me my budget health snapshot for this month"

### Specific Queries

- "How much have I spent at Starbucks this year?"
- "Find all uncategorized transactions"
- "What's my biggest expense category?"
- "Show me all recurring monthly payments"

## Response Examples

When you ask a question, Claude will use the appropriate YNAB MCP tool to fetch and analyze your data.

### Example 1: Budget Overview

**Question**: "What's my current budget status?"

**Claude's Response**:

```
I've checked your current budget status. Here's an overview:

Budget: Family Budget
Month: August 2023

Income this month: $5,250.00
Expenses this month: $3,876.42
Net balance: +$1,373.58

Top 3 expense categories:
- Housing: $1,500.00 (85% of allocated budget)
- Groceries: $743.21 (106% of allocated budget - over by $43.21)
- Transportation: $428.50 (85% of allocated budget)

You have 2 underfunded categories:
- Groceries: $43.21 over budget
- Dining Out: $12.68 over budget

Your overall budget health looks good, though you might want to transfer some money to cover those two overspent categories.
```

### Example 2: Transaction Analysis

**Question**: "How much did I spend at Jimmy John's last year?"

**Claude's Response**:

```
I've analyzed your transactions at Jimmy John's for last year. Here's what I found:

Total spent at Jimmy John's: $378.84
Number of transactions: 42
Average transaction: $9.02

Your spending by month:
- January: $45.12 (5 transactions)
- February: $36.10 (4 transactions)
- March: $27.08 (3 transactions)
- April: $36.10 (4 transactions)
- May: $45.12 (5 transactions)
- June: $36.10 (4 transactions)
- July: $27.08 (3 transactions)
- August: $27.08 (3 transactions)
- September: $36.10 (4 transactions)
- October: $18.04 (2 transactions)
- November: $27.08 (3 transactions)
- December: $18.04 (2 transactions)

Your most frequent day to visit was Wednesday with 18 transactions.
```

## Advanced Usage

The YNAB MCP Server enables Claude to perform complex financial analysis by combining multiple tools:

- "Compare my spending in all categories between this year and last year"
- "Analyze my savings rate over time and suggest improvements"
- "Find unusual or unexpected transactions in the last month"
- "Help me identify areas where I can cut back on spending"

## Troubleshooting

If Claude isn't able to access your YNAB data:

1. Verify the MCP server is running
2. Check that your YNAB API token is valid and correctly entered
3. Ensure Claude has permission to access the MCP server
4. Try disconnecting and reconnecting the server in Claude settings
