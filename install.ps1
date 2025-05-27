# Build and install ChatGPTExtractor on Windows

dotnet pack -c Release ChatGPTExtractor.csproj
dotnet tool install --global --add-source .\bin\nupkg ChatGPTExtractor --version 1.0.0
