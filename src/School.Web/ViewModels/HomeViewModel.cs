using School.Web.Chat;
using School.Web.Mcp;

namespace School.Web.ViewModels;

public sealed class HomeViewModel
{
    public static readonly string[] Suggestions =
    [
        "List all courses",
        "List all professors",
        "Find professor Ada Lovelace",
        "What courses does Grace Hopper teach?",
        "Who is enrolled in Databases?",
        "Where is Algorithms taught?"
    ];

    private readonly ISchoolMcpClient _mcp;
    private readonly SchoolQueryPlanner _planner;
    private readonly McpResponseSummarizer _summarizer;
    private string _draft = "";

    public HomeViewModel(
        ISchoolMcpClient mcp,
        SchoolQueryPlanner planner,
        McpResponseSummarizer summarizer)
    {
        _mcp = mcp;
        _planner = planner;
        _summarizer = summarizer;
    }

    public event Action? StateChanged;

    public bool IsMcpReady => _mcp.IsReady;
    public string? McpLastError => _mcp.LastError;

    private readonly List<ChatTurn> _turns =
    [
        new()
        {
            Role = "assistant",
            Text = "Hi. I know this school’s MCP tools: find a professor, list a professor’s courses, list students in a course, find a classroom, or list all courses."
        }
    ];

    public IReadOnlyList<ChatTurn> Turns => _turns;

    public string Draft
    {
        get => _draft;
        set => _draft = value;
    }

    public bool IsBusy { get; private set; }

    public Task SubmitAsync() => SendAsync(Draft);

    public async Task SendAsync(string text)
    {
        var question = text.Trim();
        if (string.IsNullOrWhiteSpace(question) || IsBusy)
        {
            return;
        }

        Draft = "";
        IsBusy = true;
        NotifyStateChanged();

        _turns.Add(new ChatTurn { Role = "user", Text = question });

        var plan = _planner.Plan(question);
        if (!plan.IsToolCall)
        {
            _turns.Add(new ChatTurn { Role = "assistant", Text = plan.Summary });
            IsBusy = false;
            NotifyStateChanged();
            return;
        }

        var args = string.Join(", ", plan.Arguments.Select(pair => $"{pair.Key}: {pair.Value}"));
        try
        {
            var json = await _mcp.CallToolAsync(
                plan.ToolName!,
                new Dictionary<string, object?>(plan.Arguments),
                CancellationToken.None);
            var summary = await _summarizer.SummarizeAsync(question, plan.ToolName!, json, CancellationToken.None);
            _turns.Add(new ChatTurn
            {
                Role = "assistant",
                Text = "Here is the result:",
                RequestSummary = plan.Summary,
                ToolName = plan.ToolName,
                ToolArguments = args,
                ResultText = summary
            });
        }
        catch (Exception ex)
        {
            _turns.Add(new ChatTurn
            {
                Role = "assistant",
                Text = "The MCP request could not be completed.",
                RequestSummary = plan.Summary,
                ToolName = plan.ToolName,
                ToolArguments = args,
                ResultText = ex.Message,
                IsError = true
            });
        }
        finally
        {
            IsBusy = false;
            NotifyStateChanged();
        }
    }

    private void NotifyStateChanged() => StateChanged?.Invoke();
}
