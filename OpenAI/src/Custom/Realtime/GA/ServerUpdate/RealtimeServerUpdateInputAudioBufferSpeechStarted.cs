using Microsoft.TypeSpec.Generator.Customizations;
using System;

namespace OpenAI.Realtime;

/// <summary>
/// Corresponds to the <c>input_audio_buffer.speech_started</c> server event.
/// Sent by the server when in <c>server_vad</c> mode to indicate that speech has been
/// detected in the audio buffer. This can happen any time audio is added to the
/// buffer (unless speech is already detected). The client may want to use this
/// event to interrupt audio playback or provide visual feedback to the user.
/// The client should expect to receive a <c>input_audio_buffer.speech_stopped</c> event
/// when speech stops. The <c>item_id</c> property is the ID of the user message item
/// that will be created when speech stops and will also be included in the
/// <c>input_audio_buffer.speech_stopped</c> event (unless the client manually commits
/// the audio buffer during VAD activation).
/// </summary>
// CUSTOM: Renamed.
[CodeGenType("RealtimeServerEventInputAudioBufferSpeechStartedGA")]
public partial class RealtimeServerUpdateInputAudioBufferSpeechStarted
{
    // CUSTOM: Renamed.
    [CodeGenMember("AudioStartMs")]
    public TimeSpan AudioStartTime { get; }
}
