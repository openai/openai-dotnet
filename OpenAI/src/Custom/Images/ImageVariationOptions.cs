using Microsoft.TypeSpec.Generator.Customizations;
using System;
using System.IO;

namespace OpenAI.Images;

/// <summary> Represents additional options available to control the behavior of an image variation operation. </summary>
[CodeGenType("CreateImageVariationRequest")]
[CodeGenVisibility(nameof(ImageVariationOptions), CodeGenVisibility.Public)]
[CodeGenSuppress(nameof(ImageVariationOptions), typeof(BinaryData))]
public partial class ImageVariationOptions
{
    // CUSTOM: Made internal. The model is specified by the client.
    [CodeGenMember("Model")]
    internal InternalCreateImageVariationRequestModel? Model { get; set; }

    // CUSTOM: Made internal. This value comes from a parameter on the client method.
    [CodeGenMember("Image")]
    internal BinaryData Image { get; set; }

    // CUSTOM: Made internal. This value comes from a parameter on the client method.
    [CodeGenMember("N")]
    internal long? N { get; set; }

    // CUSTOM: Changed property type.
    /// <summary> The format in which the generated images are returned. </summary>
    [CodeGenMember("ResponseFormat")]
    public GeneratedImageFormat? ResponseFormat { get; set; }

    // CUSTOM: Changed property type.
    /// <summary> The size of the generated images. </summary>
    [CodeGenMember("Size")]
    public GeneratedImageSize? Size { get; set; }

    // CUSTOM: Renamed.
    /// <summary>
    ///     A unique identifier representing your end-user, which can help OpenAI to monitor and detect abuse.
    ///     <see href="https://platform.openai.com/docs/guides/safety-best-practices/end-user-ids">Learn more</see>.
    /// </summary>
    [CodeGenMember("User")]
    public string EndUserId { get; set; }

    internal MultiPartFormDataBinaryContent ToMultipartContent(Stream image, string imageFilename)
    {
        MultiPartFormDataBinaryContent content = new();

        content.Add(image, "image", imageFilename);

        if (EndUserId is not null)
        {
            content.Add(EndUserId, "user");
        }

        if (Model is not null)
        {
            content.Add(Model.Value.ToString(), "model");
        }

        if (N is not null)
        {
            content.Add(N.Value, "n");
        }

        if (ResponseFormat is not null)
        {
            content.Add(ResponseFormat.Value.ToString(), "response_format");
        }

        if (Size is not null)
        {
            content.Add(Size.Value.ToString(), "size");
        }

        return content;
    }
}