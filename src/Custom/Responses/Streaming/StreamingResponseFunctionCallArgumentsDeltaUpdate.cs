using System.Diagnostics.CodeAnalysis;

namespace OpenAI.Responses;

// CUSTOM:
// - Added Experimental attribute.
// - Renamed.
[CodeGenType("ResponseFunctionCallArgumentsDeltaEvent")]

public partial class StreamingResponseFunctionCallArgumentsDeltaUpdate : StreamingResponseUpdate
{
}
