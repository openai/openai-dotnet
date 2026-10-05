using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace OpenAI.Telemetry;

internal sealed class OpenTelemetryResponseStream : Stream
{
    private readonly Func<IDisposable> _enter;
    private Action<Exception> _complete;
    private int _disposed;

    public OpenTelemetryResponseStream(Stream stream, Func<IDisposable> enter, Action<Exception> complete)
    {
        InnerStream = stream;
        _enter = enter;
        _complete = complete;
    }

    public Stream InnerStream { get; }
    public override bool CanRead => InnerStream.CanRead;
    public override bool CanSeek => InnerStream.CanSeek;
    public override bool CanWrite => InnerStream.CanWrite;
    public override bool CanTimeout => InnerStream.CanTimeout;
    public override long Length => InnerStream.Length;
    public override long Position { get => InnerStream.Position; set => InnerStream.Position = value; }
    public override int ReadTimeout { get => InnerStream.ReadTimeout; set => InnerStream.ReadTimeout = value; }
    public override int WriteTimeout { get => InnerStream.WriteTimeout; set => InnerStream.WriteTimeout = value; }

    public void Detach() => Interlocked.Exchange(ref _complete, null);

    private void Complete(Exception exception = null) => Interlocked.Exchange(ref _complete, null)?.Invoke(exception);

    public override int Read(byte[] buffer, int offset, int count)
    {
        using var activation = _enter();
        try
        {
            var read = InnerStream.Read(buffer, offset, count);
            if ((read == 0) && (count != 0))
            {
                Complete();
            }
            return read;
        }
        catch (Exception exception)
        {
            Complete(exception);
            throw;
        }
    }

    public override async Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
    {
        using var activation = _enter();
        try
        {
            var read = await InnerStream.ReadAsync(buffer, offset, count, cancellationToken).ConfigureAwait(false);
            if ((read == 0) && (count != 0))
            {
                Complete();
            }
            return read;
        }
        catch (Exception exception)
        {
            Complete(exception);
            throw;
        }
    }

#if !NETSTANDARD2_0
    public override int Read(Span<byte> buffer)
    {
        using var activation = _enter();
        try
        {
            var read = InnerStream.Read(buffer);
            if ((read == 0) && (!buffer.IsEmpty))
            {
                Complete();
            }
            return read;
        }
        catch (Exception exception)
        {
            Complete(exception);
            throw;
        }
    }

    public override async ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
    {
        using var activation = _enter();
        try
        {
            var read = await InnerStream.ReadAsync(buffer, cancellationToken).ConfigureAwait(false);
            if ((read == 0) && (!buffer.IsEmpty))
            {
                Complete();
            }
            return read;
        }
        catch (Exception exception)
        {
            Complete(exception);
            throw;
        }
    }

    public override async ValueTask DisposeAsync()
    {
        if (Interlocked.Exchange(ref _disposed, 1) != 0)
        {
            return;
        }
        using var activation = _enter();
        try
        {
            await InnerStream.DisposeAsync().ConfigureAwait(false);
        }
        catch (Exception exception)
        {
            Complete(exception);
            throw;
        }
        finally
        {
            Complete();
            GC.SuppressFinalize(this);
        }
    }
#endif

    protected override void Dispose(bool disposing)
    {
        if ((disposing) && (Interlocked.Exchange(ref _disposed, 1) == 0))
        {
            using var activation = _enter();
            try
            {
                InnerStream.Dispose();
            }
            catch (Exception exception)
            {
                Complete(exception);
                throw;
            }
            finally
            {
                Complete();
            }
        }
        base.Dispose(disposing);
    }

    public override void Flush() => InnerStream.Flush();
    public override Task FlushAsync(CancellationToken cancellationToken) => InnerStream.FlushAsync(cancellationToken);
    public override long Seek(long offset, SeekOrigin origin) => InnerStream.Seek(offset, origin);
    public override void SetLength(long value) => InnerStream.SetLength(value);
    public override void Write(byte[] buffer, int offset, int count) => InnerStream.Write(buffer, offset, count);
    public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        => InnerStream.WriteAsync(buffer, offset, count, cancellationToken);
}
