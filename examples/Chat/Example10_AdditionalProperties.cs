using NUnit.Framework;
using OpenAI.Chat;
using System;

namespace OpenAI.Examples;

#pragma warning disable SCME0001 // Type is for evaluation purposes only and is subject to change or removal in future updates.

public partial class ChatExamples
{
    public void Example10_AdditionalProperties()
    {
        ChatClient client = new(model: "gpt-5", apiKey: Environment.GetEnvironmentVariable("OPENAI_API_KEY"));

        // Add extra request fields using Patch.
        // Patch lets you set fields that aren’t modeled on ChatCompletionOptions in the request payload.
        // See the API docs https://platform.openai.com/docs/api-reference/chat/create for supported additional fields.
        ChatCompletionOptions options = new();
        options.Patch.Set("$.reasoning_effort"u8, "minimal");

        ChatCompletion completion = client.CompleteChat(
            [new UserChatMessage("Say 'this is a test.'")],
            options);

        Console.WriteLine($"[ASSISTANT]: {completion.Content[0].Text}");

        // Read extra fields from the response via Patch.
        // The service returns a field named `service_tier` that isn’t modeled on ChatCompletion.
        // You can access its value by using the path with Patch.
        // See the API docs https://platform.openai.com/docs/api-reference/chat/object for supported additional fields.
        var serviceTier = completion.Patch.GetString("$.service_tier"u8);
        Console.WriteLine($"service_tier={serviceTier}");
    }
}

#pragma warning disable SCME0001 // Type is for evaluation purposes only and is subject to change or removal in future updates.
