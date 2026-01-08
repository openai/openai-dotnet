using Microsoft.TypeSpec.Generator.Customizations;
using System.Diagnostics.CodeAnalysis;

namespace OpenAI.Realtime;

/// <summary>
/// The update (response command) of type <c>response.audio_transcript.delta</c>, which is received while populating
/// an output audio content part of a conversation item.
/// </summary>
[Experimental("OPENAI002")]
[CodeGenType("RealtimeServerEventResponseAudioTranscriptDelta")]
internal partial class InternalRealtimeServerEventResponseAudioTranscriptDelta
{ }
