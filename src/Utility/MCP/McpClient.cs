using System;
using System.ClientModel.Primitives;
using System.Threading.Tasks;

namespace OpenAI;

internal class McpClient
{
    private readonly McpSession _session;
    private readonly ClientPipeline _pipeline = ClientPipeline.Create();

    public virtual Uri ServerEndpoint { get; }

    public McpClient(Uri endpoint)
    {
        _session = new McpSession(endpoint, _pipeline);
        ServerEndpoint = endpoint;
    }

    public virtual async Task StartAsync()
    {
        await _session.EnsureInitializedAsync().ConfigureAwait(false);
    }

    public virtual async Task<BinaryData> ListToolsAsync()
    {
        if (_session == null)
            throw new InvalidOperationException("Session is not initialized. Call StartAsync() first.");

        return await _session.SendMethod("tools/list").ConfigureAwait(false);
    }

    public virtual async Task<BinaryData> CallToolAsync(string toolName, BinaryData parameters)
    {
        if (_session == null)
            throw new InvalidOperationException("Session is not initialized. Call StartAsync() first.");

        Console.WriteLine($"Calling tool {toolName}...");
        return await _session.CallTool(toolName, parameters).ConfigureAwait(false);
    }
}
