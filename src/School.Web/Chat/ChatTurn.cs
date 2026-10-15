namespace School.Web.Chat;

public sealed class ChatTurn
{
    public required string Role { get; init; }
    public required string Text { get; init; }
    public string? RequestSummary { get; init; }
    public string? ToolName { get; init; }
    public string? ToolArguments { get; init; }
    public string? ResultText { get; init; }
    public bool IsError { get; init; }
}
