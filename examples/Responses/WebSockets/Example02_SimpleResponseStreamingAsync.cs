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
    [Test]
    public async Task Example02_SimpleResponseStreamingAsync()
    {
        using CancellationTokenSource timeout = new(TimeSpan.FromMinutes(2));
        await using ResponsesWebSocketClient client = new(apiKey: Environment.GetEnvironmentVariable("OPENAI_API_KEY"));
        TaskCompletionSource<ResponseResult> response = new(TaskCreationOptions.RunContinuationsAsynchronously);

        client.MessageReceivedAsync += args =>
        {
            if (GetResponseError(args.Message) is Exception error)
            {
                response.TrySetException(error);
            }
            else if (args.Message.Update is StreamingResponseOutputTextDeltaUpdate delta)
            {
                Console.Write(delta.Delta);
            }
            else if (args.Message.Update is StreamingResponseCompletedUpdate completed)
            {
                response.TrySetResult(completed.Response);
            }

            return Task.CompletedTask;
        };
        client.ErrorOccurredAsync += args =>
        {
            response.TrySetException(args.Exception);
            return Task.CompletedTask;
        };

        await client.ConnectAsync(timeout.Token);
        Console.Write("[ASSISTANT]: ");
        // WebSocket responses always stream; leave StreamingEnabled unset.
        await client.CreateResponseAsync("gpt-5", "Say 'this is a test.'", cancellationToken: timeout.Token);
        await WaitForResponseAsync(client, response.Task, timeout.Token);
        Console.WriteLine();
        await client.CloseAsync(timeout.Token);
    }
}

#pragma warning restore OPENAI001
