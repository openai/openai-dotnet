using NUnit.Framework;
using OpenAI.Chat;
using System;

namespace OpenAI.Samples;

public partial class ChatSamples
{
    [Test]
    [Ignore("Compilation validation only")]
    public void Sample01_SimpleChat()
    {
        ChatClient client = new("gpt-4o", Environment.GetEnvironmentVariable("OPENAI_API_KEY"));

        ChatCompletion chatCompletion = client.CompleteChat(
            [
                new UserChatMessage("Say 'this is a test.'"),
            ]);
    }
}
