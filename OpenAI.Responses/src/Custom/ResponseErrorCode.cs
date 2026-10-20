using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Responses;

// CUSTOM:
// - Added Experimental attribute.
// - Renamed.
[CodeGenType("ResponseErrorCode")]
public readonly partial struct ResponseErrorCode
{
    [CodeGenMember("InvalidImageUri")]
    public static ResponseErrorCode InvalidImageUrl { get; } = new("invalid_image_url");
}