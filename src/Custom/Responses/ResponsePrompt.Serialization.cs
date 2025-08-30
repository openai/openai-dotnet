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
                if (variable.Value is JsonElement element)
                {
#if NET6_0_OR_GREATER
                    writer.WriteRawValue(element.GetRawText());
#else
                    using JsonDocument document = JsonDocument.Parse(element.GetRawText());
                    JsonSerializer.Serialize(writer, document.RootElement);
#endif
                }
                else if (variable.Value != null)
                {
                    // Handle primitive types directly
                    switch (variable.Value)
                    {
                        case string str:
                            writer.WriteStringValue(str);
                            break;
                        case int intVal:
                            writer.WriteNumberValue(intVal);
                            break;
                        case long longVal:
                            writer.WriteNumberValue(longVal);
                            break;
                        case float floatVal:
                            writer.WriteNumberValue(floatVal);
                            break;
                        case double doubleVal:
                            writer.WriteNumberValue(doubleVal);
                            break;
                        case decimal decimalVal:
                            writer.WriteNumberValue(decimalVal);
                            break;
                        case bool boolVal:
                            writer.WriteBooleanValue(boolVal);
                            break;
                        default:
                            // For other types, write as string value
                            writer.WriteStringValue(variable.Value.ToString());
                            break;
                    }
                }
                else
                {
                    writer.WriteNullValue();
                }
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
                    // Handle different JSON value types appropriately
                    object value = variable.Value.ValueKind switch
                    {
                        JsonValueKind.String => variable.Value.GetString(),
                        JsonValueKind.Number => variable.Value.TryGetInt32(out int intVal) ? intVal : variable.Value.GetDouble(),
                        JsonValueKind.True => true,
                        JsonValueKind.False => false,
                        JsonValueKind.Null => null,
                        _ => variable.Value.GetRawText() // For objects/arrays, store as raw JSON string
                    };
                    variables[variable.Name] = value;
                }
            }
        }

        var result = new ResponsePrompt();
        result.Id = id;
        result.Version = version;
        
        // Add variables to the existing dictionary (Variables property is get-only)
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
