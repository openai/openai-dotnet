using System.Diagnostics.CodeAnalysis;

namespace OpenAI.Audio;

/// <summary> The media type of an audio file supplied in a multipart request. </summary>
[Experimental("OPENAI001")]
public enum AudioFileContentType
{
    /// <summary> The <c>audio/flac</c> media type. </summary>
    Flac,

    /// <summary> The <c>audio/mpeg</c> media type, used for MP3, MPEG, and MPGA files. </summary>
    Mpeg,

    /// <summary> The <c>audio/mp4</c> media type, used for MP4 and M4A files. </summary>
    Mp4,

    /// <summary> The <c>audio/ogg</c> media type. </summary>
    Ogg,

    /// <summary> The <c>audio/wav</c> media type. </summary>
    Wav,

    /// <summary> The <c>audio/webm</c> media type. </summary>
    Webm,
}
