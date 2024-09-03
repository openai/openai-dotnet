using System;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.Text.Json;

namespace OpenAI.Chat;

[CodeGenSuppress("global::System.ClientModel.Primitives.IJsonModel<OpenAI.Chat.FunctionChatMessage>.Write", typeof(Utf8JsonWriter), typeof(ModelReaderWriterOptions))]
public partial class FunctionChatMessage : IJsonModel<FunctionChatMessage>
{
    void IJsonModel<FunctionChatMessage>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options)
        => CustomSerializationHelpers.SerializeInstance(this, SerializeFunctionChatMessage, writer, options);

    internal static void SerializeFunctionChatMessage(FunctionChatMessage instance, Utf8JsonWriter writer, ModelReaderWriterOptions options)
    {
        instance.WriteCore(writer, options);
    }

    protected internal override void WriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options)
    {
        writer.WriteStartObject();
        writer.WritePropertyName("role"u8);
        writer.WriteStringValue(Role.ToSerialString());
        writer.WritePropertyName("name"u8);
        writer.WriteStringValue(FunctionName);
        if (Optional.IsCollectionDefined(Content))
        {
            writer.WritePropertyName("content"u8);
            writer.WriteStringValue(Content?[0]?.Text);
        }
        writer.WriteSerializedAdditionalRawData(SerializedAdditionalRawData, options);
        writer.WriteEndObject();
    }
}
