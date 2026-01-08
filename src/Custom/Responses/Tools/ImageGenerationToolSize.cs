using Microsoft.TypeSpec.Generator.Customizations;
using System;

namespace OpenAI.Responses;

// CUSTOM: Added custom struct in favor of the generated extensible enum.
/// <summary> The size of the image that will be generated. </summary>
[CodeGenType("ImageGenToolSize")]
[CodeGenSuppress("GeneratedImageSize", typeof(string))]
// CUSTOM: remove the implicit operator
[CodeGenSuppress("", typeof(string))]
public readonly partial struct ImageGenerationToolSize
{

    /// <summary> Initializes a new instance of <see cref="ImageGenerationToolSize"/>. </summary>
    /// <exception cref="ArgumentNullException"> <paramref name="value"/> is null. </exception>
    internal ImageGenerationToolSize(string value)
    {
        _value = value ?? throw new ArgumentNullException(nameof(value));
    }

    /// <summary>
    /// Creates a new instance of <see cref="ImageGenerationToolSize"/>.
    /// </summary>
    /// <remarks>
    /// <b>Note:</b> arbitrary dimensions are not supported and a given model will only support a set of predefined
    /// sizes. If supported dimensions are not known, try using one of the static properties like <see cref="W1024xH1024"/>.
    /// </remarks>
    /// <param name="width"> The desired width, in pixels, for an image. </param>
    /// <param name="height"> The desired height, in pixels, for an image. </param>
    public ImageGenerationToolSize(int width, int height)
    {
        _value = $"{width}x{height}";
    }

    /// <summary>
    /// A square image with 1024 pixels of both width and height.
    /// <para>
    /// <b>Supported</b> and <b>default</b> for both <c>dall-e-2</c> and <c>dall-e-3</c> models.
    /// </para>
    /// </summary>
    [CodeGenMember("_1024x1024")]
    public static readonly ImageGenerationToolSize W1024xH1024 = new ImageGenerationToolSize(1024, 1024);

    /// <summary>
    /// An extra tall image, 1024 pixels wide by 1536 pixels high.
    /// <para>
    /// Supported <b>only</b> for the <c>dall-e-3</c> model.
    /// </para>
    /// </summary>
    [CodeGenMember("_1024x1536")]
    public static readonly ImageGenerationToolSize W1024xH1536 = new(1024, 1536);

    /// <summary>
    /// An extra wide image, 1536 pixels wide by 1024 pixels high.
    /// <para>
    /// Supported <b>only</b> for the <c>dall-e-3</c> model.
    /// </para>
    /// </summary>
    [CodeGenMember("_1536x1024")]
    public static readonly ImageGenerationToolSize W1536xH1024 = new(1536, 1024);
}
