using System.ComponentModel;

namespace OpenAI.Audio;

// CUSTOM: Renamed.
/// <summary> The format of the translation. </summary>
[CodeGenModel("CreateTranslationRequestResponseFormat")]
public readonly partial struct AudioTranslationFormat
{
    // CUSTOM:
    // - Applied the EditorBrowsable attribute.
    // - Added custom doc comments.
    /// <summary> Plain text only. </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [CodeGenMember("Text")]
    public static AudioTranslationFormat Text { get; } = new AudioTranslationFormat(TextValue);

    // CUSTOM:
    // - Renamed.
    // - Added custom doc comments.
    /// <summary> Plain text only. </summary>
    [CodeGenMember("Json")]
    public static AudioTranslationFormat Simple { get; } = new AudioTranslationFormat(SimpleValue);

    // CUSTOM:
    // - Renamed.
    // - Added custom doc comments.
    /// <summary> Plain text provided with additional metadata, such as duration and timestamps. </summary>
    [CodeGenMember("VerboseJson")]
    public static AudioTranslationFormat Verbose { get; } = new AudioTranslationFormat(VerboseValue);

    // CUSTOM: Added custom doc comments.
    /// <summary> Text formatted as SubRip (.srt) file. </summary>
    [CodeGenMember("Srt")]
    public static AudioTranslationFormat Srt { get; } = new AudioTranslationFormat(SrtValue);

    // CUSTOM: Added custom doc comments.
    /// <summary> Text formatted as a Web Video Text Tracks, a.k.a. WebVTT, (.vtt) file. </summary>
    [CodeGenMember("Vtt")]
    public static AudioTranslationFormat Vtt { get; } = new AudioTranslationFormat(VttValue);
}