using System.Diagnostics.CodeAnalysis;

namespace OpenAI.Responses;

// CUSTOM:
// - Added Experimental attribute.
// - Renamed.
[Experimental("OPENAICUA001")]
[CodeGenType("ComputerUsePreviewToolEnvironment")]
public readonly partial struct ComputerToolEnvironment
{
}
