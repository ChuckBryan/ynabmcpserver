# WireMock.NET Implementation Plan for YNAB MCP Server

*Date: May 13, 2025*

## Overview

This document outlines the implementation plan for integrating WireMock.NET with the YNAB MCP Server to enable thorough integration testing without requiring actual YNAB API access. WireMock will simulate the YNAB API responses, allowing us to test our MCP server's functionality in isolation.

## Implementation Phases

### Phase 1: Project Setup (1 hour)

#### Test Project Creation
```powershell
cd c:\projects\github\YnabMcpServer
dotnet new xunit -n YnabMcpServer.IntegrationTests
dotnet sln add YnabMcpServer.IntegrationTests
```

#### NuGet Package Dependencies
```powershell
cd YnabMcpServer.IntegrationTests
dotnet add package WireMock.Net --version 1.5.46
dotnet add package Microsoft.Extensions.Http
dotnet add package Microsoft.Extensions.DependencyInjection
dotnet add package Microsoft.Extensions.Configuration.Json
dotnet add package Microsoft.Extensions.Configuration.EnvironmentVariables
dotnet add package FluentAssertions
dotnet add package ModelContextProtocol.Clients --version 1.7.0
```

#### Project Structure
```
YnabMcpServer.IntegrationTests/
├── Fixtures/               # JSON response data for mocked endpoints
├── Helpers/                # Test utilities and factory methods
└── Tests/                  # Integration test classes
```

### Phase 2: WireMock Server Configuration (2 hours)

#### WireMockFixture Class
Create a base fixture that:
- Starts and configures a WireMock server
- Sets up routes based on YNAB API OpenAPI specification
- Provides a configured HttpClient for tests
- Handles cleanup when tests complete

#### API Configuration Override
Create a test-specific configuration for the YNAB API client to point to the WireMock server instead of the real YNAB API.

#### Key Implementation Files:
- `WireMockFixture.cs` - Manages WireMock server lifecycle
- `TestYnabServiceFactory.cs` - Creates service provider with test configuration
- `FixtureLoader.cs` - Utility for loading test response data

### Phase 3: Response Fixtures Creation (2 hours)

#### JSON Response Fixtures
Create JSON response fixtures for each YNAB API endpoint, based on the OpenAPI spec:

1. **User Info**: `Fixtures/user_info.json`
2. **Budgets List**: `Fixtures/budgets.json`
3. **Budget Details**: `Fixtures/budget_details.json`
4. **Budget Months**: `Fixtures/budget_months.json`
5. **Categories**: `Fixtures/categories.json`
6. **Category Details**: `Fixtures/category_details.json`
7. **Accounts**: `Fixtures/accounts.json`
8. **Payees**: `Fixtures/payees.json`
9. **Transactions**: `Fixtures/transactions.json`
10. **Month Snapshot**: `Fixtures/month_snapshot.json`

Each fixture will contain sample data that matches the YNAB API response format.

### Phase 4: MCP Protocol Test Infrastructure (2 hours)

#### MCP Test Client
Create a test client that can interact with our MCP server using the MCP protocol:
- Uses the ModelContextProtocol.Clients library
- Provides methods for invoking MCP tools
- Handles request/response serialization

#### Test Server Launcher
Create a helper to launch the MCP server with test configuration during integration tests:
- Sets up environment variables for test
- Configures the server to use WireMock
- Manages the process lifecycle

### Phase 5: Integration Test Implementation (3 hours)

#### Base Test Class
Create an abstract base class for all integration tests that:
- Sets up WireMock server with common stubs
- Provides helper methods for testing
- Handles test resources cleanup

#### Test Categories

1. **API Client Tests**
   - Test direct interactions with the YNAB API client
   - Verify proper request formatting and response handling

2. **MCP Protocol Tests**
   - Test end-to-end flow through the MCP protocol
   - Verify tool invocation, parameter handling, and response formatting

3. **Error Handling Tests**
   - Test error scenarios (invalid tokens, not found resources, etc.)
   - Verify proper error handling and messaging

### Phase 6: CI/CD Integration (1 hour)

#### GitHub Actions Integration
Update the GitHub Actions workflow to:
- Run integration tests in CI pipeline
- Configure environment for tests
- Report test results

### Phase 7: Documentation and Usage Examples (1 hour)

#### README Updates
- Add section about running integration tests
- Document WireMock integration approach
- Provide examples for adding new tests

#### Developer Guide
- Create guide for extending tests with new fixtures
- Document common patterns for testing new functionality

