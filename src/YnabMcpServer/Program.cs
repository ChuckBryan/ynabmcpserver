using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ModelContextProtocol;
using System.Net.Http.Headers;
using YnabMcpServer.Generated.YnabApi;
using YnabMcpServer;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Http;

var builder = Host.CreateEmptyApplicationBuilder(settings: null);

builder.Services.AddMcpServer()
    .WithStdioServerTransport()
    .WithToolsFromAssembly();

// Register YNAB API configuration and client
builder.Services.AddSingleton<IYnabApiConfiguration, YnabApiConfiguration>();
builder.Services.AddHttpClient<IClient, Client>((serviceProvider, client) =>
{
    var config = serviceProvider.GetRequiredService<IYnabApiConfiguration>();
    client.BaseAddress = new Uri(config.BaseUrl);

    if (!string.IsNullOrEmpty(config.ApiToken))
    {
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", config.ApiToken);
    }

    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("YnabMcpServer", "1.0"));
});

var app = builder.Build();

await app.RunAsync();