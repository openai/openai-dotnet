using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using OpenAI.Chat;
using OpenAI.Embeddings;

namespace OpenAI;

/// <summary>
/// Base class containing common functionality for tool management.
/// </summary>
/// <typeparam name="TTool">The concrete tool type (ChatTool or ResponseTool)</typeparam>
public abstract class ToolsBase<TTool> where TTool : class
{
    protected static readonly BinaryData s_noparams = BinaryData.FromString("""{ "type" : "object", "properties" : {} }""");

    protected readonly Dictionary<string, MethodInfo> _methods = [];
    protected readonly Dictionary<string, Func<string, BinaryData, Task<BinaryData>>> _mcpMethods = [];
    protected readonly List<TTool> _definitions = [];
    protected readonly EmbeddingClient _client;
    protected readonly List<VectorbaseEntry> _entries = [];

    protected readonly List<McpClient> _mcpClients = [];
    protected readonly Dictionary<string, McpClient> _mcpClientsByEndpoint = [];
    protected const string _mcpToolSeparator = "_._";

    protected ToolsBase(EmbeddingClient client = null)
    {
        _client = client;
    }

    public IList<TTool> Definitions => _definitions;
    public bool CanFilterTools => _client != null;

    public void AddLocalTools(params Type[] tools)
    {
        foreach (Type functionHolder in tools)
            Add(functionHolder);
    }

    public async Task AddMcpServerAsync(McpClient client)
    {
        if (client == null) throw new ArgumentNullException(nameof(client));
        _mcpClientsByEndpoint[client.ServerEndpoint.AbsoluteUri] = client;
        await client.StartAsync().ConfigureAwait(false);
        BinaryData tools = await client.ListToolsAsync().ConfigureAwait(false);
        await Add(tools, client).ConfigureAwait(false);
        _mcpClients.Add(client);
    }

    public async Task AddMcpServerAsync(Uri serverEndpoint)
    {
        var client = new McpClient(serverEndpoint);
        await AddMcpServerAsync(client).ConfigureAwait(false);
    }

    public void Add(Type functions)
    {
#pragma warning disable IL2070
        foreach (MethodInfo function in functions.GetMethods(BindingFlags.Public | BindingFlags.Static))
        {
            Add(function);
        }
#pragma warning restore IL2070
    }

    public void Add(MethodInfo function)
    {
        string name = function.Name;
        var tool = MethodInfoToTool(function);
        _definitions.Add(tool);
        _methods[name] = function;
    }

    internal abstract TTool MethodInfoToTool(MethodInfo methodInfo);

    protected abstract Task Add(BinaryData toolDefinitions, McpClient client);

    public string Call(string name, object[] arguments)
    {
        if (!_methods.TryGetValue(name, out MethodInfo method))
            return $"I don't have a tool called {name}";

        object result = method.Invoke(null, arguments);
        return result?.ToString() ?? string.Empty;
    }

    public string Call(ChatToolCall call)
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
        return Call(call.FunctionName, [.. arguments]);
    }

    protected async Task<string> CallMcp(ChatToolCall call)
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

    public IEnumerable<ToolChatMessage> CallAll(IEnumerable<ChatToolCall> toolCalls)
    {
        var messages = new List<ToolChatMessage>();
        foreach (ChatToolCall toolCall in toolCalls)
        {
            var result = Call(toolCall);
            messages.Add(new ToolChatMessage(toolCall.Id, result));
        }
        return messages;
    }

    public async Task<ToolCallChatResult> CallAllWithErrors(IEnumerable<ChatToolCall> toolCalls)
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

            var result = isMcpTool ? await CallMcp(toolCall).ConfigureAwait(false) : Call(toolCall);
            messages.Add(new ToolChatMessage(toolCall.Id, result));
        }

        return new(messages, failed);
    }

    protected static BinaryData BuildParametersJson(ParameterInfo[] parameters)
    {
        if (parameters.Length == 0)
            return s_noparams;

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

    protected async Task AddToolsToVectorBaseAsync(IEnumerable<TTool> tools)
    {
        if (!CanFilterTools) return;

        var toolsList = tools.ToList();
        OpenAIEmbeddingCollection embeddings =
            await _client!.GenerateEmbeddingsAsync(toolsList.Select(GetDescription)).ConfigureAwait(false);

        foreach (OpenAIEmbedding embedding in embeddings)
        {
            ReadOnlyMemory<float> vector = embedding.ToFloats();
            TTool item = toolsList[embedding.Index];
            BinaryData toolDefinition = SerializeTool(item);
            _entries.Add(new VectorbaseEntry(vector, toolDefinition));
        }
    }

    protected static string GetMethodDescription(MethodInfo method)
    {
        var description = method.Name;
        var attr = method.GetCustomAttribute<DescriptionAttribute>();
        if (attr != null)
            description = attr.Description;
        return description;
    }

    protected static string GetParameterDescription(ParameterInfo param)
    {
        string description = param.Name!;
        var attr = param.GetCustomAttribute<DescriptionAttribute>();
        if (attr != null)
            description = attr.Description;
        return description;
    }

    protected static ReadOnlySpan<byte> ClrToJsonTypeUtf8(Type clrType) =>
        clrType switch
        {
            Type t when t == typeof(double) => "number"u8,
            Type t when t == typeof(string) => "string"u8,
            Type t when t == typeof(bool) => "bool"u8,
            _ => throw new NotImplementedException()
        };

    protected static string ClrToJsonTypeUtf16(Type clrType) =>
        clrType switch
        {
            Type t when t == typeof(double) => "number",
            Type t when t == typeof(string) => "string",
            Type t when t == typeof(bool) => "bool",
            _ => throw new NotImplementedException()
        };

    private async Task<ReadOnlyMemory<float>> GetEmbedding(string text)
    {
        var result = await _client!.GenerateEmbeddingAsync(text).ConfigureAwait(false);
        return result.Value.ToFloats();
    }

    protected IEnumerable<TTool> RelatedTo(string prompt, int maxEntries = 5)
    {
        if (!CanFilterTools)
            return _definitions;

        var options = new ToolFindOptions { MaxEntries = maxEntries };
        return Find(prompt, options).Select(e => ParseToolDefinition(e.Data));
    }

    protected IEnumerable<VectorbaseEntry> Find(string prompt, ToolFindOptions options)
    {
        ReadOnlyMemory<float> vector = GetEmbedding(prompt).GetAwaiter().GetResult();
        lock (_entries)
        {
            var distances = _entries
                .Select((e, i) => (Distance: 1f - CosineSimilarity(e.Vector.Span, vector.Span), Index: i))
                .OrderBy(t => t.Distance)
                .Take(options.MaxEntries)
                .Where(t => t.Distance <= options.Threshold);

            return distances.Select(d => _entries[d.Index]);
        }
    }

    private static float CosineSimilarity(ReadOnlySpan<float> x, ReadOnlySpan<float> y)
    {
        float dot = 0, xSumSquared = 0, ySumSquared = 0;
        for (int i = 0; i < x.Length; i++)
        {
            dot += x[i] * y[i];
            xSumSquared += x[i] * x[i];
            ySumSquared += y[i] * y[i];
        }
        return dot / (MathF.Sqrt(xSumSquared) * MathF.Sqrt(ySumSquared));
    }

    // Abstract methods that concrete implementations must provide
    protected abstract string GetDescription(TTool tool);
    protected abstract BinaryData SerializeTool(TTool tool);
    protected abstract TTool ParseToolDefinition(BinaryData data);

    public class ToolFindOptions
    {
        public int MaxEntries { get; set; } = 3;
        public float Threshold { get; set; } = 0.29f;
    }
}