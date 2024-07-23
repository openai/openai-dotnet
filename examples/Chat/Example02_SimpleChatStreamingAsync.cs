using NUnit.Framework;
using OpenAI.Chat;
using System;
using System.ClientModel;
using System.Threading.Tasks;

namespace OpenAI.Examples;

public partial class ChatExamples
{
    [Test]
    public async Task Example02_SimpleChatStreamingAsync()
    {
        ChatClient client = new(model: "gpt-4o", Environment.GetEnvironmentVariable("OPENAI_API_KEY"));

        AsyncCollectionResult<StreamingChatCompletionUpdate> updates
            = client.CompleteChatStreamingAsync("Say 'this is a test.'");

        Console.WriteLine($"[ASSISTANT]:");
        await foreach (StreamingChatCompletionUpdate update in updates)
        {
            foreach (ChatMessageContentPart updatePart in update.ContentUpdate)
            {
                Console.Write(updatePart.Text);
            }
        }
    }
}
