using System.Diagnostics.CodeAnalysis;

namespace OpenAI.Audio;

// CUSTOM: Added Experimental attribute.
[CodeGenType("CreateTranscriptionResponseStreamEvent")]
[CodeGenSuppress(nameof(StreamingAudioTranscriptionUpdate), typeof(System.ClientModel.ClientResult))]
public partial class StreamingAudioTranscriptionUpdate
{
}