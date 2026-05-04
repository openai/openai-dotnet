using System;
using System.ClientModel.Primitives;
using System.Runtime.CompilerServices;
using System.Text.Json;

namespace OpenAI.Chat;

public partial class StreamingChatFunctionCallUpdate : IJsonModel<StreamingChatFunctionCallUpdate>
{
    // CUSTOM: Replaced the call to WriteRawValue() for a call to WriteStringValue() because even though this property
    // is supposed to be a JSON object, the REST API handles it as a string given that there is no guarantee that it
    // actually is valid JSON.
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void SerializeFunctionArgumentsUpdateValue(Utf8JsonWriter writer, ModelReaderWriterOptions options)
    {
        writer.WriteStringValue(FunctionArgumentsUpdate.ToString());
    }

    // CUSTOM: Replaced the call to GetRawText() for a call to GetString() because otherwise the starting and ending
    // quotes of the string are included in the BinaryData. While this is actually a string in the REST API, we want to
    // handle it as JSON binary data instead.
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void DeserializeFunctionArgumentsUpdateValue(JsonProperty property, ref BinaryData arguments, ModelReaderWriterOptions options = null)
    {
        if (property.Value.ValueKind == JsonValueKind.Null)
        {
            return;
        }
        arguments = BinaryData.FromString(property.Value.GetString());
    }
}