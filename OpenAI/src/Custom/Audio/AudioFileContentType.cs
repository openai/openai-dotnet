using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace OpenAI.Audio;

/// <summary> The media type of an audio file supplied in a multipart request. </summary>
[Experimental("OPENAI001")]
public readonly partial struct AudioFileContentType : IEquatable<AudioFileContentType>
{
    private readonly string _value;
    private const string FlacValue = "audio/flac";
    private const string MpegValue = "audio/mpeg";
    private const string Mp4Value = "audio/mp4";
    private const string OggValue = "audio/ogg";
    private const string WavValue = "audio/wav";
    private const string WebmValue = "audio/webm";

    public AudioFileContentType(string value)
    {
        Argument.AssertNotNull(value, nameof(value));
        _value = value;
    }

    /// <summary> The <c>audio/flac</c> media type. </summary>
    public static AudioFileContentType Flac { get; } = new(FlacValue);

    /// <summary> The <c>audio/mpeg</c> media type, used for MP3, MPEG, and MPGA files. </summary>
    public static AudioFileContentType Mpeg { get; } = new(MpegValue);

    /// <summary> The <c>audio/mp4</c> media type, used for MP4 and M4A files. </summary>
    public static AudioFileContentType Mp4 { get; } = new(Mp4Value);

    /// <summary> The <c>audio/ogg</c> media type. </summary>
    public static AudioFileContentType Ogg { get; } = new(OggValue);

    /// <summary> The <c>audio/wav</c> media type. </summary>
    public static AudioFileContentType Wav { get; } = new(WavValue);

    /// <summary> The <c>audio/webm</c> media type. </summary>
    public static AudioFileContentType Webm { get; } = new(WebmValue);

    public static bool operator ==(AudioFileContentType left, AudioFileContentType right) => left.Equals(right);

    public static bool operator !=(AudioFileContentType left, AudioFileContentType right) => !left.Equals(right);

    public static implicit operator AudioFileContentType(string value) => new(value);

    public static implicit operator AudioFileContentType?(string value) => value is null ? null : new(value);

    [EditorBrowsable(EditorBrowsableState.Never)]
    public override bool Equals(object obj) => obj is AudioFileContentType other && Equals(other);

    public bool Equals(AudioFileContentType other) => string.Equals(_value, other._value, StringComparison.InvariantCultureIgnoreCase);

    [EditorBrowsable(EditorBrowsableState.Never)]
    public override int GetHashCode() => _value is not null ? StringComparer.InvariantCultureIgnoreCase.GetHashCode(_value) : 0;

    public override string ToString() => _value;
}
