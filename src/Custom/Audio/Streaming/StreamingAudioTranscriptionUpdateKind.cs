using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Audio;

// CUSTOM: Added Experimental attribute.
[CodeGenType("CreateTranscriptionResponseStreamEventType")]
public readonly partial struct StreamingAudioTranscriptionUpdateKind
{
}