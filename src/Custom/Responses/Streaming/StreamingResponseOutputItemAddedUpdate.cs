using System.Diagnostics.CodeAnalysis;

namespace OpenAI.Responses;

// CUSTOM:
// - Added Experimental attribute.
// - Renamed.
[CodeGenType("ResponseOutputItemAddedEvent")]

public partial class StreamingResponseOutputItemAddedUpdate : StreamingResponseUpdate
{
}