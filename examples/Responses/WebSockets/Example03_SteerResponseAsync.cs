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
    /// <summary>Demonstrates redirecting an in-progress response over the Responses WebSocket.</summary>
    /// <remarks>
    /// Steering is sent after the initial response is created. The server may finish that response or mark it
    /// incomplete with reason <c>steered</c>, then creates a continuation that incorporates the new instruction.
    /// Mid-turn steering requires a GPT-6 model, such as <c>gpt-6-astra</c> or <c>gpt-6-luna</c>;
    /// GPT-5.6 and earlier models do not support it. See the
    /// <see href="https://developers.openai.com/api/docs/guides/steering">steering guide</see>.
    /// </remarks>
    [Test]
    [Explicit("Requires access to a GPT-6 model that supports steering.")]
    public async Task Example03_SteerResponseAsync()
    {
        // Steering may finish the original output item before generating the continuation.
        using CancellationTokenSource timeout = new(TimeSpan.FromMinutes(5));
        await using ResponsesWebSocketClient client = new(apiKey: Environment.GetEnvironmentVariable("OPENAI_API_KEY"));
        ResponseResult result = await CreateSteeredResponseAsync(client, timeout.Token);
        Console.WriteLine($"[ASSISTANT]: {result.GetOutputText()}");
        await client.CloseAsync(timeout.Token);
    }

    internal static async Task<ResponseResult> CreateSteeredResponseAsync(
        ResponsesWebSocketClient client,
        CancellationToken cancellationToken)
    {
        SteeredResponse steered = new(client, cancellationToken);
        client.MessageReceivedAsync += steered.OnMessageReceivedAsync;
        client.ErrorOccurredAsync += steered.OnErrorOccurredAsync;

        await client.ConnectAsync(cancellationToken);
        await client.CreateResponseAsync(
            "gpt-6-astra",
            "Draft a short three-day travel itinerary for Japan.",
            cancellationToken: cancellationToken);
        try
        {
            // Acceptance queues the steering input; the continuation contains the steered result.
            return await WaitForResponseAsync(client, steered.Response, cancellationToken);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw new TimeoutException(
                $"Timed out waiting for the steered response. Initial: {steered.InitialResponseId ?? "none"}, continuation: {steered.ContinuationResponseId ?? "none"}.");
        }
    }

    /// <summary>Steers the first created response, then completes with its continuation.</summary>
    private sealed class SteeredResponse(ResponsesWebSocketClient client, CancellationToken cancellationToken)
    {
        private readonly TaskCompletionSource<ResponseResult> _response = new(TaskCreationOptions.RunContinuationsAsynchronously);

        public Task<ResponseResult> Response => _response.Task;
        public string InitialResponseId { get; private set; }
        public string ContinuationResponseId { get; private set; }

        public async Task OnMessageReceivedAsync(ResponsesWebSocketMessageReceivedEventArgs args)
        {
            ResponsesWebSocketMessage message = args.Message;
            if (IsSteeredInitialResponse(message.Update))
            {
                // Steering can interrupt the original response before creating the continuation.
                return;
            }

            if (GetResponseError(message) is Exception error)
            {
                _response.TrySetException(error);
                return;
            }

            switch (message.Update)
            {
                case StreamingResponseCreatedUpdate created when InitialResponseId is null:
                    InitialResponseId = created.Response.Id;
                    await client.SteerResponseAsync(
                        InitialResponseId,
                        "Focus on Kyoto and include only activities reachable by train.",
                        cancellationToken);
                    break;

                case StreamingResponseCreatedUpdate created:
                    ContinuationResponseId = created.Response.Id;
                    break;

                case StreamingResponseCompletedUpdate completed
                    when ContinuationResponseId is not null && completed.Response.Id == ContinuationResponseId:
                    _response.TrySetResult(completed.Response);
                    break;
            }
        }

        public Task OnErrorOccurredAsync(ResponsesWebSocketErrorEventArgs args)
        {
            _response.TrySetException(args.Exception);
            return Task.CompletedTask;
        }

        private bool IsSteeredInitialResponse(StreamingResponseUpdate update)
            => update is StreamingResponseIncompleteUpdate incomplete
                && InitialResponseId is not null
                && incomplete.Response.Id == InitialResponseId
                && incomplete.Response.IncompleteStatusDetails?.Reason?.ToString() == "steered";
    }
}

#pragma warning restore OPENAI001
