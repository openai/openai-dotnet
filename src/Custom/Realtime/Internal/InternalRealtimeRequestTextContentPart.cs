using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Realtime;
internal partial class InternalRealtimeRequestTextContentPart : ConversationContentPart
{
    [CodeGenMember("Text")]
    public string InternalTextValue { get; set; }
}
