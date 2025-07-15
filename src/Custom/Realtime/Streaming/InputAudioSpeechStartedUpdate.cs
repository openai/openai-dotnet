using System;
using System.Collections.Generic;
using System.ClientModel.Primitives;
using System.Text.Json;
using System.Diagnostics.CodeAnalysis;

namespace OpenAI.Realtime;

/// <summary>
/// The update (response command) of type <c>input_audio_buffer.speech_started</c>, which is received when a configured
/// <c>server_vad</c> <c>turn_detection</c> (including the default) processes the end of evaluated human speech
/// within the input audio buffer.
/// </summary>
[CodeGenType("RealtimeServerEventInputAudioBufferSpeechStarted")]
public partial class InputAudioSpeechStartedUpdate
{
    [CodeGenMember("AudioStartMs")]
    private readonly int _audioStartMs;

    public TimeSpan AudioStartTime => _audioStartTime ??= TimeSpan.FromMilliseconds(_audioStartMs);
    private TimeSpan? _audioStartTime;
}
