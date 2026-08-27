using System.Diagnostics.CodeAnalysis;

namespace OpenAI.Responses;

// CUSTOM:
// - Added Experimental attribute.
// - Renamed.
// - Plain enum type, with Unknown, to convert from an underlying extensible enum.
[Experimental("OPENAI001")]
public enum MessageRole
{
    Unknown = 0,
    Assistant = 1,
    Developer = 2,
    System = 3,
    User = 4,
}