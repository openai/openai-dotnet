using NUnit.Framework;
using OpenAI.Responses;
using System;
using System.ClientModel;
using System.Threading.Tasks;

namespace OpenAI.Examples;

// This example uses experimental APIs which are subject to change. To use experimental APIs,
// please acknowledge their experimental status by suppressing the corresponding warning.

#pragma warning disable OPENAI001

public partial class ResponseExamples
{
    [Test]
    public async Task Example02_SimpleResponseStreamingAsync()
    {
        ResponsesClient client = new(apiKey: Environment.GetEnvironmentVariable("OPENAI_API_KEY"));

        AsyncCollectionResult<StreamingResponseUpdate> responseUpdates = client.CreateResponseStreamingAsync("gpt-5", "Say 'this is a test.'");

        Console.Write($"[ASSISTANT]: ");
        await foreach (StreamingResponseUpdate update in responseUpdates)
        {
            if (update is StreamingResponseOutputTextDeltaUpdate outputTextUpdate)
            {
                Console.Write(outputTextUpdate.Delta);
            }
        }
    }
}

#pragma warning restore OPENAI001