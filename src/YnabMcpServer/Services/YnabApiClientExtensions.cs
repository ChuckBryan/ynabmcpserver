using Microsoft.Extensions.Options;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using YnabMcpServer.Config;
using YnabMcpServer.Generated.YnabApi;

namespace YnabMcpServer.Services;

/// <summary>
/// Extensions and factory methods for the YnabClient class.
/// </summary>
public class YnabApiClientService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly YnabConfig _config;

    public YnabApiClientService(IHttpClientFactory httpClientFactory, IOptions<YnabConfig> config)
    {
        _httpClientFactory = httpClientFactory;
        _config = config.Value;
    }

    /// <summary>
    /// Creates a new YnabClient with the appropriate authentication.
    /// </summary>
    /// <returns>A configured YnabClient instance.</returns>
    public YnabClient CreateAuthenticatedClient()
    {
        var httpClient = _httpClientFactory.CreateClient("YnabClient");

        // Set the base URL
        httpClient.BaseAddress = new Uri(_config.ApiBaseUrl);

        // Set up authentication
        httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", _config.ApiToken);

        // Create the client
        return new YnabClient(httpClient);
    }
}

/// <summary>
/// Extension methods for the YnabClient class.
/// </summary>
public static class YnabClientExtensions
{
    /// <summary>
    /// Adds the proper API token to an existing YnabClient instance.
    /// </summary>
    /// <param name="client">The YNAB API client.</param>
    /// <param name="apiToken">The API token.</param>
    /// <returns>The configured YnabClient instance.</returns>
    public static YnabClient WithAuthentication(this YnabClient client, string apiToken)
    {
        if (client.HttpClient is HttpClient httpClient)
        {
            httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", apiToken);
        }

        return client;
    }
}
