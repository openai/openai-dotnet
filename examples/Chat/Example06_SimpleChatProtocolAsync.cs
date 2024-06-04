using NUnit.Framework;
using OpenAI.Chat;
using System;
using System.ClientModel;
using System.Text.Json;
using System.Threading.Tasks;

namespace OpenAI.Examples;

public partial class ChatExamples
{
    [Test]
    public async Task Example06_SimpleChatProtocolAsync()
    {
        ChatClient client = new("gpt-4o", Environment.GetEnvironmentVariable("OPENAI_API_KEY"));

        BinaryData input = BinaryData.FromString("""
            {
               "model": "gpt-4o",
               "messages": [
                   {
                       "role": "user",
                       "content": "How does AI work? Explain it in simple terms."
                   }
               ]
            }
            """);

        using BinaryContent content = BinaryContent.Create(input);
        ClientResult result = await client.CompleteChatAsync(content);
        BinaryData output = result.GetRawResponse().Content;

        using JsonDocument outputAsJson = JsonDocument.Parse(output.ToString());
        string message = outputAsJson.RootElement
            .GetProperty("choices")[0]
            .GetProperty("message")
            .GetProperty("content")
            .GetString();

        Console.WriteLine($"[ASSISTANT]:");
        Console.WriteLine($"{message}");
    }
}
