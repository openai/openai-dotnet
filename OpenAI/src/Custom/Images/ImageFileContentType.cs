using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace OpenAI.Images;

/// <summary> The media type of an image file supplied in a multipart request. </summary>
[Experimental("OPENAI001")]
public readonly partial struct ImageFileContentType : IEquatable<ImageFileContentType>
{
    private readonly string _value;
    private const string PngValue = "image/png";
    private const string JpegValue = "image/jpeg";
    private const string WebpValue = "image/webp";

    public ImageFileContentType(string value)
    {
        Argument.AssertNotNull(value, nameof(value));
        _value = value;
    }

    /// <summary> The <c>image/png</c> media type. </summary>
    public static ImageFileContentType Png { get; } = new(PngValue);

    /// <summary> The <c>image/jpeg</c> media type. </summary>
    public static ImageFileContentType Jpeg { get; } = new(JpegValue);

    /// <summary> The <c>image/webp</c> media type. </summary>
    public static ImageFileContentType Webp { get; } = new(WebpValue);

    public static bool operator ==(ImageFileContentType left, ImageFileContentType right) => left.Equals(right);

    public static bool operator !=(ImageFileContentType left, ImageFileContentType right) => !left.Equals(right);

    public static implicit operator ImageFileContentType(string value) => new(value);

    public static implicit operator ImageFileContentType?(string value) => value is null ? null : new(value);

    [EditorBrowsable(EditorBrowsableState.Never)]
    public override bool Equals(object obj) => obj is ImageFileContentType other && Equals(other);

    public bool Equals(ImageFileContentType other) => string.Equals(_value, other._value, StringComparison.InvariantCultureIgnoreCase);

    [EditorBrowsable(EditorBrowsableState.Never)]
    public override int GetHashCode() => _value is not null ? StringComparer.InvariantCultureIgnoreCase.GetHashCode(_value) : 0;

    public override string ToString() => _value;
}
