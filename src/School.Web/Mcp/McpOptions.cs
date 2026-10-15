namespace School.Web.Mcp;

public sealed class McpOptions
{
    public const string SectionName = "Mcp";

    public string Command { get; set; } = "dotnet";
    public string[] Arguments { get; set; } = [];
}
