using System.Diagnostics.CodeAnalysis;

namespace OpenAI.Moderations;

// CUSTOM:
// - Added Experimental attribute.
// - Renamed.
// - Plain enum type to convert from an underlying extensible enum
[Experimental("OPENAI001")]
public enum ModerationInputPartKind
{
    Text,
    Image
}
