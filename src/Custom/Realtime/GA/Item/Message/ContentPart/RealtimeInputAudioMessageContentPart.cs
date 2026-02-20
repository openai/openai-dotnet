using Microsoft.TypeSpec.Generator.Customizations;
using System;

namespace OpenAI.Realtime;

// CUSTOM: Renamed.
[CodeGenType("RealtimeConversationItemInputAudioMessageContentPartGA")]
public partial class GARealtimeInputAudioMessageContentPart
{
    // CUSTOM: Renamed.
    [CodeGenMember("Audio")]
    public BinaryData AudioBytes { get; set; }
}