using Microsoft.TypeSpec.Generator.Customizations;
using System;

namespace OpenAI.Realtime;

// CUSTOM: Renamed.
[CodeGenType("TranscriptTextUsageDurationGA")]
public partial class GARealtimeTranscriptionDurationUsage
{
    // CUSTOM: Renamed.
    [CodeGenMember("Seconds")]
    public TimeSpan Duration { get; }
}
