using System.ClientModel.Primitives;
using System.Text.Json;

namespace OpenAI.RealtimeConversation;

[CodeGenSuppress(nameof(DeserializeConversationTurnDetectionOptions), typeof(JsonElement), typeof(ModelReaderWriterOptions))]
public partial class ConversationTurnDetectionOptions
{
    internal static ConversationTurnDetectionOptions DeserializeConversationTurnDetectionOptions(JsonElement element, ModelReaderWriterOptions options = null)
    {
        options ??= ModelSerializationExtensions.WireOptions;

        if (element.ValueKind == JsonValueKind.Null)
        {
            return InternalRealtimeNoTurnDetection.DeserializeInternalRealtimeNoTurnDetection(element, options);
        }
        if (element.TryGetProperty("type", out JsonElement discriminator))
        {
            switch (discriminator.GetString())
            {
                case "none": return InternalRealtimeNoTurnDetection.DeserializeInternalRealtimeNoTurnDetection(element, options);
                case "server_vad": return InternalRealtimeServerVadTurnDetection.DeserializeInternalRealtimeServerVadTurnDetection(element, options);
                default: return null;
            }
        }
        return UnknownRealtimeTurnDetection.DeserializeUnknownRealtimeTurnDetection(element, options);
    }
}