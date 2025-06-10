namespace OllamaChat.Models;

public record ChatMessage
{
    public string role { get; init; } = string.Empty;
    public string content { get; init; } = string.Empty;
}
