using System.Diagnostics;

namespace OllamaChat;

public static class DockerHelper
{
    private static async Task<string> RunAsync(string fileName, string args)
    {
        var psi = new ProcessStartInfo(fileName, args)
        {
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false
        };
        try
        {
            using var process = Process.Start(psi)!;
            var output = await process.StandardOutput.ReadToEndAsync();
            await process.WaitForExitAsync();
            return output.Trim();
        }
        catch
        {
            return string.Empty;
        }
    }

    public static async Task EnsureOllamaRunningAsync()
    {
        // check if docker exists
        var dockerVersion = await RunAsync("docker", "--version");
        if (string.IsNullOrWhiteSpace(dockerVersion))
        {
            Console.WriteLine("Docker not found. Please install Docker and ensure it is in PATH.");
            return;
        }

        var running = await RunAsync("docker", "ps --filter name=ollama --filter status=running -q");
        if (string.IsNullOrWhiteSpace(running))
        {
            var exists = await RunAsync("docker", "ps -a --filter name=ollama -q");
            if (string.IsNullOrWhiteSpace(exists))
            {
                Console.WriteLine("Starting new Ollama container...");
                await RunAsync("docker", "run -d --name ollama -p 11434:11434 -v ollama:/root/.ollama ollama/ollama");
            }
            else
            {
                Console.WriteLine("Starting existing Ollama container...");
                await RunAsync("docker", "start ollama");
            }
        }
        await RunAsync("docker", "exec ollama ollama pull llama3:3.2");
    }
}
