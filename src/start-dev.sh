#!/bin/bash

# Function to handle script termination
cleanup() {
    echo "Stopping all processes..."
    kill $(jobs -p) 2>/dev/null
    exit
}

# Set up trap to catch termination signal
trap cleanup SIGINT SIGTERM

# Start the API project
echo "Starting API project..."
cd UIOMatic.Front.API
dotnet run &
API_PID=$!

# Start the SPA project
echo "Starting SPA project..."
cd ../UIOMatic.SPA
npm run dev &
SPA_PID=$!

# Wait for both processes
wait $API_PID $SPA_PID 