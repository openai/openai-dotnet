using NUnit.Framework;
using OpenAI.Chat;
using System;
using System.ClientModel;
using System.Text.Json;

namespace OpenAI.Examples;

public partial class ChatExamples
{
    [Test]
    public void Example07_ChatProtocol()
    {
        ChatClient client = new("gpt-4o", Environment.GetEnvironmentVariable("OPENAI_API_KEY"));

        BinaryData input = BinaryData.FromBytes("""
            {
               "model": "gpt-4o",
               "messages": [
                   {
                       "role": "user",
                       "content": "Say 'this is a test.'"
                   }
               ]
            }
            """u8.ToArray());

        using BinaryContent content = BinaryContent.Create(input);
        ClientResult result = client.CompleteChat(content);
        BinaryData output = result.GetRawResponse().Content;

        using JsonDocument outputAsJson = JsonDocument.Parse(output.ToString());
        string message = outputAsJson.RootElement
            .GetProperty("choices"u8)[0]
            .GetProperty("message"u8)
            .GetProperty("content"u8)
            .GetString();

        Console.WriteLine($"[ASSISTANT]: {message}");
    }
}
