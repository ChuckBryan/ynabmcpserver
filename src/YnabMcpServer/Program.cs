using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ModelContextProtocol;
using System.Net.Http.Headers;
using YnabMcpServer.Generated.YnabApi;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using YnabMcpServer.Configuration;
using YnabMcpServer.Services;

// Create a host builder with configuration from appsettings.json
var builder = Host.CreateEmptyApplicationBuilder(settings: null);

// Configure host with configuration
builder.Configuration.AddJsonFile("appsettings.json", optional: false)
    .AddJsonFile("appsettings.Development.json", optional: true)
    .AddEnvironmentVariables();

// Check for API token in environment variables
var apiToken = builder.Configuration["YNAB_API_TOKEN"] ?? builder.Configuration["YnabApi:ApiToken"];
if (!string.IsNullOrEmpty(apiToken))
{
    builder.Configuration["YnabApi:ApiToken"] = apiToken;
}

// Register MCP server
builder.Services.AddMcpServer()
    .WithStdioServerTransport()
    .WithToolsFromAssembly();

// Register YNAB API configuration using IOptions pattern
builder.Services.Configure<YnabApiConfiguration>(
    builder.Configuration.GetSection(YnabApiConfiguration.SectionName));

// Register concrete YnabApiConfiguration
builder.Services.AddSingleton(sp =>
{
    var options = sp.GetRequiredService<IOptions<YnabApiConfiguration>>();
    return options.Value;
});

// Register IYnabApiConfiguration interface
builder.Services.AddSingleton<IYnabApiConfiguration>(sp =>
    sp.GetRequiredService<YnabApiConfiguration>());

// Register HttpClient using the extension method
builder.Services.AddYnabHttpClient(client =>
{
    client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("YnabMcpServer", "1.0"));
});

// Register YNAB API client
builder.Services.AddTransient<IClient>(sp =>
{
    var httpClient = sp.GetRequiredService<IHttpClientFactory>().CreateClient("YnabApi");
    var config = sp.GetRequiredService<YnabApiConfiguration>();
    return new Client(config, httpClient);
});

var app = builder.Build();

await app.RunAsync();

