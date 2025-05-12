using System.Net.Http.Headers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Http;

namespace YnabMcpServer;

/// <summary>
/// Extension methods for registering YNAB HTTP client
/// </summary>
public static class YnabHttpClientExtensions
{
    /// <summary>
    /// Adds YNAB HTTP client to the service collection
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="configureClient">Action to configure the HTTP client</param>
    /// <returns>The service collection</returns>
    public static IServiceCollection AddYnabHttpClient(this IServiceCollection services, Action<HttpClient> configureClient = null)
    {
        services.AddSingleton<IYnabApiConfiguration, YnabApiConfiguration>(); services.AddHttpClient("YnabApi", (sp, client) =>
        {
            var config = sp.GetRequiredService<IYnabApiConfiguration>();
            client.BaseAddress = new Uri(config.BaseUrl);

            if (!string.IsNullOrEmpty(config.ApiToken))
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", config.ApiToken);
            }

            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            configureClient?.Invoke(client);
        });

        return services;
    }
}
