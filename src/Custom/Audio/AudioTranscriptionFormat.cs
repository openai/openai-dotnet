using System.ComponentModel;

namespace OpenAI.Audio;

// CUSTOM: Renamed.
/// <summary> The format of the transcription. </summary>
[CodeGenModel("CreateTranscriptionRequestResponseFormat1")]
public readonly partial struct AudioTranscriptionFormat
{
    // CUSTOM:
    // - Applied the EditorBrowsable attribute.
    // - Added custom doc comments.
    /// <summary> Plain text only. </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [CodeGenMember("Text")]
    public static AudioTranscriptionFormat Text { get; } = new AudioTranscriptionFormat(TextValue);

    // CUSTOM:
    // - Renamed.
    // - Added custom doc comments.
    /// <summary> Plain text only. </summary>
    [CodeGenMember("Json")]
    public static AudioTranscriptionFormat Simple { get; } = new AudioTranscriptionFormat(SimpleValue);

    // CUSTOM:
    // - Renamed.
    // - Added custom doc comments.
    /// <summary> Plain text provided with additional metadata, such as duration and timestamps. </summary>
    [CodeGenMember("VerboseJson")]
    public static AudioTranscriptionFormat Verbose { get; } = new AudioTranscriptionFormat(VerboseValue);

    // CUSTOM: Added custom doc comments.
    /// <summary> Text formatted as SubRip (.srt) file. </summary>
    [CodeGenMember("Srt")]
    public static AudioTranscriptionFormat Srt { get; } = new AudioTranscriptionFormat(SrtValue);

    // CUSTOM: Added custom doc comments.
    /// <summary> Text formatted as a Web Video Text Tracks, a.k.a. WebVTT, (.vtt) file. </summary>
    [CodeGenMember("Vtt")]
    public static AudioTranscriptionFormat Vtt { get; } = new AudioTranscriptionFormat(VttValue);
}