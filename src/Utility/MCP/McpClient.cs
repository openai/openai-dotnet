using System;
using System.ClientModel.Primitives;
using System.Threading.Tasks;

namespace OpenAI.Agents;

/// <summary>
/// Client for interacting with a Model Context Protocol (MCP) server.
/// </summary>
//[Experimental("OPENAIMCP001")]
public class McpClient
{
    private readonly McpSession _session;
    private readonly ClientPipeline _pipeline;

    /// <summary>
    /// Gets the endpoint URI of the MCP server.
    /// </summary>
    public virtual Uri Endpoint { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="McpClient"/> class.
    /// </summary>
    /// <param name="endpoint">The URI endpoint of the MCP server.</param>
    public McpClient(Uri endpoint)
    {
        _pipeline = ClientPipeline.Create();
        _session = new McpSession(endpoint, _pipeline);
        Endpoint = endpoint;
    }

    /// <summary>
    /// Starts the MCP client session by initializing the connection to the server.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public virtual async Task StartAsync()
    {
        await _session.EnsureInitializedAsync().ConfigureAwait(false);
    }

    /// <summary>
    /// Lists all available tools from the MCP server.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains the binary data representing the tools list.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the session is not initialized.</exception>
    public virtual async Task<BinaryData> ListToolsAsync()
    {
        if (_session == null)
            throw new InvalidOperationException("Session is not initialized. Call StartAsync() first.");

        return await _session.SendMethod("tools/list").ConfigureAwait(false);
    }

    /// <summary>
    /// Calls a specific tool on the MCP server.
    /// </summary>
    /// <param name="toolName">The name of the tool to call.</param>
    /// <param name="parameters">The parameters to pass to the tool as binary data.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the binary data representing the tool's response.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the session is not initialized.</exception>
    public virtual async Task<BinaryData> CallToolAsync(string toolName, BinaryData parameters)
    {
        if (_session == null)
            throw new InvalidOperationException("Session is not initialized. Call StartAsync() first.");

        return await _session.CallTool(toolName, parameters).ConfigureAwait(false);
    }
}
