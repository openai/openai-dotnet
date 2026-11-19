using NUnit.Framework;
using OpenAI.Responses;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace OpenAI.Examples;

// This example uses experimental APIs which are subject to change. To use experimental APIs,
// please acknowledge their experimental status by suppressing the corresponding warning.
#pragma warning disable OPENAI001

public partial class ResponsesWebSocketExamples
{
    /// <summary>Runs two conversations in parallel, then forks one onto a new stream.</summary>
    /// <remarks>
    /// The fork and the original branch both continue from the same previous response ID.
    /// Stream IDs only route events; history comes from the previous response ID.
    /// Mirrors the <see href="https://developers.openai.com/api/docs/guides/websocket-mode#complete-example">WebSocket mode complete example</see>.
    /// </remarks>
    [Test]
    [Explicit("Requires access to gpt-6-astra.")]
    public async Task Example06_ForkConversationAsync()
    {
        using CancellationTokenSource timeout = new(TimeSpan.FromMinutes(5));
        await using ResponsesWebSocketClient client = new(apiKey: Environment.GetEnvironmentVariable("OPENAI_API_KEY"));

        foreach ((string streamId, ResponseResult response) in await CreateForkedConversationsAsync(client, timeout.Token))
        {
            Console.WriteLine($"[{streamId}]: {response.GetOutputText()}");
        }
        await client.CloseAsync(timeout.Token);
    }

    internal static async Task<IReadOnlyList<(string StreamId, ResponseResult Response)>> CreateForkedConversationsAsync(
        ResponsesWebSocketClient client,
        CancellationToken cancellationToken)
    {
        ConversationStreams streams = new(client, cancellationToken);
        client.MessageReceivedAsync += streams.OnMessageReceivedAsync;
        client.ErrorOccurredAsync += streams.OnErrorOccurredAsync;
        await client.ConnectAsync(cancellationToken);

        // Run two independent conversations in parallel.
        await streams.CreateAsync("planner", "Draft a deployment plan for a stateless API service.");
        await streams.CreateAsync("research", "List common deployment risks for a stateless API service.");
        ResponseResult[] first = await streams.WaitForCompletedAsync("planner", "research");

        // Fork the planner conversation and continue its original branch in parallel.
        string plannerResponseId = first[0].Id;
        await streams.CreateAsync("critic", "Find gaps in this deployment plan.", plannerResponseId);
        // Let the fork load its parent before advancing the original stream.
        await streams.WaitForInProgressAsync("critic");
        await streams.CreateAsync("planner", "Add rollback and monitoring steps to the plan.", plannerResponseId);
        ResponseResult[] second = await streams.WaitForCompletedAsync("critic", "planner");

        return
        [
            ("planner", first[0]),
            ("research", first[1]),
            ("critic", second[0]),
            ("planner", second[1]),
        ];
    }

    /// <summary>Tracks the in-flight response for each stream ID.</summary>
    private sealed class ConversationStreams(ResponsesWebSocketClient client, CancellationToken cancellationToken)
    {
        private readonly ConcurrentDictionary<string, TaskCompletionSource> _inProgress = new();
        private readonly ConcurrentDictionary<string, TaskCompletionSource<ResponseResult>> _completed = new();

        public Task CreateAsync(string streamId, string text, string previousResponseId = null)
        {
            // Register before sending so early events are not missed.
            _inProgress[streamId] = new(TaskCreationOptions.RunContinuationsAsynchronously);
            _completed[streamId] = new(TaskCreationOptions.RunContinuationsAsynchronously);

            CreateResponseOptions options = new("gpt-6-astra", [ResponseItem.CreateUserMessageItem(text)])
            {
                StoredOutputEnabled = false,
                PreviousResponseId = previousResponseId,
            };
            return client.CreateResponseAsync(options, new ResponsesWebSocketSendOptions { StreamId = streamId }, cancellationToken);
        }

        public Task WaitForInProgressAsync(string streamId)
            => WaitForResponseAsync(client, _inProgress[streamId].Task, cancellationToken);

        public async Task<ResponseResult[]> WaitForCompletedAsync(params string[] streamIds)
        {
            Task<ResponseResult[]> responses = Task.WhenAll(streamIds.Select(id => _completed[id].Task));
            await WaitForResponseAsync(client, responses, cancellationToken);
            return await responses;
        }

        public Task OnMessageReceivedAsync(ResponsesWebSocketMessageReceivedEventArgs args)
        {
            if (GetResponseError(args.Message) is Exception error)
            {
                FailAll(error);
            }
            else if (args.Message.StreamId is string streamId)
            {
                if (args.Message.Update is StreamingResponseInProgressUpdate
                    && _inProgress.TryGetValue(streamId, out TaskCompletionSource started))
                {
                    started.TrySetResult();
                }
                else if (args.Message.Update is StreamingResponseCompletedUpdate completed
                    && _completed.TryGetValue(streamId, out TaskCompletionSource<ResponseResult> response))
                {
                    response.TrySetResult(completed.Response);
                }
            }

            return Task.CompletedTask;
        }

        public Task OnErrorOccurredAsync(ResponsesWebSocketErrorEventArgs args)
        {
            FailAll(args.Exception);
            return Task.CompletedTask;
        }

        private void FailAll(Exception error)
        {
            foreach (TaskCompletionSource started in _inProgress.Values)
            {
                started.TrySetException(error);
            }
            foreach (TaskCompletionSource<ResponseResult> response in _completed.Values)
            {
                response.TrySetException(error);
            }
        }
    }
}

#pragma warning restore OPENAI001
