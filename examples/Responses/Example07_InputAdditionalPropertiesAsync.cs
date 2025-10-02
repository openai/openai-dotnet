using NUnit.Framework;
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

        // you can use the Patch property to set additional properties in the input
        ResponseCreationOptions options = new();
        options.Patch.Set("$.reasoning.effort"u8, "high");
        options.Patch.Set("$.text.verbosity"u8, "medium");

        OpenAIResponse response = await client.CreateResponseAsync("What is the answer to the ultimate question of life, the universe, and everything?", options);

        Console.WriteLine($"[ASSISTANT]: {response.GetOutputText()}");

        // you can also read those properties back from the response
        var effort = response.Patch.GetString("$.reasoning.effort"u8);
        var verbosity = response.Patch.GetString("$.text.verbosity"u8);
        Console.WriteLine($"effort={effort}, verbosity={verbosity}");
    }
}

#pragma warning disable SCME0001 // Type is for evaluation purposes only and is subject to change or removal in future updates.
#pragma warning restore OPENAI001
