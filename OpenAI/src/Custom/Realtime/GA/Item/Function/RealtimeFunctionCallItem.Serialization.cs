using Microsoft.TypeSpec.Generator.Customizations;
using System;
using System.ClientModel.Primitives;
using System.Runtime.CompilerServices;
using System.Text.Json;

namespace OpenAI.Realtime;

[CodeGenSerialization(nameof(FunctionArguments), SerializationValueHook = nameof(SerializeFunctionArgumentsValue), DeserializationValueHook = nameof(DeserializeFunctionArgumentsValue))]
public partial class RealtimeFunctionCallItem
{
    // CUSTOM: The REST API serializes this as a string.
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void SerializeFunctionArgumentsValue(Utf8JsonWriter writer, ModelReaderWriterOptions options)
    {
        string value = FunctionArguments.ToMemory().IsEmpty
            ? string.Empty
            : FunctionArguments.ToString();
        writer.WriteStringValue(value);
    }

    // CUSTOM: Replaced the call to GetRawText() for a call to GetString() because otherwise the starting and ending
    // quotes of the string are included in the BinaryData. While this is actually a string in the REST API, we want to
    // handle it as JSON binary data instead.
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void DeserializeFunctionArgumentsValue(JsonProperty property, ref BinaryData functionArguments, ModelReaderWriterOptions options = null)
    {
        functionArguments = BinaryData.FromString(property.Value.GetString());
    }
}