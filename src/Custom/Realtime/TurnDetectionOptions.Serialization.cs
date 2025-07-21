using System.ClientModel.Primitives;
using System.Text.Json;

namespace OpenAI.Realtime;

[CodeGenSuppress(nameof(DeserializeTurnDetectionOptions), typeof(JsonElement), typeof(ModelReaderWriterOptions))]
public partial class TurnDetectionOptions
{
    internal static TurnDetectionOptions DeserializeTurnDetectionOptions(JsonElement element, ModelReaderWriterOptions options = null)
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
                case "semantic_vad": return InternalRealtimeSemanticVadTurnDetection.DeserializeInternalRealtimeSemanticVadTurnDetection(element, options);
                default: return null;
            }
        }
        return UnknownRealtimeTurnDetection.DeserializeUnknownRealtimeTurnDetection(element, options);
    }
}