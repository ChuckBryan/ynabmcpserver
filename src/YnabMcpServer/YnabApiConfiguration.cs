namespace YnabMcpServer;

/// <summary>
/// Configuration for YNAB API
/// </summary>
public class YnabApiConfiguration : IYnabApiConfiguration
{
    public string BaseUrl { get; set; } = "https://api.ynab.com/v1";
    public string ApiToken { get; set; } = string.Empty;
}

/// <summary>
/// Interface for YNAB API configuration
/// </summary>
public interface IYnabApiConfiguration
{
    string BaseUrl { get; set; }
    string ApiToken { get; set; }
}
