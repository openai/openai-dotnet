using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Audio;

// CUSTOM: Added Experimental attribute.
[CodeGenType("DotNetCreateTranscriptionStreamingResponse")]
[CodeGenSuppress(nameof(StreamingAudioTranscriptionUpdate), typeof(System.ClientModel.ClientResult))]
public partial class StreamingAudioTranscriptionUpdate
{
}