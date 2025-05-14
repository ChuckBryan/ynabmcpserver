# Building a YNAB MCP Server with AI and .NET 9

_Date: May 14, 2025_

AI assistants are getting smarter every day, but they're limited by what they can access. I wanted to bridge that gap for my personal finance data, so I built an MCP (Model Context Protocol) server that connects YNAB (You Need A Budget) to AI assistants like Claude. In this article, I'll walk through how I did it using AI-powered development, .NET 9, and Docker.

## What is MCP and Why Does It Matter?

The Model Context Protocol (MCP) is a standard that enables AI models to interact with external services in a structured, secure way. It's what allows AI assistants to:

- Access your real-time data (beyond their training cutoff dates)
- Call functions in your apps and services
- Work with APIs on your behalf
- Provide insights based on your personal data

For personal finance, this is game-changing. Instead of vague financial advice, your AI assistant can analyze your actual spending patterns, budget categories, and account balances to provide tailored recommendations.

## The YNAB MCP Server Project

My goal was to create a server that:

1. Securely connects to the YNAB API
2. Exposes useful financial functions through MCP
3. Keeps sensitive data and API tokens secure
4. Is easy to install and use

The resulting [YNAB MCP Server](https://github.com/ChuckBryan/ynabmcpserver) does exactly that. It provides a comprehensive set of tools for budget analysis, transaction searching, account management, and financial insights—all available directly through AI assistants that support MCP.

## Using AI to Accelerate Development

What made this project particularly interesting was using AI tools like GitHub Copilot to accelerate the development process. Here's how I approached it:

### 1. Teaching Copilot About the YNAB API

First, I downloaded the OpenAPI specification from YNAB and stored it in my project's `/docs` folder. This gave Copilot a detailed understanding of:

- All available endpoints
- Request and response formats
- Authentication requirements
- Data models and structures

With this knowledge, Copilot could suggest accurate implementations whenever I was working with the YNAB API.

### 2. Creating Custom Instructions with copilot-instructions.md

Beyond just having the API documentation in the repository, I created a dedicated `.github/copilot-instructions.md` file to provide explicit guidance to GitHub Copilot. This special file functions as a project-specific instruction set for AI coding assistance.

#### What is copilot-instructions.md?

The `copilot-instructions.md` file is a repository-level configuration file that GitHub Copilot reads to understand the context, goals, and technical requirements of a project. Unlike regular documentation that's written for humans, this file is specifically designed to "teach" Copilot about your project so it can generate more relevant and accurate code suggestions.

When placed in the `.github` directory of your repository, Copilot automatically discovers and processes these instructions, applying them to all code suggestions throughout the project.

Here's the file I created for the YNAB MCP Server:

```markdown
# GitHub Copilot Custom Instructions

This file provides custom instructions for GitHub Copilot in this repository.

## Reference

- [Model Context Protocol SDK](https://github.com/modelcontextprotocol/create-python-server)

Add any additional instructions or guidelines for Copilot usage below.

- I am building an MCP Server using .NET that will interact with YNAB's API.
- This will be the YnabMcpServer
- use the /docs/mcp.md for quickstart instructions on how to create an MCP Server using C#.
- One alteration to the mcp.md is that we should use .NET 9 and not .NET 8
- use the /docs/ynabapi.md for the details of the features of the ynab api.
- use the /docs/open_api_spec.yml to generate the http client.
- use the EnableSdkContainerSupport in the CSPROJ so that I do not need a docker file
```

#### How It Benefits Development

This approach transformed my development workflow in several ways:

- **Project-Specific Context**: Provides Copilot with clear understanding of the project's goals, architecture, and constraints
- **Documentation References**: Points Copilot to the specific files it should use as knowledge sources
- **Technology Preferences**: Specifies exact technologies and versions (like .NET 9 instead of .NET 8)
- **Architectural Decisions**: Communicates important design choices (like using SDK Container Support)
- **Consistency**: Creates uniform code suggestions that align with project standards across all development sessions
- **Reduced Cognitive Load**: I didn't need to repeatedly instruct Copilot about project details in each coding session

By explicitly telling Copilot how to interpret the documentation and what technologies to prefer, I significantly improved the quality and relevance of its suggestions. Instead of generic C# code, Copilot generated MCP-specific implementations with proper YNAB API integration. This proved to be a crucial step in streamlining the development process, essentially turning Copilot into a specialized assistant for this particular project.

### 3. Learning MCP Server Development

Next, I gathered documentation from [ModelContextProtocol.io](https://modelcontextprotocol.github.io/) on how to build an MCP server in C#. The quickstart guide was particularly helpful, showing how to:

- Structure an MCP server project
- Implement and expose tools
- Handle authentication and configuration
- Document functions for AI consumption

I saved these resources in my `/docs` folder, making them available to Copilot when I needed implementation suggestions.

### 4. The Development Process

With Copilot trained on both the YNAB API and MCP server patterns, the development process became remarkably fluid:

1. **Project scaffolding**: Copilot helped create the initial project structure and .NET 9 configuration.

2. **API client generation**: Using the OpenAPI spec, we generated a strongly-typed C# client for the YNAB API.

3. **MCP tools implementation**: For each YNAB feature I wanted to expose:

   - I described the function in natural language
   - Copilot suggested a method signature and implementation
   - I refined the code and added proper error handling

4. **Configuration and security**: We implemented a secure approach for handling the YNAB API token through environment variables.

5. **Docker packaging**: Configured the project to support SDK Container Support for easy Docker deployment.

The result was development at a pace that would have been impossible without AI assistance.

## Key Implementation Details

### Project Structure

The server follows a clean architecture:

```
YnabMcpServer/
├── Configuration/   # API configuration settings
├── Generated/       # Auto-generated API client code
├── Services/        # Core services for interacting with YNAB
├── Tools/           # MCP tool implementations
└── Program.cs       # Application entry point
```

### MCP Tools Interface

Each MCP tool is implemented as a method in `YnabTools.cs` and decorated with the `[McpServerTool]` attribute. For example:

```csharp
[McpServerTool]
[Description("Get details for a specific budget")]
public Task<BudgetDetailResponse> GetBudgetDetails(string budgetId)
{
    // Implementation that calls the YNAB API
}
```

These methods become the functions that AI assistants can call when connected to your MCP server.

### Features Implemented

The server exposes a comprehensive set of YNAB functionality:

**User and Budget Information**:

- Retrieve user information
- List all budgets
- Get detailed budget information
- Browse budget months

**Categories and Transactions**:

- List all budget categories
- Get category details
- Search transactions with filters
- Browse account transactions

**Accounts and Payees**:

- List all accounts
- List all payees

**Financial Analysis**:

- Get current month budget snapshots
- Generate activity summaries
- Compare income versus expenses

## Packaging for Easy Distribution

To make the server accessible to anyone, I packaged it as a Docker image and created a simple installation process through VS Code:

1. **SDK Container Support**: Used .NET 9's built-in container support to simplify Docker image creation.

2. **MCP Manifest**: Created an MCP manifest file that VS Code can use to install and configure the server.

3. **One-Click Installation**: Added installation buttons to the README that automatically configure VS Code to use the server.

Now users can install and run the YNAB MCP Server with just a couple of clicks, without needing to understand the underlying code or Docker configuration.

## Security Considerations

When connecting financial data to AI systems, security is paramount. The YNAB MCP Server addresses this by:

1. **Local execution**: The server runs locally on the user's machine, not in the cloud.

2. **Secure token handling**: The YNAB API token is stored as an environment variable, not hardcoded.

3. **No third-party sharing**: The server communicates directly between YNAB and the AI client, with no intermediaries.

4. **Private data**: Financial data is only shared during active MCP sessions and isn't stored by the AI provider.

## Using the YNAB MCP Server

Once installed, using the server is simple:

1. Click one of the installation buttons in the README.
2. When prompted, enter your YNAB API token.
3. Start the server with F5 or through the Run menu in VS Code.
4. Connect your MCP-compatible AI client (like Claude for Desktop).

Now your AI assistant can access and analyze your YNAB data. You can ask questions like:

- "How much did I spend on groceries last month?"
- "What's my current balance across all accounts?"
- "Am I on track to meet my savings goal this month?"
- "What category am I overspending in most frequently?"

The assistant will use the MCP tools to query your YNAB data and provide informed answers based on your actual financial situation.

## Lessons Learned

Building this MCP server taught me several valuable lessons:

1. **AI-assisted development is transformative**: Using Copilot with the right context documents dramatically accelerated development.

2. **OpenAPI specs are gold**: Having a formal API specification made integration much smoother and less error-prone.

3. **MCP opens new possibilities**: The protocol creates powerful new ways for AI to interact with our personal data and services.

4. **Docker simplifies distribution**: Packaging as a container made it much easier to distribute a complex application.

5. **.NET 9 container support is excellent**: The new SDK container support streamlined the Docker packaging process.

## Next Steps

While the current implementation covers most YNAB functionality, there's always room for improvement:

- Adding more analytical tools (spending trends, budget optimization suggestions)
- Supporting webhook notifications for real-time updates
- Implementing caching to improve performance
- Adding visualization capabilities for charts and graphs

## Conclusion

Building an MCP server for YNAB demonstrates the power of connecting AI assistants to our personal data in secure, structured ways. By leveraging AI tools like GitHub Copilot during development, even complex integration projects become more manageable.

The combination of MCP, .NET 9, and Docker creates a powerful, accessible bridge between AI assistants and personal finance data—helping users get more value from both their YNAB subscription and their AI tools.

If you're interested in trying it out yourself or contributing to the project, check out the [GitHub repository](https://github.com/ChuckBryan/ynabmcpserver) or try the one-click installation through VS Code.

---

_This project was developed using .NET 9, the Model Context Protocol, and the YNAB API. All code is available under the MIT license._
