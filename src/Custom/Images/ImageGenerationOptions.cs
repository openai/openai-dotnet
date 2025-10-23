using System.ClientModel;
using System.Diagnostics.CodeAnalysis;

namespace OpenAI.Images;

/// <summary> Represents additional options available to control the behavior of an image generation operation. </summary>
[CodeGenType("CreateImageRequest")]
[CodeGenVisibility(nameof(ImageGenerationOptions), CodeGenVisibility.Public)]
[CodeGenSuppress(nameof(ImageGenerationOptions), typeof(string))]
public partial class ImageGenerationOptions
{
    // CUSTOM: Made internal. The model is specified by the client.
    [CodeGenMember("Model")]
    internal InternalCreateImageRequestModel? Model { get; set; }

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

    // CUSTOM: Renamed.
    /// <summary> Control the content-moderation level for the generated images. </summary>
    [Experimental("OPENAI001")]
    [CodeGenMember("Moderation")]
    public GeneratedImageModerationLevel? ModerationLevel { get; set; }

    // CUSTOM: Renamed.
    /// <summary> The compression level (0-100%) for the generated images. </summary>
    [Experimental("OPENAI001")]
    [CodeGenMember("OutputCompression")]
    public int? OutputCompressionFactor { get; set; }

    // CUSTOM: Renamed.
    /// <summary> The format in which the generated images are returned. </summary>
    [Experimental("OPENAI001")]
    [CodeGenMember("OutputFormat")]
    public GeneratedImageFileFormat? OutputFileFormat { get; set; }

    // CUSTOM: Renamed.
    /// <summary>
    ///     A unique identifier representing your end-user, which can help OpenAI to monitor and detect abuse.
    ///     <see href="https://platform.openai.com/docs/guides/safety-best-practices/end-user-ids">Learn more</see>.
    /// </summary>
    [CodeGenMember("User")]
    public string EndUserId { get; set; }

    internal BinaryContent ToBinaryContent() => BinaryContent.Create(this, ModelSerializationExtensions.WireOptions);
}