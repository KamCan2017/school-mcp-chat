using School.Web.Mcp;

namespace School.Web.ViewModels;

public sealed class ToolsViewModel
{
    private readonly ISchoolMcpClient _mcp;

    public ToolsViewModel(ISchoolMcpClient mcp) => _mcp = mcp;

    public bool IsMcpReady => _mcp.IsReady;
    public string? McpLastError => _mcp.LastError;
    public int ToolCount => _mcp.Tools.Count;

    public IReadOnlyList<ToolListItem> Tools =>
        _mcp.Tools.Select(tool => new ToolListItem(tool.Name, tool.Description)).ToList();

    public string McpStatusCssClass => IsMcpReady ? "online" : "offline";
    public string McpStatusText => IsMcpReady ? "MCP connected" : "MCP connecting";

    public string DisconnectedStateCssClass =>
        string.IsNullOrWhiteSpace(McpLastError) ? "waiting" : "failed";

    public string DisconnectedStateIcon =>
        string.IsNullOrWhiteSpace(McpLastError) ? "…" : "!";

    public string DisconnectedStateTitle =>
        string.IsNullOrWhiteSpace(McpLastError)
            ? "Connecting to the MCP server"
            : "Could not connect to the MCP server";

    public string DisconnectedStateMessage =>
        McpLastError ?? "The tool list will appear here once the server is ready.";
}

public sealed record ToolListItem(string Name, string? Description);
