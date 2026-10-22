using System.Diagnostics.CodeAnalysis;

namespace OpenAI.Responses;

/// <summary>WebSocket-specific settings for sending a response request.</summary>
[Experimental("OPENAI001")]
public class ResponsesWebSocketSendOptions
{
    /// <summary>The ordered server stream. Null selects the default stream; an empty string is invalid.</summary>
    public string StreamId { get; set; }

    /// <summary>Maps to generate. Set false to warm up state without generating model output.</summary>
    public bool? GenerationEnabled { get; set; }
}
