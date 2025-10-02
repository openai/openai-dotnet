using NUnit.Framework;
using OpenAI.Chat;
using System;

namespace OpenAI.Examples;

#pragma warning disable SCME0001 // Type is for evaluation purposes only and is subject to change or removal in future updates.

public partial class ChatExamples
{
    [Test]
    public void Example10_AdditionalProperties()
    {
        ChatClient client = new(model: "gpt-5", apiKey: Environment.GetEnvironmentVariable("OPENAI_API_KEY"));

        // You can use the Patch property to set additional properties in the request
        ChatCompletionOptions options = new();
        options.Patch.Set("$.reasoning_effort"u8, "minimal");

        ChatCompletion completion = client.CompleteChat([new UserChatMessage("Say 'this is a test.'")], options);

        Console.WriteLine($"[ASSISTANT]: {completion.Content[0].Text}");

        // You can also read additional properties back from the response
        var serviceTier = completion.Patch.GetString("$.service_tier"u8);
        Console.WriteLine($"service_tier={serviceTier}");
    }
}

#pragma warning disable SCME0001 // Type is for evaluation purposes only and is subject to change or removal in future updates.
