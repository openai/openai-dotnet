using System.Diagnostics.CodeAnalysis;

namespace OpenAI.Responses;

// CUSTOM:
// - Added Experimental attribute.
// - Renamed.
[CodeGenType("ResponseTextDeltaEvent")]
public partial class StreamingResponseOutputTextDeltaUpdate : StreamingResponseUpdate
{
}
