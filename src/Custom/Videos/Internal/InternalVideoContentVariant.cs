using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Videos;

// CUSTOM:
// - Renamed.
// - Made internal until we support the convenience methods.
[CodeGenType("VideoContentVariant")]
internal readonly partial struct InternalVideoContentVariant
{
}
