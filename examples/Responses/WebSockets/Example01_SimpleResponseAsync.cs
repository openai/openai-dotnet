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
    public async Task Example01_SimpleResponseAsync()
    {
        using CancellationTokenSource timeout = new(TimeSpan.FromMinutes(2));
        await using ResponsesWebSocketClient client = new(apiKey: Environment.GetEnvironmentVariable("OPENAI_API_KEY"));
        TaskCompletionSource<ResponseResult> response = new(TaskCreationOptions.RunContinuationsAsynchronously);

        // Subscribe before connecting. Server errors arrive through MessageReceivedAsync.
        client.MessageReceivedAsync += args =>
        {
            if (GetResponseError(args.Message) is Exception error)
            {
                response.TrySetException(error);
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
        await client.CreateResponseAsync("gpt-5", "Say 'this is a test.'", cancellationToken: timeout.Token);

        // CreateResponseAsync only writes the command; response.completed supplies the result.
        ResponseResult result = await WaitForResponseAsync(client, response.Task, timeout.Token);
        Console.WriteLine($"[ASSISTANT]: {result.GetOutputText()}");
        await client.CloseAsync(timeout.Token);
    }
}

#pragma warning restore OPENAI001
