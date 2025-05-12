using System.Text.Json;
using YnabMcpServer.Configuration;

namespace YnabMcpServer.Services;

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
    }    // Constructor expected by NSwag-generated code
    public YnabHttpClientBase(IYnabApiConfiguration configuration)
    {
        _configuration = configuration;
        _httpClient = new HttpClient();
        _settings = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };
    }

    public YnabHttpClientBase(IYnabApiConfiguration configuration, HttpClient httpClient)
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
