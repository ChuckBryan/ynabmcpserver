namespace YnabMcpServer.Config;

public class YnabConfig
{
    public string ApiBaseUrl { get; set; } = "https://api.ynab.com/v1";
    public string ApiToken { get; set; } = string.Empty;
}
