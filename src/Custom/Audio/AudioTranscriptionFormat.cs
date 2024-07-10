using System.ComponentModel;

namespace OpenAI.Audio;

/// <summary>
/// Specifies the format of the audio transcription.
/// </summary>
[CodeGenModel("CreateTranscriptionRequestResponseFormat")]
public enum AudioTranscriptionFormat
{
    /// <summary> Text. </summary>
    [CodeGenMember("Text")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    Text,

    /// <summary> Simple. </summary>
    [CodeGenMember("Json")]
    Simple,

    /// <summary> Verbose. </summary>
    [CodeGenMember("VerboseJson")]
    Verbose,

    /// <summary> SRT. </summary>
    [CodeGenMember("Srt")]
    Srt,

    /// <summary> VTT. </summary>
    [CodeGenMember("Vtt")]
    Vtt,
}