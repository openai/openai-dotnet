using System.Diagnostics.CodeAnalysis;

namespace OpenAI.Responses;

// CUSTOM:
// - Added Experimental attribute.
// - Renamed.
[CodeGenType("ResponseStreamEvent")]
[CodeGenSuppress("StreamingResponseUpdate", typeof(System.ClientModel.ClientResult))]
public partial class StreamingResponseUpdate
{
}