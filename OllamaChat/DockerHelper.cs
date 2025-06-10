using System.Diagnostics;

namespace OllamaChat;

public static class DockerHelper
{
    public static async Task RunSetupScriptAsync()
    {
        var scriptPath = Path.Combine("..", "simple_script.sh");
        if (!File.Exists(scriptPath))
            return;

        var psi = new ProcessStartInfo
        {
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false
        };

        if (OperatingSystem.IsWindows())
        {
            psi.FileName = "wsl";
            psi.ArgumentList.Add("-e");
            psi.ArgumentList.Add(scriptPath);
        }
        else
        {
            psi.FileName = "bash";
            psi.ArgumentList.Add(scriptPath);
        }

        try
        {
            using var process = Process.Start(psi)!;
            var output = await process.StandardOutput.ReadToEndAsync();
            var error = await process.StandardError.ReadToEndAsync();
            await process.WaitForExitAsync();
            if (!string.IsNullOrWhiteSpace(output))
                Console.WriteLine(output.Trim());
            if (process.ExitCode != 0 && !string.IsNullOrWhiteSpace(error))
                Console.WriteLine(error.Trim());
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to run setup script: {ex.Message}");
        }
    }
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

    private static string DockerCommand => OperatingSystem.IsWindows() ? "wsl" : "docker";

    private static string FormatArgs(string args)
    {
        return OperatingSystem.IsWindows() ? $"docker {args}" : args;
    }

    private static Task<string> RunDockerAsync(string args) => RunAsync(DockerCommand, FormatArgs(args));

    public static async Task EnsureOllamaRunningAsync()
    {
        // check if docker exists
        var dockerVersion = await RunDockerAsync("--version");
        if (string.IsNullOrWhiteSpace(dockerVersion))
        {
            Console.WriteLine("Docker not found. Please install Docker and ensure it is in PATH.");
            return;
        }

        var running = await RunDockerAsync("ps --filter name=ollama --filter status=running -q");
        if (string.IsNullOrWhiteSpace(running))
        {
            var exists = await RunDockerAsync("ps -a --filter name=ollama -q");
            if (string.IsNullOrWhiteSpace(exists))
            {
                Console.WriteLine("Starting new Ollama container...");
                await RunDockerAsync("run -d --name ollama -p 11434:11434 -v ollama:/root/.ollama ollama/ollama");
            }
            else
            {
                Console.WriteLine("Starting existing Ollama container...");
                await RunDockerAsync("start ollama");
            }
        }
        await RunDockerAsync("exec ollama ollama pull llama3:3.2");
    }
}
