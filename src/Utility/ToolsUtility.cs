using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using OpenAI.Agents;
using OpenAI.Embeddings;

namespace OpenAI;

internal static class ToolsUtility
{
    internal static readonly BinaryData EmptyParameters = BinaryData.FromString("""{ "type" : "object", "properties" : {} }""");
    internal const string McpToolSeparator = "_-_";

    internal static string GetMethodDescription(MethodInfo method)
    {
        var description = method.Name;
        var attr = method.GetCustomAttribute<DescriptionAttribute>();
        if (attr != null)
            description = attr.Description;
        return description;
    }

    internal static string GetParameterDescription(ParameterInfo param)
    {
        string description = param.Name!;
        var attr = param.GetCustomAttribute<DescriptionAttribute>();
        if (attr != null)
            description = attr.Description;
        return description;
    }

    internal static ReadOnlySpan<byte> ClrToJsonTypeUtf8(Type clrType) =>
        clrType switch
        {
            Type t when t == typeof(double) => "number"u8,
            Type t when t == typeof(int) => "number"u8,
            Type t when t == typeof(long) => "number"u8,
            Type t when t == typeof(float) => "number"u8,
            Type t when t == typeof(string) => "string"u8,
            Type t when t == typeof(bool) => "bool"u8,
            _ => throw new NotImplementedException()
        };

    internal static BinaryData BuildParametersJson(ParameterInfo[] parameters)
    {
        if (parameters.Length == 0)
            return EmptyParameters;

        var required = new List<string>();
        using var stream = new MemoryStream();
        using var writer = new Utf8JsonWriter(stream);
        writer.WriteStartObject();
        writer.WriteString("type"u8, "object"u8);
        writer.WriteStartObject("properties"u8);

        foreach (ParameterInfo parameter in parameters)
        {
            writer.WriteStartObject(parameter.Name!);
            writer.WriteString("type"u8, ClrToJsonTypeUtf8(parameter.ParameterType));
            writer.WriteString("description"u8, GetParameterDescription(parameter));
            writer.WriteEndObject();

            if (!parameter.IsOptional || (parameter.HasDefaultValue && parameter.DefaultValue is not null))
                required.Add(parameter.Name!);
        }

        writer.WriteEndObject(); // properties
        writer.WriteStartArray("required");
        foreach (string param in required)
            writer.WriteStringValue(param);
        writer.WriteEndArray();
        writer.WriteEndObject();
        writer.Flush();
        stream.Position = 0;
        return BinaryData.FromStream(stream);
    }

    internal static async Task<ReadOnlyMemory<float>> GetEmbeddingAsync(EmbeddingClient client, string text)
    {
        var result = await client.GenerateEmbeddingAsync(text).ConfigureAwait(false);
        return result.Value.ToFloats();
    }

    internal static ReadOnlyMemory<float> GetEmbedding(EmbeddingClient client, string text)
    {
        var result = client.GenerateEmbedding(text);
        return result.Value.ToFloats();
    }

    internal static float CosineSimilarity(ReadOnlySpan<float> x, ReadOnlySpan<float> y)
    {
        float dot = 0, xSumSquared = 0, ySumSquared = 0;
        for (int i = 0; i < x.Length; i++)
        {
            dot += x[i] * y[i];
            xSumSquared += x[i] * x[i];
            ySumSquared += y[i] * y[i];
        }
#if NETSTANDARD2_0
        return dot / (float)(Math.Sqrt(xSumSquared) * (float)Math.Sqrt(ySumSquared));
#else
        return dot / (MathF.Sqrt(xSumSquared) * MathF.Sqrt(ySumSquared));
#endif
    }

    internal static IEnumerable<(string name, string description, string inputSchema)> ParseMcpToolDefinitions(BinaryData toolDefinitions, McpClient client)
    {
        using var document = JsonDocument.Parse(toolDefinitions);
        if (!document.RootElement.TryGetProperty("tools", out JsonElement toolsElement))
            throw new JsonException("The JSON document must contain a 'tools' array.");

        var serverKey = client.Endpoint.Host + client.Endpoint.Port.ToString();
        var result = new List<(string name, string description, string inputSchema)>();

        foreach (var tool in toolsElement.EnumerateArray())
        {
            var name = $"{serverKey}{McpToolSeparator}{tool.GetProperty("name").GetString()!}";
            var description = tool.GetProperty("description").GetString()!;
            var inputSchemaElement = tool.GetProperty("inputSchema");
            string inputSchema;
            using (var stream = new MemoryStream())
            {
                using (var writer = new Utf8JsonWriter(stream))
                {
                    inputSchemaElement.WriteTo(writer);
                }
                inputSchema = System.Text.Encoding.UTF8.GetString(stream.ToArray());
            }

            result.Add((name, description, inputSchema));
        }

        return result;
    }

