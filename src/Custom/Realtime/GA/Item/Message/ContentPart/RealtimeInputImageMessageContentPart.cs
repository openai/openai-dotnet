using Microsoft.TypeSpec.Generator.Customizations;
using System;

namespace OpenAI.Realtime;

// CUSTOM: Renamed.
[CodeGenType("RealtimeConversationItemInputImageMessageContentPartGA")]
public partial class GARealtimeInputImageMessageContentPart
{
    // CUSTOM: Renamed.
    [CodeGenMember("ImageUrl")]
    public Uri ImageUri { get; set; }

}