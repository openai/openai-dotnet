using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;

namespace OpenAI.Responses;

/// <summary>A complete server message with routing metadata and its original payload.</summary>
/// <remarks>Instances are created by the receive loop.</remarks>
[Experimental("OPENAI001")]
public class ResponsesWebSocketMessage
{
    protected ResponsesWebSocketMessage()
    {
    }

    internal ResponsesWebSocketMessage(ResponsesWebSocketMessageKind kind, string streamId,
        StreamingResponseUpdate update, ResponseError error, int? status, BinaryData rawData)
    {
        Kind = kind;
        StreamId = streamId;
        Update = update;
        Error = error;
        Status = status;
        RawData = rawData;
    }

    /// <summary>The wire event type, including event types unknown to this SDK version.</summary>
    public virtual ResponsesWebSocketMessageKind Kind { get; }

    /// <summary>The server lane, or null for an unlabelled event. Does not identify a response chain.</summary>
    public virtual string StreamId { get; }

    /// <summary>The typed Responses streaming update, or null for messages outside the existing streaming model.</summary>
    public virtual StreamingResponseUpdate Update { get; }

    /// <summary>The nested error for a WebSocket error message, otherwise null.</summary>
    public virtual ResponseError Error { get; }

    /// <summary>The status on a WebSocket error message, if supplied.</summary>
    public virtual int? Status { get; }

    /// <summary>The complete original JSON, preserving unknown types and fields. May contain sensitive customer data.</summary>
    public virtual BinaryData RawData { get; }
}

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

/// <summary>Context for a local failure, distinct from a server protocol error message.</summary>
[Experimental("OPENAI001")]
public class ResponsesWebSocketErrorEventArgs : EventArgs
{
    protected ResponsesWebSocketErrorEventArgs()
    {
    }

    internal ResponsesWebSocketErrorEventArgs(Exception exception, bool isTerminal, CancellationToken cancellationToken)
    {
        Exception = exception;
        IsTerminal = isTerminal;
        CancellationToken = cancellationToken;
    }

    public virtual Exception Exception { get; }

    /// <summary>Whether this failure stops client processing without further automatic recovery, even when reconnect is enabled.</summary>
    public virtual bool IsTerminal { get; }

    /// <summary>Signals cooperative cancellation of error processing when the client shuts down.</summary>
    public virtual CancellationToken CancellationToken { get; }
}
