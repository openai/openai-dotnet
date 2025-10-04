using System;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.Text.Json;


namespace OpenAI.Responses;

public partial class CodeInterpreterToolContainer
{
    // CUSTOM: Edited to remove calls to WriteStartObject() and WriteEndObject(). 
    void IJsonModel<CodeInterpreterToolContainer>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options)
    {
        JsonModelWriteCore(writer, options);
    }

    // CUSTOM:
    // - Edited to serialize the container ID component as a string value.
    // - Edited to serialize the container configuration component as an object value.
    // - Removed serialization of additional properties.
    protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options)
    {
        string format = options.Format == "W" ? ((IPersistableModel<CodeInterpreterToolContainer>)this).GetFormatFromOptions(options) : options.Format;
        if (format != "J")
        {
            throw new FormatException($"The model {nameof(CodeInterpreterToolContainer)} does not support writing '{format}' format.");
        }
        if (Optional.IsDefined(ContainerId) && _patch.Contains("$.container_id"u8) != true)
        {
            writer.WriteStringValue(ContainerId);
        }
        if (Optional.IsDefined(ContainerConfiguration) && _patch.Contains("$.container"u8) != true)
        {
            writer.WriteObjectValue(ContainerConfiguration, options);
        }
    }

    // CUSTOM:
    // - Edited to deserialize a string value into a container ID component.
    // - Edited to deserialize an object value into a container configuration component.
    internal static CodeInterpreterToolContainer DeserializeCodeInterpreterToolContainer(JsonElement element, BinaryData data, ModelReaderWriterOptions options)
    {
        if (element.ValueKind == JsonValueKind.Null)
        {
            return null;
        }

        string containerId = default;
        CodeInterpreterToolContainerConfiguration container = default;
#pragma warning disable SCME0001 // Type is for evaluation purposes only and is subject to change or removal in future updates.
        JsonPatch patch = new JsonPatch(data is null ? ReadOnlyMemory<byte>.Empty : data.ToMemory());
#pragma warning restore SCME0001 // Type is for evaluation purposes only and is subject to change or removal in future updates.

        if (element.ValueKind == JsonValueKind.String)
        {
            containerId = element.GetString();
        }
        else
        {
            container = CodeInterpreterToolContainerConfiguration.DeserializeCodeInterpreterToolContainerConfiguration(element, element.GetUtf8Bytes(), options);
        }

        return new CodeInterpreterToolContainer(containerId, container, patch);
    }
}
