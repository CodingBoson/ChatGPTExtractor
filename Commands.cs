using System.Text;
using System.Text.Json;
using Cocona;

namespace ChatGPT;

public sealed class Commands 
{
    [Command("extract")]
    public static async Task Extract(
        [Option('p', Description = "Path to conversations.json")] string path, 
        [Option('o', Description = "Destination folder")] string output,
        [Option('n', Description = "User name")] string user = "User",
        [Option('i', Description = "Include index in file names")] bool includeIndex = false,
        [Option('v', Description = "Verbose mode")] bool verbose = false)
    {
        // Verify that the specified JSON file exists
        if (!File.Exists(path))
        {
            Console.WriteLine("Error: The specified conversations.json file could not be found.");
            return;
        }

        // Ensure the output directory exists or create it
        if (!Directory.Exists(output))
        {
            Directory.CreateDirectory(output);
            if (verbose) Console.WriteLine($"Created output directory at {output}");
        }

        // Try extracting chats and handle errors
        try 
        {
            await ExtractChatsImpl(path, output, user, includeIndex, verbose);
        }
        catch (JsonException ex) 
        {
            Console.WriteLine($"Parsing Error: {ex.Message}");
        }
        catch (IOException ex) 
        {
            Console.WriteLine($"IO Error: {ex.Message} (Check file permissions or paths).");
        }
        catch (Exception ex) 
        {
            Console.WriteLine($"Unexpected Error: {ex.Message}");
        }
    }

    private static async Task ExtractChatsImpl(string path, string output, string userName, bool includeIndex, bool verbose)
    {
        // Parse JSON document
        var document = JsonDocument.Parse(await File.ReadAllTextAsync(path));
        int chatIndex = 0;

        // Loop through each chat in the JSON array
        foreach (var chat in document.RootElement.EnumerateArray())
        {
            var title = chat.GetProperty("title").GetString();

            // Skip chats without titles
            if (string.IsNullOrEmpty(title))
                continue;

            if (verbose) Console.WriteLine($"Extracting chat titled: {title}");

            // Build chat content into a markdown string
            var chatBuilder = new StringBuilder();
            foreach (var map in chat.GetProperty("mapping").EnumerateObject())
            {
                var message = map.Value.GetProperty("message");

                // Process only messages with valid authors and recipients
                if (message.ValueKind != JsonValueKind.Null && message.TryGetProperty("author", out var author))
                {
                    var role = author.GetProperty("role").GetString();

                    // Skip system messages
                    if (string.IsNullOrEmpty(role) || role == "system")
                        continue;

                    var recipient = message.GetProperty("recipient").GetString();
                    if (string.IsNullOrEmpty(recipient) || recipient != "all")
                        continue;

                    var content = message.GetProperty("content");

                    // Process content parts and append to chatBuilder
                    if (content.TryGetProperty("parts", out var parts))
                    {
                        foreach (var part in parts.EnumerateArray())
                        {
                            if (part.ValueKind == JsonValueKind.String)
                            {
                                var text = part.GetString();
                                if (string.IsNullOrEmpty(text)) continue;

                                chatBuilder.AppendLine($"## {(role == "assistant" ? "ChatGPT" : userName)}");
                                chatBuilder.AppendLine(text);
                                chatBuilder.AppendLine();
                                break;
                            }
                        }
                    }
                }
            }

            // Generate safe file path for chat
            var chatPath = includeIndex
                ? Path.Combine(output, $"{chatIndex + 1}. {GetSafePath(title)}.md")
                : Path.Combine(output, $"{GetSafePath(title)}.md");

            // Write the chat content to a markdown file
            await File.WriteAllTextAsync(chatPath, chatBuilder.ToString());

            // Set timestamps for the chat file
            var time = DateTime.UtcNow.AddSeconds(chatIndex * -1);
            File.SetCreationTimeUtc(chatPath, time);
            File.SetLastAccessTimeUtc(chatPath, time);
            File.SetLastWriteTimeUtc(chatPath, time);

            if (verbose) Console.WriteLine($"Successfully extracted chat to: {chatPath}");
            chatIndex++;
        }

        if (verbose) Console.WriteLine("All chats extracted successfully.");
    }

    private static string GetSafePath(string text)
    {
        // Replace specific words and handle invalid characters for file names
        text = text.Replace("C#", "CSharp");

        var illegalChars = Path.GetInvalidFileNameChars();
        foreach (var c in illegalChars)
            text = text.Replace(c, '-');

        return text;
    }
}
