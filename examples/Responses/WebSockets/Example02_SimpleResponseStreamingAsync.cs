using NUnit.Framework;
using OpenAI.Responses;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace OpenAI.Examples;

// This example uses experimental APIs which are subject to change. To use experimental APIs,
// please acknowledge their experimental status by suppressing the corresponding warning.
#pragma warning disable OPENAI001

public partial class ResponsesWebSocketExamples
{
    /// <summary>Prints response text as it is generated.</summary>
    /// <remarks>
    /// WebSocket responses always stream; <see cref="CreateResponseOptions.StreamingEnabled"/> does not apply and must be unset.
    /// Unlike Example01_SimpleResponseAsync, which waits for <c>response.completed</c>, this sample writes each text delta as it arrives.
    /// </remarks>
    [Test]
    public async Task Example02_SimpleResponseStreamingAsync()
    {
        using CancellationTokenSource timeout = new(TimeSpan.FromMinutes(2));
        await using ResponsesWebSocketClient client = new(apiKey: Environment.GetEnvironmentVariable("OPENAI_API_KEY"));
        TaskCompletionSource completion = new(TaskCreationOptions.RunContinuationsAsynchronously);

        client.MessageReceivedAsync += args =>
        {
            if (GetResponseError(args.Message) is Exception error)
            {
                completion.TrySetException(error);
            }
            else if (args.Message.Update is StreamingResponseOutputTextDeltaUpdate delta)
            {
                Console.Write(delta.Delta);
            }
            else if (args.Message.Update is StreamingResponseCompletedUpdate)
            {
                completion.TrySetResult();
            }

            return Task.CompletedTask;
        };
        client.ErrorOccurredAsync += args =>
        {
            completion.TrySetException(args.Exception);
            return Task.CompletedTask;
        };

        await client.ConnectAsync(timeout.Token);
        Console.Write("[ASSISTANT]: ");
        await client.CreateResponseAsync("gpt-5", "Say 'this is a test.'", cancellationToken: timeout.Token);
        await WaitForResponseAsync(client, completion.Task, timeout.Token);
        Console.WriteLine();
        await client.CloseAsync(timeout.Token);
    }
}

#pragma warning restore OPENAI001
