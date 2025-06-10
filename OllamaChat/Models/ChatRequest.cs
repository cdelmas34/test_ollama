using System.Collections.Generic;

namespace OllamaChat.Models;

public record ChatRequest
{
    public string model { get; init; } = string.Empty;
    public List<ChatMessage> messages { get; init; } = new();
    public bool stream { get; init; }
}
