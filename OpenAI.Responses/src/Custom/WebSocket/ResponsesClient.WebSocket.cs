using System;
using System.Diagnostics.CodeAnalysis;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace OpenAI.Responses;

public partial class ResponsesClient
{
    /// <summary>Creates a disconnected Responses API WebSocket client using this client's endpoint and credentials.</summary>
    /// <remarks>
    /// Subscribe to MessageReceivedAsync and ErrorOccurredAsync before calling <see cref="ResponsesWebSocketClient.ConnectAsync"/>.
    /// Options are snapshotted when the client is created. The returned client owns its connection.
    /// The initial design uses default WebSocket networking and reuses this client's authentication and headers.
    /// Custom HTTP transports and opaque, prebuilt pipelines are not supported by the initial design.
    /// Creating the client does not open a socket; call ConnectAsync to connect.
    /// </remarks>
    [Experimental("OPENAI001")]
    public virtual ResponsesWebSocketClient CreateWebSocketClient(ResponsesWebSocketClientOptions options = null)
    {
        if (_webSocketAuthenticationPolicy is null || _webSocketClientOptions.Transport != null)
        {
            throw new NotSupportedException("Responses WebSockets require a client with an authentication policy and the default HTTP transport.");
        }
        return new ResponsesWebSocketClient(this, options ?? new ResponsesWebSocketClientOptions());
    }

    internal async Task AuthenticateWebSocketAsync(ClientWebSocket socket, Uri endpoint, CancellationToken cancellationToken)
    {
        if (_webSocketAuthenticationPolicy is null)
        {
            throw new NotSupportedException("The parent client does not expose an authentication policy.");
        }
        using PipelineMessage message = Pipeline.CreateMessage();
        message.Request.Method = "GET";
        UriBuilder authenticationEndpoint = new(endpoint);
        authenticationEndpoint.Scheme = endpoint.Scheme == "ws" ? "http" : "https";
        message.Request.Uri = authenticationEndpoint.Uri;
        message.Apply(new RequestOptions { CancellationToken = cancellationToken });
        PipelinePolicy[] policies = { _webSocketAuthenticationPolicy, new WebSocketAuthenticationTerminalPolicy() };
        await _webSocketAuthenticationPolicy.ProcessAsync(message, policies, 0).ConfigureAwait(false);
        foreach (KeyValuePair<string, string> header in message.Request.Headers)
        {
            socket.Options.SetRequestHeader(header.Key, header.Value);
        }
        if (!string.IsNullOrEmpty(_webSocketClientOptions?.OrganizationId))
        {
            socket.Options.SetRequestHeader("OpenAI-Organization", _webSocketClientOptions.OrganizationId);
        }
        if (!string.IsNullOrEmpty(_webSocketClientOptions?.ProjectId))
        {
            socket.Options.SetRequestHeader("OpenAI-Project", _webSocketClientOptions.ProjectId);
        }
    }

    // Run authentication without sending an HTTP request through the REST transport.
    private sealed class WebSocketAuthenticationTerminalPolicy : PipelinePolicy
    {
        public override void Process(PipelineMessage message, IReadOnlyList<PipelinePolicy> pipeline, int currentIndex) { }
        public override ValueTask ProcessAsync(PipelineMessage message, IReadOnlyList<PipelinePolicy> pipeline, int currentIndex)
            => default;
    }
}