using NUnit.Framework;
using OpenAI.Chat;
using System;
using System.Threading.Tasks;

namespace OpenAI.Examples;

public partial class ChatExamples
{
    [Test]
    public async Task Example01_SimpleChatAsync()
    {
        ChatClient client = new("gpt-4o", Environment.GetEnvironmentVariable("OPENAI_API_KEY"));

        ChatCompletion chatCompletion = await client.CompleteChatAsync(
            [
                new UserChatMessage("Say 'this is a test.'"),
            ]);

        Console.WriteLine($"[ASSISTANT]:");
        Console.WriteLine($"{chatCompletion.Content[0].Text}");
    }
}
