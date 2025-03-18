using NUnit.Framework;
using OpenAI.Responses;
using System;

namespace OpenAI.Examples;

public partial class ResponseExamples
{
    [Test]
    public void Example02_HelloWorldStreaming()
    {
        OpenAIResponseClient client = new(
            model: "gpt-4o-mini",
            apiKey: Environment.GetEnvironmentVariable("OPENAI_API_KEY"));

        Console.Write($"Streaming text: ");

        foreach (StreamingResponseUpdate update
            in client.CreateResponseStreaming("Hello, world!"))
        {
            if (update is StreamingResponseOutputTextDeltaUpdate outputTextUpdate)
            {
                // Streamed text will arrive as it's generated via delta events
                Console.Write(outputTextUpdate.Delta);
            }
            else if (update is StreamingResponseCompletedUpdate responseCompletedUpdate)
            {
                // Item and response completed events have aggregated text available
                Console.WriteLine();
                Console.WriteLine($"Final text: {responseCompletedUpdate.Response.GetOutputText()}");
            }
        }
    }
}
