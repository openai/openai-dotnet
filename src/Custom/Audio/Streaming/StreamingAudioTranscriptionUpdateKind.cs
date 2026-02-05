using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Audio;

// CUSTOM: Added Experimental attribute.
[CodeGenType("DotNetCreateTranscriptionStreamingResponseType")]
public readonly partial struct StreamingAudioTranscriptionUpdateKind
{
}