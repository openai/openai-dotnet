using NUnit.Framework;
using OpenAI.Chat;
using System;

namespace OpenAI.Examples;

public partial class ChatExamples
{
    [Test]
    public void Example01_SimpleChat()
    {
        ChatClient client = new(model: "gpt-4o", apiKey: Environment.GetEnvironmentVariable("OPENAI_API_KEY"));

        ChatCompletion completion = client.CompleteChat("Say 'this is a test.'");

        Console.WriteLine($"[ASSISTANT]: {completion}");
    }
}
