#nullable enable

using System;
using System.Diagnostics.CodeAnalysis;

namespace OpenAI.Responses;

/// <summary>WebSocket-specific settings for sending a response request.</summary>
[Experimental("OPENAI001")]
public class ResponsesWebSocketSendOptions
{
    private string? _streamId;

    /// <summary>The ordered server stream. Null selects the default stream; an empty string is invalid.</summary>
    /// <exception cref="ArgumentException">The value is empty, exceeds 256 characters, or contains characters other than ASCII letters, digits, underscores, hyphens, and periods.</exception>
    public string? StreamId
    {
        get => _streamId;
        set
        {
            if (value is not null)
            {
                if (value.Length == 0)
                {
                    throw new ArgumentException("StreamId must not be empty.", nameof(value));
                }
                if (value.Length > 256)
                {
                    throw new ArgumentException("StreamId must be at most 256 characters.", nameof(value));
                }
                foreach (char character in value)
                {
                    if (!(character >= 'a' && character <= 'z') && !(character >= 'A' && character <= 'Z')
                        && !(character >= '0' && character <= '9') && character != '_' && character != '-' && character != '.')
                    {
                        throw new ArgumentException("StreamId may contain only ASCII letters, digits, underscores, hyphens, and periods.", nameof(value));
                    }
                }
            }
            _streamId = value;
        }
    }

    /// <summary>Maps to generate. Set false to warm up state without generating model output.</summary>
    public bool? GenerationEnabled { get; set; }
}
