using System;
using System.ClientModel.Primitives;
using System.Text.Json;

namespace OpenAI.Responses;

public partial class ResponseToolChoice : IJsonModel<ResponseToolChoice>
{
    void IJsonModel<ResponseToolChoice>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options)
        => CustomSerializationHelpers.SerializeInstance(this, SerializeResponseToolChoice, writer, options);

    ResponseToolChoice IJsonModel<ResponseToolChoice>.Create(ref Utf8JsonReader reader, ModelReaderWriterOptions options)
        => CustomSerializationHelpers.DeserializeNewInstance(this, DeserializeResponseToolChoice, ref reader, options);

    BinaryData IPersistableModel<ResponseToolChoice>.Write(ModelReaderWriterOptions options)
        => CustomSerializationHelpers.SerializeInstance(this, options);

    ResponseToolChoice IPersistableModel<ResponseToolChoice>.Create(BinaryData data, ModelReaderWriterOptions options)
        => CustomSerializationHelpers.DeserializeNewInstance(this, DeserializeResponseToolChoice, data, options);

    string IPersistableModel<ResponseToolChoice>.GetFormatFromOptions(ModelReaderWriterOptions options) => "J";

    internal static void SerializeResponseToolChoice(
        ResponseToolChoice instance,
        Utf8JsonWriter writer,
        ModelReaderWriterOptions options)
    {
        if (instance._toolChoiceOption is not null)
        {
            writer.WriteStringValue(instance._toolChoiceOption.ToString());
        }
        else if (instance._toolChoiceObject is not null)
        {
            writer.WriteObjectValue(instance._toolChoiceObject, options);
        }
    }

    internal static ResponseToolChoice DeserializeResponseToolChoice(
        JsonElement element,
        ModelReaderWriterOptions options)
    {
        if (element.ValueKind == JsonValueKind.String)
        {
            return new ResponseToolChoice(
                new InternalToolChoiceOptions(element.GetString()));
        }
        else if (element.ValueKind == JsonValueKind.Object)
        {
            return new ResponseToolChoice(
                InternalToolChoiceObject
                    .DeserializeInternalToolChoiceObject(
                        element,
                        options));
        }
        return null;
    }
}