## Detailed Implementation Examples

### WireMockFixture Example

```csharp
using System;
using System.Net.Http;
using WireMock.Server;

namespace YnabMcpServer.IntegrationTests.Helpers
{
    public class WireMockFixture : IDisposable
    {
        public WireMockServer Server { get; }
        public string BaseUrl { get; }
        public HttpClient HttpClient { get; }
        
        public WireMockFixture()
        {
            // Start WireMock server on dynamic port
            Server = WireMockServer.Start();
            BaseUrl = Server.Urls[0];
            
            // Create HttpClient pointing to WireMock
            HttpClient = new HttpClient
            {
                BaseAddress = new Uri(BaseUrl)
            };
            
            // Setup OpenAPI mappings
            SetupApiMappings();
        }
        
        private void SetupApiMappings()
        {
            // Load OpenAPI spec
            var openApiSpecPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, 
                "../../../../docs/open_api_spec.yml");
            
            // Configure WireMock with OpenAPI
            Server.WithOpenApiSpecFromFile(openApiSpecPath);
        }
        
        public void Dispose()
        {
            HttpClient?.Dispose();
            Server?.Stop();
            Server?.Dispose();
        }
    }
}
```

### Test Class Example

```csharp
using System.Threading.Tasks;
using FluentAssertions;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using Xunit;
using YnabMcpServer.IntegrationTests.Helpers;

namespace YnabMcpServer.IntegrationTests.Tests
{
    public class GetUserInfoTests : TestBase
    {
        [Fact]
        public async Task GetUserInfo_WithValidToken_ReturnsUserId()
        {
            // Arrange
            var userJson = FixtureLoader.LoadFixture("user_info.json");
            
            WireMockFixture.Server
                .Given(Request.Create().WithPath("/v1/user").UsingGet()
                    .WithHeader("Authorization", "Bearer test_token"))
                .RespondWith(Response.Create()
                    .WithStatusCode(200)
                    .WithHeader("Content-Type", "application/json")
                    .WithBody(userJson));
                    
            var mcpClient = CreateMcpClient();
            
            // Act
            var response = await mcpClient.InvokeToolAsync("GetUserInfo", new {});
            
            // Assert
            response.Status.Should().Be("success");
            var result = JsonDocument.Parse(response.Result).RootElement;
            result.GetProperty("userId").GetString().Should().Be("test-user-id");
        }
        
        [Fact]
        public async Task GetUserInfo_WithInvalidToken_ReturnsError()
        {
            // Arrange
            WireMockFixture.Server
                .Given(Request.Create().WithPath("/v1/user").UsingGet())
                .RespondWith(Response.Create()
                    .WithStatusCode(401)
                    .WithHeader("Content-Type", "application/json")
                    .WithBody("{\"error\":{\"id\":\"401\",\"name\":\"unauthorized\",\"detail\":\"Invalid API token\"}}"));
                    
            var mcpClient = CreateMcpClient();
            
            // Act
            var response = await mcpClient.InvokeToolAsync("GetUserInfo", new {});
            
            // Assert
            response.Status.Should().Be("error");
            response.Error.Should().Contain("Invalid API token");
        }
    }
}
```

## Implementation Timeline

| Phase | Description | Time Estimate |
|-------|-------------|---------------|
| 1 | Project Setup | 1 hour |
| 2 | WireMock Configuration | 2 hours |
| 3 | Response Fixtures | 2 hours |
| 4 | MCP Test Infrastructure | 2 hours |
| 5 | Integration Tests | 3 hours |
| 6 | CI/CD Integration | 1 hour |
| 7 | Documentation | 1 hour |
| | **Total** | **12 hours** |

## Technical Considerations

### YNAB API Simulation Challenges

1. **Authentication**: WireMock needs to verify authentication headers match expected values
2. **Dynamic Response Composition**: Some endpoints may require dynamic response generation
3. **Stateful Behavior**: Some YNAB operations may be stateful (changes in one affect others)
4. **Complex Response Schemas**: YNAB has complex nested responses that must be accurately modeled

### Testing Scope Recommendations

1. **Unit Tests**: Continue using unit tests for individual components
2. **Integration Tests**: Use WireMock for testing interactions between components
3. **End-to-End Tests**: Consider a smaller set of E2E tests with real API for validation

## Next Steps

1. Create the integration test project structure
2. Implement WireMock server configuration
3. Create fixtures for key API responses
4. Implement first integration test for basic functionality
5. Expand test coverage to all MCP tools
6. Integrate with CI/CD pipeline
