using Microsoft.TypeSpec.Generator.Customizations;
using System.Collections.Generic;

namespace OpenAI.Realtime;

// CUSTOM: Renamed.
[CodeGenType("RealtimeResponseCreateParamsGA")]
public partial class GARealtimeResponseOptions
{
    // CUSTOM: Renamed.
    [CodeGenMember("Audio")]
    public GARealtimeResponseAudioOptions AudioOptions { get; set; }

    // CUSTOM: Renamed.
    [CodeGenMember("MaxOutputTokens")]
    public int? MaxOutputTokenCount { get; set; }

    // CUSTOM: Renamed.
    [CodeGenMember("Conversation")]
    public GARealtimeResponseDefaultConversationConfiguration? DefaultConversationConfiguration { get; set; }

    // CUSTOM: Renamed.
    [CodeGenMember("Input")]
    public IList<GARealtimeItem> InputItems { get; }
}