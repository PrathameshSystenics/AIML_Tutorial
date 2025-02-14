#!/bin/bash

# Start Ollama in the background.
ollama serve &
# Record Process ID.
pid=$!

# Pause for Ollama to start.
sleep 5

echo "🔴 Retrieve models"
ollama pull deepseek-r1:1.5b 
ollama pull phi3:3.8b 
ollama pull qwen:1.8b
echo "🟢 Done!"

# Wait for Ollama process to finish.
wait $pid