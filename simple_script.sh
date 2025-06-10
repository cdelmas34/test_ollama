#!/bin/bash
set -e

# Install Docker if it is not present (useful inside WSL)
if ! command -v docker >/dev/null 2>&1; then
    echo "Docker not found. Installing..."
    sudo apt-get update
    sudo apt-get install -y docker.io
fi

# Ensure the Docker daemon is running
if ! pgrep -x dockerd >/dev/null; then
    echo "Starting Docker daemon..."
    sudo service docker start
fi

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
