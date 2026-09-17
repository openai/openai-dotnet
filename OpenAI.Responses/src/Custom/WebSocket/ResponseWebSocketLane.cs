using System;
using System.Diagnostics.CodeAnalysis;
using System.ClientModel.Primitives;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace OpenAI.Responses;

/// <summary>A single stream identifier routed by the connection's shared reader.</summary>
[Experimental("OPENAI001")]
public sealed class ResponseWebSocketLane : IDisposable
{
    private readonly ResponseWebSocketConnection _connection;
    private readonly ResponseWebSocketQueue _events;
    private int _disposed;
    internal ResponseWebSocketLane(ResponseWebSocketConnection connection, string streamId, ResponseWebSocketQueue events)
    { _connection = connection; StreamId = streamId; _events = events; }
    public string StreamId { get; }
    /// <summary>Receives the next event for this lane. Cancellation affects only this wait.</summary>
    public Task<ResponseWebSocketServerEvent> ReceiveAsync(CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();
        return _events.ReceiveAsync(cancellationToken);
    }
    /// <summary>Sends a command on this lane without modifying the caller's command.</summary>
    public Task SendAsync(ResponseWebSocketCommand command, CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();
        if (command == null) throw new ArgumentNullException(nameof(command));
        return _connection.SendCoreAsync(() => Prepare(command), cancellationToken);
    }

    private BinaryData Prepare(ResponseWebSocketCommand command)
    {
        var data = ModelReaderWriter.Write(command, ModelReaderWriterOptions.Json, OpenAIContext.Default);
        using var document = JsonDocument.Parse(data, ModelSerializationExtensions.JsonDocumentOptions);
        if (!document.RootElement.TryGetProperty("type", out var type) || type.GetString() != "response.create")
            return data;
        if (document.RootElement.TryGetProperty("stream_id", out var existing)
            && existing.ValueKind != JsonValueKind.Null && existing.GetString() != StreamId)
            throw new ArgumentException("The command belongs to a different lane.", nameof(command));
        using var buffer = new MemoryStream();
        using (var writer = new Utf8JsonWriter(buffer))
        {
            writer.WriteStartObject();
            foreach (var property in document.RootElement.EnumerateObject())
                if (property.Name != "stream_id") property.WriteTo(writer);
            writer.WriteString("stream_id", StreamId);
            writer.WriteEndObject();
        }
        return BinaryData.FromBytes(buffer.ToArray());
    }

    /// <summary>Reads to a terminal event and returns its complete response, including all output and tools.</summary>
    /// <remarks>Failed and incomplete responses are returned with their status intact. Protocol errors throw.</remarks>
    public Task<ResponseResult> ReceiveResponseAsync(CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();
        return ReadResponseAsync(_events, cancellationToken, defaultLane: false);
    }

    internal static async Task<ResponseResult> ReadResponseAsync(
        ResponseWebSocketQueue events, CancellationToken cancellationToken, bool defaultLane)
    {
        events.EnterRead();
        try
        {
            while (true)
            {
                var item = await events.ReceiveCoreAsync(cancellationToken).ConfigureAwait(false);
                if (defaultLane && !string.IsNullOrEmpty(item.StreamId)) continue;
                if (item is ResponseWebSocketErrorEvent error) throw new ResponseWebSocketException(error);
                switch (item.Update)
                {
                    case StreamingResponseCompletedUpdate completed: return completed.Response;
                    case StreamingResponseFailedUpdate failed: return failed.Response;
                    case StreamingResponseIncompleteUpdate incomplete: return incomplete.Response;
                }
            }
        }
        finally { events.ExitRead(); }
    }
    /// <summary>Detaches this helper without closing the socket. Subsequent events are delivered by the connection.</summary>
    public void Dispose()
    {
        if (Interlocked.Exchange(ref _disposed, 1) == 0) _connection.Detach(StreamId, _events);
    }

    private void ThrowIfDisposed()
    {
        if (Volatile.Read(ref _disposed) != 0) throw new ObjectDisposedException(nameof(ResponseWebSocketLane));
    }
}

/// <summary>A protocol error received while waiting for a final response.</summary>
[Experimental("OPENAI001")]
public sealed class ResponseWebSocketException : Exception
{
    public ResponseWebSocketErrorEvent Error { get; }
    internal ResponseWebSocketException(ResponseWebSocketErrorEvent error)
        : base(error.Error?.Message ?? "The server returned a WebSocket protocol error without a message.") => Error = error;
}