    internal static void ParseFunctionCallArgs(MethodInfo method, BinaryData functionCallArguments, out List<object> arguments)
    {
        arguments = new List<object>();
        using var document = JsonDocument.Parse(functionCallArguments);
        var parameters = method.GetParameters();
        var argumentsByName = document.RootElement.EnumerateObject().ToDictionary(p => p.Name, p => p.Value);

        foreach (var param in parameters)
        {
            if (!argumentsByName.TryGetValue(param.Name!, out var value))
            {
                if (param.HasDefaultValue)
                {
                    arguments.Add(param.DefaultValue!);
                    continue;
                }
                throw new JsonException($"Required parameter '{param.Name}' not found in function call arguments.");
            }

            arguments.Add(value.ValueKind switch
            {
                JsonValueKind.String => value.GetString()!,
                JsonValueKind.Number when param.ParameterType == typeof(int) => value.GetInt32(),
                JsonValueKind.Number when param.ParameterType == typeof(long) => value.GetInt64(),
                JsonValueKind.Number when param.ParameterType == typeof(double) => value.GetDouble(),
                JsonValueKind.Number when param.ParameterType == typeof(float) => value.GetSingle(),
                JsonValueKind.True => true,
                JsonValueKind.False => false,
                JsonValueKind.Null when param.HasDefaultValue => param.DefaultValue!,
                _ => throw new NotImplementedException($"Conversion from {value.ValueKind} to {param.ParameterType.Name} is not implemented.")
            });
        }
    }

    internal static IEnumerable<VectorDatabaseEntry> GetClosestEntries(List<VectorDatabaseEntry> entries, int maxTools, float minVectorDistance, ReadOnlyMemory<float> vector)
    {
        var distances = entries
                        .Select((e, i) => (Distance: 1f - ToolsUtility.CosineSimilarity(e.Vector.Span, vector.Span), Index: i))
                        .OrderBy(t => t.Distance)
                        .Take(maxTools)
                        .Where(t => t.Distance <= minVectorDistance);

        return distances.Select(d => entries[d.Index]);
    }

    internal static BinaryData SerializeTool(string name, string description, BinaryData parameters)
    {
        using var stream = new MemoryStream();
        using var writer = new Utf8JsonWriter(stream, new JsonWriterOptions { Indented = true });

        writer.WriteStartObject();
        writer.WriteString("name", name);
        writer.WriteString("description", description);
        writer.WritePropertyName("inputSchema");
        using (var doc = JsonDocument.Parse(parameters))
            doc.RootElement.WriteTo(writer);
        writer.WriteEndObject();
        writer.Flush();

        stream.Position = 0;
        return BinaryData.FromStream(stream);
    }

    internal static async Task<string> CallFunctionToolAsync(Dictionary<string, MethodInfo> methods, string name, object[] arguments)
    {
        if (!methods.TryGetValue(name, out MethodInfo method))
            throw new InvalidOperationException($"Tool not found: {name}");

        object result;
        if (IsGenericTask(method.ReturnType, out Type taskResultType))
        {
            // Method is async, invoke and await
            var task = (Task)method.Invoke(null, arguments);
            await task.ConfigureAwait(false);
            // Get the Result property from the Task
            result = taskResultType.GetProperty("Result").GetValue(task);
        }
        else
        {
            // Method is synchronous
            result = method.Invoke(null, arguments);
        }

        return result?.ToString() ?? string.Empty;
    }

    private static bool IsGenericTask(Type type, out Type taskResultType)
    {
        while (type != null && type != typeof(object))
        {
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Task<>))
            {
                taskResultType = type;//type.GetGenericArguments()[0];
                return true;
            }

            type = type.BaseType!;
        }

        taskResultType = null;
        return false;
    }
}