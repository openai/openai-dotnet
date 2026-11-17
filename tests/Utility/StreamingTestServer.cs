using Microsoft.ClientModel.TestFramework.Mocks;
using System;
using System.ClientModel.Primitives;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace OpenAI.Tests.Utility;

/// <summary>
/// An asynchronous mock transport that serves a fixed server-sent event payload and records each
/// request and the response stream it handed out.
/// </summary>
internal sealed class StreamingTestServer
{
    public const string RequestId = "req_streaming_test";

    private readonly string _content;
    private readonly int _status;
    private int _requestCount;

    public StreamingTestServer(string content, int status = 200)
    {
        _content = content;
        _status = status;
        Transport = new MockPipelineTransport(CreateResponse)
        {
            ExpectSyncPipeline = false,
        };
    }

    public MockPipelineTransport Transport { get; }

    public int RequestCount => Volatile.Read(ref _requestCount);

    public bool? LastBufferResponse { get; private set; }

    public Uri LastRequestUri { get; private set; }

    public string LastRequestText { get; private set; }

    public TrackingStream LastResponseStream { get; private set; }

    public JsonElement LastRequestJson => JsonDocument.Parse(LastRequestText).RootElement;

    private MockPipelineResponse CreateResponse(MockPipelineMessage message)
    {
        Interlocked.Increment(ref _requestCount);
        LastBufferResponse = message.BufferResponse;
        LastRequestUri = message.Request.Uri;

        if (message.Request.Content is not null)
        {
            using MemoryStream requestContent = new();
            message.Request.Content.WriteTo(requestContent);
            LastRequestText = Encoding.UTF8.GetString(requestContent.ToArray());
        }

        LastResponseStream = new TrackingStream(Encoding.UTF8.GetBytes(_content));
        MockPipelineResponse response = new MockPipelineResponse(_status, _status == 200 ? "OK" : "Error")
            .WithHeader("x-request-id", RequestId)
            .WithHeader("Content-Type", _status == 200 ? "text/event-stream" : "application/json");
        response.ContentStream = LastResponseStream;

        return response;
    }
}

/// <summary>
/// A read-only response stream that records how much was read and whether it was disposed.
/// </summary>
/// <remarks>
/// This deliberately does not derive from <see cref="MemoryStream"/>, because the mock response
/// buffers <see cref="MemoryStream"/> content when it is disposed, which would hide unread data.
/// </remarks>
internal sealed class TrackingStream : Stream
{
    private readonly MemoryStream _inner;

    public TrackingStream(byte[] content)
    {
        _inner = new MemoryStream(content, writable: false);
    }

    public long BytesRead { get; private set; }

    public bool IsDisposed { get; private set; }

    public override bool CanRead => !IsDisposed;

    public override bool CanSeek => false;

    public override bool CanWrite => false;

    public override long Length => throw new NotSupportedException();

    public override long Position
    {
        get => throw new NotSupportedException();
        set => throw new NotSupportedException();
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
        ObjectDisposedException.ThrowIf(IsDisposed, this);
        int read = _inner.Read(buffer, offset, count);
        BytesRead += read;
        return read;
    }

    public override async Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        => await ReadAsync(buffer.AsMemory(offset, count), cancellationToken).ConfigureAwait(false);

    public override async ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(IsDisposed, this);
        await Task.Yield();
        cancellationToken.ThrowIfCancellationRequested();
        int read = _inner.Read(buffer.Span);
        BytesRead += read;
        return read;
    }

    public override void Flush()
    {
    }

    public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();

    public override void SetLength(long value) => throw new NotSupportedException();

    public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();

    protected override void Dispose(bool disposing)
    {
        IsDisposed = true;
        base.Dispose(disposing);
    }
}
