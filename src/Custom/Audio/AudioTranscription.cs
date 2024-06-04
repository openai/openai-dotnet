using System;

namespace OpenAI.Audio;

[CodeGenModel("CreateTranscriptionResponseVerboseJson")]
public partial class AudioTranscription
{
    // CUSTOM: Made private. This property does not add value in the context of a strongly-typed class.
    private InternalCreateTranscriptionResponseVerboseJsonTask Task { get; } = InternalCreateTranscriptionResponseVerboseJsonTask.Transcribe;

    // CUSTOM: Made nullable because this is an optional property.
    /// <summary> The duration of the input audio. </summary>
    public TimeSpan? Duration { get; }
}