using System;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace OpenAI.Responses;

public partial class ResponsesWebSocketClient
{
    private async Task AuthenticateWebSocketAsync(ClientWebSocket socket, Uri endpoint, CancellationToken cancellationToken)
    {
        using PipelineMessage message = _pipeline.CreateMessage();
        message.Request.Method = "GET";
        UriBuilder authenticationEndpoint = new(endpoint);
        authenticationEndpoint.Scheme = endpoint.Scheme == "ws" ? "http" : "https";
        message.Request.Uri = authenticationEndpoint.Uri;
        message.Apply(new RequestOptions { CancellationToken = cancellationToken });
        // Authentication policies advance the pipeline; terminate it here so credentials are applied without
        // letting the normal HTTP transport send this synthetic upgrade request.
        PipelinePolicy[] policies = { _authenticationPolicy, new WebSocketAuthenticationTerminalPolicy() };
        await _authenticationPolicy.ProcessAsync(message, policies, 0).ConfigureAwait(false);
        foreach (KeyValuePair<string, string> header in message.Request.Headers)
        {
            socket.Options.SetRequestHeader(header.Key, header.Value);
        }
        if (!string.IsNullOrEmpty(_organizationId))
        {
            socket.Options.SetRequestHeader("OpenAI-Organization", _organizationId);
        }
        if (!string.IsNullOrEmpty(_projectId))
        {
            socket.Options.SetRequestHeader("OpenAI-Project", _projectId);
        }
    }

    // The authentication policy calls ProcessNext after setting headers. This terminal step intentionally
    // does nothing: the WebSocket handshake consumes the headers, not the REST HTTP pipeline.
    private sealed class WebSocketAuthenticationTerminalPolicy : PipelinePolicy
    {
        public override void Process(PipelineMessage message, IReadOnlyList<PipelinePolicy> pipeline, int currentIndex) { }
        public override ValueTask ProcessAsync(PipelineMessage message, IReadOnlyList<PipelinePolicy> pipeline, int currentIndex)
            => default;
    }
}