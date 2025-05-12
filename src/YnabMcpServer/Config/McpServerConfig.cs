namespace YnabMcpServer.Config;

public class McpServerConfig
{
    public string ServerName { get; set; } = "YnabMcpServer";
    public string ServerVersion { get; set; } = "1.0.0";
    public string TransportType { get; set; } = "Stdio";
}
