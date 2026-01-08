using Microsoft.TypeSpec.Generator.Customizations;
using System.Diagnostics.CodeAnalysis;

namespace OpenAI.Responses;

// CUSTOM: Renamed.
[Experimental("OPENAI001")]
[CodeGenType("ImageGenToolCallItemResourceStatus")]
public enum ImageGenerationCallStatus
{
    InProgress,
    Completed,
    Generating,
    Failed
}
