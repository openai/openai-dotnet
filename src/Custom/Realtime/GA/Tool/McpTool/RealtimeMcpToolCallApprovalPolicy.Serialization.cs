using System;
using System.ClientModel.Primitives;
using System.Text.Json;

namespace OpenAI.Realtime;

// CUSTOM: This type is not its own object. Instead, it represents a union, and as such, it must directly forward
// its serialization and deserialization logic to the components of said union.
public partial class RealtimeMcpToolCallApprovalPolicy
{
    // CUSTOM: Edited to remove calls to WriteStartObject() and WriteEndObject(). 
    void IJsonModel<RealtimeMcpToolCallApprovalPolicy>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options)
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

    // CUSTOM: Edited to serialize the different components of the union.
    protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options)
    {
        string format = options.Format == "W" ? ((IPersistableModel<RealtimeMcpToolCallApprovalPolicy>)this).GetFormatFromOptions(options) : options.Format;
        if (format != "J")
        {
            throw new FormatException($"The model {nameof(RealtimeMcpToolCallApprovalPolicy)} does not support writing '{format}' format.");
        }
#pragma warning disable SCME0001 // Type is for evaluation purposes only and is subject to change or removal in future updates.
        if (Optional.IsDefined(DefaultPolicy) && !Patch.Contains("$.default_policy"u8))
        {
            writer.WriteStringValue(DefaultPolicy.Value.ToString());
        }
        if (Optional.IsDefined(CustomPolicy) && !Patch.Contains("$.custom_policy"u8))
        {
            writer.WriteObjectValue(CustomPolicy, options);
        }
#pragma warning restore SCME0001 // Type is for evaluation purposes only and is subject to change or removal in future updates.
    }

    // CUSTOM: Edited to deserialize the different components of the union.
    internal static RealtimeMcpToolCallApprovalPolicy DeserializeRealtimeMcpToolCallApprovalPolicy(JsonElement element, BinaryData data, ModelReaderWriterOptions options)
    {
        if (element.ValueKind == JsonValueKind.Null)
        {
            return null;
        }

        RealtimeDefaultMcpToolCallApprovalPolicy? defaultPolicy = default;
        RealtimeCustomMcpToolCallApprovalPolicy customPolicy = default;
#pragma warning disable SCME0001 // Type is for evaluation purposes only and is subject to change or removal in future updates.
        JsonPatch patch = new JsonPatch(data is null ? ReadOnlyMemory<byte>.Empty : data.ToMemory());
#pragma warning restore SCME0001 // Type is for evaluation purposes only and is subject to change or removal in future updates.

        if (element.ValueKind == JsonValueKind.String)
        {
            defaultPolicy = new RealtimeDefaultMcpToolCallApprovalPolicy(element.GetString());
        }
        else
        {
            customPolicy = RealtimeCustomMcpToolCallApprovalPolicy.DeserializeRealtimeCustomMcpToolCallApprovalPolicy(element, element.GetUtf8Bytes(), options);
        }

        return new RealtimeMcpToolCallApprovalPolicy(defaultPolicy, customPolicy, patch);
    }
}