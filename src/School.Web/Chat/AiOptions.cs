namespace School.Web.Chat;

public sealed class AiOptions
{
    public const string SectionName = "Ai";

    public string Endpoint { get; set; } = "https://api.openai.com/v1/chat/completions";
    public string Model { get; set; } = "gpt-4o-mini";
    public string? ApiKey { get; set; }
}
