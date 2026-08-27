using Microsoft.TypeSpec.Generator.Customizations;
using System;
using System.Diagnostics.CodeAnalysis;

namespace OpenAI.Images;

// CUSTOM:
// - Renamed.
// - Suppressed the implicit operators that convert from string to GeneratedImageSize.
/// <summary> The size of the image that will be generated. </summary>
[CodeGenType("CreateImageRequestSize")]
[CodeGenSuppress("", typeof(string))]
public readonly partial struct GeneratedImageSize
{
    // CUSTOM: Made internal in favor of the constructor that takes numeric width and height.
    internal GeneratedImageSize(string value)
    {
        _value = value ?? throw new ArgumentNullException(nameof(value));
    }

    // CUSTOM: Added as a convenience.
    /// <summary> Creates a new instance of <see cref="GeneratedImageSize"/>. </summary>
    /// <param name="width"> The desired width for an image in pixels. </param>
    /// <param name="height"> The desired height for an image in pixels. </param>
    public GeneratedImageSize(int width, int height)
    {
        _value = $"{width}x{height}";
    }

    // CUSTOM: Renamed.
    /// <summary> Width and height of 256 pixels. </summary>
    /// <remarks> Supported only by the <c>dall-e-2</c> model. </remarks>
    [CodeGenMember("_256x256")]
    public static readonly GeneratedImageSize W256xH256 = new(256, 256);

    // CUSTOM: Renamed.
    /// <summary> Width and height of 512 pixels. </summary>
    /// <remarks> Supported only by the <c>dall-e-2</c> model. </remarks>
    [CodeGenMember("_512x512")]
    public static readonly GeneratedImageSize W512xH512 = new(512, 512);

    // CUSTOM: Renamed.
    /// <summary> Width and height of 1024 pixels. </summary>
    [CodeGenMember("_1024x1024")]
    public static readonly GeneratedImageSize W1024xH1024 = new(1024, 1024);

    // CUSTOM: Renamed.
    /// <summary> Width of 1024 pixels and height of 1536 pixels. </summary>
    [Experimental("OPENAI001")]
    [CodeGenMember("_1024x1536")]
    public static readonly GeneratedImageSize W1024xH1536 = new(1024, 1536);

    // CUSTOM: Renamed.
    /// <summary> Width of 1536 pixels and height of 1024 pixels. </summary>
    [Experimental("OPENAI001")]
    [CodeGenMember("_1536x1024")]
    public static readonly GeneratedImageSize W1536xH1024 = new(1536, 1024);

    // CUSTOM: Renamed.
    /// <summary> Width of 1024 pixels and height of 1792 pixels. </summary>
    /// <remarks> Supported only by the <c>dall-e-3</c> model. </remarks>
    [CodeGenMember("_1792x1024")]
    public static readonly GeneratedImageSize W1024xH1792 = new(1024, 1792);

    // CUSTOM: Renamed.
    /// <summary> Width of 1792 pixels and height of 1024 pixels. </summary>
    /// <remarks> Supported only by the <c>dall-e-3</c> model. </remarks>
    [CodeGenMember("_1024x1792")]
    public static readonly GeneratedImageSize W1792xH1024 = new(1792, 1024);
}