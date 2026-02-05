using Microsoft.TypeSpec.Generator.Customizations;
using System;

namespace OpenAI.Responses;

// CUSTOM: Renamed.
[CodeGenType("ImageGenToolInputImageMask")]
public partial class ImageGenerationToolInputImageMask
{
    internal ImageGenerationToolInputImageMask() { }

    public ImageGenerationToolInputImageMask(string fileId)
    {
        Argument.AssertNotNullOrEmpty(fileId, nameof(fileId));

        FileId = fileId;
    }

    public ImageGenerationToolInputImageMask(BinaryData imageBytes, string imageBytesMediaType)
    {
        Argument.AssertNotNull(imageBytes, nameof(imageBytes));
        Argument.AssertNotNullOrEmpty(imageBytesMediaType, nameof(imageBytesMediaType));

        string base64EncodedData = Convert.ToBase64String(imageBytes.ToArray());
        ImageUrl = $"data:{imageBytesMediaType};base64,{base64EncodedData}";
    }

    public ImageGenerationToolInputImageMask(Uri imageUri)
    {
        Argument.AssertNotNull(imageUri, nameof(imageUri));

        ImageUrl = imageUri?.AbsoluteUri;
    }

    // CUSTOM: Removed setter.
    [CodeGenMember("ImageUrl")]
    public string ImageUrl { get; }

    // CUSTOM: Removed setter.
    [CodeGenMember("FileId")]
    public string FileId { get; }
}
