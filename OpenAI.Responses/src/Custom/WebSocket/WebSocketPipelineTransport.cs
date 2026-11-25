using System;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.IO;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace OpenAI.Responses;

internal sealed class WebSocketPipelineTransport : PipelineTransport
{
    protected override PipelineMessage CreateMessageCore()
        => new WebSocketPipelineMessage(HttpClientPipelineTransport.Shared.CreateMessage());

    protected override void ProcessCore(PipelineMessage message)
        => throw new NotSupportedException("The WebSocket transport supports asynchronous operations only.");

    protected override async ValueTask ProcessCoreAsync(PipelineMessage message)
    {
        if (message is not WebSocketPipelineMessage upgrade)
        {
            throw new ArgumentException("The message was not created by the WebSocket transport.", nameof(message));
        }

        upgrade.ResetAttempt();
        if (message.Request.Method != "GET" || message.Request.Content is not null)
        {
            throw new NotSupportedException("WebSocket upgrades require a GET request without content.");
        }

        Uri requestUri = message.Request.Uri
            ?? throw new InvalidOperationException("The WebSocket upgrade request URI is not set.");
        string scheme = requestUri.Scheme switch
        {
            "http" => "ws",
            "https" => "wss",
            "ws" => "ws",
            "wss" => "wss",
            _ => throw new NotSupportedException("WebSocket upgrade requests require an HTTP or WebSocket URI."),
        };
        Uri endpoint = new UriBuilder(requestUri) { Scheme = scheme }.Uri;
        ClientWebSocket socket = new ClientWebSocket();
        try
        {
#if NET8_0_OR_GREATER
            socket.Options.CollectHttpResponseDetails = true;
#endif
            foreach (KeyValuePair<string, string> header in message.Request.Headers)
            {
                socket.Options.SetRequestHeader(header.Key, header.Value);
            }

            await socket.ConnectAsync(endpoint, message.CancellationToken).ConfigureAwait(false);
#if NET8_0_OR_GREATER
            int status = (int)socket.HttpStatusCode;
            if (status == 0)
            {
                throw new InvalidOperationException("The WebSocket upgrade did not expose its response status.");
            }
            upgrade.SetSuccess(socket, new WebSocketUpgradeResponse(status, socket.HttpResponseHeaders));
#else
            // A successful HTTP/1.1 WebSocket upgrade always returns 101.
            upgrade.SetSuccess(socket, new WebSocketUpgradeResponse(101, null));
#endif
            socket = null;
        }
#if NET8_0_OR_GREATER
        catch (WebSocketException exception) when (socket is not null && (int)socket.HttpStatusCode != 0)
        {
            upgrade.SetFailure(exception, new WebSocketUpgradeResponse((int)socket.HttpStatusCode, socket.HttpResponseHeaders));
        }
#endif
        finally
        {
            socket?.Dispose();
        }
    }
}

internal sealed class WebSocketPipelineMessage : PipelineMessage
{
    private readonly PipelineMessage _requestOwner;
    private ClientWebSocket _socket;

    internal WebSocketPipelineMessage(PipelineMessage requestOwner) : base(requestOwner.Request)
        => _requestOwner = requestOwner;

    internal WebSocketException HandshakeException { get; private set; }

    internal void ResetAttempt()
    {
        Response?.Dispose();
        Response = null;
        _socket?.Dispose();
        _socket = null;
        HandshakeException = null;
    }

    internal void SetSuccess(ClientWebSocket socket, PipelineResponse response)
    {
        _socket = socket;
        Response = response;
    }

    internal void SetFailure(WebSocketException exception, PipelineResponse response)
    {
        HandshakeException = exception;
        Response = response;
    }

    internal ClientWebSocket TakeSocket()
    {
        ClientWebSocket socket = _socket;
        _socket = null;
        return socket;
    }

    protected override void Dispose(bool disposing)
    {
        try
        {
            if (disposing)
            {
                _socket?.Dispose();
                _socket = null;
            }
        }
        finally
        {
            try
            {
                base.Dispose(disposing);
            }
            finally
            {
                if (disposing)
                {
                    _requestOwner.Dispose();
                }
            }
        }
    }
}

internal sealed class WebSocketUpgradeResponse : PipelineResponse
{
    private readonly int _status;
    private readonly PipelineResponseHeaders _headers;

    internal WebSocketUpgradeResponse(int status, IReadOnlyDictionary<string, IEnumerable<string>> headers)
    {
        _status = status;
        _headers = new WebSocketUpgradeResponseHeaders(headers);
    }

    public override int Status => _status;
    public override string ReasonPhrase => string.Empty;
    protected override PipelineResponseHeaders HeadersCore => _headers;

    public override Stream ContentStream
    {
        get => null;
        set => throw new NotSupportedException("WebSocket upgrade response bodies are not available.");
    }

    public override BinaryData Content => _status == 101
        ? BinaryData.Empty
        : throw new NotSupportedException("WebSocket upgrade response bodies are not available.");

    public override BinaryData BufferContent(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Content;
    }

    public override ValueTask<BinaryData> BufferContentAsync(CancellationToken cancellationToken = default)
        => new ValueTask<BinaryData>(BufferContent(cancellationToken));

    public override void Dispose()
    {
    }

    private sealed class WebSocketUpgradeResponseHeaders : PipelineResponseHeaders
    {
        private readonly Dictionary<string, IReadOnlyList<string>> _headers
            = new Dictionary<string, IReadOnlyList<string>>(StringComparer.OrdinalIgnoreCase);

        internal WebSocketUpgradeResponseHeaders(IReadOnlyDictionary<string, IEnumerable<string>> headers)
        {
            if (headers is not null)
            {
                foreach (KeyValuePair<string, IEnumerable<string>> header in headers)
                {
                    _headers[header.Key] = new List<string>(header.Value);
                }
            }
        }

        public override bool TryGetValue(string name, out string value)
        {
            if (_headers.TryGetValue(name, out IReadOnlyList<string> values))
            {
                value = string.Join(",", values);
                return true;
            }

            value = null;
            return false;
        }

        public override bool TryGetValues(string name, out IEnumerable<string> values)
        {
            if (_headers.TryGetValue(name, out IReadOnlyList<string> stored))
            {
                values = stored;
                return true;
            }

            values = null;
            return false;
        }

        public override IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        {
            foreach (KeyValuePair<string, IReadOnlyList<string>> header in _headers)
            {
                yield return new KeyValuePair<string, string>(header.Key, string.Join(",", header.Value));
            }
        }
    }
}
