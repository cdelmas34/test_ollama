#!/bin/bash
set -e

# Ensure the Ollama container exists
if ! docker ps -a --format '{{.Names}}' | grep -q '^ollama$'; then
    echo "Creating new Ollama container..."
    docker run -d --name ollama -p 11434:11434 -v ollama:/root/.ollama ollama/ollama
else
    echo "Ollama container already exists."
fi

# Start the container if it is not running
if ! docker ps --format '{{.Names}}' | grep -q '^ollama$'; then
    echo "Starting existing Ollama container..."
    docker start ollama
fi

# Pull the model image
docker exec ollama ollama pull llama3:3.2
