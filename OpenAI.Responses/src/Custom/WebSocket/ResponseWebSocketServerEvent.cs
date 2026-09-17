using System;
using System.ClientModel.Primitives;
using System.IO;
using System.Text.Json;
using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Responses;

[CodeGenType("ResponseWebSocketServerEvent")]
public partial class ResponseWebSocketServerEvent
{
    /// <summary>The complete original event, including unknown fields and future event types.</summary>
    public BinaryData RawData { get; internal set; }
    /// <summary>The ordinary Responses update, or null for a WebSocket error or unknown event type.</summary>
    public StreamingResponseUpdate Update { get; internal set; }

    internal static ResponseWebSocketServerEvent Parse(BinaryData data)
    {
        using var document = JsonDocument.Parse(data, ModelSerializationExtensions.JsonDocumentOptions);
        if (document.RootElement.ValueKind != JsonValueKind.Object
            || !document.RootElement.TryGetProperty("type", out var type)
            || type.ValueKind != JsonValueKind.String)
            throw new InvalidDataException("A Responses event must be an object with a string type.");
        var item = DeserializeResponseWebSocketServerEvent(document.RootElement, data, ModelReaderWriterOptions.Json);
        item.RawData = data;
        if (type.GetString().StartsWith("response.", StringComparison.Ordinal))
            item.Update = StreamingResponseUpdate.DeserializeStreamingResponseUpdate(document.RootElement, data, ModelReaderWriterOptions.Json);
        return item;
    }
}
