using System;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.IO;
using System.Net.ServerSentEvents;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

#nullable enable

namespace OpenAI;

/// <summary>
/// Creates <see cref="AsyncStreamingResult{T}"/> instances that read typed updates from server-sent events.
/// </summary>
internal static class SseStreamingResult
{
    private static ReadOnlySpan<byte> TerminalData => "[DONE]"u8;

    public static Task<AsyncStreamingResult<T>> CreateAsync<T>(
        Func<Task<ClientResult>> sendRequestAsync,
        Func<JsonElement, ModelReaderWriterOptions, T> jsonSingleDeserializerFunc,
        CancellationToken cancellationToken,
        IDisposable? requestResource = null)
            => CreateAsync(
                sendRequestAsync,
                AsyncSseUpdateCollection<T>.DeserializeSseToSingleViaJson(jsonSingleDeserializerFunc),
                cancellationToken,
                requestResource: requestResource);

    public static Task<AsyncStreamingResult<T>> CreateAsync<T>(
        Func<Task<ClientResult>> sendRequestAsync,
        Func<JsonElement, BinaryData, ModelReaderWriterOptions, T> jsonSingleDeserializerFunc,
        CancellationToken cancellationToken,
        Func<SseLifecycle<T>?>? lifecycleFactory = null,
        Func<PipelineResponse, Func<Stream, CancellationToken, IAsyncEnumerable<T>>, CancellationToken, AsyncStreamingResult<T>>? resultFactory = null)
            => CreateAsync(
                sendRequestAsync,
                AsyncSseUpdateCollection<T>.DeserializeSseToSingleViaJson(jsonSingleDeserializerFunc),
                cancellationToken,
                lifecycleFactory,
                resultFactory);

    /// <summary>
    /// Sends the streaming request and creates a one-shot result that owns the established response.
    /// </summary>
    /// <param name="requestResource">A resource used by the request content that is released once the request is sent.</param>
    /// <param name="resultFactory">Creates the result from the response and the update producer; defaults to <see cref="AsyncStreamingResult.Create{T}"/>.</param>
    public static async Task<AsyncStreamingResult<T>> CreateAsync<T>(
        Func<Task<ClientResult>> sendRequestAsync,
        Func<SseItem<byte[]>, IEnumerable<T>> eventDeserializerFunc,
        CancellationToken cancellationToken,
        Func<SseLifecycle<T>?>? lifecycleFactory = null,
        Func<PipelineResponse, Func<Stream, CancellationToken, IAsyncEnumerable<T>>, CancellationToken, AsyncStreamingResult<T>>? resultFactory = null,
        IDisposable? requestResource = null)
    {
        Argument.AssertNotNull(sendRequestAsync, nameof(sendRequestAsync));
        Argument.AssertNotNull(eventDeserializerFunc, nameof(eventDeserializerFunc));

        SseLifecycle<T>? lifecycle = null;
        PipelineResponse response;

        try
        {
            lifecycle = lifecycleFactory?.Invoke();

            using (lifecycle?.Enter())
            {
                try
                {
                    response = (await sendRequestAsync().ConfigureAwait(false)).GetRawResponse();
                }
                catch (Exception exception)
                {
                    lifecycle?.OnException(exception);

                    throw;
                }
            }
        }
        catch
        {
            requestResource?.Dispose();

            throw;
        }

        try
        {
            requestResource?.Dispose();

            EnumerationGate<T> gate = new(lifecycle);

            if (lifecycle is not null)
            {
                lifecycle.OnTypedResponse();

                if (response.ContentStream is { } contentStream)
                {
                    // A result that is disposed without being enumerated never runs the parser,
                    // so closing the content stream is the only signal that ends the lifecycle.
                    response.ContentStream = new DisposalObservingStream(contentStream, gate.OnContentStreamDisposed);
                }
            }

            Func<Stream, CancellationToken, IAsyncEnumerable<T>> producer =
                (stream, producerCancellationToken) => ReadUpdatesAsync(stream, eventDeserializerFunc, gate, producerCancellationToken);

            return resultFactory is null
                ? AsyncStreamingResult.Create(response, producer, cancellationToken)
                : resultFactory(response, producer, cancellationToken);
        }
        catch (Exception exception)
        {
            try
            {
                lifecycle?.OnException(exception);
            }
            finally
            {
                response.Dispose();
            }

            throw;
        }
    }

