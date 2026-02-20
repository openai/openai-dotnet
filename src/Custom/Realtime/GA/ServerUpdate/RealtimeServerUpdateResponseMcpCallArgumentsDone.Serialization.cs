using Microsoft.TypeSpec.Generator.Customizations;
using System;
using System.ClientModel.Primitives;
using System.Runtime.CompilerServices;
using System.Text.Json;

namespace OpenAI.Realtime;

[CodeGenSerialization(nameof(ToolArguments), SerializationValueHook = nameof(SerializeToolArgumentsValue), DeserializationValueHook = nameof(DeserializeToolArgumentsValue))]
public partial class GARealtimeServerUpdateResponseMcpCallArgumentsDone
{
    // CUSTOM: The REST API serializes this as a string.
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void SerializeToolArgumentsValue(Utf8JsonWriter writer, ModelReaderWriterOptions options)
    {
        string value = ToolArguments.ToMemory().IsEmpty
            ? string.Empty
            : ToolArguments.ToString();
        writer.WriteStringValue(value);
    }

    // CUSTOM: Replaced the call to GetRawText() for a call to GetString() because otherwise the starting and ending
    // quotes of the string are included in the BinaryData. While this is actually a string in the REST API, we want to
    // handle it as JSON binary data instead.
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void DeserializeToolArgumentsValue(JsonProperty property, ref BinaryData toolArguments, ModelReaderWriterOptions options = null)
    {
        toolArguments = BinaryData.FromString(property.Value.GetString());
    }
}