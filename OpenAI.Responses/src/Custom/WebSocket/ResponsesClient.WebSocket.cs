using System;
using System.ClientModel.Primitives;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace OpenAI.Responses;

public partial class ResponsesClient
{
    private ResponseWebSocketHandshake _webSocketHandshake;

    internal void SetWebSocketHandshake(ResponseWebSocketHandshake handshake) => _webSocketHandshake = handshake;

    /// <summary>Opens a Responses API WebSocket using this client's endpoint, policies, and credentials.</summary>
    /// <remarks>
    /// The cancellation token bounds opening only. Dispose the returned connection to end its lifetime.
    /// A custom HTTP transport requires an explicit <see cref="ResponseWebSocketOptions.Connector"/>.
    /// Clients built from an opaque, prebuilt pipeline do not support this method.
    /// </remarks>
    [Experimental("OPENAI001")]
    public virtual async Task<ResponseWebSocketConnection> ConnectWebSocketAsync(
        ResponseWebSocketOptions options = null, CancellationToken cancellationToken = default)
    {
        if (_webSocketHandshake == null)
            throw new NotSupportedException("A client built from a prebuilt pipeline cannot expose its authentication and transport policies for a WebSocket upgrade. Construct the client with its authentication policy and options.");
        options ??= new ResponseWebSocketOptions();
        var connectionOptions = options.Snapshot();
        var socket = await _webSocketHandshake.ConnectAsync(_endpoint, connectionOptions, cancellationToken).ConfigureAwait(false);
        return new ResponseWebSocketConnection(socket, connectionOptions, token => ConnectWebSocketAsync(options, token));
    }
}
