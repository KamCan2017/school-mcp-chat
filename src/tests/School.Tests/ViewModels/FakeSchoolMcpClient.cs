using School.Web.Mcp;

namespace School.Tests.ViewModels;

internal sealed class FakeSchoolMcpClient : ISchoolMcpClient
{
    public bool IsReady { get; set; }
    public string? LastError { get; set; }
    public IReadOnlyList<McpToolDescriptor> Tools { get; set; } = [];

    public Func<string, IReadOnlyDictionary<string, object?>, CancellationToken, Task<string>>? CallToolHandler { get; set; }

    public Task<string> CallToolAsync(
        string toolName,
        IReadOnlyDictionary<string, object?> arguments,
        CancellationToken cancellationToken)
    {
        if (CallToolHandler is null)
        {
            throw new InvalidOperationException("CallToolHandler was not configured.");
        }

        return CallToolHandler(toolName, arguments, cancellationToken);
    }
}
