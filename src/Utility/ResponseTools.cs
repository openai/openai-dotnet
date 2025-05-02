using System;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using OpenAI.Agents;
using OpenAI.Embeddings;
using OpenAI.Responses;

namespace OpenAI.Responses;

/// <summary>
/// Provides functionality to manage and execute OpenAI function tools for responses.
/// </summary>
public class ResponseTools
{
    private readonly Dictionary<string, MethodInfo> _methods = [];
    private readonly Dictionary<string, Func<string, BinaryData, Task<BinaryData>>> _mcpMethods = [];
    private readonly List<ResponseTool> _tools = [];
    private readonly EmbeddingClient _client;
    private readonly List<VectorbaseEntry> _entries = [];
    private readonly List<McpClient> _mcpClients = [];
    private readonly Dictionary<string, McpClient> _mcpClientsByEndpoint = [];

    /// <summary>
    /// Initializes a new instance of the ResponseTools class with an optional embedding client.
    /// </summary>
    /// <param name="client">The embedding client used for tool vectorization, or null to disable vectorization.</param>
    public ResponseTools(EmbeddingClient client = null)
    {
        _client = client;
    }

    /// <summary>
    /// Initializes a new instance of the ResponseTools class with the specified tool types.
    /// </summary>
    /// <param name="tools">Additional tool types to add.</param>
    public ResponseTools(params Type[] tools) : this((EmbeddingClient)null)
    {
        foreach (var t in tools)
            AddLocalTool(t);
    }

    /// <summary>
    /// Gets the list of defined tools.
    /// </summary>
    public IList<ResponseTool> Tools => _tools;

    /// <summary>
    /// Gets whether tools can be filtered using embeddings provided by the provided <see cref="EmbeddingClient"/> .
    /// </summary>
    public bool CanFilterTools => _client != null;

    /// <summary>
    /// Adds local tool implementations from the provided types.
    /// </summary>
    /// <param name="tools">Types containing static methods to be used as tools.</param>
    public void AddLocalTools(params Type[] tools)
    {
        foreach (Type functionHolder in tools)
            AddLocalTool(functionHolder);
    }

    /// <summary>
    /// Adds all public static methods from the specified type as tools.
    /// </summary>
    /// <param name="tool">The type containing tool methods.</param>
    internal void AddLocalTool(Type tool)
    {
#pragma warning disable IL2070
        foreach (MethodInfo function in tool.GetMethods(BindingFlags.Public | BindingFlags.Static))
        {
            AddLocalTool(function);
        }
#pragma warning restore IL2070
    }

    internal void AddLocalTool(MethodInfo function)
    {
        string name = function.Name;
        var tool = ResponseTool.CreateFunctionTool(name, ToolsUtility.GetMethodDescription(function), ToolsUtility.BuildParametersJson(function.GetParameters()));
        _tools.Add(tool);
        _methods[name] = function;
    }

    /// <summary>
    /// Adds a remote MCP server as a tool provider.
    /// </summary>
    /// <param name="client">The MCP client instance.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task AddMcpToolsAsync(McpClient client)
    {
        if (client == null) throw new ArgumentNullException(nameof(client));
        _mcpClientsByEndpoint[client.Endpoint.AbsoluteUri] = client;
        await client.StartAsync().ConfigureAwait(false);
        BinaryData tools = await client.ListToolsAsync().ConfigureAwait(false);
        await AddToolsAsync(tools, client).ConfigureAwait(false);
        _mcpClients.Add(client);
    }

    /// <summary>
    /// Adds a remote MCP server as a tool provider.
    /// </summary>
    /// <param name="mcpEndpoint">The URI endpoint of the MCP server.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task AddMcpToolsAsync(Uri mcpEndpoint)
    {
        var client = new McpClient(mcpEndpoint);
        await AddMcpToolsAsync(client).ConfigureAwait(false);
    }

