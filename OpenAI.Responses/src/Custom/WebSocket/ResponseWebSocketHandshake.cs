using System;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.IO;
using System.Net.WebSockets;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace OpenAI.Responses;

internal sealed class ResponseWebSocketHandshake
{
    private readonly AuthenticationPolicy _authentication;
    private readonly ClientPipelineOptions _options;
    private readonly string _applicationId;
    private readonly string _organization;
    private readonly string _project;
    private readonly bool _customTransport;
#if NETSTANDARD2_0
    private static readonly System.Reflection.MethodInfo s_connectWithInvoker = typeof(ClientWebSocket).GetMethod(
        "ConnectAsync", new[] { typeof(Uri), typeof(HttpMessageInvoker), typeof(CancellationToken) });
#endif

    internal ResponseWebSocketHandshake(AuthenticationPolicy authentication, ClientPipelineOptions options,
        string applicationId, string organization, string project)
    {
        _authentication = authentication;
        _options = options.Clone();
        _customTransport = options.Transport != null && !ReferenceEquals(options.Transport, HttpClientPipelineTransport.Shared);
        _applicationId = applicationId;
        _organization = organization;
        _project = project;
    }

    internal async Task<WebSocket> ConnectAsync(Uri endpoint, ResponseWebSocketOptions options, CancellationToken cancellationToken)
    {
#if NETSTANDARD2_0
        if (options.Connector == null && s_connectWithInvoker == null)
            throw new PlatformNotSupportedException("The default Responses WebSocket transport requires a runtime with ClientWebSocket.ConnectAsync(Uri, HttpMessageInvoker, CancellationToken). Provide ResponseWebSocketOptions.Connector on older runtimes.");
#endif
        if (_customTransport && options.Connector == null)
            throw new NotSupportedException("A custom HTTP transport requires ResponseWebSocketOptions.Connector so its proxy, TLS, and other transport settings are preserved.");
        if (_options.RetryPolicy != null && _options.RetryPolicy.GetType() != typeof(ClientRetryPolicy))
            throw new NotSupportedException("A custom retry policy cannot be used for a WebSocket upgrade because it may replay the handshake. Add request header or authentication policies using AddPolicy instead.");
        if (endpoint.Scheme != "https" && endpoint.Scheme != "http")
            throw new ArgumentException("The client endpoint must use HTTP or HTTPS.", nameof(endpoint));
        var address = new UriBuilder(endpoint) { Path = endpoint.AbsolutePath.TrimEnd('/') + "/responses" };
        var transport = new UpgradeTransport(options, address.Uri);
        var pipelineOptions = _options.Clone();
        pipelineOptions.Transport = transport;
        // A failed upgrade is never replayed by HTTP retry policy.
        pipelineOptions.RetryPolicy = new ClientRetryPolicy(0);
        var pipeline = OpenAIClientUtilities.CreatePipeline(_authentication, pipelineOptions, _applicationId, _organization, _project);
        using var message = pipeline.CreateMessage();
        message.Request.Method = "GET";
        message.Request.Uri = address.Uri;
        message.Apply(new RequestOptions { CancellationToken = cancellationToken });
        foreach (var header in options.Headers) message.Request.Headers.Set(header.Key, header.Value);
        try
        {
            await pipeline.SendAsync(message).ConfigureAwait(false);
            return transport.Socket ?? throw new InvalidOperationException("A client policy prevented the WebSocket upgrade.");
        }
        catch
        {
            try { transport.Socket?.Dispose(); }
            catch (Exception) { } // Preserve the handshake or policy failure.
            throw;
        }
    }

    private sealed class UpgradeTransport : PipelineTransport
    {
        private readonly ResponseWebSocketOptions _options;
        private readonly Uri _expectedUri;
        internal WebSocket Socket { get; private set; }
        internal UpgradeTransport(ResponseWebSocketOptions options, Uri expectedUri) { _options = options; _expectedUri = expectedUri; }
        protected override PipelineMessage CreateMessageCore() => new UpgradeMessage();
        protected override void ProcessCore(PipelineMessage message) => ProcessCoreAsync(message).GetAwaiter().GetResult();
        protected override async ValueTask ProcessCoreAsync(PipelineMessage message)
        {
            if (Socket != null) throw new InvalidOperationException("A WebSocket upgrade cannot be retried.");
            var uri = message.Request.Uri;
            if (uri.Scheme != _expectedUri.Scheme || uri.Host != _expectedUri.Host || uri.Port != _expectedUri.Port)
                throw new InvalidOperationException("Client policies cannot redirect a WebSocket upgrade to another origin.");
            var address = new UriBuilder(uri) { Scheme = uri.Scheme == "https" ? "wss" : "ws" };
            var headers = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            foreach (var header in message.Request.Headers) headers.Add(header.Key, header.Value);
            if (_options.Connector != null)
            {
                Socket = await _options.Connector(address.Uri, headers, message.CancellationToken).ConfigureAwait(false);
                if (Socket == null || Socket.State != WebSocketState.Open)
                    throw new InvalidOperationException("The WebSocket connector must return an open socket.");
            }
            else
            {
                var socket = new ClientWebSocket();
                Socket = socket;
                using var handler = new HttpClientHandler();
                _options.ConfigureTransport?.Invoke(handler);
                handler.AllowAutoRedirect = false;
                using var invoker = new HttpMessageInvoker(handler, disposeHandler: false);
                foreach (var header in headers) socket.Options.SetRequestHeader(header.Key, header.Value);
#if NETSTANDARD2_0
                var connect = (Func<Uri, HttpMessageInvoker, CancellationToken, Task>)s_connectWithInvoker.CreateDelegate(
                    typeof(Func<Uri, HttpMessageInvoker, CancellationToken, Task>), socket);
                await connect(address.Uri, invoker, message.CancellationToken).ConfigureAwait(false);
#else
                await socket.ConnectAsync(address.Uri, invoker, message.CancellationToken).ConfigureAwait(false);
#endif
            }
            ((UpgradeMessage)message).SetResponse(new UpgradeResponse());
        }
    }

    private sealed class UpgradeMessage : PipelineMessage
    {
        internal UpgradeMessage() : base(HttpClientPipelineTransport.Shared.CreateMessage().Request) { }
        internal void SetResponse(PipelineResponse response) => Response = response;
    }

    private sealed class UpgradeResponse : PipelineResponse
    {
        public override int Status => 101;
        public override string ReasonPhrase => "Switching Protocols";
        protected override PipelineResponseHeaders HeadersCore { get; } = new EmptyHeaders();
        public override Stream ContentStream { get; set; } = Stream.Null;
        public override BinaryData Content => BinaryData.FromBytes(Array.Empty<byte>());
        protected override bool IsErrorCore { get; set; }
        public override BinaryData BufferContent(CancellationToken cancellationToken = default) => Content;
        public override ValueTask<BinaryData> BufferContentAsync(CancellationToken cancellationToken = default) => new(Content);
        public override void Dispose() { }
    }

    private sealed class EmptyHeaders : PipelineResponseHeaders
    {
        public override bool TryGetValue(string name, out string value) { value = null; return false; }
        public override bool TryGetValues(string name, out IEnumerable<string> values) { values = null; return false; }
        public override IEnumerator<KeyValuePair<string, string>> GetEnumerator() => ((IEnumerable<KeyValuePair<string, string>>)Array.Empty<KeyValuePair<string, string>>()).GetEnumerator();
    }
}
