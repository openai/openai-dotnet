using NUnit.Framework;
using OpenAI.Chat;
using System;

namespace OpenAI.Samples;

public partial class ChatSamples
{
    [Test]
    public void Sample01_SimpleChat()
    {
        ChatClient client = new("gpt-4o", Environment.GetEnvironmentVariable("OPENAI_API_KEY"));

        ChatCompletion chatCompletion = client.CompleteChat(
            [
                new UserChatMessage("Say 'this is a test.'"),
            ]);

        Console.WriteLine($"[ASSISTANT]:");
        Console.WriteLine($"{chatCompletion.Content[0].Text}");
    }
}
