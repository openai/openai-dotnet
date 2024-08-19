using System;
using System.IO;

namespace OpenAI.Images;

/// <summary>
/// Represents additional options available to control the behavior of an image generation operation.
/// </summary>
[CodeGenModel("CreateImageVariationRequest")]
[CodeGenSuppress("ImageVariationOptions", typeof(BinaryData))]
public partial class ImageVariationOptions
{
    // CUSTOM: Made internal. The model is specified by the client.
    /// <summary> The model to use for image generation. Only `dall-e-2` is supported at this time. </summary>
    internal InternalCreateImageVariationRequestModel? Model { get; set; }

    // CUSTOM:
    // - Made internal. This value comes from a parameter on the client method.
    // - Added setter.
    /// <summary>
    /// The image to use as the basis for the variation(s). Must be a valid PNG file, less than 4MB,
    /// and square.
    /// <para>
    /// To assign a byte[] to this property use <see cref="BinaryData.FromBytes(byte[])"/>.
    /// The byte[] will be serialized to a Base64 encoded string.
    /// </para>
    /// <para>
    /// Examples:
    /// <list type="bullet">
    /// <item>
    /// <term>BinaryData.FromBytes(new byte[] { 1, 2, 3 })</term>
    /// <description>Creates a payload of "AQID".</description>
    /// </item>
    /// </list>
    /// </para>
    /// </summary>
    internal BinaryData Image { get; set; }

    // CUSTOM: Made internal. This value comes from a parameter on the client method.
    /// <summary> The number of images to generate. Must be between 1 and 10. </summary>
    internal long? N { get; set; }

    // CUSTOM: Made public now that there are no required properties.
    /// <summary> Initializes a new instance of <see cref="ImageVariationOptions"/>. </summary>
    public ImageVariationOptions()
    {
    }

    // CUSTOM: Changed property type.
    /// <summary> The size of the generated images. Must be one of `256x256`, `512x512`, or `1024x1024`. </summary>
    [CodeGenMember("Size")]
    public GeneratedImageSize? Size { get; set; }

    // CUSTOM: Changed property type.
    /// <summary> The format in which the generated images are returned. Must be one of `url` or `b64_json`. </summary>
    [CodeGenMember("ResponseFormat")]
    public GeneratedImageFormat? ResponseFormat { get; set; }

    // CUSTOM: Renamed.
    /// <summary>
    ///     A unique identifier representing your end-user, which can help OpenAI to monitor and detect abuse.
    ///     <see href="https://platform.openai.com/docs/guides/safety-best-practices/end-user-ids">Learn more</see>.
    /// </summary>
    [CodeGenMember("User")]
    public string EndUserId { get; set; }

    internal MultipartFormDataBinaryContent ToMultipartContent(Stream image, string imageFilename)
    {
        MultipartFormDataBinaryContent content = new();

        content.Add(image, "image", imageFilename);
        content.Add(Model.Value.ToString(), "model");

        if (N is not null)
        {
            content.Add(N.Value, "n");
        }

        if (ResponseFormat is not null)
        {
            string format = ResponseFormat switch
            {
                GeneratedImageFormat.Uri => "url",
                GeneratedImageFormat.Bytes => "b64_json",
                _ => throw new ArgumentException(nameof(ResponseFormat)),
            };

            content.Add(format, "response_format");
        }

        if (Size is not null)
        {
            content.Add(Size.ToString(), "size");
        }

        if (EndUserId is not null)
        {
            content.Add(EndUserId, "user");
        }

        return content;
    }
}