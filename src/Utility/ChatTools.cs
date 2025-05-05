using System;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using OpenAI.Agents;
using OpenAI.Chat;
using OpenAI.Embeddings;

namespace OpenAI.Chat;

/// <summary>
/// Provides functionality to manage and execute OpenAI function tools for chat completions.
/// </summary>
public class ChatTools
{
    private readonly Dictionary<string, MethodInfo> _methods = [];
    private readonly Dictionary<string, Func<string, BinaryData, Task<BinaryData>>> _mcpMethods = [];
    private readonly List<ChatTool> _tools = [];
    private readonly EmbeddingClient _client;
    private readonly List<VectorbaseEntry> _entries = [];
    private readonly List<McpClient> _mcpClients = [];
    private readonly Dictionary<string, McpClient> _mcpClientsByEndpoint = [];

    /// <summary>
    /// Initializes a new instance of the ChatTools class with an optional embedding client.
    /// </summary>
    /// <param name="client">The embedding client used for tool vectorization, or null to disable vectorization.</param>
    public ChatTools(EmbeddingClient client = null)
    {
        _client = client;
    }

    /// <summary>
    /// Initializes a new instance of the ChatTools class with the specified tool types.
    /// </summary>
    /// <param name="tools">Additional tool types to add.</param>
    public ChatTools(params Type[] tools) : this((EmbeddingClient)null)
    {
        foreach (var t in tools)
            AddFunctionTool(t);
    }

    /// <summary>
    /// Gets the list of defined tools.
    /// </summary>
    public IList<ChatTool> Tools => _tools;

    /// <summary>
    /// Gets whether tools can be filtered using embeddings provided by the provided <see cref="EmbeddingClient"/> .
    /// </summary>
    public bool CanFilterTools => _client != null;

    /// <summary>
    /// Adds local tool implementations from the provided types.
    /// </summary>
    /// <param name="tools">Types containing static methods to be used as tools.</param>
    public void AddFunctionTools(params Type[] tools)
    {
        foreach (Type functionHolder in tools)
            AddFunctionTool(functionHolder);
    }

    /// <summary>
    /// Adds all public static methods from the specified type as tools.
    /// </summary>
    /// <param name="tool">The type containing tool methods.</param>
    internal void AddFunctionTool(Type tool)
    {
#pragma warning disable IL2070
        foreach (MethodInfo function in tool.GetMethods(BindingFlags.Public | BindingFlags.Static))
        {
            AddFunctionTool(function);
        }
#pragma warning restore IL2070
    }

