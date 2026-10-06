using System;

namespace OpenAI.Images;

internal static class ImageFileContentTypeExtensions
{
    internal static string ToSerialString(this ImageFileContentType value) => value switch
    {
        ImageFileContentType.Png => "image/png",
        ImageFileContentType.Jpeg => "image/jpeg",
        ImageFileContentType.Webp => "image/webp",
        _ => throw new ArgumentOutOfRangeException(nameof(value), value, "Unknown image file content type."),
    };
}
