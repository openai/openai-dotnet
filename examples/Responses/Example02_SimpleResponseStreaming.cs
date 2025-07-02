using NUnit.Framework;
using OpenAI.Responses;
using System;
using System.ClientModel;

namespace OpenAI.Examples;

// This example uses experimental APIs which are subject to change. To use experimental APIs,
// please acknowledge their experimental status by suppressing the corresponding warning.
#pragma warning disable OPENAI001

public partial class ResponseExamples
{
    [Test]
    public void Example02_SimpleResponseStreaming()
    {
        OpenAIResponseClient client = new(model: "gpt-4o-mini", apiKey: Environment.GetEnvironmentVariable("OPENAI_API_KEY"));

        CollectionResult<StreamingResponseUpdate> responseUpdates = client.CreateResponseStreaming("Say 'this is a test.'");

        Console.Write($"[ASSISTANT]: ");
        foreach (StreamingResponseUpdate update in responseUpdates)
        {
            if (update is StreamingResponseOutputTextDeltaUpdate outputTextUpdate)
            {
                Console.Write(outputTextUpdate.Delta);
            }
        }
    }
}

#pragma warning restore OPENAI001