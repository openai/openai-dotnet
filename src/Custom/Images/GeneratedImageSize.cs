using System;
using System.Diagnostics.CodeAnalysis;

namespace OpenAI.Images;

// CUSTOM: Added custom struct in favor of the generated extensible enum.
/// <summary> The size of the image that will be generated. </summary>
[CodeGenType("CreateImageRequestSize")]
[CodeGenSuppress("GeneratedImageSize", typeof(string))]
// CUSTOM: remove the implicit operator
[CodeGenSuppress("", typeof(string))]
public readonly partial struct GeneratedImageSize : IEquatable<GeneratedImageSize>
{
    /// <summary> Initializes a new instance of <see cref="GeneratedImageSize"/>. </summary>
    /// <exception cref="ArgumentNullException"> <paramref name="value"/> is null. </exception>
    internal GeneratedImageSize(string value)
    {
        _value = value ?? throw new ArgumentNullException(nameof(value));
    }

    /// <summary>
    /// Creates a new instance of <see cref="GeneratedImageSize"/>.
    /// </summary>
    /// <remarks>
    /// <b>Note:</b> arbitrary dimensions are not supported and a given model will only support a set of predefined
    /// sizes. If supported dimensions are not known, try using one of the static properties like <see cref="W1024xH1024"/>.
    /// </remarks>
    /// <param name="width"> The desired width, in pixels, for an image. </param>
    /// <param name="height"> The desired height, in pixels, for an image. </param>
    public GeneratedImageSize(int width, int height)
    {
        _value = $"{width}x{height}";
    }

    /// <summary>
    /// A small, square image with 256 pixels of both width and height.
    /// <para>
    /// Supported <b>only</b> for the older <c>dall-e-2</c> model.
    /// </para>
    /// </summary>
    [CodeGenMember("_256x256")]
    public static readonly GeneratedImageSize W256xH256 = new(256, 256);

    /// <summary>
    /// A medium-small, square image with 512 pixels of both width and height.
    /// <para>
    /// Supported <b>only</b> for the older <c>dall-e-2</c> model.
    /// </para>
    /// </summary>
    [CodeGenMember("_512x512")]
    public static readonly GeneratedImageSize W512xH512 = new(512, 512);

    /// <summary>
    /// A square image with 1024 pixels of both width and height.
    /// <para>
    /// <b>Supported</b> and <b>default</b> for both <c>dall-e-2</c> and <c>dall-e-3</c> models.
    /// </para>
    /// </summary>
    [CodeGenMember("_1024x1024")]
    public static readonly GeneratedImageSize W1024xH1024 = new(1024, 1024);

    /// <summary>
    /// An extra tall image, 1024 pixels wide by 1792 pixels high.
    /// <para>
    /// Supported <b>only</b> for the <c>dall-e-3</c> model.
    /// </para>
    /// </summary>
    [CodeGenMember("_1792x1024")]
    public static readonly GeneratedImageSize W1024xH1792 = new(1024, 1792);

    /// <summary>
    /// An extra wide image, 1792 pixels wide by 1024 pixels high.
    /// <para>
    /// Supported <b>only</b> for the <c>dall-e-3</c> model.
    /// </para>
    /// </summary>
    [CodeGenMember("_1024x1792")]
    public static readonly GeneratedImageSize W1792xH1024 = new(1792, 1024);

    // CUSTOM:
    // - Added Experimental attribute.
    [Experimental("OPENAI001")]
    [CodeGenMember("_1024x1536")]
    public static readonly GeneratedImageSize W1024xH1536 = new(1024, 1536);

    // CUSTOM:
    // - Added Experimental attribute.
    [Experimental("OPENAI001")]
    [CodeGenMember("_1536x1024")]
    public static readonly GeneratedImageSize W1536xH1024 = new(1536, 1024);
}