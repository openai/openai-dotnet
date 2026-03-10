using Microsoft.TypeSpec.Generator.Customizations;
using System;

namespace OpenAI.Responses;

// CUSTOM: Renamed.
[CodeGenType("ImageGenToolInputImageMask")]
[CodeGenVisibility(nameof(ImageGenerationToolInputImageMask), CodeGenVisibility.Internal)]
public partial class ImageGenerationToolInputImageMask
{
    public ImageGenerationToolInputImageMask(Uri imageUri)
    {
        Argument.AssertNotNull(imageUri, nameof(imageUri));

        ImageUri = imageUri;
    }

    public ImageGenerationToolInputImageMask(string fileId)
    {
        Argument.AssertNotNullOrEmpty(fileId, nameof(fileId));

        FileId = fileId;
    }

    // CUSTOM:
    // - Renamed.
    // - Removed setter.
    [CodeGenMember("ImageUrl")]
    public Uri ImageUri { get; }

    // CUSTOM: Removed setter.
    [CodeGenMember("FileId")]
    public string FileId { get; }
}
