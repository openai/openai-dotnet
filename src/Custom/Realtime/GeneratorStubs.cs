using System.ClientModel;
using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Realtime;

[CodeGenType("RealtimeAudioFormat")] public readonly partial struct RealtimeAudioFormat { }
[CodeGenType("RealtimeAudioInputTranscriptionModel")] public readonly partial struct InputTranscriptionModel { }
[CodeGenType("RealtimeAudioInputTranscriptionSettings")] public partial class InputTranscriptionOptions { }
[CodeGenType("RealtimeCreateClientSecretRequest")] public partial class RealtimeCreateClientSecretRequest { }
[CodeGenType("RealtimeCreateClientSecretRequestExpiresAfter")] public partial class RealtimeCreateClientSecretRequestExpiresAfter { }
[CodeGenType("RealtimeCreateClientSecretRequestExpiresAfterAnchor")] public readonly partial struct RealtimeCreateClientSecretRequestExpiresAfterAnchor { }
[CodeGenType("RealtimeCreateClientSecretResponse")] public partial class RealtimeCreateClientSecretResponse { }
[CodeGenType("RealtimeItemStatus")] public readonly partial struct ConversationItemStatus { }
[CodeGenType("RealtimeMessageRole")] public readonly partial struct ConversationMessageRole { }
[CodeGenType("RealtimeResponseStatus")] public readonly partial struct ConversationStatus { }
[CodeGenType("RealtimeResponseUsage")] public partial class ConversationTokenUsage { }
[CodeGenType("RealtimeSessionCreateRequestUnion")] public partial class RealtimeSessionCreateRequestUnion { }
[CodeGenType("RealtimeSessionCreateRequestUnionType")] public readonly partial struct RealtimeSessionCreateRequestUnionType { }
[CodeGenType("RealtimeSessionCreateResponseUnion")] public partial class RealtimeSessionCreateResponseUnion { }
[CodeGenType("RealtimeSessionCreateResponseUnionType")] public readonly partial struct RealtimeSessionCreateResponseUnionType { }
[CodeGenType("RealtimeToolType")] public readonly partial struct ConversationToolKind { }
[CodeGenType("UnknownRealtimeSessionCreateRequestUnion")] internal partial class UnknownRealtimeSessionCreateRequestUnion { }
[CodeGenType("UnknownRealtimeSessionCreateResponseUnion")] internal partial class UnknownRealtimeSessionCreateResponseUnion { }
[CodeGenType("DotNetRealtimeVoiceIds")] public readonly partial struct ConversationVoice { }
[CodeGenType("RealtimeSemanticVadTurnDetectionEagerness")] public readonly partial struct SemanticEagernessLevel { }

public partial class RealtimeCreateClientSecretRequest
{
    public static implicit operator BinaryContent(RealtimeCreateClientSecretRequest realtimeCreateClientSecretRequest)
    {
        if (realtimeCreateClientSecretRequest == null)
        {
            return null;
        }
        return BinaryContent.Create(realtimeCreateClientSecretRequest, ModelSerializationExtensions.WireOptions);
    }
}