using NUnit.Framework;
using OpenAI.Responses;
using System;
using System.ClientModel;
using System.Diagnostics;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace OpenAI.Examples;

// This example uses experimental APIs which are subject to change. To use experimental APIs,
// please acknowledge their experimental status by suppressing the corresponding warning.
#pragma warning disable OPENAI001

public partial class ResponsesWebSocketExamples
{
    /// <summary>Exercises repeated requests over a single long-lived WebSocket connection.</summary>
    /// <remarks>
    /// Sends 50 short, independent requests to <c>gpt-6-luna</c>, pausing ten seconds between responses.
    /// Reconnection is disabled so a replacement connection cannot hide a dropped connection.
    /// This opt-in example makes live API calls and may take several minutes.
    /// </remarks>
    [Test]
    [Explicit("Makes 50 live API requests over one connection.")]
    public async Task Example04_LongRunningSessionAsync()
    {
        using CancellationTokenSource timeout = new(TimeSpan.FromMinutes(30));
        await using ResponsesWebSocketClient client = new(
            new ApiKeyCredential(Environment.GetEnvironmentVariable("OPENAI_API_KEY")),
            new ResponsesWebSocketClientOptions { ShouldReconnect = false });

        await RunLongRunningSessionAsync(client, 50, TimeSpan.FromSeconds(10), timeout.Token);
        await client.CloseAsync(timeout.Token);
    }

    internal static async Task RunLongRunningSessionAsync(
        ResponsesWebSocketClient client,
        int responseCount,
        TimeSpan interval,
        CancellationToken cancellationToken)
    {
        Channel<ResponseResult> responses = Channel.CreateUnbounded<ResponseResult>(
            new UnboundedChannelOptions { SingleReader = true });
        using CancellationTokenSource session = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        Stopwatch elapsed = Stopwatch.StartNew();

        // Subscribe once for the session, rather than adding handlers for every request.
        client.MessageReceivedAsync += args =>
        {
            if (GetResponseError(args.Message) is Exception error)
            {
                responses.Writer.TryComplete(error);
            }
            else if (args.Message.Update is StreamingResponseCompletedUpdate completed)
            {
                responses.Writer.TryWrite(completed.Response);
            }

            return Task.CompletedTask;
        };
        client.ErrorOccurredAsync += args =>
        {
            responses.Writer.TryComplete(args.Exception);
            return Task.CompletedTask;
        };

        try
        {
            await client.ConnectAsync(session.Token);
            for (int index = 1; index <= responseCount; index++)
            {
                if (responses.Reader.Completion.IsCompleted)
                {
                    await responses.Reader.Completion;
                }

                await client.CreateResponseAsync(
                    "gpt-6-luna",
                    $"Reply with exactly: pong {index}.",
                    cancellationToken: session.Token);

                Task<ResponseResult> response = responses.Reader.ReadAsync(session.Token).AsTask();
                await WaitForResponseAsync(client, response, session.Token,
                    $"The WebSocket closed before the session finished (response {index}/{responseCount}).")
                    .WaitAsync(TimeSpan.FromMinutes(2), session.Token);
                ResponseResult result = await response;
                Console.WriteLine($"[{elapsed.Elapsed.TotalSeconds:F1}s] {index}/{responseCount}: {result.GetOutputText()}");
                if (index < responseCount)
                {
                    await Task.Delay(interval, session.Token);
                }
            }
        }
        finally
        {
            session.Cancel();
        }
    }
}

#pragma warning restore OPENAI001
