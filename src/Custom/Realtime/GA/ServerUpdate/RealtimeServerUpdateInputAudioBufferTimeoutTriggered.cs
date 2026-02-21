using Microsoft.TypeSpec.Generator.Customizations;
using System;

namespace OpenAI.Realtime;

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
