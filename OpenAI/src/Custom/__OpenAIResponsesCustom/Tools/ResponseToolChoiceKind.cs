using System.Diagnostics.CodeAnalysis;

namespace OpenAI.Responses;

// CUSTOM:
// - Added Experimental attribute.
// - Renamed.
[Experimental("OPENAI001")]
public enum ResponseToolChoiceKind
{
    Unknown = 0,
    Auto = 1,
    None = 2,
    Required = 3,
    Function = 4,
    FileSearch = 5,
    WebSearch = 6,
    Computer = 7,
}
