using System;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.Text.Json;

namespace OpenAI.Responses;

// CUSTOM: This type is not its own object. Instead, it represents a union, and as such, it must directly forward
// its serialization and deserialization logic to the components of said union.
public partial class MCPToolCallApprovalPolicy
{
    // CUSTOM: Edited to remove calls to WriteStartObject() and WriteEndObject(). 
    void IJsonModel<MCPToolCallApprovalPolicy>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options)
    {
        JsonModelWriteCore(writer, options);
    }

    // CUSTOM:
    // - Edited to serialize the GlobalPolicy component as a string value.
    // - Edited to serialize the CustomPolicy component as an object value.
    // - Removed serialization of additional properties.
    protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options)
    {
        string format = options.Format == "W" ? ((IPersistableModel<MCPToolCallApprovalPolicy>)this).GetFormatFromOptions(options) : options.Format;
        if (format != "J")
        {
            throw new FormatException($"The model {nameof(MCPToolCallApprovalPolicy)} does not support writing '{format}' format.");
        }
        if (Optional.IsDefined(GlobalPolicy) && _additionalBinaryDataProperties?.ContainsKey("global_policy") != true)
        {
            writer.WriteStringValue(GlobalPolicy.Value.ToString());
        }
        if (Optional.IsDefined(CustomPolicy) && _additionalBinaryDataProperties?.ContainsKey("custom_policy") != true)
        {
            writer.WriteObjectValue(CustomPolicy, options);
        }
    }

    // CUSTOM:
    // - Edited to deserialize a string value into a GlobalPolicy component.
    // - Edited to deserialize an object value into a CustomPolicy component.
    internal static MCPToolCallApprovalPolicy DeserializeMCPToolCallApprovalPolicy(JsonElement element, ModelReaderWriterOptions options)
    {
        if (element.ValueKind == JsonValueKind.Null)
        {
            return null;
        }

        GlobalMCPToolCallApprovalPolicy? globalPolicy = default;
        CustomMCPToolCallApprovalPolicy customPolicy = default;
        IDictionary<string, BinaryData> additionalBinaryDataProperties = new ChangeTrackingDictionary<string, BinaryData>();

        if (element.ValueKind == JsonValueKind.String)
        {
            globalPolicy = new GlobalMCPToolCallApprovalPolicy(element.GetString());
        }
        else
        {
            customPolicy = CustomMCPToolCallApprovalPolicy.DeserializeCustomMCPToolCallApprovalPolicy(element, options);
        }

        return new MCPToolCallApprovalPolicy(globalPolicy, customPolicy, additionalBinaryDataProperties);
    }
}