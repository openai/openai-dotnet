using Microsoft.TypeSpec.Generator.Customizations;
using System.Diagnostics.CodeAnalysis;

namespace OpenAI.Images;

// CUSTOM: Renamed.
/// <summary>
///     The quality of the image that will be generated. <see cref="High"/> creates images with finer details and
///     greater consistency across the image.
/// </summary>
[CodeGenType("CreateImageRequestQuality")]
public readonly partial struct GeneratedImageQuality
{
    // CUSTOM: Renamed.
    [CodeGenMember("Hd")]
    public static GeneratedImageQuality High { get; } = new GeneratedImageQuality(HdValue);

    // CUSTOM: Renamed.
    [Experimental("OPENAI001")]
    [CodeGenMember("Low")]
    public static GeneratedImageQuality LowQuality { get; } = new GeneratedImageQuality(LowValue);

    // CUSTOM: Renamed.
    [Experimental("OPENAI001")]
    [CodeGenMember("Medium")]
    public static GeneratedImageQuality MediumQuality { get; } = new GeneratedImageQuality(MediumValue);

    // CUSTOM: Renamed.
    [Experimental("OPENAI001")]
    [CodeGenMember("High")]
    public static GeneratedImageQuality HighQuality { get; } = new GeneratedImageQuality(HighValue);
}