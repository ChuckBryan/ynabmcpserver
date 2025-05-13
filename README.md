# YNAB MCP Server

A Model Context Protocol (MCP) server for integrating YNAB (You Need A Budget) with AI systems. This project allows AI assistants to securely access and analyze your YNAB financial data.

[![Build](https://github.com/ChuckBryan/ynabmcpserver/actions/workflows/build.yml/badge.svg)](https://github.com/ChuckBryan/ynabmcpserver/actions/workflows/build.yml)
[![Docker Image](https://img.shields.io/docker/v/swampyfox/ynabmcp?label=docker&sort=semver&style=flat-square)](https://hub.docker.com/r/swampyfox/ynabmcp)
[![Platform](https://img.shields.io/badge/platform-linux%2Famd64%20%7C%20linux%2Farm64-lightgrey?style=flat-square)](https://hub.docker.com/r/swampyfox/ynabmcp/tags)

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

- [YNAB Account](https://www.youneedabudget.com/) with API access
- An MCP-compatible client (e.g., Claude for Desktop)
- Either:
  - [Docker](https://www.docker.com/products/docker-desktop/) (for Docker installation)
  - [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0) (for local installation)

### Installation

Click one of these buttons to automatically install the YNAB MCP Server in VS Code:

[![Install in VS Code](https://img.shields.io/badge/Install%20in-VS%20Code-007ACC?style=flat-square&logo=visualstudiocode)](vscode://ms-vscode.mcp/install?url=https://raw.githubusercontent.com/ChuckBryan/ynabmcpserver/main/mcp-manifest.json)
[![Install in VS Code Insiders](https://img.shields.io/badge/Install%20in-VS%20Code%20Insiders-3EA055?style=flat-square&logo=visualstudiocode)](vscode-insiders://ms-vscode.mcp/install?url=https://raw.githubusercontent.com/ChuckBryan/ynabmcpserver/main/mcp-manifest.json)

This will:

1. Configure VS Code to use the YNAB MCP Server Docker image
2. Prompt you for your YNAB API token when needed
3. Allow you to start the server with F5 or the Run menu

Once installed, you can start the server by pressing F5 in VS Code or using the Run menu. When prompted, enter your YNAB API token and the server will start automatically in a Docker container.

### Available Environment Variables

- `YNAB_API_TOKEN`: Your YNAB API token (required)

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
- [Versioning and Release Process](./VERSIONING.md)
