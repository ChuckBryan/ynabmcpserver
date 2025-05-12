namespace YnabMcpServer.Config;

public class AppSettings
{
    public YnabConfig YnabConfig { get; set; } = new YnabConfig();
    public McpServerConfig McpServerConfig { get; set; } = new McpServerConfig();
}
