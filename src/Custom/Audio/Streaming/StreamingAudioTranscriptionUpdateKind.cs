using System.Diagnostics.CodeAnalysis;

namespace OpenAI.Audio;

// CUSTOM: Added Experimental attribute.
[Experimental("OPENAI001")]
[CodeGenType("CreateTranscriptionResponseStreamEventType")]
public readonly partial struct StreamingAudioTranscriptionUpdateKind
{
}