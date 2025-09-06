using NUnit.Framework;
using OpenAI.Responses;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OpenAI.Examples;

public partial class ResponseExamples
{
    [Test]
    public async Task Example03_StoredPromptsAsync()
    {
        OpenAIResponseClient client = new(model: "gpt-4o", apiKey: Environment.GetEnvironmentVariable("OPENAI_API_KEY"));

        // Create options using a stored prompt
        ResponseCreationOptions options = new()
        {
            Prompt = new ResponsePrompt
            {
                Id = "your-stored-prompt-id",
                Version = "v1.0"
            }
        };

        // Add variables to substitute in the prompt template
        options.Prompt.Variables["location"] = "San Francisco";
        options.Prompt.Variables["unit"] = "celsius";

        // Use stored prompt with variables
        IEnumerable<ResponseItem> inputItems = Array.Empty<ResponseItem>();
        OpenAIResponse response = await client.CreateResponseAsync(inputItems, options);

        Console.WriteLine($"[ASSISTANT]: {response.GetOutputText()}");
    }
}
