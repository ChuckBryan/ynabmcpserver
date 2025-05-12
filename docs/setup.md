# Technical Setup Guide

This guide provides detailed instructions for setting up and deploying the YNAB MCP Server.

## System Requirements

- **.NET 9 SDK** (or newer)
- **Operating System**: Windows, macOS, or Linux
- **IDE**: Visual Studio 2022 (recommended), Visual Studio Code, or JetBrains Rider
- **YNAB Account**: With API access enabled

## Development Environment Setup

### 1. Clone and Build the Project

```bash
# Clone repository
git clone https://github.com/yourusername/YnabMcpServer.git
cd YnabMcpServer

# Restore dependencies
dotnet restore

# Build the project
dotnet build
```

### 2. Configure YNAB API Access

To use this server, you need a YNAB API token:

1. Log in to your YNAB account at https://app.youneedabudget.com
2. Go to Account Settings > Developer Settings
3. Under "Personal Access Tokens", click "New Token"
4. Give your token a name (e.g., "MCP Server")
5. Copy the generated token

### 3. Configure the Server

You can provide your YNAB API token using one of these methods:

#### Method 1: Environment Variable

Set the `YNAB_API_TOKEN` environment variable:

```bash
# For macOS/Linux
export YNAB_API_TOKEN=your_api_token_here

# For Windows PowerShell
$env:YNAB_API_TOKEN = "your_api_token_here"

# For Windows Command Prompt
set YNAB_API_TOKEN=your_api_token_here
```

You can use the provided script in `scripts/create-ynab-env.ps1` to set this up:

```powershell
. ./scripts/create-ynab-env.ps1 your_api_token_here
```

#### Method 2: VS Code MCP Configuration

If you're using VS Code, the server is configured to prompt for your API token:

1. Open the project in VS Code
2. Press F5 or use the Run and Debug menu
3. When prompted, enter your YNAB API token

The configuration is in `.vscode/mcp.json`:

```json
{
  "inputs": [
    {
      "id": "ynab-api-token",
      "type": "promptString",
      "password": true,
      "description": "Enter your YNAB API Token"
    }
  ],
  "servers": {
    "ynab": {
      "command": "dotnet",
      "args": [
        "run",
        "--project",
        "${workspaceFolder}/src/YnabMcpServer/YnabMcpServer.csproj"
      ],
      "env": {
        "YNAB_API_TOKEN": "${input:ynab-api-token}"
      }
    }
  }
}
```

#### Method 3: Application Settings

You can add your API token to `appsettings.Development.json`:

```json
{
  "YnabApi": {
    "ApiToken": "your_api_token_here",
    "BaseUrl": "https://api.youneedabudget.com/v1/"
  }
}
```

**Note**: This method is recommended only for development. Don't commit this file to source control.

### 4. Testing API Access

The project includes HTTP request files to test the YNAB API directly:

```bash
# Navigate to requests directory
cd requests

# Run a sample request (requires VS Code REST Client extension)
# Open ynab-api-requests.http and send requests
```

### 5. Running the Server

```bash
dotnet run --project src/YnabMcpServer/YnabMcpServer.csproj
```

The server will start and listen for MCP protocol connections. You should see output indicating the server is running.

## Connecting to Claude for Desktop

1. Open Claude for Desktop
2. Go to Settings > Tools > Add Local Tool Server
3. Browse to your YNAB MCP Server executable or select it from the list
4. Follow the prompts to connect

## Building for Production

```bash
# Build in Release mode
dotnet build -c Release

# Publish a self-contained executable
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true
```

The published executable will be in `src/YnabMcpServer/bin/Release/net9.0/win-x64/publish/`.

Replace `win-x64` with `osx-x64` for macOS or `linux-x64` for Linux.

## Troubleshooting

### API Connection Issues

If you're having trouble connecting to the YNAB API:

1. Verify your API token is valid and not expired
2. Check your internet connection
3. Ensure the YNAB API is operational: https://status.youneedabudget.com/

You can use the `YnabApiTester` tool to verify connection:

```bash
dotnet run --project src/YnabMcpServer/YnabMcpServer.csproj -- test-api
```

### Server Not Starting

If the server fails to start:

1. Check for any error messages in the console output
2. Verify your .NET SDK version with `dotnet --version`
3. Try rebuilding the project with `dotnet build --clean`

### Claude Not Connecting

If Claude cannot connect to your server:

1. Ensure the server is running
2. Check if your firewall is blocking the connection
3. Try restarting both Claude and the server
