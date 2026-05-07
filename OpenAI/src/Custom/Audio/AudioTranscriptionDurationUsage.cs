using Microsoft.TypeSpec.Generator.Customizations;
using System;

namespace OpenAI.Audio;

// CUSTOM: Renamed.
[CodeGenType("TranscriptTextUsageDuration")]
public partial class AudioTranscriptionDurationUsage
{
    // CUSTOM: Renamed.
    [CodeGenMember("Seconds")]
    public TimeSpan Duration { get; }
}
