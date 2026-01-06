using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Videos;

// CUSTOM:
// - Renamed.
// - Made internal until we support the convenience methods.
[CodeGenType("VideoCollectionOrder")]
internal readonly partial struct InternalVideoCollectionOrder
{
}
