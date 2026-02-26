using Microsoft.TypeSpec.Generator.Customizations;
using System;

namespace OpenAI.Realtime;

/// <summary>
/// Corresponds to the <c>input_audio_buffer.speech_stopped</c> server event.
/// Returned in <c>server_vad</c> mode when the server detects the end of speech in
/// the audio buffer. The server will also send an <c>conversation.item.created</c>
/// event with the user message item that is created from the audio buffer.
/// </summary>
// CUSTOM: Renamed.
[CodeGenType("RealtimeServerEventInputAudioBufferSpeechStoppedGA")]
public partial class GARealtimeServerUpdateInputAudioBufferSpeechStopped
{
    // CUSTOM: Renamed.
    [CodeGenMember("AudioEndMs")]
    public TimeSpan AudioEndTime { get; }
}
