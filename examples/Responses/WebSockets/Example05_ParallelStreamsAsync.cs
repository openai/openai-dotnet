using NUnit.Framework;
using OpenAI.Responses;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace OpenAI.Examples;

// This example uses experimental APIs which are subject to change. To use experimental APIs,
// please acknowledge their experimental status by suppressing the corresponding warning.
#pragma warning disable OPENAI001

public partial class ResponsesWebSocketExamples
{
    /// <summary>Runs independent responses concurrently on one WebSocket connection.</summary>
    /// <remarks>
    /// Stream IDs route interleaved events; they do not establish conversation history.
    /// Use a previous response ID to continue a conversation.
    /// </remarks>
    [Test]
    [Explicit("Requires access to gpt-6-astra.")]
    public async Task Example05_ParallelStreamsAsync()
    {
        using CancellationTokenSource timeout = new(TimeSpan.FromMinutes(2));
        await using ResponsesWebSocketClient client = new(apiKey: Environment.GetEnvironmentVariable("OPENAI_API_KEY"));

        ResponseResult[] responses = await CreateParallelResponsesAsync(client, timeout.Token);
        Console.WriteLine($"[travel]: {responses[0].GetOutputText()}");
        Console.WriteLine($"[food]: {responses[1].GetOutputText()}");
        await client.CloseAsync(timeout.Token);
    }

    internal static async Task<ResponseResult[]> CreateParallelResponsesAsync(
        ResponsesWebSocketClient client,
        CancellationToken cancellationToken)
    {
        Dictionary<string, TaskCompletionSource<ResponseResult>> streams = new()
        {
            ["travel"] = new(TaskCreationOptions.RunContinuationsAsynchronously),
            ["food"] = new(TaskCreationOptions.RunContinuationsAsynchronously),
        };

        void FailAll(Exception error)
        {
            foreach (TaskCompletionSource<ResponseResult> response in streams.Values)
            {
                response.TrySetException(error);
            }
        }

        client.MessageReceivedAsync += args =>
        {
            if (GetResponseError(args.Message) is Exception error)
            {
                FailAll(error);
            }
            else if (args.Message.Update is StreamingResponseCompletedUpdate completed)
            {
                if (args.Message.StreamId is string streamId && streams.TryGetValue(streamId, out var response))
                {
                    response.TrySetResult(completed.Response);
                }
                else
                {
                    FailAll(new InvalidOperationException("Received a completed response for an unknown stream."));
                }
            }

            return Task.CompletedTask;
        };
        client.ErrorOccurredAsync += args =>
        {
            FailAll(args.Exception);
            return Task.CompletedTask;
        };

        await client.ConnectAsync(cancellationToken);
        await client.CreateResponseAsync(
            "gpt-6-astra",
            "Suggest one place to visit in Kyoto in one sentence.",
            new ResponsesWebSocketSendOptions { StreamId = "travel" },
            cancellationToken: cancellationToken);
        await client.CreateResponseAsync(
            "gpt-6-astra",
            "Suggest one dish to try in Kyoto in one sentence.",
            new ResponsesWebSocketSendOptions { StreamId = "food" },
            cancellationToken: cancellationToken);

        Task<ResponseResult[]> responses = Task.WhenAll(streams["travel"].Task, streams["food"].Task);
        await WaitForResponseAsync(client, responses, cancellationToken, "The WebSocket closed before both responses completed.");
        return await responses;
    }
}

#pragma warning restore OPENAI001
