using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using NUnit.Framework;
using School.Web.Chat;
using School.Web.Mcp;
using School.Web.ViewModels;

namespace School.Tests.ViewModels;

[TestFixture]
public sealed class HomeViewModelTests
{
    [Test]
    public void InitialState_HasWelcomeTurnAndIsNotBusy()
    {
        var viewModel = CreateViewModel(new FakeSchoolMcpClient());

        Assert.That(viewModel.Draft, Is.Empty);
        Assert.That(viewModel.IsBusy, Is.False);
        Assert.That(viewModel.Turns, Has.Count.EqualTo(1));
        Assert.That(viewModel.Turns[0].Role, Is.EqualTo("assistant"));
    }

    [Test]
    public void McpStatus_ReflectsFakeClient()
    {
        var mcp = new FakeSchoolMcpClient { IsReady = true, LastError = "startup failed" };
        var viewModel = CreateViewModel(mcp);

        Assert.That(viewModel.IsMcpReady, Is.True);
        Assert.That(viewModel.McpLastError, Is.EqualTo("startup failed"));
    }

    [Test]
    public async Task SendAsync_IgnoresBlankInput()
    {
        var viewModel = CreateViewModel(new FakeSchoolMcpClient());

        await viewModel.SendAsync("   ");

        Assert.That(viewModel.Turns, Has.Count.EqualTo(1));
        Assert.That(viewModel.IsBusy, Is.False);
    }

    [Test]
    public async Task SendAsync_AddsAssistantReplyWhenPlannerDoesNotCallTool()
    {
        var viewModel = CreateViewModel(new FakeSchoolMcpClient());

        await viewModel.SendAsync("hello there");

        Assert.That(viewModel.Turns, Has.Count.EqualTo(3));
        Assert.That(viewModel.Turns[1].Role, Is.EqualTo("user"));
        Assert.That(viewModel.Turns[1].Text, Is.EqualTo("hello there"));
        Assert.That(viewModel.Turns[2].Role, Is.EqualTo("assistant"));
        Assert.That(viewModel.Turns[2].Text, Does.Contain("suggested questions"));
        Assert.That(viewModel.Draft, Is.Empty);
        Assert.That(viewModel.IsBusy, Is.False);
    }

    [Test]
    public async Task SendAsync_CallsMcpAndAddsSummarizedResult()
    {
        var mcp = new FakeSchoolMcpClient
        {
            IsReady = true,
            CallToolHandler = (_, _, _) => Task.FromResult("""[{"name":"Calculus"}]""")
        };
        var viewModel = CreateViewModel(mcp);

        await viewModel.SendAsync("List all courses");

        Assert.That(viewModel.Turns, Has.Count.EqualTo(3));
        var assistantTurn = viewModel.Turns[2];
        Assert.That(assistantTurn.Role, Is.EqualTo("assistant"));
        Assert.That(assistantTurn.Text, Is.EqualTo("Here is the result:"));
        Assert.That(assistantTurn.ToolName, Is.EqualTo("GetAllCourses"));
        Assert.That(assistantTurn.RequestSummary, Is.EqualTo("List every course in the school."));
        Assert.That(assistantTurn.ResultText, Does.Contain("Name: Calculus"));
        Assert.That(assistantTurn.IsError, Is.False);
    }

    [Test]
    public async Task SendAsync_AddsErrorTurnWhenMcpCallFails()
    {
        var mcp = new FakeSchoolMcpClient
        {
            CallToolHandler = (_, _, _) => throw new InvalidOperationException("MCP offline")
        };
        var viewModel = CreateViewModel(mcp);

        await viewModel.SendAsync("List all professors");

        var assistantTurn = viewModel.Turns[^1];
        Assert.That(assistantTurn.Role, Is.EqualTo("assistant"));
        Assert.That(assistantTurn.Text, Is.EqualTo("The MCP request could not be completed."));
        Assert.That(assistantTurn.ToolName, Is.EqualTo("GetAllProfessors"));
        Assert.That(assistantTurn.ResultText, Is.EqualTo("MCP offline"));
        Assert.That(assistantTurn.IsError, Is.True);
    }

    [Test]
    public async Task SendAsync_IgnoresSecondMessageWhileBusy()
    {
        var mcp = new FakeSchoolMcpClient
        {
            CallToolHandler = async (_, _, _) =>
            {
                await Task.Delay(200);
                return "{}";
            }
        };
        var viewModel = CreateViewModel(mcp);

        var first = viewModel.SendAsync("List all courses");
        await viewModel.SendAsync("List all professors");

        await first;

        Assert.That(viewModel.Turns.Count(turn => turn.Role == "user"), Is.EqualTo(1));
        Assert.That(viewModel.Turns[1].Text, Is.EqualTo("List all courses"));
    }

    [Test]
    public async Task SubmitAsync_UsesDraft()
    {
        var viewModel = CreateViewModel(new FakeSchoolMcpClient());
        viewModel.Draft = "hello there";

        await viewModel.SubmitAsync();

        Assert.That(viewModel.Turns[1].Text, Is.EqualTo("hello there"));
        Assert.That(viewModel.Draft, Is.Empty);
    }

    [Test]
    public async Task SendAsync_RaisesStateChanged()
    {
        var viewModel = CreateViewModel(new FakeSchoolMcpClient());
        var notifications = 0;
        viewModel.StateChanged += () => notifications++;

        await viewModel.SendAsync("hello there");

        Assert.That(notifications, Is.GreaterThanOrEqualTo(2));
    }

    private static HomeViewModel CreateViewModel(ISchoolMcpClient mcp)
    {
        var summarizer = new McpResponseSummarizer(
            new HttpClient(),
            Options.Create(new AiOptions()),
            NullLogger<McpResponseSummarizer>.Instance);

        return new HomeViewModel(mcp, new SchoolQueryPlanner(), summarizer);
    }
}
