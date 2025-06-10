using System.Collections.Generic;

namespace OllamaChat.Models;

public record ChatResponse
{
    public List<ChatChoice> choices { get; init; } = new();
}