    private async Task AddToolsAsync(BinaryData toolDefinitions, McpClient client)
    {
        using var document = JsonDocument.Parse(toolDefinitions);
        if (!document.RootElement.TryGetProperty("tools", out JsonElement toolsElement))
            throw new JsonException("The JSON document must contain a 'tools' array.");

        var serverKey = client.Endpoint.Host + client.Endpoint.Port.ToString();
        List<ResponseTool> toolsToVectorize = new();

        foreach (var tool in toolsElement.EnumerateArray())
        {
            var name = $"{serverKey}{ToolsUtility.McpToolSeparator}{tool.GetProperty("name").GetString()!}";
            var description = tool.GetProperty("description").GetString()!;
#pragma warning disable IL2026, IL3050
            var inputSchema = JsonSerializer.Serialize(
                JsonSerializer.Deserialize<JsonElement>(tool.GetProperty("inputSchema").GetRawText()),
                new JsonSerializerOptions { Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping });
#pragma warning restore IL2026, IL3050

            var responseTool = ResponseTool.CreateFunctionTool(name, description, BinaryData.FromString(inputSchema));
            _tools.Add(responseTool);
            toolsToVectorize.Add(responseTool);
            _mcpMethods[name] = client.CallToolAsync;
        }

        if (_client != null)
        {
            var embeddings = await _client.GenerateEmbeddingsAsync(toolsToVectorize.ConvertAll(GetDescription)).ConfigureAwait(false);
            foreach (var embedding in embeddings.Value)
            {
                var vector = embedding.ToFloats();
                var item = toolsToVectorize[embedding.Index];
                var toolDefinition = SerializeTool(item);
                _entries.Add(new VectorbaseEntry(vector, toolDefinition));
            }
        }
    }

    private string GetDescription(ResponseTool tool) => (tool as InternalResponsesFunctionTool)?.Description ?? "";

    private BinaryData SerializeTool(ResponseTool tool)
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

    private ResponseTool ParseToolDefinition(BinaryData data)
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
    public ResponseCreationOptions CreateResponseOptions()
    {
        var options = new ResponseCreationOptions();
        foreach (var tool in _tools)
            options.Tools.Add(tool);
        return options;
    }

    /// <summary>
    /// Converts the tools collection to <see cref="ResponseCreationOptions">, filtered by relevance to the given prompt.
    /// </summary>
    /// <param name="prompt">The prompt to find relevant tools for.</param>
    /// <param name="maxTools">The maximum number of tools to return. Default is 5.</param>
    /// <param name="minVectorDistance">The similarity threshold for including tools. Default is 0.29.</param>
    /// <returns>A new ResponseCreationOptions containing the most relevant tools.</returns>
    public ResponseCreationOptions CreateResponseOptions(string prompt, int maxTools = 5, float minVectorDistance = 0.29f)
    {
        if (!CanFilterTools)
            return CreateResponseOptions();

        var completionOptions = new ResponseCreationOptions();
        foreach (var tool in FindRelatedTools(prompt, maxTools, minVectorDistance))
            completionOptions.Tools.Add(tool);
        return completionOptions;
    }

    private IEnumerable<ResponseTool> FindRelatedTools(string prompt, int maxTools, float minVectorDistance)
    {
        if (!CanFilterTools)
            return _tools;

        return FindVectorMatches(prompt, maxTools, minVectorDistance).Select(e => ParseToolDefinition(e.Data));
    }

    private IEnumerable<VectorbaseEntry> FindVectorMatches(string prompt, int maxTools, float minVectorDistance)
    {
        var vector = ToolsUtility.GetEmbedding(_client, prompt).GetAwaiter().GetResult();
        lock (_entries)
        {
            var distances = _entries
                .Select((e, i) => (Distance: 1f - ToolsUtility.CosineSimilarity(e.Vector.Span, vector.Span), Index: i))
                .OrderBy(t => t.Distance)
                .Take(maxTools)
                .Where(t => t.Distance <= minVectorDistance);

            return distances.Select(d => _entries[d.Index]);
        }
    }

    /// <summary>
    /// Implicitly converts ResponseTools to ResponseCreationOptions.
    /// </summary>
    /// <param name="tools">The ResponseTools instance to convert.</param>
    public static implicit operator ResponseCreationOptions(ResponseTools tools) => tools.CreateResponseOptions();

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
                    JsonValueKind.Number => argument.Value.GetInt32(),
                    JsonValueKind.True => true,
                    JsonValueKind.False => false,
                    _ => throw new NotImplementedException()
                });
            }
        }
        return CallLocal(call.FunctionName, [.. arguments]);
    }

    private string CallLocal(string name, object[] arguments)
    {
        if (!_methods.TryGetValue(name, out MethodInfo method))
            return $"I don't have a tool called {name}";

        object result = method.Invoke(null, arguments);
        return result?.ToString() ?? string.Empty;
    }

    internal async Task<string> CallMcpAsync(FunctionCallResponseItem call)
    {
        if (!_mcpMethods.TryGetValue(call.FunctionName, out var method))
            throw new NotImplementedException($"MCP tool {call.FunctionName} not found.");

#if !NETSTANDARD2_0
        var actualFunctionName = call.FunctionName.Split(ToolsUtility.McpToolSeparator, 2)[1];
#else
        var index = call.FunctionName.IndexOf(ToolsUtility.McpToolSeparator);
        var actualFunctionName = call.FunctionName.Substring(index + ToolsUtility.McpToolSeparator.Length);
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

