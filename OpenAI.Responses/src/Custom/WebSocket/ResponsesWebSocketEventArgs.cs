using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;

namespace OpenAI.Responses;

/// <summary>A complete server message with routing metadata and its original payload.</summary>
/// <remarks>API proposal only. Instances will be created by the receive loop.</remarks>
[Experimental("OPENAI001")]
public class ResponsesWebSocketMessage
{
    protected ResponsesWebSocketMessage()
    {
    }

    /// <summary>The wire event type, including event types unknown to this SDK version.</summary>
    public virtual ResponsesWebSocketMessageKind Kind => throw new NotImplementedException("Responses WebSocket API proposal only.");

    /// <summary>The server lane, or null for an unlabelled event. Does not identify a response chain.</summary>
    public virtual string StreamId => throw new NotImplementedException("Responses WebSocket API proposal only.");

    /// <summary>The typed Responses streaming update, or null for messages outside the existing streaming model.</summary>
    public virtual StreamingResponseUpdate Update => throw new NotImplementedException("Responses WebSocket API proposal only.");

    /// <summary>The nested error for a WebSocket error message, otherwise null.</summary>
    public virtual ResponseError Error => throw new NotImplementedException("Responses WebSocket API proposal only.");

    /// <summary>The status on a WebSocket error message, if supplied.</summary>
    public virtual int? Status => throw new NotImplementedException("Responses WebSocket API proposal only.");

    /// <summary>The complete original JSON, preserving unknown types and fields. May contain sensitive customer data.</summary>
    public virtual BinaryData RawData => throw new NotImplementedException("Responses WebSocket API proposal only.");
}

/// <summary>Context for an awaitable message callback.</summary>
[Experimental("OPENAI001")]
public class ResponsesWebSocketMessageReceivedEventArgs : EventArgs
{
    protected ResponsesWebSocketMessageReceivedEventArgs()
    {
    }

    public virtual ResponsesWebSocketMessage Message => throw new NotImplementedException("Responses WebSocket API proposal only.");

    /// <summary>Signals cooperative cancellation of message processing during client shutdown.</summary>
    public virtual CancellationToken CancellationToken => throw new NotImplementedException("Responses WebSocket API proposal only.");
}

/// <summary>Context for a local failure, distinct from a server protocol error message.</summary>
[Experimental("OPENAI001")]
public class ResponsesWebSocketErrorEventArgs : EventArgs
{
    protected ResponsesWebSocketErrorEventArgs()
    {
    }

    public virtual Exception Exception => throw new NotImplementedException("Responses WebSocket API proposal only.");

    /// <summary>Whether this failure stops client processing without further automatic recovery, even when reconnect is enabled.</summary>
    public virtual bool IsTerminal => throw new NotImplementedException("Responses WebSocket API proposal only.");
}
