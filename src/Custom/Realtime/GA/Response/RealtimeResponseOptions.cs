using Microsoft.TypeSpec.Generator.Customizations;
using System;
using System.Collections.Generic;

namespace OpenAI.Realtime;

// CUSTOM: Renamed.
[CodeGenType("RealtimeResponseCreateParamsGA")]
public partial class GARealtimeResponseOptions
{
    // CUSTOM: Renamed.
    [CodeGenMember("Audio")]
    public GARealtimeResponseAudioOptions AudioOptions { get; set; }

    // CUSTOM:
    // - Renamed.
    // - Changed type from BinaryData (generated from the original union) to a custom type.
    [CodeGenMember("MaxOutputTokens")]
    public GARealtimeMaxOutputTokenCount MaxOutputTokenCount { get; set; }

    // CUSTOM: Renamed.
    [CodeGenMember("Conversation")]
    public GARealtimeResponseDefaultConversationConfiguration? DefaultConversationConfiguration { get; set; }

    // CUSTOM: Renamed.
    [CodeGenMember("Input")]
    public IList<GARealtimeItem> InputItems { get; }

    // CUSTOM: Changed type from BinaryData (generated from the original union) to a custom type.
    [CodeGenMember("ToolChoice")]
    public GARealtimeToolChoice ToolChoice { get; set; }
}