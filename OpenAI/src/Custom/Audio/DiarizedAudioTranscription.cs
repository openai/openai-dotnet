using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Audio;

// CUSTOM: Renamed.
[CodeGenType("CreateTranscriptionResponseDiarizedJson")]
public partial class DiarizedAudioTranscription
{
    // CUSTOM: Made internal
    internal string Task { get; } = "transcribe";
}
