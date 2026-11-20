using OpenAI.Responses;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace OpenAI.Examples;

#pragma warning disable OPENAI001

/// <summary>Responses WebSocket samples. Each sample is in its own <c>ExampleNN_*.cs</c> file.</summary>
/// <remarks>This file contains only shared helpers for error handling and waiting for completion.</remarks>
public partial class ResponsesWebSocketExamples
{
    private static InvalidOperationException GetResponseError(ResponsesWebSocketMessage message)
    {
        if (message.Kind == ResponsesWebSocketMessageKind.Error)
        {
            return new InvalidOperationException($"WebSocket protocol error: {message.Error?.Message}");
        }

        if (message.Update is StreamingResponseFailedUpdate failed)
        {
            return new InvalidOperationException($"Response failed: {failed.Response.Error?.Message}");
        }

        if (message.Update is StreamingResponseIncompleteUpdate)
        {
            return new InvalidOperationException("The response ended before completion.");
        }

        if (message.Kind == ResponsesWebSocketMessageKind.ResponseSteerFailed)
        {
            return new InvalidOperationException("The server rejected the steering request.");
        }

        return null;
    }

    private static async Task<ResponseResult> WaitForResponseAsync(
        ResponsesWebSocketClient client,
        Task<ResponseResult> response,
        CancellationToken cancellationToken)
    {
        await WaitForResponseAsync(client, (Task)response, cancellationToken);
        return await response;
    }

    internal static async Task WaitForResponseAsync(
        ResponsesWebSocketClient client,
        Task response,
        CancellationToken cancellationToken,
        string closedMessage = "The WebSocket closed before the response completed.")
    {
        Task finished = await Task.WhenAny(response, client.Completion).WaitAsync(cancellationToken);
        if (finished != response && !response.IsCompleted)
        {
            await client.Completion;
            await client.DisposeAsync().AsTask().WaitAsync(cancellationToken);
            if (!response.IsCompleted)
            {
                throw new InvalidOperationException(closedMessage);
            }
        }

        await response.WaitAsync(cancellationToken);
    }
}

#pragma warning restore OPENAI001
