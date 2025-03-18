using NUnit.Framework;
using OpenAI.Responses;
using System;

namespace OpenAI.Examples;

public partial class ResponseExamples
{
    [Test]
    public void Example01_HelloWorld()
    {
        OpenAIResponseClient client = new(model: "gpt-4o", apiKey: Environment.GetEnvironmentVariable("OPENAI_API_KEY"));

        OpenAIResponse response = client.CreateResponse("Say 'this is a test.'");

        Console.WriteLine($"[ASSISTANT]: {response.GetOutputText()}");
    }
}
