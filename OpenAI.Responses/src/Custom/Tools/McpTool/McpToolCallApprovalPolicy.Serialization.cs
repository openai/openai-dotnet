using System;
using System.ClientModel.Primitives;
using System.Text.Json;

namespace OpenAI.Responses;

// CUSTOM: This type is not its own object. Instead, it represents a union, and as such, it must directly forward
// its serialization and deserialization logic to the components of said union.
public partial class McpToolCallApprovalPolicy
{
    // CUSTOM: Edited to remove calls to WriteStartObject() and WriteEndObject(). 
    void IJsonModel<McpToolCallApprovalPolicy>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options)
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
    // - Edited to serialize the GlobalPolicy component as a string value.
    // - Edited to serialize the CustomPolicy component as an object value.
    // - Removed serialization of additional properties.
    protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options)
    {
        string format = options.Format == "W" ? ((IPersistableModel<McpToolCallApprovalPolicy>)this).GetFormatFromOptions(options) : options.Format;
        if (format != "J")
        {
            throw new FormatException($"The model {nameof(McpToolCallApprovalPolicy)} does not support writing '{format}' format.");
        }
        if (Optional.IsDefined(GlobalPolicy))
        {
            writer.WriteStringValue(GlobalPolicy.Value.ToString());
        }
        else if (Optional.IsDefined(CustomPolicy))
        {
            writer.WriteObjectValue(CustomPolicy, options);
        }
    }

    // CUSTOM:
    // - Edited to deserialize a string value into a GlobalPolicy component.
    // - Edited to deserialize an object value into a CustomPolicy component.
    internal static McpToolCallApprovalPolicy DeserializeMcpToolCallApprovalPolicy(JsonElement element, BinaryData data, ModelReaderWriterOptions options)
    {
        if (element.ValueKind == JsonValueKind.Null)
        {
            return null;
        }

        GlobalMcpToolCallApprovalPolicy? globalPolicy = default;
        CustomMcpToolCallApprovalPolicy customPolicy = default;
#pragma warning disable SCME0001 // Type is for evaluation purposes only and is subject to change or removal in future updates.
        JsonPatch patch = new JsonPatch(data is null ? ReadOnlyMemory<byte>.Empty : data.ToMemory());
#pragma warning restore SCME0001 // Type is for evaluation purposes only and is subject to change or removal in future updates.

        if (element.ValueKind == JsonValueKind.String)
        {
            globalPolicy = new GlobalMcpToolCallApprovalPolicy(element.GetString());
        }
        else if (element.ValueKind == JsonValueKind.Object)
        {
            customPolicy = CustomMcpToolCallApprovalPolicy.DeserializeCustomMcpToolCallApprovalPolicy(element, element.GetUtf8Bytes(), options);
        }
        else
        {
            throw new JsonException($"Expected MCP tool call approval policy to be null, an object, or a string but found {element.ValueKind}.");
        }

        return new McpToolCallApprovalPolicy(globalPolicy, customPolicy, patch);
    }

#pragma warning disable SCME0001 // Type is for evaluation purposes only and is subject to change or removal in future updates.
    private bool PropagateGet(ReadOnlySpan<byte> jsonPath, out JsonPatch.EncodedValue value)
    {
        value = default;
        return CustomPolicy is not null
            && !jsonPath.SequenceEqual("$"u8)
            && CustomPolicy.Patch.TryGetEncodedValue(jsonPath, out value);
    }

    private bool PropagateSet(ReadOnlySpan<byte> jsonPath, JsonPatch.EncodedValue value)
    {
        if (CustomPolicy is null || jsonPath.SequenceEqual("$"u8))
        {
            return false;
        }

        CustomPolicy.Patch.Set(jsonPath, value);
        return true;
    }
#pragma warning restore SCME0001 // Type is for evaluation purposes only and is subject to change or removal in future updates.
}