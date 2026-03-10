using Microsoft.TypeSpec.Generator.Customizations;
using System;
using System.Collections.Generic;

namespace OpenAI.Realtime;

// CUSTOM: Renamed.
[CodeGenType("RealtimeResponseCreateParamsGA")]
public partial class RealtimeResponseOptions
{
    // CUSTOM: Renamed.
    [CodeGenMember("Audio")]
    public RealtimeResponseAudioOptions AudioOptions { get; set; }

    // CUSTOM:
    // - Renamed.
    // - Changed type from BinaryData (generated from the original union) to a custom type.
    [CodeGenMember("MaxOutputTokens")]
    public RealtimeMaxOutputTokenCount MaxOutputTokenCount { get; set; }

    // CUSTOM: Renamed.
    [CodeGenMember("Conversation")]
    public RealtimeResponseDefaultConversationConfiguration? DefaultConversationConfiguration { get; set; }

    // CUSTOM: Renamed.
    [CodeGenMember("Input")]
    public IList<RealtimeItem> InputItems { get; }

    // CUSTOM: Changed type from BinaryData (generated from the original union) to a custom type.
    [CodeGenMember("ToolChoice")]
    public RealtimeToolChoice ToolChoice { get; set; }
}