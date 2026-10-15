namespace School.Web.Mcp;

public sealed record McpToolDescriptor(string Name, string? Description);

public interface ISchoolMcpClient
{
    bool IsReady { get; }
    string? LastError { get; }
    IReadOnlyList<McpToolDescriptor> Tools { get; }

    Task<string> CallToolAsync(
        string toolName,
        IReadOnlyDictionary<string, object?> arguments,
        CancellationToken cancellationToken);
}
