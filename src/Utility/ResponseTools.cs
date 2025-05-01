using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using OpenAI.Embeddings;
using OpenAI.Responses;

namespace OpenAI.Responses;

/// <summary>
/// Provides functionality to manage and execute OpenAI function tools for responses.
/// </summary>
public class ResponseTools : ToolsBase<ResponseTool>
{
    /// <summary>
    /// Initializes a new instance of the ResponseTools class with an optional embedding client.
    /// </summary>
    /// <param name="client">The embedding client used for tool vectorization, or null to disable vectorization.</param>
    public ResponseTools(EmbeddingClient client = null) : base(client) { }

    /// <summary>
    /// Initializes a new instance of the ResponseTools class with the specified tool types.
    /// </summary>
    /// <param name="tool">The primary tool type to add.</param>
    /// <param name="additionalTools">Additional tool types to add.</param>
    public ResponseTools(Type tool, params Type[] additionalTools) : this((EmbeddingClient)null)
    {
        Add(tool);
        if (additionalTools != null)
            foreach (var t in additionalTools)
                Add(t);
    }

    internal override ResponseTool MethodInfoToTool(MethodInfo methodInfo) =>
        ResponseTool.CreateFunctionTool(methodInfo.Name, GetMethodDescription(methodInfo), BuildParametersJson(methodInfo.GetParameters()));

    internal override async Task Add(BinaryData toolDefinitions, McpClient client)
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

    /// <summary>
    /// Converts the tools collection to <see cref="ResponseCreationOptions"> configured with the tools contained in this instance..
    /// </summary>
    /// <returns>A new ResponseCreationOptions containing all defined tools.</returns>
    public ResponseCreationOptions ToOptions()
    {
        var options = new ResponseCreationOptions();
        foreach (var tool in _definitions)
            options.Tools.Add(tool);
        return options;
    }

    /// <summary>
    /// Converts the tools collection to <see cref="ResponseCreationOptions">, filtered by relevance to the given prompt. Filtering is only applied if <see cref="CanFilterTools"/> is true.
    /// </summary>
    /// <param name="prompt">The prompt to find relevant tools for.</param>
    /// <param name="options">Options for filtering tools, including maximum number of tools to return.</param>
    /// <returns>A new ResponseCreationOptions containing the most relevant tools.</returns>
    public ResponseCreationOptions ToOptions(string prompt, ToolFindOptions options = null)
    {
        if (!CanFilterTools)
            return ToOptions();

        var completionOptions = new ResponseCreationOptions();
        foreach (var tool in RelatedTo(prompt, options?.MaxEntries ?? 5))
            completionOptions.Tools.Add(tool);
        return completionOptions;
    }

    /// <summary>
    /// Implicitly converts ResponseTools to ResponseCreationOptions.
    /// </summary>
    /// <param name="tools">The ResponseTools instance to convert.</param>
    public static implicit operator ResponseCreationOptions(ResponseTools tools) => tools.ToOptions();

    internal string CallLocal(FunctionCallResponseItem call)
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

    internal async Task<string> CallMcpAsync(FunctionCallResponseItem call)
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

    /// <summary>
    /// Executes a function call and returns its result as a FunctionCallOutputResponseItem.
    /// </summary>
    /// <param name="toolCall">The function call to execute.</param>
    /// <returns>A task that represents the asynchronous operation and contains the function call result.</returns>
    public async Task<FunctionCallOutputResponseItem> CallAsync(FunctionCallResponseItem toolCall)
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
                return new FunctionCallOutputResponseItem(toolCall.CallId, $"I don't have a tool called {toolCall.FunctionName}");
            }
        }

        var result = isMcpTool ? await CallMcpAsync(toolCall).ConfigureAwait(false) : CallLocal(toolCall);
        return new FunctionCallOutputResponseItem(toolCall.CallId, result);
    }
}

