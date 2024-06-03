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
        ChatClient client = new("gpt-4o", Environment.GetEnvironmentVariable("OPENAI_API_KEY"));

        AsyncResultCollection<StreamingChatCompletionUpdate> asyncChatUpdates
            = client.CompleteChatStreamingAsync(
                [
                    new UserChatMessage("Say 'this is a test.'"),
                ]);

        Console.WriteLine($"[ASSISTANT]:");
        await foreach (StreamingChatCompletionUpdate chatUpdate in asyncChatUpdates)
        {
            foreach (ChatMessageContentPart contentPart in chatUpdate.ContentUpdate)
            {
                Console.Write(contentPart.Text);
            }
        }
    }
}
