using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Videos;

// CUSTOM:
// - Renamed.
// - Made internal until we support the convenience methods.
[CodeGenType("OrderEnum")]
internal readonly partial struct InternalOrderEnum
{
}
