using Microsoft.TypeSpec.Generator.Customizations;
using System.Collections.Generic;

namespace OpenAI.Realtime;

// CUSTOM: Renamed.
[CodeGenType("RealtimeResponseGA")]
public partial class RealtimeResponse
{
    // CUSTOM: Renamed.
    [CodeGenMember("Output")]
    public IList<RealtimeItem> OutputItems { get; }

    // CUSTOM: Renamed.
    [CodeGenMember("Audio")]
    public RealtimeResponseAudioOptions AudioOptions { get; }

    // CUSTOM:
    // - Renamed.
    // - Changed type from BinaryData (generated from the original union) to a custom type.
    [CodeGenMember("MaxOutputTokens")]
    public RealtimeMaxOutputTokenCount MaxOutputTokenCount { get; }
}