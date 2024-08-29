using System.ClientModel.Primitives;
using System.Text.Json;

namespace OpenAI.Assistants;

[CodeGenSuppress("global::System.ClientModel.Primitives.IJsonModel<OpenAI.Assistants.FileSearchToolDefinition>.Write", typeof(Utf8JsonWriter), typeof(ModelReaderWriterOptions))]
public partial class FileSearchToolDefinition : IJsonModel<FileSearchToolDefinition>
{
    void IJsonModel<FileSearchToolDefinition>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options)
        => CustomSerializationHelpers.SerializeInstance(this, SerializeFileSearchToolDefinition, writer, options);

    internal static void SerializeFileSearchToolDefinition(FileSearchToolDefinition instance, Utf8JsonWriter writer, ModelReaderWriterOptions options)
        => instance.WriteCore(writer, options);

    protected override void WriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options)
    {
        writer.WriteStartObject();
        writer.WritePropertyName("type"u8);
        writer.WriteStringValue(Type);
        if (Optional.IsDefined(MaxResults))
        {
            writer.WritePropertyName("file_search"u8);
            writer.WriteStartObject();
            writer.WritePropertyName("max_num_results"u8);
            writer.WriteNumberValue(MaxResults.Value);
            writer.WriteEndObject();
        }
        writer.WriteSerializedAdditionalRawData(SerializedAdditionalRawData, options);
        writer.WriteEndObject();
    }
}
