using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;

namespace OpenAI.Responses;

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
