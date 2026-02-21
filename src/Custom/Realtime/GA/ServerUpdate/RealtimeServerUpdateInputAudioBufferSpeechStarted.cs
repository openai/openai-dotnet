using Microsoft.TypeSpec.Generator.Customizations;
using System;

namespace OpenAI.Realtime;

// CUSTOM: Renamed.
[CodeGenType("RealtimeServerEventInputAudioBufferSpeechStartedGA")]
public partial class GARealtimeServerUpdateInputAudioBufferSpeechStarted
{
    // CUSTOM: Renamed.
    [CodeGenMember("AudioStartMs")]
    public TimeSpan AudioStartTime { get; }
}
