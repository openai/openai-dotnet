using System.ComponentModel;

namespace OpenAI.Audio;

/// <summary> The format of the transcription. </summary>
[CodeGenType("TranscriptionAudioResponseFormat")]
public readonly partial struct AudioTranscriptionFormat
{
    // CUSTOM: Hide from browsing as this is equivalent to Simple
    /// <summary> Plain text only. </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [CodeGenMember("Text")]
    public static AudioTranscriptionFormat Text { get; } = new AudioTranscriptionFormat(TextValue);

    // CUSTOM: Rename, reflecting convergence with Text
    /// <summary> Plain text only. </summary>
    [CodeGenMember("Json")]
    public static AudioTranscriptionFormat Simple { get; } = new AudioTranscriptionFormat(JsonValue);

    // CUSTOM: Rename.
    /// <summary> Plain text provided with additional metadata, such as duration and timestamps. </summary>
    [CodeGenMember("VerboseJson")]
    public static AudioTranscriptionFormat Verbose { get; } = new AudioTranscriptionFormat(VerboseJsonValue);

    // CUSTOM: Added custom doc comments.
    /// <summary> Text formatted as SubRip (.srt) file. </summary>
    [CodeGenMember("Srt")]
    public static AudioTranscriptionFormat Srt { get; } = new AudioTranscriptionFormat(SrtValue);

    // CUSTOM: Added custom doc comments.
    /// <summary> Text formatted as a Web Video Text Tracks, a.k.a. WebVTT, (.vtt) file. </summary>
    [CodeGenMember("Vtt")]
    public static AudioTranscriptionFormat Vtt { get; } = new AudioTranscriptionFormat(VttValue);
}