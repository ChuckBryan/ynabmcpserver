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

// Register MCP server
builder.Services.AddMcpServer()
    .WithStdioServerTransport()
    .WithToolsFromAssembly();

// Register YNAB API configuration using IOptions pattern
builder.Services.Configure<YnabApiConfiguration>(
    builder.Configuration.GetSection(YnabApiConfiguration.SectionName));

// Register HttpClient
builder.Services.AddSingleton(sp =>
{
    // Get options directly
    var options = sp.GetRequiredService<IOptions<YnabApiConfiguration>>();
    var config = options.Value;

    var client = new HttpClient() { BaseAddress = new Uri(config.BaseUrl) };
    client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("YnabMcpServer", "1.0"));

    if (!string.IsNullOrEmpty(config.ApiToken))
    {
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", config.ApiToken);
    }

    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    return client;
});

// Register YNAB API client
builder.Services.AddTransient<IClient>(sp =>
{
    // Get options directly
    var options = sp.GetRequiredService<IOptions<YnabApiConfiguration>>();
    var config = options.Value;

    var httpClient = sp.GetRequiredService<HttpClient>();
    return new Client(config, httpClient);
});

var app = builder.Build();

await app.RunAsync();