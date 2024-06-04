using System;

namespace OpenAI.Audio;

/// <summary>
/// Represents the available text-to-speech voices.
/// </summary>
[CodeGenModel("SpeechGenerationOptionsVoice")]
public enum GeneratedSpeechVoice
{
    /// <summary> Alloy. </summary>
    [CodeGenMember("Alloy")]
    Alloy,

    /// <summary> Echo. </summary>
    [CodeGenMember("Echo")]
    Echo,

    /// <summary> Fable. </summary>
    [CodeGenMember("Fable")]
    Fable,

    /// <summary> Onyx. </summary>
    [CodeGenMember("Onyx")]
    Onyx,

    /// <summary> Nova. </summary>
    [CodeGenMember("Nova")]
    Nova,

    /// <summary> Shimmer. </summary>
    [CodeGenMember("Shimmer")]
    Shimmer,
}