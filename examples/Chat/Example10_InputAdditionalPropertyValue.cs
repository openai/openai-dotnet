using NUnit.Framework;
using OpenAI.Chat;
using System;
using System.Collections.Generic;

namespace OpenAI.Examples;

#pragma warning disable SCME0001 // Type is for evaluation purposes only and is subject to change or removal in future updates.
public partial class ChatExamples
{
    [Test]
    public void Example07_InputAdditionalPropertyValue()
    {
        ChatClient client = new(model: "gpt-5", apiKey: Environment.GetEnvironmentVariable("OPENAI_API_KEY"));

        // you can use the Patch property to set additional properties in the input
        ChatCompletionOptions options = new();
        options.Patch.Set("$.reasoning_effort"u8, "minimal");

        List<ChatMessage> messages =
        [
            new UserChatMessage("What's the weather like today?"),
        ];
        ChatCompletion completion = client.CompleteChat(messages, options);

        Console.WriteLine($"[ASSISTANT]: {completion.Content[0].Text}");

        // you can also read those properties back from the response
        var effort = completion.Patch.GetString("$.reasoning_effort"u8);
        Console.WriteLine($"effort={effort}");
    }
}

#pragma warning disable SCME0001 // Type is for evaluation purposes only and is subject to change or removal in future updates.
