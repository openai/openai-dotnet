using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Responses;

// CUSTOM: Renamed. Consolidated from duplicate SearchContextSize to use shared WebSearchContextSize from common.
[CodeGenType("WebSearchContextSize")]
public readonly partial struct WebSearchToolContextSize
{
}
