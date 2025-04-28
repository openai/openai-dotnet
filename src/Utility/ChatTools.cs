using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using OpenAI.Chat;
using OpenAI.Embeddings;

namespace OpenAI;

/// <summary>
/// The service client for OpenAI Chat Completions endpoint tools.
/// </summary>
public class ChatTools : ToolsBase<ChatTool>
{
    public ChatTools(EmbeddingClient client = null) : base(client) { }

    public ChatTools(Type tool, params Type[] additionalTools) : this((EmbeddingClient)null)
    {
        Add(tool);
        if (additionalTools != null)
            foreach (var t in additionalTools)
                Add(t);
    }

    internal override ChatTool MethodInfoToTool(MethodInfo methodInfo) =>
        ChatTool.CreateFunctionTool(methodInfo.Name, GetMethodDescription(methodInfo), BuildParametersJson(methodInfo.GetParameters()));

    protected override async Task Add(BinaryData toolDefinitions, McpClient client)
    {
        using var document = JsonDocument.Parse(toolDefinitions);
        if (!document.RootElement.TryGetProperty("tools", out JsonElement toolsElement))
            throw new JsonException("The JSON document must contain a 'tools' array.");

        var serverKey = client.ServerEndpoint.Host + client.ServerEndpoint.Port.ToString();
        List<ChatTool> toolsToVectorize = new();

        foreach (var tool in toolsElement.EnumerateArray())
        {
            var name = $"{serverKey}{_mcpToolSeparator}{tool.GetProperty("name").GetString()!}";
            var description = tool.GetProperty("description").GetString()!;
#pragma warning disable IL2026, IL3050
            var inputSchema = JsonSerializer.Serialize(
                JsonSerializer.Deserialize<JsonElement>(tool.GetProperty("inputSchema").GetRawText()),
                new JsonSerializerOptions { Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping });
#pragma warning restore IL2026, IL3050

            var chatTool = ChatTool.CreateFunctionTool(name, description, BinaryData.FromString(inputSchema));
            _definitions.Add(chatTool);
            toolsToVectorize.Add(chatTool);
            _mcpMethods[name] = client.CallToolAsync;
        }

        await AddToolsToVectorBaseAsync(toolsToVectorize).ConfigureAwait(false);
    }

    protected override string GetDescription(ChatTool tool) => tool.FunctionDescription;

    protected override BinaryData SerializeTool(ChatTool tool)
    {
        using var stream = new MemoryStream();
        using var writer = new Utf8JsonWriter(stream, new JsonWriterOptions { Indented = true });

        writer.WriteStartObject();
        writer.WriteString("name", tool.FunctionName);
        writer.WriteString("description", tool.FunctionDescription);
        writer.WritePropertyName("inputSchema");
        using (var doc = JsonDocument.Parse(tool.FunctionParameters))
            doc.RootElement.WriteTo(writer);
        writer.WriteEndObject();
        writer.Flush();

        stream.Position = 0;
        return BinaryData.FromStream(stream);
    }

    protected override ChatTool ParseToolDefinition(BinaryData data)
    {
        using var document = JsonDocument.Parse(data);
        var root = document.RootElement;

        return ChatTool.CreateFunctionTool(
            root.GetProperty("name").GetString()!,
            root.GetProperty("description").GetString()!,
            BinaryData.FromString(root.GetProperty("inputSchema").GetRawText()));
    }

    public ChatCompletionOptions ToOptions()
    {
        var options = new ChatCompletionOptions();
        foreach (var tool in _definitions)
            options.Tools.Add(tool);
        return options;
    }

    public ChatCompletionOptions ToOptions(string prompt, ToolFindOptions options = null)
    {
        if (!CanFilterTools)
            return ToOptions();

        var completionOptions = new ChatCompletionOptions();
        foreach (var tool in RelatedTo(prompt, options?.MaxEntries ?? 5))
            completionOptions.Tools.Add(tool);
        return completionOptions;
    }

    public static implicit operator ChatCompletionOptions(ChatTools tools) => tools.ToOptions();
}

