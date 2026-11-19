using OpenAI.Responses;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace OpenAI.Examples;

#pragma warning disable OPENAI001

public partial class ResponsesWebSocketExamples
{
    private static Exception GetResponseError(ResponsesWebSocketMessage message)
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
        Task finished = await Task.WhenAny(response, client.Completion).WaitAsync(cancellationToken);
        if (finished != response && !response.IsCompleted)
        {
            await client.Completion;
        }

        // A normal close can precede dispatch of the queued response.completed callback.
        return await response.WaitAsync(cancellationToken);
    }
}

#pragma warning restore OPENAI001
