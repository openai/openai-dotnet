﻿using NUnit.Framework;
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
    public void Example07_InputAdditionalProperties()
    {
        OpenAIResponseClient client = new(model: "gpt-5", apiKey: Environment.GetEnvironmentVariable("OPENAI_API_KEY"));

        ResponseCreationOptions options = new();
        options.Patch.Set("$.reasoning.effort"u8, "low");
        options.Patch.Set("$.text.verbosity"u8, "medium");

        Console.WriteLine($"JsonPatch input - {options.Patch}");
        OpenAIResponse response = client.CreateResponse("What is the answer to the ultimate question of life, the universe, and everything?");

        Console.WriteLine($"[ASSISTANT]: {response.GetOutputText()}");
        string effort = response.Patch.GetString("$.reasoning.effort"u8);
        string verbosity = response.Patch.GetString("$.text.verbosity"u8);
        Console.WriteLine($"effort - {effort}");
        Console.WriteLine($"verbosity - {verbosity}");
    }
}
#pragma warning disable SCME0001 // Type is for evaluation purposes only and is subject to change or removal in future updates.
#pragma warning restore OPENAI001
