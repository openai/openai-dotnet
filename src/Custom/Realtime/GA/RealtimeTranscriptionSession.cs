using Microsoft.TypeSpec.Generator.Customizations;
using System.Collections.Generic;

namespace OpenAI.Realtime;

// CUSTOM: Renamed.
[CodeGenType("RealtimeTranscriptionSessionCreateResponseGAGA")]
public partial class GARealtimeTranscriptionSession
{
    // CUSTOM: Renamed.
    [CodeGenMember("Include")]
    public IList<GARealtimeIncludedProperty> IncludedProperties { get; }

    // CUSTOM: Renamed.
    [CodeGenMember("Audio")]
    public GARealtimeTranscriptionSessionAudioOptions AudioOptions { get; set; }
}