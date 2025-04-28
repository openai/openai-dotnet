using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using OpenAI.Embeddings;
using OpenAI.Responses;

namespace OpenAI;

/// <summary>
/// The service client for OpenAI Responses endpoint tools.
/// </summary>
public class ResponseTools : ToolsBase<ResponseTool>
{
    public ResponseTools(EmbeddingClient client = null) : base(client) { }

    public ResponseTools(Type tool, params Type[] additionalTools) : this((EmbeddingClient)null)
    {
        Add(tool);
        if (additionalTools != null)
            foreach (var t in additionalTools)
                Add(t);
    }

    internal override ResponseTool MethodInfoToTool(MethodInfo methodInfo) =>
        ResponseTool.CreateFunctionTool(methodInfo.Name, GetMethodDescription(methodInfo), BuildParametersJson(methodInfo.GetParameters()));

    protected override async Task Add(BinaryData toolDefinitions, McpClient client)
    {
        using var document = JsonDocument.Parse(toolDefinitions);
        if (!document.RootElement.TryGetProperty("tools", out JsonElement toolsElement))
            throw new JsonException("The JSON document must contain a 'tools' array.");

        var serverKey = client.ServerEndpoint.Host + client.ServerEndpoint.Port.ToString();
        List<ResponseTool> toolsToVectorize = new();

        foreach (var tool in toolsElement.EnumerateArray())
        {
            var name = $"{serverKey}{_mcpToolSeparator}{tool.GetProperty("name").GetString()!}";
            var description = tool.GetProperty("description").GetString()!;
#pragma warning disable IL2026, IL3050
            var inputSchema = JsonSerializer.Serialize(
                JsonSerializer.Deserialize<JsonElement>(tool.GetProperty("inputSchema").GetRawText()),
                new JsonSerializerOptions { Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping });
#pragma warning restore IL2026, IL3050

            var responseTool = ResponseTool.CreateFunctionTool(name, description, BinaryData.FromString(inputSchema));
            _definitions.Add(responseTool);
            toolsToVectorize.Add(responseTool);
            _mcpMethods[name] = client.CallToolAsync;
        }

        await AddToolsToVectorBaseAsync(toolsToVectorize).ConfigureAwait(false);
    }

    protected override string GetDescription(ResponseTool tool) => (tool as InternalResponsesFunctionTool)?.Description ?? "";

    protected override BinaryData SerializeTool(ResponseTool tool)
    {
        var functionTool = tool as InternalResponsesFunctionTool;
        using var stream = new MemoryStream();
        using var writer = new Utf8JsonWriter(stream, new JsonWriterOptions { Indented = true });

        writer.WriteStartObject();
        writer.WriteString("name", functionTool?.Name);
        writer.WriteString("description", functionTool?.Description);
        writer.WritePropertyName("inputSchema");
        using (var doc = JsonDocument.Parse(functionTool?.Parameters ?? BinaryData.FromString("{}")))
            doc.RootElement.WriteTo(writer);
        writer.WriteEndObject();
        writer.Flush();

        stream.Position = 0;
        return BinaryData.FromStream(stream);
    }

    protected override ResponseTool ParseToolDefinition(BinaryData data)
    {
        using var document = JsonDocument.Parse(data);
        var root = document.RootElement;

        return ResponseTool.CreateFunctionTool(
            root.GetProperty("name").GetString()!,
            root.GetProperty("description").GetString()!,
            BinaryData.FromString(root.GetProperty("inputSchema").GetRawText()));
    }

    public ResponseCreationOptions ToOptions()
    {
        var options = new ResponseCreationOptions();
        foreach (var tool in _definitions)
            options.Tools.Add(tool);
        return options;
    }

    public ResponseCreationOptions ToOptions(string prompt, ToolFindOptions options = null)
    {
        if (!CanFilterTools)
            return ToOptions();

        var completionOptions = new ResponseCreationOptions();
        foreach (var tool in RelatedTo(prompt, options?.MaxEntries ?? 5))
            completionOptions.Tools.Add(tool);
        return completionOptions;
    }

    public static implicit operator ResponseCreationOptions(ResponseTools tools) => tools.ToOptions();
}

