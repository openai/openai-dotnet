using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace OpenAI.Images;

/// <summary> Represents additional options available to control the behavior of an image edit operation. </summary>
[CodeGenType("CreateImageEditRequest")]
[CodeGenVisibility(nameof(ImageEditOptions), CodeGenVisibility.Public)]
[CodeGenSuppress(nameof(ImageEditOptions), typeof(BinaryData), typeof(string))]
public partial class ImageEditOptions
{
    // CUSTOM: Made internal. The model is specified by the client.
    [CodeGenMember("Model")]
    internal InternalCreateImageEditRequestModel? Model { get; set; }

    // CUSTOM: Made internal. This value comes from a parameter on the client method.
    [CodeGenMember("Image")]
    internal BinaryData Image { get; set; }

    // CUSTOM: Made internal. This value comes from a parameter on the client method.
    [CodeGenMember("Mask")]
    internal BinaryData Mask { get; set; }

    // CUSTOM: Made internal. This value comes from a parameter on the client method.
    [CodeGenMember("N")]
    internal long? N { get; set; }

    // CUSTOM: Made internal. This value comes from a parameter on the client method.
    [CodeGenMember("Prompt")]
    internal string Prompt { get; set; }

    // CUSTOM: Temporarily made internal. This value should be exposed through a dedicated client method.
    [CodeGenMember("Stream")]
    internal bool? Stream { get; set; }

    // CUSTOM: Temporarily made internal. This value should be exposed once streaming is supported.
    [CodeGenMember("PartialImages")]
    internal int? PartialImages { get; set; }

    // CUSTOM: Changed property type.
    /// <summary> Allows to set transparency for the background of the generated image(s). </summary>
    [Experimental("OPENAI001")]
    [CodeGenMember("Background")]
    public GeneratedImageBackground? Background { get; set; }

    // CUSTOM: Renamed.
    /// <summary> The compression level (0-100%) for the generated images. </summary>
    [Experimental("OPENAI001")]
    [CodeGenMember("OutputCompression")]
    public int? OutputCompressionFactor { get; set; }

    // CUSTOM:
    // - Renamed.
    // - Changed property type.
    /// <summary> The format in which the generated images are returned. </summary>
    [Experimental("OPENAI001")]
    [CodeGenMember("OutputFormat")]
    public GeneratedImageFileFormat? OutputFileFormat { get; set; }

    // CUSTOM: Changed property type.
    /// <summary> The quality of the image that will be generated. </summary>
    [Experimental("OPENAI001")]
    [CodeGenMember("Quality")]
    public GeneratedImageQuality? Quality { get; set; }

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

    internal MultiPartFormDataBinaryContent ToMultipartContent(Stream image, string imageFilename, Stream mask, string maskFilename)
    {
        MultiPartFormDataBinaryContent content = new();

        content.Add(image, "image", imageFilename);

        content.Add(Prompt, "prompt");

        if (Background is not null)
        {
            content.Add(Background.Value.ToString(), "background");
        }

        if (EndUserId is not null)
        {
            content.Add(EndUserId, "user");
        }

        if (InputFidelity is not null)
        {
            content.Add(InputFidelity.Value.ToString(), "input_fidelity");
        }

        if (mask is not null)
        {
            content.Add(mask, "mask", maskFilename);
        }

        if (Model is not null)
        {
            content.Add(Model.Value.ToString(), "model");
        }

        if (N is not null)
        {
            content.Add(N.Value, "n");
        }

        if (OutputCompressionFactor is not null)
        {
            content.Add(OutputCompressionFactor.Value, "output_compression");
        }

        if (OutputFileFormat is not null)
        {
            content.Add(OutputFileFormat.Value.ToString(), "output_format");
        }

        if (PartialImages is not null)
        {
            content.Add(PartialImages.Value, "partial_images");
        }

        if (Quality is not null)
        {
            content.Add(Quality.Value.ToString(), "quality");
        }

        if (ResponseFormat is not null)
        {
            content.Add(ResponseFormat.Value.ToString(), "response_format");
        }

        if (Size is not null)
        {
            content.Add(Size.ToString(), "size");
        }

        if (Stream is not null)
        {
            content.Add(Stream.Value, "stream");
        }

        return content;
    }
}