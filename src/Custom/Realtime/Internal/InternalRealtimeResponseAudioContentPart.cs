using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Realtime;

[CodeGenType("RealtimeResponseAudioContentPart")]
internal partial class InternalRealtimeResponseAudioContentPart : ConversationContentPart
{
    [CodeGenMember("Transcript")]
    public string InternalTranscriptValue { get; }
}
