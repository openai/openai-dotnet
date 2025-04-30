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

    internal override async Task Add(BinaryData toolDefinitions, McpClient client)
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

    internal string CallLocal(ChatToolCall call)
    {
        var arguments = new List<object>();
        if (call.FunctionArguments != null)
        {
            using var document = JsonDocument.Parse(call.FunctionArguments);
            foreach (JsonProperty argument in document.RootElement.EnumerateObject())
            {
                arguments.Add(argument.Value.ValueKind switch
                {
                    JsonValueKind.String => argument.Value.GetString()!,
                    JsonValueKind.Number => argument.Value.GetDouble(),
                    JsonValueKind.True => true,
                    JsonValueKind.False => false,
                    _ => throw new NotImplementedException()
                });
            }
        }
        return CallLocal(call.FunctionName, [.. arguments]);
    }

    internal async Task<string> CallMcpAsync(ChatToolCall call)
    {
        if (!_mcpMethods.TryGetValue(call.FunctionName, out var method))
            throw new NotImplementedException($"MCP tool {call.FunctionName} not found.");

#if !NETSTANDARD2_0
        var actualFunctionName = call.FunctionName.Split(_mcpToolSeparator, 2)[1];
#else
        var index = call.FunctionName.IndexOf(_mcpToolSeparator);
        var actualFunctionName = call.FunctionName.Substring(index + _mcpToolSeparator.Length);
#endif
        var result = await method(actualFunctionName, call.FunctionArguments).ConfigureAwait(false);
        return result.ToString();
    }

    public async Task<IEnumerable<ToolChatMessage>> CallAllAsync(IEnumerable<ChatToolCall> toolCalls)
    {
        var messages = new List<ToolChatMessage>();
        foreach (ChatToolCall toolCall in toolCalls)
        {
            bool isMcpTool = false;
            if (!_methods.ContainsKey(toolCall.FunctionName))
            {
                if (_mcpMethods.ContainsKey(toolCall.FunctionName))
                {
                    isMcpTool = true;
                }
                else
                {
                    throw new InvalidOperationException("Tool not found: " + toolCall.FunctionName);
                }
            }

            var result = isMcpTool ? await CallMcpAsync(toolCall).ConfigureAwait(false) : CallLocal(toolCall);
            messages.Add(new ToolChatMessage(toolCall.Id, result));
        }

        return messages;
    }

    public async Task<ToolCallChatResult> CallAllWithErrorsAsync(IEnumerable<ChatToolCall> toolCalls)
    {
        List<string> failed = null;
        var messages = new List<ToolChatMessage>();

        foreach (ChatToolCall toolCall in toolCalls)
        {
            bool isMcpTool = false;
            if (!_methods.ContainsKey(toolCall.FunctionName))
            {
                if (_mcpMethods.ContainsKey(toolCall.FunctionName))
                {
                    isMcpTool = true;
                }
                else
                {
                    failed ??= new();
                    failed.Add(toolCall.FunctionName);
                    continue;
                }
            }

            var result = isMcpTool ? await CallMcpAsync(toolCall).ConfigureAwait(false) : CallLocal(toolCall);
            messages.Add(new ToolChatMessage(toolCall.Id, result));
        }

        return new(messages, failed);
    }
}

