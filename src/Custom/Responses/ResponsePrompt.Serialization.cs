using System;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.Text.Json;

namespace OpenAI.Responses;

public partial class ResponsePrompt : IJsonModel<ResponsePrompt>
{
    void IJsonModel<ResponsePrompt>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options)
        => CustomSerializationHelpers.SerializeInstance(this, SerializeResponsePrompt, writer, options);

    ResponsePrompt IJsonModel<ResponsePrompt>.Create(ref Utf8JsonReader reader, ModelReaderWriterOptions options)
        => CustomSerializationHelpers.DeserializeNewInstance(this, DeserializeResponsePrompt, ref reader, options);

    BinaryData IPersistableModel<ResponsePrompt>.Write(ModelReaderWriterOptions options)
        => CustomSerializationHelpers.SerializeInstance(this, options);

    ResponsePrompt IPersistableModel<ResponsePrompt>.Create(BinaryData data, ModelReaderWriterOptions options)
        => CustomSerializationHelpers.DeserializeNewInstance(this, DeserializeResponsePrompt, data, options);

    string IPersistableModel<ResponsePrompt>.GetFormatFromOptions(ModelReaderWriterOptions options) => "J";

    internal static void SerializeResponsePrompt(ResponsePrompt instance, Utf8JsonWriter writer, ModelReaderWriterOptions options)
    {
        writer.WriteStartObject();
        
        if (instance.Id != null)
        {
            writer.WritePropertyName("id");
            writer.WriteStringValue(instance.Id);
        }
        
        if (instance.Version != null)
        {
            writer.WritePropertyName("version");
            writer.WriteStringValue(instance.Version);
        }
        
        if (instance.Variables != null && instance.Variables.Count > 0)
        {
            writer.WritePropertyName("variables");
            writer.WriteStartObject();
            foreach (var variable in instance.Variables)
            {
                writer.WritePropertyName(variable.Key);
                JsonSerializer.Serialize(writer, variable.Value, options.Format == "W" ? null : (JsonSerializerOptions)null);
            }
            writer.WriteEndObject();
        }
        
        writer.WriteEndObject();
    }

    internal static ResponsePrompt DeserializeResponsePrompt(JsonElement element, ModelReaderWriterOptions options = null)
    {
        if (element.ValueKind == JsonValueKind.Null)
        {
            return null;
        }

        string id = null;
        string version = null;
        Dictionary<string, object> variables = new Dictionary<string, object>();

        foreach (var property in element.EnumerateObject())
        {
            if (property.NameEquals("id"))
            {
                id = property.Value.GetString();
            }
            else if (property.NameEquals("version"))
            {
                version = property.Value.GetString();
            }
            else if (property.NameEquals("variables"))
            {
                foreach (var variable in property.Value.EnumerateObject())
                {
                    variables[variable.Name] = JsonSerializer.Deserialize<object>(variable.Value.GetRawText());
                }
            }
        }

        var result = new ResponsePrompt();
        result.Id = id;
        result.Version = version;
        
        if (variables.Count > 0)
        {
            foreach (var variable in variables)
            {
                result.Variables[variable.Key] = variable.Value;
            }
        }

        return result;
    }
}
