using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace OpenAI.Responses;

/// <summary>Configures a Responses API WebSocket client.</summary>
[Experimental("OPENAI001")]
public class ResponsesWebSocketClientOptions
{
    /// <summary>Whether to reconnect after a recoverable transport failure. Defaults to true. Commands are never replayed.</summary>
    public bool ShouldReconnect { get; set; } = true;

    /// <summary>Maximum queued incoming messages, excluding the active callback. Defaults to 1024. Must be positive.</summary>
    /// <remarks>Overflow terminates the client with an error rather than dropping ordered updates.</remarks>
    public int MaxBufferedMessages { get; set; } = 1024;

    /// <summary>Maximum queued incoming UTF-8 payload bytes. Zero (the default) disables this limit.</summary>
    public long MaxBufferedBytes { get; set; }

    /// <summary>Maximum UTF-8 payload bytes in one incoming or outgoing message. Zero (the default) disables this limit.</summary>
    public int MaxMessageBytes { get; set; }

    /// <summary>Additional upgrade request headers. Authentication headers remain owned by the parent client.</summary>
    public IDictionary<string, string> Headers { get; } = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

}