using Microsoft.TypeSpec.Generator.Customizations;
using System;

namespace OpenAI.Realtime;

// CUSTOM: Renamed.
[CodeGenType("RealtimeConversationItemInputImageMessageContentPartGA")]
public partial class RealtimeInputImageMessageContentPart
{
    // CUSTOM: Renamed.
    [CodeGenMember("ImageUrl")]
    public Uri ImageUri { get; set; }

}