using System.Text.Json;

namespace YnabMcpServer;

/// <summary>
/// Base class for YNAB API HTTP client
/// </summary>
public partial class YnabHttpClientBase : IYnabHttpClient
{
    protected readonly HttpClient _httpClient;
    protected readonly JsonSerializerOptions _settings;
    private readonly IYnabApiConfiguration _configuration;

    public string BaseUrl
    {
        get { return _configuration.BaseUrl; }
        set { _configuration.BaseUrl = value; }
    }

    public YnabHttpClientBase(HttpClient httpClient, IYnabApiConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _settings = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };

        if (_httpClient.BaseAddress == null)
        {
            _httpClient.BaseAddress = new Uri(_configuration.BaseUrl);
        }

        UpdateJsonSerializerSettings(_settings);
    }

    partial void UpdateJsonSerializerSettings(JsonSerializerOptions settings);
}

/// <summary>
/// Interface for YNAB API HTTP client
/// </summary>
public interface IYnabHttpClient
{
    string BaseUrl { get; set; }
}
