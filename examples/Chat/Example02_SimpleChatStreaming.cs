using NUnit.Framework;
using OpenAI.Chat;
using System;
using System.ClientModel;

namespace OpenAI.Examples;

public partial class ChatExamples
{
    [Test]
    public void Example02_SimpleChatStreaming()
    {
        ChatClient client = new(model: "gpt-4o", Environment.GetEnvironmentVariable("OPENAI_API_KEY"));

        CollectionResult<StreamingChatCompletionUpdate> updates
            = client.CompleteChatStreaming("Say 'this is a test.'");

        Console.WriteLine($"[ASSISTANT]:");
        foreach (StreamingChatCompletionUpdate update in updates)
        {
            foreach (ChatMessageContentPart updatePart in update.ContentUpdate)
            {
                Console.Write(updatePart);
            }
        }
    }
}
