using System.Diagnostics.CodeAnalysis;

namespace OpenAI.Responses;

// CUSTOM:
// - Added Experimental attribute.
// - Renamed.
[Experimental("OPENAI001")]
[CodeGenType("ResponseOutputTextAnnotationAddedEvent")]
public partial class StreamingResponseTextAnnotationAddedUpdate
{
}