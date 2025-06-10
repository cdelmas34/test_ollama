namespace OllamaChat.Models;

public record ChatChoice
{
    public int index { get; init; }
    public ChatMessage message { get; init; } = new();
    public string finish_reason { get; init; } = string.Empty;
}
