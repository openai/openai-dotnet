using System;
using System.Diagnostics.CodeAnalysis;

namespace OpenAI.Responses;

public partial class ResponsesClient
{
    /// <summary>Creates a disconnected Responses API WebSocket client using this client's endpoint and credentials.</summary>
    /// <remarks>
    /// Subscribe to MessageReceivedAsync and ErrorOccurredAsync before calling <see cref="ResponsesWebSocketClient.ConnectAsync"/>.
    /// Options are snapshotted when the client is created. The returned client owns its connection.
    /// The initial design uses default WebSocket networking and reuses this client's authentication and headers.
    /// Custom HTTP transports and opaque, prebuilt pipelines are not supported by the initial design.
    /// This API is a proposal stub and does not open a socket.
    /// </remarks>
    [Experimental("OPENAI001")]
    public virtual ResponsesWebSocketClient CreateWebSocketClient(ResponsesWebSocketClientOptions options = null)
        => throw new NotImplementedException("Responses WebSocket API proposal only.");
}