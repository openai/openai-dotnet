using System;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.Text.Json;

namespace OpenAI.Assistants;

[CodeGenSuppress("global::System.ClientModel.Primitives.IJsonModel<OpenAI.Assistants.AssistantResponseFormat>.Write", typeof(Utf8JsonWriter), typeof(ModelReaderWriterOptions))]
[CodeGenSuppress("global::System.ClientModel.Primitives.IJsonModel<OpenAI.Assistants.AssistantResponseFormat>.Create", typeof(Utf8JsonReader), typeof(ModelReaderWriterOptions))]
[CodeGenSuppress("global::System.ClientModel.Primitives.IPersistableModel<OpenAI.Assistants.AssistantResponseFormat>.Write", typeof(ModelReaderWriterOptions))]
[CodeGenSuppress("global::System.ClientModel.Primitives.IPersistableModel<OpenAI.Assistants.AssistantResponseFormat>.Create", typeof(BinaryData), typeof(ModelReaderWriterOptions))]
[CodeGenSuppress("global::System.ClientModel.Primitives.IPersistableModel<OpenAI.Assistants.AssistantResponseFormat>.GetFormatFromOptions", typeof(ModelReaderWriterOptions))]
public partial class AssistantResponseFormat : IJsonModel<AssistantResponseFormat>
{
    internal static void SerializeAssistantResponseFormat(AssistantResponseFormat instance, Utf8JsonWriter writer, ModelReaderWriterOptions options = null)
    {
        throw new InvalidOperationException();
    }

    internal static AssistantResponseFormat DeserializeAssistantResponseFormat(JsonElement element, ModelReaderWriterOptions options = null)
    {
        return element.ValueKind switch
        {
            JsonValueKind.String => InternalAssistantResponseFormatPlainTextNoObject.DeserializeInternalAssistantResponseFormatPlainTextNoObject(element, options),
            JsonValueKind.Object when element.TryGetProperty("type", out JsonElement discriminatorElement)
                => discriminatorElement.GetString() switch
                {
                    "json_object" => InternalDotNetAssistantResponseFormatJsonObject.DeserializeInternalDotNetAssistantResponseFormatJsonObject(element, options),
                    "json_schema" => InternalDotNetAssistantResponseFormatJsonSchema.DeserializeInternalDotNetAssistantResponseFormatJsonSchema(element, options),
                    "text" => InternalDotNetAssistantResponseFormatText.DeserializeInternalDotNetAssistantResponseFormatText(element, options),
                    _ => InternalUnknownDotNetAssistantResponseFormat.DeserializeInternalUnknownDotNetAssistantResponseFormat(element, options),
                },
            _ => null,
        };
    }

    void IJsonModel<AssistantResponseFormat>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options)
        => CustomSerializationHelpers.SerializeInstance(this, SerializeAssistantResponseFormat, writer, options);

    AssistantResponseFormat IJsonModel<AssistantResponseFormat>.Create(ref Utf8JsonReader reader, ModelReaderWriterOptions options)
        => CustomSerializationHelpers.DeserializeNewInstance(this, DeserializeAssistantResponseFormat, ref reader, options);

    BinaryData IPersistableModel<AssistantResponseFormat>.Write(ModelReaderWriterOptions options)
        => CustomSerializationHelpers.SerializeInstance(this, options);

    AssistantResponseFormat IPersistableModel<AssistantResponseFormat>.Create(BinaryData data, ModelReaderWriterOptions options)
        => CustomSerializationHelpers.DeserializeNewInstance(this, DeserializeAssistantResponseFormat, data, options);

    string IPersistableModel<AssistantResponseFormat>.GetFormatFromOptions(ModelReaderWriterOptions options) => "J";

    internal static AssistantResponseFormat FromResponse(PipelineResponse response)
    {
        throw new InvalidOperationException();
    }

    internal virtual BinaryContent ToBinaryContent()
    {
        return BinaryContent.Create(this, ModelSerializationExtensions.WireOptions);
    }
}
