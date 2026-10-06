using System;

namespace OpenAI.Audio;

internal static class AudioFileContentTypeExtensions
{
    internal static string ToSerialString(this AudioFileContentType value) => value switch
    {
        AudioFileContentType.Flac => "audio/flac",
        AudioFileContentType.Mpeg => "audio/mpeg",
        AudioFileContentType.Mp4 => "audio/mp4",
        AudioFileContentType.Ogg => "audio/ogg",
        AudioFileContentType.Wav => "audio/wav",
        AudioFileContentType.Webm => "audio/webm",
        _ => throw new ArgumentOutOfRangeException(nameof(value), value, "Unknown audio file content type."),
    };
}