    internal void AddFunctionTool(MethodInfo function)
    {
        string name = function.Name;
        var tool = ChatTool.CreateFunctionTool(name, ToolsUtility.GetMethodDescription(function), ToolsUtility.BuildParametersJson(function.GetParameters()));
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
        await AddMcpToolsAsync(tools, client).ConfigureAwait(false);
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

    private async Task AddMcpToolsAsync(BinaryData toolDefinitions, McpClient client)
    {
        List<ChatTool> toolsToVectorize = new();
        var parsedTools = ToolsUtility.ParseMcpToolDefinitions(toolDefinitions, client);

        foreach (var (name, description, inputSchema) in parsedTools)
        {
            var chatTool = ChatTool.CreateFunctionTool(name, description, BinaryData.FromString(inputSchema));
            _tools.Add(chatTool);
            toolsToVectorize.Add(chatTool);
            _mcpMethods[name] = client.CallToolAsync;
        }

        if (_client != null)
        {
            var embeddings = await _client.GenerateEmbeddingsAsync(toolsToVectorize.Select(t => t.FunctionDescription).ToList()).ConfigureAwait(false);
            foreach (var embedding in embeddings.Value)
            {
                var vector = embedding.ToFloats();
                var item = toolsToVectorize[embedding.Index];
                var toolDefinition = SerializeTool(item);
                _entries.Add(new VectorbaseEntry(vector, toolDefinition));
            }
        }
    }

    private BinaryData SerializeTool(ChatTool tool)
    {
        return ToolsUtility.SerializeTool(tool.FunctionName, tool.FunctionDescription, tool.FunctionParameters);
    }

    private ChatTool ParseToolDefinition(BinaryData data)
    {
        using var document = JsonDocument.Parse(data);
        var root = document.RootElement;

        return ChatTool.CreateFunctionTool(
            root.GetProperty("name").GetString()!,
            root.GetProperty("description").GetString()!,
            BinaryData.FromString(root.GetProperty("inputSchema").GetRawText()));
    }

    /// <summary>
    /// Converts the tools collection to chat completion options.
    /// </summary>
    /// <returns>A new ChatCompletionOptions containing all defined tools.</returns>
    public ChatCompletionOptions ToChatCompletionOptions()
    {
        var options = new ChatCompletionOptions();
        foreach (var tool in _tools)
            options.Tools.Add(tool);
        return options;
    }

    /// <summary>
    /// Converts the tools collection to <see cref="ChatCompletionOptions"/>, filtered by relevance to the given prompt.
    /// </summary>
    /// <param name="prompt">The prompt to find relevant tools for.</param>
    /// <param name="maxTools">The maximum number of tools to return. Default is 3.</param>
    /// <param name="minVectorDistance">The similarity threshold for including tools. Default is 0.29.</param>
    /// <returns>A new <see cref="ChatCompletionOptions"/> containing the most relevant tools.</returns>
    public ChatCompletionOptions CreateCompletionOptions(string prompt, int maxTools = 5, float minVectorDistance = 0.29f)
    {
        if (!CanFilterTools)
            return ToChatCompletionOptions();

        var completionOptions = new ChatCompletionOptions();
        foreach (var tool in FindRelatedTools(false, prompt, maxTools, minVectorDistance).GetAwaiter().GetResult())
            completionOptions.Tools.Add(tool);
        return completionOptions;
    }

    /// <summary>
    /// Converts the tools collection to <see cref="ChatCompletionOptions"/>, filtered by relevance to the given prompt.
    /// </summary>
    /// <param name="prompt">The prompt to find relevant tools for.</param>
    /// <param name="maxTools">The maximum number of tools to return. Default is 3.</param>
    /// <param name="minVectorDistance">The similarity threshold for including tools. Default is 0.29.</param>
    /// <returns>A new <see cref="ChatCompletionOptions"/> containing the most relevant tools.</returns>
    public async Task<ChatCompletionOptions> ToChatCompletionOptions(string prompt, int maxTools = 5, float minVectorDistance = 0.29f)
    {
        if (!CanFilterTools)
            return ToChatCompletionOptions();

        var completionOptions = new ChatCompletionOptions();
        foreach (var tool in await FindRelatedTools(true, prompt, maxTools, minVectorDistance).ConfigureAwait(false))
            completionOptions.Tools.Add(tool);
        return completionOptions;
    }

    private async Task<IEnumerable<ChatTool>> FindRelatedTools(bool async, string prompt, int maxTools, float minVectorDistance)
    {
        if (!CanFilterTools)
            return _tools;

        return (await FindVectorMatches(async, prompt, maxTools, minVectorDistance).ConfigureAwait(false))
            .Select(e => ParseToolDefinition(e.Data));
    }

    private async Task<IEnumerable<VectorbaseEntry>> FindVectorMatches(bool async, string prompt, int maxTools, float minVectorDistance)
    {
        var vector = async ?
            await ToolsUtility.GetEmbeddingAsync(_client, prompt).ConfigureAwait(false) :
            ToolsUtility.GetEmbedding(_client, prompt);

        lock (_entries)
        {
            return ToolsUtility.GetClosestEntries(_entries, maxTools, minVectorDistance, vector);
        }
    }

    internal string CallLocal(ChatToolCall call)
    {
        var arguments = new List<object>();
        if (call.FunctionArguments != null)
        {
            ToolsUtility.ParseFunctionCallArgs(call.FunctionArguments, out arguments);
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

    internal async Task<string> CallMcpAsync(ChatToolCall call)
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
        if (result == null)
            throw new InvalidOperationException($"MCP tool {call.FunctionName} returned null. Function tools should always return a value.");
        return result.ToString();
    }

    /// <summary>
    /// Executes all tool calls and returns their results.
    /// </summary>
    /// <param name="toolCalls">The collection of tool calls to execute.</param>
    /// <returns>A collection of tool chat messages containing the results.</returns>
    public async Task<IEnumerable<ToolChatMessage>> CallAsync(IEnumerable<ChatToolCall> toolCalls)
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
}

