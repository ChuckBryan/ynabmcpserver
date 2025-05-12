# YNAB MCP Server

A Model Context Protocol (MCP) server for integrating YNAB (You Need A Budget) with AI systems. This project allows AI assistants to securely access and analyze your YNAB financial data.

## What is MCP?

The Model Context Protocol (MCP) is a standard that allows AI assistants to interact with external tools and services. It enables AI models like Claude to:

1. **Access real-time data** beyond their training cutoff date
2. **Call functions** in external systems
3. **Interact with APIs** securely
4. **Provide contextual analysis** based on your personal data

Instead of just answering based on general knowledge, MCP allows AI to work with your specific information.

## About YNAB MCP Server

This project creates a secure bridge between your YNAB financial data and AI assistants. It allows you to:

- Query your budget information
- Analyze spending patterns
- Check account balances
- Track financial goals
- Get insights about your financial habits

All while keeping your YNAB API token secure and your financial data private.

## Features

This MCP server implements a comprehensive set of tools for interacting with the YNAB API:

### User and Budget Information

- `GetUserInfo` - Retrieve your YNAB user ID
- `ListBudgets` - Get all budgets in your YNAB account
- `GetBudgetDetails` - Detailed information about a specific budget
- `GetBudgetMonths` - List budget months

### Categories and Transactions

- `ListCategories` - All categories in a budget
- `GetCategoryDetails` - Detailed information about a category
- `SearchTransactions` - Find transactions with custom filters
- `ListAccountTransactions` - Get transactions for a specific account

### Accounts and Payees

- `ListAccounts` - Get all accounts in a budget
- `ListPayees` - Get all payees in a budget

### Financial Analysis

- `GetCurrentMonthSnapshot` - Summary of the current month's budget
- `GetRecentActivitySummary` - Summary of recent financial activity
- `GetIncomeVsExpenseSummary` - Compare income to expenses over time

## Getting Started

### Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [YNAB Account](https://www.youneedabudget.com/) with API access
- An MCP-compatible client (e.g., Claude for Desktop)

### Installation

1. Clone the repository:

   ```bash
   git clone https://github.com/yourusername/YnabMcpServer.git
   cd YnabMcpServer
   ```

2. Build the project:

   ```bash
   dotnet build
   ```

3. Configure your YNAB API token:

   You can either:

   - Set it as an environment variable: `YNAB_API_TOKEN=your-token-here`
   - Use the `.vscode/mcp.json` configuration when launching from VS Code
   - Store it in `appsettings.json` or `appsettings.Development.json` (not recommended for production)

4. Run the server:
   ```bash
   dotnet run --project src/YnabMcpServer/YnabMcpServer.csproj
   ```

### Using with Claude for Desktop

1. Open Claude for Desktop
2. Go to Settings > Tools > Add Local Tool Server
3. Select your YnabMcpServer
4. You can now ask Claude about your YNAB data!

Example queries:

- "What's my current budget status?"
- "How much did I spend on groceries last month?"
- "Show me all my transactions with Jimmy John's"
- "What's my income vs. expenses for the past 3 months?"

## Security

This MCP server runs locally on your machine and does not share your YNAB API token with third parties. The API token is only used to communicate directly with the YNAB API.

## Development

### Project Structure

```
YnabMcpServer/
├── .vscode/            # VS Code configuration
├── docs/               # Documentation
├── requests/           # HTTP request samples for testing
├── scripts/            # Utility scripts
└── src/                # Source code
    └── YnabMcpServer/  # Main project
        ├── Configuration/ # Configuration classes
        ├── Generated/     # Generated API client code
        ├── Services/      # Service implementations
        ├── Tools/         # MCP tool implementations
        └── Program.cs     # Application entry point
```

### Extending the Server

To add new MCP tools:

1. Add new methods to `YnabTools.cs`
2. Decorate them with `[McpServerTool]` and `[Description]` attributes
3. Rebuild and restart the server

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

## License

This project is licensed under the MIT License - see the LICENSE file for details.

## Acknowledgments

- [YNAB API](https://api.youneedabudget.com/)
- [Model Context Protocol](https://modelcontextprotocol.github.io/)
