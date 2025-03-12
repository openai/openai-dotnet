using System.ComponentModel;

namespace OpenAI.Audio;

/// <summary> The format of the transcription. </summary>
[CodeGenType("TranslationAudioResponseFormat")]
public readonly partial struct AudioTranslationFormat
{
    // CUSTOM: Hide from browsing as this is equivalent to Simple
    /// <summary> Plain text only. </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [CodeGenMember("Text")]
    public static AudioTranslationFormat Text { get; } = new AudioTranslationFormat(TextValue);

    // CUSTOM: Rename, reflecting convergence with Text
    /// <summary> Plain text only. </summary>
    [CodeGenMember("Json")]
    public static AudioTranslationFormat Simple { get; } = new AudioTranslationFormat(JsonValue);

    // CUSTOM: Rename.
    /// <summary> Plain text provided with additional metadata, such as duration and timestamps. </summary>
    [CodeGenMember("VerboseJson")]
    public static AudioTranslationFormat Verbose { get; } = new AudioTranslationFormat(VerboseJsonValue);

    // CUSTOM: Added custom doc comments.
    /// <summary> Text formatted as SubRip (.srt) file. </summary>
    [CodeGenMember("Srt")]
    public static AudioTranslationFormat Srt { get; } = new AudioTranslationFormat(SrtValue);

    // CUSTOM: Added custom doc comments.
    /// <summary> Text formatted as a Web Video Text Tracks, a.k.a. WebVTT, (.vtt) file. </summary>
    [CodeGenMember("Vtt")]
    public static AudioTranslationFormat Vtt { get; } = new AudioTranslationFormat(VttValue);
}