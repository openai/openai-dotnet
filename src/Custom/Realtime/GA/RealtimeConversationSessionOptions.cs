using Microsoft.TypeSpec.Generator.Customizations;
using System.Collections.Generic;

namespace OpenAI.Realtime;

// CUSTOM: Renamed.
[CodeGenType("RealtimeSessionCreateRequestGAGA")]
public partial class GARealtimeConversationSessionOptions
{
    // CUSTOM: Changed type.
    [CodeGenMember("Model")]
    public string Model { get; set; }

    // CUSTOM: Renamed.
    [CodeGenMember("Include")]
    public IList<GARealtimeIncludedProperty> IncludedProperties { get; }

    // CUSTOM: Renamed.
    [CodeGenMember("Audio")]
    public GARealtimeConversationSessionAudioOptions AudioOptions { get; set; }

    // CUSTOM: Changed type from BinaryData (generated from the original union) to a custom type.
    [CodeGenMember("ToolChoice")]
    public GARealtimeToolChoice ToolChoice { get; set; }

    // CUSTOM: Changed type from BinaryData (generated from the original union) to a custom type.
    [CodeGenMember("Tracing")]
    public GARealtimeTracing Tracing { get; set; }

    // CUSTOM: Changed type from BinaryData (generated from the original union) to a custom type.
    [CodeGenMember("Truncation")]
    public GARealtimeTruncation Truncation { get; set; }

    // CUSTOM:
    // - Renamed.
    // - Changed type from BinaryData (generated from the original union) to a custom type.
    [CodeGenMember("MaxOutputTokens")]
    public GARealtimeMaxOutputTokenCount MaxOutputTokenCount { get; set; }
}
