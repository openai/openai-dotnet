using NUnit.Framework;
using OpenAI.Responses;
using System;

namespace OpenAI.Examples;

// This example uses experimental APIs which are subject to change. To use experimental APIs,
// please acknowledge their experimental status by suppressing the corresponding warning.
#pragma warning disable OPENAI001

public partial class ResponseExamples
{
    [Test]
    public void Example01_SimpleResponse()
    {
        OpenAIResponseClient client = new(model: "gpt-4o", apiKey: Environment.GetEnvironmentVariable("OPENAI_API_KEY"));

        OpenAIResponse response = client.CreateResponse("Say 'this is a test.'");

        Console.WriteLine($"[ASSISTANT]: {response.GetOutputText()}");
    }
}

#pragma warning restore OPENAI001