using System;
using System.Collections.Generic;
using System.ClientModel.Primitives;
using System.Text.Json;
using System.Diagnostics.CodeAnalysis;

namespace OpenAI.RealtimeConversation;

/// <summary>
/// The update (response command) of type <c>input_audio_buffer.speech_started</c>, which is received when a configured
/// <c>server_vad</c> <c>turn_detection</c> (including the default) processes the beginning of evaluated human speech
/// within the input audio buffer. This will be paired with a matching
/// <see cref="ConversationInputSpeechFinishedUpdate"/> when the end of speech is detected.
/// </summary>
[Experimental("OPENAI002")]
[CodeGenModel("RealtimeServerEventInputAudioBufferSpeechStopped")]
public partial class ConversationInputSpeechFinishedUpdate
{
    [CodeGenMember("AudioEndMs")]
    private readonly int _audioEndMs;

    public TimeSpan AudioEndTime => _audioEndTime ??= TimeSpan.FromMilliseconds(_audioEndMs);
    private TimeSpan? _audioEndTime;
}
