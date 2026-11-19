using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;

namespace OpenAI.Responses;

/// <summary>Context for an awaitable message callback.</summary>
[Experimental("OPENAI001")]
public class ResponsesWebSocketMessageReceivedEventArgs : EventArgs
{
    protected ResponsesWebSocketMessageReceivedEventArgs()
    {
    }

    internal ResponsesWebSocketMessageReceivedEventArgs(ResponsesWebSocketMessage message, CancellationToken cancellationToken)
    {
        Message = message;
        CancellationToken = cancellationToken;
    }

    public virtual ResponsesWebSocketMessage Message { get; }

    /// <summary>Signals cooperative cancellation of message processing during client shutdown.</summary>
    public virtual CancellationToken CancellationToken { get; }
}
