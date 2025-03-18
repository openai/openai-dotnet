using NUnit.Framework;
using OpenAI.Responses;
using System;
using System.Threading.Tasks;

namespace OpenAI.Examples;

public partial class ResponseExamples
{
    [Test]
    public async Task Example01_HelloWorldAsync()
    {
        OpenAIResponseClient client = new(model: "gpt-4o", apiKey: Environment.GetEnvironmentVariable("OPENAI_API_KEY"));

        OpenAIResponse response = await client.CreateResponseAsync("Say 'this is a test.'");

        Console.WriteLine($"[ASSISTANT]: {response.GetOutputText()}");
    }
}
