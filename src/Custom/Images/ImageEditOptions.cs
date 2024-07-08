﻿using System;
using System.IO;

namespace OpenAI.Images;

/// <summary>
/// Represents additional options available to control the behavior of an image generation operation.
/// </summary>
[CodeGenModel("CreateImageEditRequest")]
[CodeGenSuppress("ImageEditOptions", typeof(BinaryData), typeof(string))]
public partial class ImageEditOptions
{
    // CUSTOM: Made internal. The model is specified by the client.
    /// <summary> The model to use for image generation. Only `dall-e-2` is supported at this time. </summary>
    internal InternalCreateImageEditRequestModel? Model { get; set; }

    // CUSTOM:
    // - Made internal. This value comes from a parameter on the client method.
    // - Added setter.
    /// <summary>
    /// The image to edit. Must be a valid PNG file, less than 4MB, and square. If mask is not
    /// provided, image must have transparency, which will be used as the mask.
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
    internal BinaryData Image { get; set;  }

    // CUSTOM:
    // - Made internal. This value comes from a parameter on the client method.
    // - Added setter.
    /// <summary> A text description of the desired image(s). The maximum length is 1000 characters. </summary>
    internal string Prompt { get; set; }

    // CUSTOM: Made internal. This value comes from a parameter on the client method.
    /// <summary>
    /// An additional image whose fully transparent areas (e.g. where alpha is zero) indicate where
    /// `image` should be edited. Must be a valid PNG file, less than 4MB, and have the same dimensions
    /// as `image`.
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
    internal BinaryData Mask { get; set; }

    // CUSTOM: Made internal. This value comes from a parameter on the client method.
    /// <summary> The number of images to generate. Must be between 1 and 10. </summary>
    internal long? N { get; set; }

    // CUSTOM: Made public now that there are no required properties.
    /// <summary> Initializes a new instance of <see cref="ImageEditOptions"/>. </summary>
    public ImageEditOptions()
    {
    }

    // CUSTOM: Changed property type.
    /// <summary> The size of the generated images. Must be one of `256x256`, `512x512`, or `1024x1024`. </summary>
    public GeneratedImageSize? Size { get; init; }

    // CUSTOM: Changed property type.
    /// <summary> The format in which the generated images are returned. Must be one of `url` or `b64_json`. </summary>
    public GeneratedImageFormat? ResponseFormat { get; init; }

    internal MultipartFormDataBinaryContent ToMultipartContent(Stream image, string imageFilename, Stream mask, string maskFilename)
    {
        MultipartFormDataBinaryContent content = new();

        content.Add(image, "image", imageFilename);
        content.Add(Prompt, "prompt");
        content.Add(Model.Value.ToString(), "model");

        if (mask is not null)
        {
            content.Add(mask, "mask", maskFilename);
        }

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

        if (User is not null)
        {
            content.Add(User, "user");
        }

        return content;
    }
}