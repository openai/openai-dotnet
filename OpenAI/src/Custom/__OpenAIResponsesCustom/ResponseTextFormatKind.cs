using System.Diagnostics.CodeAnalysis;

namespace OpenAI.Responses;

// CUSTOM:
// - Added Experimental attribute.
// - Renamed.
// - Plain enum type, with Unknown, to convert from an underlying extensible enum
[Experimental("OPENAI001")]
public enum ResponseTextFormatKind
{
    Unknown = 0,
    Text = 1,
    JsonObject = 2,
    JsonSchema = 3,
}