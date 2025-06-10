# Ollama Chat Example

This repository contains a simple C# console application that communicates with a local Ollama server. It targets .NET 8 and demonstrates how to chat with the `llama3:3.2` model.

## Requirements
- .NET 8 SDK
- Docker

## Running Ollama with Docker
The application will start the Docker container for you if it is not already
running. Ensure Docker is installed and the current user has permission to run
Docker commands. If you prefer to start the container manually, you can run:
```bash
docker run -d --name ollama -p 11434:11434 -v ollama:/root/.ollama ollama/ollama
docker exec ollama ollama pull llama3:3.2
```

## Running the console app
1. Open `OllamaChat.sln` in Visual Studio (or another IDE) or navigate to the `OllamaChat` directory.
2. Build and run the application:
   ```bash
   dotnet run --project OllamaChat.csproj
   ```
3. Type your messages at the prompt. Enter `exit` to quit.
