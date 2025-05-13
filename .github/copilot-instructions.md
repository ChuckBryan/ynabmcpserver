# GitHub Copilot Custom Instructions

This file provides custom instructions for GitHub Copilot in this repository.

## Reference

- [Model Context Protocol SDK](https://github.com/modelcontextprotocol/create-python-server)

Add any additional instructions or guidelines for Copilot usage below.

- I am building an MCP Server using .NET that will interact with YNAB's API.
- This will be the YnabMcpServer
- use the /docs/mcp.md for quickstart instructions on how to create an MCP Server using C#.
- One alteration to the mcp.md is that we should use .NET 9 and and not .NET 8
- use the /docs/ynabapi.md for the details of the features of the ynab api.
- use the /docs/open_api_spec.yml to generate the http client.
- use the EnableSdkContainerSupport in the CSPROJ so that I do not need a docker file
- the release should support the semantic-release library
  - support tag the release
  - should update a directory build props file (just a bare bones implementation)
  - should create the changelog.md
  - do not use in commit linters in this first go round.
  - should use the dotnet pusblish to deploy to docker hub
  - the docker hub pat will be set in secrets
  - the github pat will be set in secrets
