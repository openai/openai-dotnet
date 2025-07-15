using System.Diagnostics.CodeAnalysis;

namespace OpenAI.Responses;

// CUSTOM:
// - Added Experimental attribute.
// - Renamed.
[CodeGenType("CreateResponseText")]
public partial class ResponseTextOptions
{
    // CUSTOM: Renamed.
    [CodeGenMember("Format")]
    public ResponseTextFormat TextFormat { get; set; }
}