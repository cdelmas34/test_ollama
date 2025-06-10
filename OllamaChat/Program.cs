using System.Net.Http.Json;
using OllamaChat.Models;
using OllamaChat;

await DockerHelper.EnsureOllamaRunningAsync();

var http = new HttpClient { BaseAddress = new Uri("http://localhost:11434") };
var messages = new List<ChatMessage>();

Console.WriteLine("Enter 'exit' to quit.");
while (true)
{
    Console.Write("You: ");
    var input = Console.ReadLine();
    if (string.IsNullOrWhiteSpace(input))
        continue;
    if (input.Equals("exit", StringComparison.OrdinalIgnoreCase))
        break;

    messages.Add(new ChatMessage { role = "user", content = input });
    var request = new ChatRequest
    {
        model = "llama3:3.2",
        messages = messages,
        stream = false
    };

    var response = await http.PostAsJsonAsync("/v1/chat/completions", request);
    response.EnsureSuccessStatusCode();

    var result = await response.Content.ReadFromJsonAsync<ChatResponse>();
    var content = result?.choices.FirstOrDefault()?.message.content?.Trim();
    if (!string.IsNullOrEmpty(content))
    {
        Console.WriteLine($"Llama: {content}");
        messages.Add(new ChatMessage { role = "assistant", content = content });
    }
    else
    {
        Console.WriteLine("[No response]");
    }
}
