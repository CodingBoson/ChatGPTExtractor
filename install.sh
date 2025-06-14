#!/bin/bash

# Build and install ChatGPTExtractor on Linux/OSX

dotnet pack -c Release ChatGPTExtractor.csproj
dotnet tool install --global --add-source ./bin/nupkg ChatGPTExtractor --version 1.0.0
