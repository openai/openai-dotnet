using Microsoft.TypeSpec.Generator.Customizations;
using System.Collections.Generic;

namespace OpenAI.Realtime;

// CUSTOM: Renamed.
[CodeGenType("RealtimeTranscriptionSessionCreateRequestGAGA")]
public partial class RealtimeTranscriptionSessionOptions
{
    // CUSTOM: Renamed.
    [CodeGenMember("Include")]
    public IList<RealtimeIncludedProperty> IncludedProperties { get; }

    // CUSTOM: Renamed.
    [CodeGenMember("Audio")]
    public RealtimeTranscriptionSessionAudioOptions AudioOptions { get; set; }
}
