using Microsoft.TypeSpec.Generator.Customizations;
using System;

namespace OpenAI.Realtime;

/// <summary>
/// Corresponds to the <c>input_audio_buffer.timeout_triggered</c> server event.
/// Returned when the Server VAD timeout is triggered for the input audio buffer. This is configured
/// with <c>idle_timeout_ms</c> in the <c>turn_detection</c> settings of the session, and it indicates that
/// there hasn't been any speech detected for the configured duration.
/// <para>
/// The <c>audio_start_ms</c> and <c>audio_end_ms</c> fields indicate the segment of audio after the last
/// model response up to the triggering time, as an offset from the beginning of audio written
/// to the input audio buffer. This means it demarcates the segment of audio that was silent and
/// the difference between the start and end values will roughly match the configured timeout.
/// </para>
/// <para>
/// The empty audio will be committed to the conversation as an <c>input_audio</c> item (there will be a
/// <c>input_audio_buffer.committed</c> event) and a model response will be generated. There may be speech
/// that didn't trigger VAD but is still detected by the model, so the model may respond with
/// something relevant to the conversation or a prompt to continue speaking.
/// </para>
/// </summary>
// CUSTOM: Renamed.
[CodeGenType("RealtimeServerEventInputAudioBufferTimeoutTriggeredGA")]
public partial class GARealtimeServerUpdateInputAudioBufferTimeoutTriggered
{
    // CUSTOM: Renamed.
    [CodeGenMember("AudioStartMs")]
    public TimeSpan AudioStartTime { get; }

    // CUSTOM: Renamed.
    [CodeGenMember("AudioEndMs")]
    public TimeSpan AudioEndTime { get; }
}
