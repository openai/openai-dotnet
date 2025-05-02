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
            Type t when t == typeof(string) => "string"u8,
            Type t when t == typeof(bool) => "bool"u8,
            _ => throw new NotImplementedException()
        };

    internal static string ClrToJsonTypeUtf16(Type clrType) =>
        clrType switch
        {
            Type t when t == typeof(double) => "number",
            Type t when t == typeof(int) => "number",
            Type t when t == typeof(string) => "string",
            Type t when t == typeof(bool) => "bool",
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

    internal static void ParseFunctionCallArgs(BinaryData functionCallArguments, out List<object> arguments)
    {
        arguments = new List<object>();
        using var document = JsonDocument.Parse(functionCallArguments);
        foreach (JsonProperty argument in document.RootElement.EnumerateObject())
        {
            arguments.Add(argument.Value.ValueKind switch
            {
                JsonValueKind.String => argument.Value.GetString()!,
                JsonValueKind.Number => argument.Value.GetInt32(),
                JsonValueKind.True => true,
                JsonValueKind.False => false,
                _ => throw new NotImplementedException()
            });
        }
    }

    internal static IEnumerable<VectorbaseEntry> GetClosestEntries(List<VectorbaseEntry> entries, int maxTools, float minVectorDistance, ReadOnlyMemory<float> vector)
    {
        var distances = entries
                        .Select((e, i) => (Distance: 1f - ToolsUtility.CosineSimilarity(e.Vector.Span, vector.Span), Index: i))
                        .OrderBy(t => t.Distance)
                        .Take(maxTools)
                        .Where(t => t.Distance <= minVectorDistance);

        return distances.Select(d => entries[d.Index]);
    }
}