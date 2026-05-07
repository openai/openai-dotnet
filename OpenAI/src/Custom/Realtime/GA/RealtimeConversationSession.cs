using Microsoft.TypeSpec.Generator.Customizations;
using System.Collections.Generic;

namespace OpenAI.Realtime;

// CUSTOM: Renamed.
[CodeGenType("RealtimeSessionCreateResponseGAGA")]
public partial class RealtimeConversationSession
{
    // CUSTOM: Changed type.
    [CodeGenMember("Model")]
    public string Model { get; set; }

    // CUSTOM: Renamed.
    [CodeGenMember("Include")]
    public IList<RealtimeIncludedProperty> IncludedProperties { get; }

    // CUSTOM: Renamed.
    [CodeGenMember("Audio")]
    public RealtimeConversationSessionAudioOptions AudioOptions { get; set; }

    // CUSTOM: Changed type from BinaryData (generated from the original union) to a custom type.
    [CodeGenMember("ToolChoice")]
    public RealtimeToolChoice ToolChoice { get; set; }

    // CUSTOM: Changed type from BinaryData (generated from the original union) to a custom type.
    [CodeGenMember("Tracing")]
    public RealtimeTracing Tracing { get; set; }

    // CUSTOM: Changed type from BinaryData (generated from the original union) to a custom type.
    [CodeGenMember("Truncation")]
    public RealtimeTruncation Truncation { get; set; }

    // CUSTOM:
    // - Renamed.
    // - Changed type from BinaryData (generated from the original union) to a custom type.
    [CodeGenMember("MaxOutputTokens")]
    public RealtimeMaxOutputTokenCount MaxOutputTokenCount { get; set; }
}
