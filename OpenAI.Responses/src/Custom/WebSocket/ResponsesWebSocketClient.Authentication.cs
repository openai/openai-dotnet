using System;
using System.ClientModel.Primitives;
using System.Net.WebSockets;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;

namespace OpenAI.Responses;

public partial class ResponsesWebSocketClient
{
    private async Task<WebSocket> ConnectThroughPipelineAsync(CancellationToken cancellationToken)
    {
        using WebSocketPipelineMessage message = (WebSocketPipelineMessage)_pipeline.CreateMessage();
        UriBuilder endpoint = new UriBuilder(_websocketEndpoint)
        {
            Scheme = _websocketEndpoint.Scheme switch
            {
                "ws" => "http",
                "wss" => "https",
                _ => throw new NotSupportedException("WebSocket endpoints require an HTTP or WebSocket URI."),
            },
        };
        message.Request.Uri = endpoint.Uri;
        message.Request.Method = "GET";
        message.Apply(new RequestOptions { CancellationToken = cancellationToken });
        foreach (var header in _options.Headers)
        {
            message.Request.Headers.Set(header.Key, header.Value);
        }

        await _pipeline.SendAsync(message).ConfigureAwait(false);
        if (message.HandshakeException is WebSocketException exception)
        {
            ExceptionDispatchInfo.Capture(exception).Throw();
        }

        return message.TakeSocket()
            ?? throw new InvalidOperationException("The WebSocket pipeline did not establish a connection.");
    }
}
