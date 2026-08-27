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

        ImageUri = imageUri.AbsoluteUri;
    }

    public ImageGenerationToolInputImageMask(BinaryData imageBytes)
    {
        Argument.AssertNotNull(imageBytes, nameof(imageBytes));
        Argument.AssertNotNullOrEmpty(imageBytes.MediaType, nameof(imageBytes.MediaType));

        ImageUri = DataEncodingHelpers.CreateDataUri(imageBytes, imageBytes.MediaType);
    }

    public ImageGenerationToolInputImageMask(string fileId)
    {
        Argument.AssertNotNullOrEmpty(fileId, nameof(fileId));

        FileId = fileId;
    }
}
