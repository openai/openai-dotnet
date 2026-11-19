using System;
using System.Diagnostics.CodeAnalysis;

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
