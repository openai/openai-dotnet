using Microsoft.TypeSpec.Generator.Customizations;
using System;

namespace OpenAI.Realtime;

// CUSTOM: Renamed.
[CodeGenType("RealtimeServerEventInputAudioBufferSpeechStoppedGA")]
public partial class GARealtimeServerUpdateInputAudioBufferSpeechStopped
{
    // CUSTOM: Renamed.
    [CodeGenMember("AudioEndMs")]
    public TimeSpan AudioEndTime { get; }
}
