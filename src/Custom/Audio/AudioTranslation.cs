using System;

namespace OpenAI.Audio;

[CodeGenType("CreateTranslationResponseVerboseJson")]
public partial class AudioTranslation
{
    // CUSTOM: Made private. This property does not add value in the context of a strongly-typed class.
    private string Task { get; } = "translate";

    // CUSTOM: Made nullable because this is an optional property.
    /// <summary> The duration of the input audio. </summary>
    public TimeSpan? Duration { get; }
}