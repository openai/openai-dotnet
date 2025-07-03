using System.ClientModel.Primitives;
using System.Text.Json;

namespace OpenAI.Realtime;

[CodeGenSuppress(nameof(DeserializeInputNoiseReductionOptions), typeof(JsonElement), typeof(ModelReaderWriterOptions))]
public partial class InputNoiseReductionOptions
{
    internal static InputNoiseReductionOptions DeserializeInputNoiseReductionOptions(JsonElement element, ModelReaderWriterOptions options = null)
    {
        options ??= ModelSerializationExtensions.WireOptions;

        if (element.ValueKind == JsonValueKind.Null)
        {
            return InternalRealtimeAudioDisabledNoiseReduction
                .DeserializeInternalRealtimeAudioDisabledNoiseReduction(element, options);
        }
        if (element.TryGetProperty("type", out JsonElement discriminator))
        {
            return discriminator.GetString() switch
            {
                "near_field" => (InputNoiseReductionOptions)InternalRealtimeAudioNearFieldNoiseReduction.DeserializeInternalRealtimeAudioNearFieldNoiseReduction(element, options),
                "far_field" => (InputNoiseReductionOptions)InternalRealtimeAudioFarFieldNoiseReduction.DeserializeInternalRealtimeAudioFarFieldNoiseReduction(element, options),
                _ => (InputNoiseReductionOptions)InternalUnknownRealtimeAudioNoiseReduction.DeserializeInternalUnknownRealtimeAudioNoiseReduction(element, options),
            };
        }
        return InternalUnknownRealtimeAudioNoiseReduction.DeserializeInternalUnknownRealtimeAudioNoiseReduction(element, options);
    }
}