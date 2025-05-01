using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using OpenAI.Embeddings;

namespace OpenAI;

/// <summary>
/// Base class containing common functionality for tool management.
/// </summary>
/// <typeparam name="TTool">The concrete tool type (<see cref="ChatTool"/> or <see cref="ResponseTool"/>)</typeparam>
public abstract class Tools<TTool> where TTool : class
{
    internal static readonly BinaryData s_noparams = BinaryData.FromString("""{ "type" : "object", "properties" : {} }""");

    internal readonly Dictionary<string, MethodInfo> _methods = [];
    internal readonly Dictionary<string, Func<string, BinaryData, Task<BinaryData>>> _mcpMethods = [];
    internal readonly List<TTool> _tools = [];
    internal readonly EmbeddingClient _client;
    internal readonly List<VectorbaseEntry> _entries = [];

    internal readonly List<McpClient> _mcpClients = [];
    internal readonly Dictionary<string, McpClient> _mcpClientsByEndpoint = [];
    internal const string _mcpToolSeparator = "_-_";

    protected Tools(EmbeddingClient client = null)
    {
        _client = client;
    }

    /// <summary>
    /// Gets the list of defined tools.
    /// </summary>
    public IList<TTool> ToolList => _tools;

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
    /// Adds a remote MCP server as a tool provider.
    /// </summary>
    /// <param name="client">The MCP client instance.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    internal async Task AddMcpServerAsync(McpClient client)
    {
        if (client == null) throw new ArgumentNullException(nameof(client));
        _mcpClientsByEndpoint[client.ServerEndpoint.AbsoluteUri] = client;
        await client.StartAsync().ConfigureAwait(false);
        BinaryData tools = await client.ListToolsAsync().ConfigureAwait(false);
        await Add(tools, client).ConfigureAwait(false);
        _mcpClients.Add(client);
    }

    /// <summary>
    /// Adds a remote MCP server as a tool provider.
    /// </summary>
    /// <param name="serverEndpoint">The URI endpoint of the MCP server.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task AddMcpServerAsync(Uri serverEndpoint)
    {
        var client = new McpClient(serverEndpoint);
        await AddMcpServerAsync(client).ConfigureAwait(false);
    }

    /// <summary>
    /// Adds all public static methods from the specified type as tools.
    /// </summary>
    /// <param name="functions">The type containing tool methods.</param>
    public void AddLocalTool(Type functions)
    {
#pragma warning disable IL2070
        foreach (MethodInfo function in functions.GetMethods(BindingFlags.Public | BindingFlags.Static))
        {
            AddLocalTool(function);
        }
#pragma warning restore IL2070
    }

    public void AddLocalTool(MethodInfo function)
    {
        string name = function.Name;
        var tool = MethodInfoToTool(function);
        _tools.Add(tool);
        _methods[name] = function;
    }

    internal abstract TTool MethodInfoToTool(MethodInfo methodInfo);

    internal abstract Task Add(BinaryData toolDefinitions, McpClient client);

    internal string CallLocal(string name, object[] arguments)
    {
        if (!_methods.TryGetValue(name, out MethodInfo method))
            return $"I don't have a tool called {name}";

        object result = method.Invoke(null, arguments);
        return result?.ToString() ?? string.Empty;
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
            return _tools;

        var options = new ToolSelectionOptions { MaxTools = maxEntries };
        return Find(prompt, options).Select(e => ParseToolDefinition(e.Data));
    }

    protected IEnumerable<VectorbaseEntry> Find(string prompt, ToolSelectionOptions options)
    {
        ReadOnlyMemory<float> vector = GetEmbedding(prompt).GetAwaiter().GetResult();
        lock (_entries)
        {
            var distances = _entries
                .Select((e, i) => (Distance: 1f - CosineSimilarity(e.Vector.Span, vector.Span), Index: i))
                .OrderBy(t => t.Distance)
                .Take(options.MaxTools)
                .Where(t => t.Distance <= options.MinVectorDistance);

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
#if NETSTANDARD2_0

        return dot / (float)(Math.Sqrt(xSumSquared) * (float)Math.Sqrt(ySumSquared));
#else
        return dot / (MathF.Sqrt(xSumSquared) * MathF.Sqrt(ySumSquared));
#endif
    }

    // Abstract methods that concrete implementations must provide
    protected abstract string GetDescription(TTool tool);
    protected abstract BinaryData SerializeTool(TTool tool);
    protected abstract TTool ParseToolDefinition(BinaryData data);

    /// <summary>
    /// Options for finding related tools.
    /// </summary>
    public class ToolSelectionOptions
    {
        /// <summary>
        /// Gets or sets the maximum number of tools to return. Default is 3.
        /// </summary>
        public int MaxTools { get; set; } = 3;

        /// <summary>
        /// Gets or sets the similarity threshold for including tools. Default is 0.29.
        /// </summary>
        public float MinVectorDistance { get; set; } = 0.29f;
    }
}