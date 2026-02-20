using Microsoft.TypeSpec.Generator.Customizations;
using System;

namespace OpenAI.Realtime;

// CUSTOM: Renamed.
[CodeGenType("RealtimeClientEventConversationItemTruncateGA")]
public partial class GARealtimeClientCommandConversationItemTruncate
{
    // CUSTOM: Renamed.
    [CodeGenMember("AudioEndMs")]
    public TimeSpan AudioEndTime { get; }
}