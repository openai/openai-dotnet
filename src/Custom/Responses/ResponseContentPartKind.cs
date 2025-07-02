using System.Diagnostics.CodeAnalysis;

namespace OpenAI.Responses;

// CUSTOM:
// - Added Experimental attribute.
// - Renamed.
// - Plain enum type, with Unknown, to convert from an underlying extensible enum
[Experimental("OPENAI001")]
public enum ResponseContentPartKind
{
    Unknown,
    InputText,
    InputImage,
    InputFile,
    OutputText,
    Refusal,
}