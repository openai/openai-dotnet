using System;
using System.ClientModel.Primitives;
using System.Text.Json;

namespace OpenAI.Responses;

public partial class CodeInterpreterToolContainer
{
    // CUSTOM: Edited to remove calls to WriteStartObject() and WriteEndObject(). 
    void IJsonModel<CodeInterpreterToolContainer>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options)
    {
#pragma warning disable SCME0001 // Type is for evaluation purposes only and is subject to change or removal in future updates.
        if (Patch.Contains("$"u8))
        {
            writer.WriteRawValue(Patch.GetJson("$"u8));
            return;
        }
#pragma warning restore SCME0001 // Type is for evaluation purposes only and is subject to change or removal in future updates.

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
        if (Optional.IsDefined(ContainerId))
        {
            writer.WriteStringValue(ContainerId);
        }
        else if (Optional.IsDefined(ContainerConfiguration))
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
        else if (element.ValueKind == JsonValueKind.Object)
        {
            container = CodeInterpreterToolContainerConfiguration.DeserializeCodeInterpreterToolContainerConfiguration(element, element.GetUtf8Bytes(), options);
        }
        else
        {
            throw new JsonException($"Expected code interpreter tool container to be null, an object, or a string but found {element.ValueKind}.");
        }

        return new CodeInterpreterToolContainer(containerId, container, patch);
    }

#pragma warning disable SCME0001 // Type is for evaluation purposes only and is subject to change or removal in future updates.
    private bool PropagateGet(ReadOnlySpan<byte> jsonPath, out JsonPatch.EncodedValue value)
    {
        value = default;
        return ContainerConfiguration is not null
            && !jsonPath.SequenceEqual("$"u8)
            && ContainerConfiguration.Patch.TryGetEncodedValue(jsonPath, out value);
    }

    private bool PropagateSet(ReadOnlySpan<byte> jsonPath, JsonPatch.EncodedValue value)
    {
        if (ContainerConfiguration is null || jsonPath.SequenceEqual("$"u8))
        {
            return false;
        }

        ContainerConfiguration.Patch.Set(jsonPath, value);
        return true;
    }
#pragma warning restore SCME0001 // Type is for evaluation purposes only and is subject to change or removal in future updates.
}
