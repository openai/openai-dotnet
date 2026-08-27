using System;
using System.ClientModel.Primitives;
using System.IO;
using System.Net;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace OpenAI.Realtime;

/// <summary>
/// Represents a <see cref="PipelineResponse"/> encapsulating the data from a WebSocket response message.
/// </summary>
/// <remarks>
/// WebSocket messages can be split across multiple network receive operations and a single
/// <see cref="WebsocketPipelineResponse"/> may thus ingest and present data across several such operations.
/// </remarks>
internal class WebsocketPipelineResponse : PipelineResponse
{
    public override int Status => _status;
    private int _status;

    public override string ReasonPhrase => _reasonPhrase;
    private string _reasonPhrase;

    public override Stream ContentStream
    {
        get => _contentStream ??= new MemoryStream();
        set => throw new NotImplementedException();
    }
    private MemoryStream _contentStream;

    public override BinaryData Content => _content ??= new(_contentStream.ToArray());
    private BinaryData _content;

    public bool IsComplete { get; private set; } = false;

    protected override PipelineResponseHeaders HeadersCore => throw new NotImplementedException();

    public WebsocketPipelineResponse()
    {
    }

    public void IngestReceivedResult(WebSocketReceiveResult receivedResult, BinaryData receivedBytes)
    {
        if (ContentStream.Length == 0)
        {
            _status = ConvertWebsocketCloseStatusToHttpStatus(receivedResult.CloseStatus ?? WebSocketCloseStatus.Empty);
            _reasonPhrase = receivedResult.CloseStatusDescription
                ?? (receivedResult.CloseStatus ?? WebSocketCloseStatus.Empty).ToString();
        }
        else if (receivedResult.MessageType != WebSocketMessageType.Text)
        {
            throw new NotImplementedException($"{nameof(WebsocketPipelineResponse)} currently supports only text messages.");
        }
        byte[] rawReceivedBytes = receivedBytes.ToArray();
        _contentStream.Position = _contentStream.Length;
        _contentStream.Write(rawReceivedBytes, 0, rawReceivedBytes.Length);
        _contentStream.Position = 0;
        IsComplete = receivedResult.EndOfMessage;
    }

    public override BinaryData BufferContent(CancellationToken cancellationToken = default)
        => Content;

    public override ValueTask<BinaryData> BufferContentAsync(CancellationToken cancellationToken = default)
        => new ValueTask<BinaryData>(Task.FromResult(Content));

    public override void Dispose()
    {
        ContentStream?.Dispose();
    }

    private static int ConvertWebsocketCloseStatusToHttpStatus(WebSocketCloseStatus closeStatus)
    {
        return closeStatus switch
        {
            WebSocketCloseStatus.Empty
            or WebSocketCloseStatus.NormalClosure => (int)HttpStatusCode.OK,
            WebSocketCloseStatus.EndpointUnavailable
            or WebSocketCloseStatus.ProtocolError
            or WebSocketCloseStatus.InvalidMessageType
            or WebSocketCloseStatus.InvalidPayloadData
            or WebSocketCloseStatus.PolicyViolation => (int)HttpStatusCode.BadRequest,
            WebSocketCloseStatus.MessageTooBig => (int)HttpStatusCode.RequestEntityTooLarge,
            WebSocketCloseStatus.MandatoryExtension => 418, // https://developer.mozilla.org/en-US/docs/Web/HTTP/Status/418
            WebSocketCloseStatus.InternalServerError => (int)HttpStatusCode.InternalServerError,
            _ => (int)HttpStatusCode.InternalServerError,
        };
    }
}