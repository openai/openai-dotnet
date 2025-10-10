﻿using NUnit.Framework;
using System;
using OpenAI.Responses;
using System.Threading.Tasks;

namespace OpenAI.Examples;

// This example uses experimental APIs which are subject to change. To use experimental APIs,
// please acknowledge their experimental status by suppressing the corresponding warning.
#pragma warning disable OPENAI001
#pragma warning disable SCME0001 // Type is for evaluation purposes only and is subject to change or removal in future updates.

public partial class ResponseExamples
{
    [Test]
    public async Task Example07_InputAdditionalPropertiesAsync()
    {
        OpenAIResponseClient client = new(model: "gpt-5", apiKey: Environment.GetEnvironmentVariable("OPENAI_API_KEY"));

        // Add extra request fields using Patch.
        // Patch lets you set fields like `reasoning.effort` and `text.verbosity` that aren’t modeled on ResponseCreationOptions in the request payload.
        // See the API docs https://platform.openai.com/docs/api-reference/responses/create for supported additional fields.
        ResponseCreationOptions options = new();
        options.Patch.Set("$.reasoning.effort"u8, "high");
        options.Patch.Set("$.text.verbosity"u8, "medium");

        OpenAIResponse response = await client.CreateResponseAsync("What is the answer to the ultimate question of life, the universe, and everything?", options);

        Console.WriteLine($"[ASSISTANT]: {response.GetOutputText()}");

        // Read extra fields from the response via Patch.
        // The service returns fields like `reasoning.effort` and `text.verbosity` that aren’t modeled on OpenAIResponse.
        // You can access their values by using the path with Patch.
        // See the API docs https://platform.openai.com/docs/api-reference/responses/object for supported additional fields.
        var effort = response.Patch.GetString("$.reasoning.effort"u8);
        var verbosity = response.Patch.GetString("$.text.verbosity"u8);
        Console.WriteLine($"effort={effort}, verbosity={verbosity}");
    }
}

#pragma warning disable SCME0001 // Type is for evaluation purposes only and is subject to change or removal in future updates.
#pragma warning restore OPENAI001
