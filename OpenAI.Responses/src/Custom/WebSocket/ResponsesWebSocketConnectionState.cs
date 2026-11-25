using System.Diagnostics.CodeAnalysis;

namespace OpenAI.Responses;

/// <summary>The logical state of an event-driven Responses WebSocket client.</summary>
/// <remarks>
/// Describes the lifetime of a <see cref="ResponsesWebSocketClient"/>, not a single underlying WebSocket.
/// Reconnecting can replace the underlying connection while preserving the client and its event subscriptions.
/// </remarks>
[Experimental("OPENAI001")]
public enum ResponsesWebSocketConnectionState
{
    /// <summary>The client has been created but has not started opening a connection.</summary>
    Created,

    /// <summary>The client is opening its initial connection.</summary>
    Connecting,

    /// <summary>The connection is open and the client can send commands and receive server messages.</summary>
    Connected,

    /// <summary>The client is attempting to replace a lost connection.</summary>
    /// <remarks>
    /// Reconnecting does not resume interrupted responses or replay commands; the application must explicitly
    /// restore conversation context.
    /// </remarks>
    Reconnecting,

    /// <summary>The client is shutting down its connection and stopping any reconnect attempts.</summary>
    Closing,

    /// <summary>The connection has ended and the client is no longer sending or receiving messages.</summary>
    Closed,

    /// <summary>The client has been disposed and its resources have been released.</summary>
    Disposed
}
