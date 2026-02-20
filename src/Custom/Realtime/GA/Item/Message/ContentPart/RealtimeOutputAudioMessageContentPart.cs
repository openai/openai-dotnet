using Microsoft.TypeSpec.Generator.Customizations;
using System;

namespace OpenAI.Realtime;

// CUSTOM: Renamed.
[CodeGenType("RealtimeConversationItemOutputAudioMessageContentPartGA")]
public partial class GARealtimeOutputAudioMessageContentPart
{
    // CUSTOM: Renamed.
    [CodeGenMember("Audio")]
    public BinaryData AudioBytes { get; set; }
}