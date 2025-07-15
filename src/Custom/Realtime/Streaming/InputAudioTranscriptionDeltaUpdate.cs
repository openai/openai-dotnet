using System;
using System.Collections.Generic;
using System.ClientModel.Primitives;
using System.Text.Json;
using System.Diagnostics.CodeAnalysis;
using OpenAI.Internal;

namespace OpenAI.Realtime;

/// <summary>
/// The update (response command) of type <c>conversation.item.input_audio_transcription.delta</c>, which is received
/// when a problem is encountered while processing transcription of the user audio input buffer via configured
/// <see cref="InputTranscriptionOptions"/> (<c>input_audio_transcription</c> in <c>session.update</c>)
/// settings.
/// </summary>
[CodeGenType("RealtimeServerEventConversationItemInputAudioTranscriptionDelta")]
public partial class InputAudioTranscriptionDeltaUpdate
{
    private IList<InternalDotNetRealtimeLogProbProperties> Logprobs { get; }
}