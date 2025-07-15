using System;
using System.Collections.Generic;
using System.ClientModel.Primitives;
using System.Text.Json;
using System.Diagnostics.CodeAnalysis;

namespace OpenAI.Realtime;

/// <summary>
/// The update (response command) of type <c>input_audio_buffer.speech_stopped</c>, which is received when a configured
/// <c>server_vad</c> <c>turn_detection</c> (including the default) detects the end of evaluated human speech
/// within the input audio buffer.
/// </summary>
[CodeGenType("RealtimeServerEventInputAudioBufferSpeechStopped")]
public partial class InputAudioSpeechFinishedUpdate
{
    [CodeGenMember("AudioEndMs")]
    private readonly int _audioEndMs;

    public TimeSpan AudioEndTime => _audioEndTime ??= TimeSpan.FromMilliseconds(_audioEndMs);
    private TimeSpan? _audioEndTime;
}
