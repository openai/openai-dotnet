using Microsoft.TypeSpec.Generator.Customizations;
using System.Collections.Generic;

namespace OpenAI.Realtime;

// CUSTOM: Renamed.
[CodeGenType("RealtimeResponseGA")]
public partial class GARealtimeResponse
{
    // CUSTOM: Renamed.
    [CodeGenMember("Output")]
    public IList<GARealtimeItem> OutputItems { get; }

    // CUSTOM: Renamed.
    [CodeGenMember("Audio")]
    public GARealtimeResponseAudioOptions AudioOptions { get; }

    // CUSTOM:
    // - Renamed.
    // - Changed type from BinaryData (generated from the original union) to a custom type.
    [CodeGenMember("MaxOutputTokens")]
    public GARealtimeMaxOutputTokenCount MaxOutputTokenCount { get; }
}