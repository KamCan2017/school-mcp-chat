using NUnit.Framework;
using School.Web.Mcp;
using School.Web.ViewModels;

namespace School.Tests.ViewModels;

[TestFixture]
public sealed class ToolsViewModelTests
{
    [Test]
    public void WhenConnecting_UsesWaitingDisconnectedPresentation()
    {
        var viewModel = CreateViewModel(new FakeSchoolMcpClient { IsReady = false });

        Assert.That(viewModel.IsMcpReady, Is.False);
        Assert.That(viewModel.McpStatusCssClass, Is.EqualTo("offline"));
        Assert.That(viewModel.McpStatusText, Is.EqualTo("MCP connecting"));
        Assert.That(viewModel.DisconnectedStateCssClass, Is.EqualTo("waiting"));
        Assert.That(viewModel.DisconnectedStateIcon, Is.EqualTo("…"));
        Assert.That(viewModel.DisconnectedStateTitle, Is.EqualTo("Connecting to the MCP server"));
        Assert.That(viewModel.DisconnectedStateMessage, Is.EqualTo("The tool list will appear here once the server is ready."));
        Assert.That(viewModel.ToolCount, Is.Zero);
    }

    [Test]
    public void WhenConnectionFails_UsesFailedDisconnectedPresentation()
    {
        var viewModel = CreateViewModel(new FakeSchoolMcpClient
        {
            IsReady = false,
            LastError = "stdio failed"
        });

        Assert.That(viewModel.DisconnectedStateCssClass, Is.EqualTo("failed"));
        Assert.That(viewModel.DisconnectedStateIcon, Is.EqualTo("!"));
        Assert.That(viewModel.DisconnectedStateTitle, Is.EqualTo("Could not connect to the MCP server"));
        Assert.That(viewModel.DisconnectedStateMessage, Is.EqualTo("stdio failed"));
    }

    [Test]
    public void WhenReady_ExposesOnlineStatusAndTools()
    {
        var viewModel = CreateViewModel(new FakeSchoolMcpClient
        {
            IsReady = true,
            Tools =
            [
                new McpToolDescriptor("GetAllCourses", "Lists every course."),
                new McpToolDescriptor("FindProfessorByName", "Finds a professor by name.")
            ]
        });

        Assert.That(viewModel.IsMcpReady, Is.True);
        Assert.That(viewModel.McpStatusCssClass, Is.EqualTo("online"));
        Assert.That(viewModel.McpStatusText, Is.EqualTo("MCP connected"));
        Assert.That(viewModel.ToolCount, Is.EqualTo(2));
        Assert.That(viewModel.Tools, Has.Count.EqualTo(2));
        Assert.That(viewModel.Tools[0].Name, Is.EqualTo("GetAllCourses"));
        Assert.That(viewModel.Tools[0].Description, Is.EqualTo("Lists every course."));
        Assert.That(viewModel.Tools[1].Name, Is.EqualTo("FindProfessorByName"));
    }

    private static ToolsViewModel CreateViewModel(ISchoolMcpClient mcp) => new(mcp);
}
