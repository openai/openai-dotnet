using NUnit.Framework;
using OpenAI.Responses;
using System;

namespace OpenAI.Examples;

// This example uses experimental APIs which are subject to change. To use experimental APIs,
// please acknowledge their experimental status by suppressing the corresponding warning.
#pragma warning disable OPENAI001
#pragma warning disable SCME0001 // Type is for evaluation purposes only and is subject to change or removal in future updates.

public partial class ResponseExamples
{
    [Test]
    public void Example09_ModelOverridePerRequest()
    {
        OpenAIResponseClient client = new(model: "gpt-4o", apiKey: Environment.GetEnvironmentVariable("OPENAI_API_KEY"));

        // Add extra request fields using Patch.
        // Patch lets you set fields like `model` that aren't exposed on ResponseCreationOptions.
        // See the API docs https://platform.openai.com/docs/api-reference/responses/create for supported additional fields.
        ResponseCreationOptions options = new();
        options.Patch.Set("$.model"u8, "gpt-5");

        OpenAIResponse response = client.CreateResponse("Say 'this is a test.", options);

        Console.WriteLine($"[ASSISTANT]: {response.GetOutputText()}, [Mode]: {response.Model}");
    }
}

#pragma warning disable SCME0001 // Type is for evaluation purposes only and is subject to change or removal in future updates.
#pragma warning restore OPENAI001
