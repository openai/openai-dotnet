using Microsoft.TypeSpec.Generator.Customizations;
using System.Collections.Generic;

namespace OpenAI.Realtime;

// CUSTOM: Renamed.
[CodeGenType("RealtimeTranscriptionSessionCreateRequestGAGA")]
public partial class GARealtimeTranscriptionSessionOptions
{
    // CUSTOM: Renamed.
    [CodeGenMember("Include")]
    public IList<GARealtimeIncludedProperty> IncludedProperties { get; }

    // CUSTOM: Renamed.
    [CodeGenMember("Audio")]
    public GARealtimeTranscriptionSessionAudioOptions AudioOptions { get; set; }
}
