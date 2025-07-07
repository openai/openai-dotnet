using System.Diagnostics.CodeAnalysis;

namespace OpenAI.Responses;

// CUSTOM:
// - Added Experimental attribute.
// - Renamed.
[Experimental("OPENAI001")]
[CodeGenType("ResponseContentPartAddedEvent")]
public partial class StreamingResponseContentPartAddedUpdate
{
    // CUSTOM: Apply generalized content type.
    [CodeGenMember("Part")]
    public ResponseContentPart Part { get; }
}