    /// <summary>
    /// Reads the typed updates of the server-sent events in <paramref name="stream"/> until the terminal event or the
    /// end of the stream. The result that owns the stream enforces single enumeration and disposes the response.
    /// </summary>
    private static async IAsyncEnumerable<T> ReadUpdatesAsync<T>(
        Stream stream,
        Func<SseItem<byte[]>, IEnumerable<T>> eventDeserializerFunc,
        EnumerationGate<T> gate,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        SseLifecycle<T>? lifecycle = gate.BeginEnumeration();
        IAsyncEnumerator<SseItem<byte[]>>? events = null;
        IEnumerator<T>? updates = null;

        try
        {
            while (true)
            {
                bool hasNext = false;
                T current = default!;

                using (lifecycle?.Enter())
                {
                    try
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        events ??= SseParser.Create(stream, (_, bytes) => bytes.ToArray()).EnumerateAsync().GetAsyncEnumerator(cancellationToken);

                        if ((updates is not null) && (updates.MoveNext()))
                        {
                            current = updates.Current;
                            hasNext = true;
                        }

                        // An event can deserialize to no updates, for example an unmodeled event, so the stream
                        // continues until an update, the terminal event, or the end of the stream.
                        while (!hasNext)
                        {
                            // Buffered events are returned without a read, so cancellation is checked for every event.
                            cancellationToken.ThrowIfCancellationRequested();

                            if (!await events.MoveNextAsync().ConfigureAwait(false))
                            {
                                break;
                            }

                            lifecycle?.OnEvent();

                            if (events.Current.Data.AsSpan().SequenceEqual(TerminalData))
                            {
                                break;
                            }

                            updates = eventDeserializerFunc(events.Current).GetEnumerator();

                            if (updates.MoveNext())
                            {
                                current = updates.Current;
                                hasNext = true;
                            }
                        }

                        if (hasNext)
                        {
                            lifecycle?.OnUpdate(current);
                        }
                        else
                        {
                            lifecycle?.Complete(SseCompletionKind.EndOfStream);
                        }
                    }
                    catch (Exception exception)
                    {
                        lifecycle?.OnException(exception);

                        throw;
                    }
                }

                if (!hasNext)
                {
                    yield break;
                }

                yield return current;
            }
        }
        finally
        {
            using (lifecycle?.Enter())
            {
                try
                {
                    updates?.Dispose();

                    if (events is not null)
                    {
                        await events.DisposeAsync().ConfigureAwait(false);
                    }
                }
                catch (Exception exception)
                {
                    lifecycle?.OnException(exception);

                    throw;
                }
                finally
                {
                    lifecycle?.Complete(SseCompletionKind.Disposed);
                }
            }
        }
    }

    /// <summary>
    /// Hands the lifecycle to the first enumeration, or completes it when the result is disposed before
    /// being enumerated, so that exactly one of them observes it.
    /// </summary>
    private sealed class EnumerationGate<T>
    {
        private const int NotStarted = 0;
        private const int Enumerating = 1;
        private const int Abandoned = 2;

        private readonly SseLifecycle<T>? _lifecycle;
        private int _state;

        public EnumerationGate(SseLifecycle<T>? lifecycle)
        {
            _lifecycle = lifecycle;
        }

        public SseLifecycle<T>? BeginEnumeration()
            => Interlocked.CompareExchange(ref _state, Enumerating, NotStarted) == NotStarted ? _lifecycle : null;

        public void OnContentStreamDisposed()
        {
            if ((_lifecycle is not null) && (Interlocked.CompareExchange(ref _state, Abandoned, NotStarted) == NotStarted))
            {
                using (_lifecycle.Enter())
                {
                    _lifecycle.Complete(SseCompletionKind.Disposed);
                }
            }
        }
    }

    private sealed class DisposalObservingStream : Stream
    {
        private readonly Stream _inner;
        private Action? _onDisposed;

        public DisposalObservingStream(Stream inner, Action onDisposed)
        {
            _inner = inner;
            _onDisposed = onDisposed;
        }

        public override bool CanRead => _inner.CanRead;

        public override bool CanSeek => _inner.CanSeek;

        public override bool CanWrite => _inner.CanWrite;

        public override bool CanTimeout => _inner.CanTimeout;

        public override long Length => _inner.Length;

        public override long Position
        {
            get => _inner.Position;
            set => _inner.Position = value;
        }

        public override int ReadTimeout
        {
            get => _inner.ReadTimeout;
            set => _inner.ReadTimeout = value;
        }

        public override int WriteTimeout
        {
            get => _inner.WriteTimeout;
            set => _inner.WriteTimeout = value;
        }

        public override int Read(byte[] buffer, int offset, int count) => _inner.Read(buffer, offset, count);

        public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
            => _inner.ReadAsync(buffer, offset, count, cancellationToken);

#if !NETSTANDARD2_0
        public override int Read(Span<byte> buffer) => _inner.Read(buffer);

        public override ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
            => _inner.ReadAsync(buffer, cancellationToken);

        public override async ValueTask DisposeAsync()
        {
            try
            {
                await _inner.DisposeAsync().ConfigureAwait(false);
            }
            finally
            {
                Interlocked.Exchange(ref _onDisposed, null)?.Invoke();
                GC.SuppressFinalize(this);
            }
        }
#endif

        public override void Flush() => _inner.Flush();

        public override Task FlushAsync(CancellationToken cancellationToken) => _inner.FlushAsync(cancellationToken);

        public override long Seek(long offset, SeekOrigin origin) => _inner.Seek(offset, origin);

        public override void SetLength(long value) => _inner.SetLength(value);

        public override void Write(byte[] buffer, int offset, int count) => _inner.Write(buffer, offset, count);

        protected override void Dispose(bool disposing)
        {
            try
            {
                if (disposing)
                {
                    _inner.Dispose();
                }
            }
            finally
            {
                if (disposing)
                {
                    Interlocked.Exchange(ref _onDisposed, null)?.Invoke();
                }

                base.Dispose(disposing);
            }
        }
    }
}
