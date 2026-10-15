using Microsoft.Extensions.Options;
using ModelContextProtocol.Client;
using ModelContextProtocol.Protocol;

namespace School.Web.Mcp;

public sealed class SchoolMcpClient : IHostedService, IAsyncDisposable, ISchoolMcpClient
{
    private readonly ILogger<SchoolMcpClient> _logger;
    private readonly McpOptions _options;
    private readonly SemaphoreSlim _gate = new(1, 1);
    private McpClient? _client;
    private IList<McpClientTool> _tools = [];

    public SchoolMcpClient(ILogger<SchoolMcpClient> logger, IOptions<McpOptions> options)
    {
        _logger = logger;
        _options = options.Value;
    }

    public bool IsReady => _client is not null;
    public string? LastError { get; private set; }
    public IReadOnlyList<McpToolDescriptor> Tools =>
        _tools.Select(tool => new McpToolDescriptor(tool.Name, tool.Description)).ToList();

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        try
        {
            var transport = new StdioClientTransport(new StdioClientTransportOptions
            {
                Name = "School MCP",
                Command = _options.Command,
                Arguments = ResolveArguments()
            });

            _client = await McpClient.CreateAsync(transport, cancellationToken: cancellationToken);
            _tools = await _client.ListToolsAsync(cancellationToken: cancellationToken);
            LastError = null;
            _logger.LogInformation("Connected to school MCP server with {Count} tools.", _tools.Count);
        }
        catch (Exception ex)
        {
            LastError = ex.Message;
            _logger.LogError(ex, "Failed to start the school MCP server.");
        }
    }

    public Task StopAsync(CancellationToken cancellationToken) => DisposeAsync().AsTask();

    public async Task<string> CallToolAsync(
        string toolName,
        IReadOnlyDictionary<string, object?> arguments,
        CancellationToken cancellationToken)
    {
        if (_client is null)
        {
            throw new InvalidOperationException(
                LastError is null
                    ? "The MCP server is not connected yet."
                    : $"The MCP server is not connected: {LastError}");
        }

        await _gate.WaitAsync(cancellationToken);
        try
        {
            var result = await _client.CallToolAsync(toolName, arguments, cancellationToken: cancellationToken);
            var text = string.Join(
                Environment.NewLine,
                result.Content.OfType<TextContentBlock>().Select(block => block.Text));

            if (result.IsError == true)
            {
                throw new InvalidOperationException(string.IsNullOrWhiteSpace(text) ? "MCP tool returned an error." : text);
            }

            return string.IsNullOrWhiteSpace(text) ? "{}" : text;
        }
        finally
        {
            _gate.Release();
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_client is not null)
        {
            await _client.DisposeAsync();
            _client = null;
        }

        _gate.Dispose();
    }

    private string[] ResolveArguments()
    {
        var baseDir = AppContext.BaseDirectory.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
        return _options.Arguments
            .Select(argument => argument.Contains("{BaseDir}", StringComparison.Ordinal)
                ? Path.GetFullPath(argument.Replace("{BaseDir}", baseDir))
                : argument)
            .ToArray();
    }
}
