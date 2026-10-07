using Microsoft.ClientModel.TestFramework.Mocks;
using NUnit.Framework;
using System;
using System.ClientModel;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.ServerSentEvents;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace OpenAI.Tests.Utility;

/// <summary>
/// Tests for the shared SSE collection types that back every streaming API.
/// </summary>
/// <remarks>
/// These exercise the multi-value deserializer shape directly. The single-value
/// overloads cannot reach the behavior under test, because
/// <c>DeserializeSseToSingleViaJson</c> wraps its result in a one-element
/// collection and therefore always produces exactly one update per event.
/// </remarks>
[Parallelizable(ParallelScope.All)]
[Category("Smoke")]
public class SseUpdateCollectionTests
{
    private const string UnknownEventType = "unknown";

    // A modeled event that yields one update, then an event the deserializer does not
    // recognize and maps to an empty sequence, then a second modeled event, then the
    // terminal event. Each event needs its own trailing blank line, otherwise the
    // parser never dispatches it.
    private const string StreamContent =
        "event: modeled\ndata: {\"value\":\"A\"}\n\n" +
        "event: " + UnknownEventType + "\ndata: {\"value\":\"skipped\"}\n\n" +
        "event: modeled\ndata: {\"value\":\"B\"}\n\n" +
        "data: [DONE]\n\n";

    [Test]
    public void EventWithNoUpdatesDoesNotEndTheStream()
    {
        SseUpdateCollection<string> collection = new(
            CreatePage,
            DeserializeEvent,
            CancellationToken.None);

        Assert.That(collection.ToList(), Is.EqualTo(new[] { "A", "B" }));
    }

    [Test]
    public async Task EventWithNoUpdatesDoesNotEndTheStreamAsync()
    {
        AsyncSseUpdateCollection<string> collection = new(
            () => Task.FromResult(CreatePage()),
            DeserializeEvent,
            CancellationToken.None);

        List<string> updates = [];

        await foreach (string update in collection)
        {
            updates.Add(update);
        }

        Assert.That(updates, Is.EqualTo(new[] { "A", "B" }));
    }

    // A run of events that yield no updates, so that the skip loop stays inside a single
    // MoveNext call across several events.
    private const string StreamContentWithConsecutiveUnknownEvents =
        "event: modeled\ndata: {\"value\":\"A\"}\n\n" +
        "event: " + UnknownEventType + "\ndata: {\"value\":\"skipped\"}\n\n" +
        "event: " + UnknownEventType + "\ndata: {\"value\":\"skipped\"}\n\n" +
        "event: modeled\ndata: {\"value\":\"B\"}\n\n" +
        "data: [DONE]\n\n";

    [Test]
    public void SkippingEventsWithNoUpdatesObservesCancellation()
    {
        using CancellationTokenSource source = new();

        SseUpdateCollection<string> collection = new(
            () => CreatePage(StreamContentWithConsecutiveUnknownEvents),
            item => DeserializeEventAndCancelOnUnknown(item, source),
            source.Token);

        using IEnumerator<string> enumerator = collection.GetEnumerator();

        Assert.That(enumerator.MoveNext(), Is.True);
        Assert.That(enumerator.Current, Is.EqualTo("A"));

        // The token is canceled while the second call is already inside the skip loop.
        Assert.Throws<OperationCanceledException>(() => enumerator.MoveNext());
    }

    [Test]
    public void SkippingEventsWithNoUpdatesObservesCancellationAsync()
    {
        using CancellationTokenSource source = new();

        AsyncSseUpdateCollection<string> collection = new(
            () => Task.FromResult(CreatePage(StreamContentWithConsecutiveUnknownEvents)),
            item => DeserializeEventAndCancelOnUnknown(item, source),
            source.Token);

        Assert.That(
            async () =>
            {
                await foreach (string update in collection)
                {
                    Assert.That(update, Is.EqualTo("A"));
                }
            },
            Throws.TypeOf<OperationCanceledException>());
    }

    // The terminal event is the next frame after the one that cancels, so the parser has it
    // buffered and can return it without another read.
    private const string StreamContentUnknownThenTerminal =
        "event: modeled\ndata: {\"value\":\"A\"}\n\n" +
        "event: " + UnknownEventType + "\ndata: {\"value\":\"skipped\"}\n\n" +
        "data: [DONE]\n\n";

    [Test]
    public void CancellationIsNotOvertakenByABufferedTerminalEvent()
    {
        using CancellationTokenSource source = new();

        SseUpdateCollection<string> collection = new(
            () => CreatePage(StreamContentUnknownThenTerminal),
            item => DeserializeEventAndCancelOnUnknown(item, source),
            source.Token);

        using IEnumerator<string> enumerator = collection.GetEnumerator();

        Assert.That(enumerator.MoveNext(), Is.True);
        Assert.Throws<OperationCanceledException>(() => enumerator.MoveNext());
    }

    [Test]
    public void CancellationIsNotOvertakenByABufferedTerminalEventAsync()
    {
        using CancellationTokenSource source = new();

        AsyncSseUpdateCollection<string> collection = new(
            () => Task.FromResult(CreatePage(StreamContentUnknownThenTerminal)),
            item => DeserializeEventAndCancelOnUnknown(item, source),
            source.Token);

        // Without the check ahead of the read, the buffered terminal event is reached first
        // and enumeration ends normally having produced only "A".
        Assert.That(
            async () =>
            {
                await foreach (string update in collection)
                {
                    Assert.That(update, Is.EqualTo("A"));
                }
            },
            Throws.TypeOf<OperationCanceledException>());
    }

    [Test]
    public void CancellationDoesNotWaitForTheNextEvent()
    {
        using CancellationTokenSource source = new();
        using ManualResetEventSlim neverSignaled = new(initialState: false);

        // The stream serves the first two events and then blocks forever, standing in for a
        // server that has gone quiet. Cancellation has to be seen without another event.
        Stream content = new BlockingStream(
            Encoding.UTF8.GetBytes(
                "event: modeled\ndata: {\"value\":\"A\"}\n\n" +
                "event: " + UnknownEventType + "\ndata: {\"value\":\"skipped\"}\n\n"),
            neverSignaled);

        MockPipelineResponse response = new(200, "OK") { ContentStream = content };

        SseUpdateCollection<string> collection = new(
            () => ClientResult.FromResponse(response),
            item => DeserializeEventAndCancelOnUnknown(item, source),
            source.Token);

        using IEnumerator<string> enumerator = collection.GetEnumerator();
        Assert.That(enumerator.MoveNext(), Is.True);

        Task<Exception> attempt = Task.Run(() =>
        {
            try
            {
                enumerator.MoveNext();

                return (Exception)null;
            }
            catch (Exception ex)
            {
                return ex;
            }
        });

        Assert.That(attempt.Wait(TimeSpan.FromSeconds(10)), Is.True,
            "MoveNext blocked on a read instead of observing the canceled token.");
        Assert.That(attempt.Result, Is.InstanceOf<OperationCanceledException>());
    }

    [TestCase(false)]
    [TestCase(true)]
    public async Task LifecycleIsLazyAndIndependentForInterleavedEnumerations(bool useAsync)
    {
        var lifecycles = new List<ProbeLifecycle>();
        ProbeLifecycle CreateLifecycle()
        {
            var lifecycle = new ProbeLifecycle();
            lifecycles.Add(lifecycle);

            return lifecycle;
        }

        if (useAsync)
        {
            var collection = new AsyncSseUpdateCollection<string>(
                () => Task.FromResult(CreatePage()), DeserializeEvent, CancellationToken.None)
            {
                LifecycleFactory = CreateLifecycle,
            };
            await using var first = collection.GetAsyncEnumerator();
            await using var second = collection.GetAsyncEnumerator();
            Assert.That(lifecycles, Is.Empty);
            Assert.That(await first.MoveNextAsync(), Is.True);
            Assert.That(await second.MoveNextAsync(), Is.True);
            await first.DisposeAsync();
            await first.DisposeAsync();

            while (await second.MoveNextAsync())
            {
            }
        }
        else
        {
            var collection = new SseUpdateCollection<string>(CreatePage, DeserializeEvent, CancellationToken.None)
            {
                LifecycleFactory = CreateLifecycle,
            };
            using var first = collection.GetEnumerator();
            using var second = collection.GetEnumerator();
            Assert.That(lifecycles, Is.Empty);
            Assert.That(first.MoveNext(), Is.True);
            Assert.That(second.MoveNext(), Is.True);
            first.Dispose();
            first.Dispose();

            while (second.MoveNext())
            {
            }
        }

        Assert.That(lifecycles, Has.Count.EqualTo(2));
        Assert.That(lifecycles[0].Updates, Is.EqualTo(new[] { "A" }));
        Assert.That(lifecycles[0].CompletionKind, Is.EqualTo(SseCompletionKind.Disposed));
        Assert.That(lifecycles[1].Updates, Is.EqualTo(new[] { "A", "B" }));
        Assert.That(lifecycles[1].Events, Is.EqualTo(4));
        Assert.That(lifecycles[1].CompletionKind, Is.EqualTo(SseCompletionKind.EndOfStream));
        Assert.That(lifecycles.All(lifecycle => lifecycle.Depth == 0), Is.True);
        Assert.That(lifecycles.All(lifecycle => lifecycle.TypedResponses == 1), Is.True);
    }

    [TestCase(false, "send")]
    [TestCase(true, "send")]
    [TestCase(false, "read")]
    [TestCase(true, "read")]
    [TestCase(false, "deserialize")]
    [TestCase(true, "deserialize")]
    [TestCase(false, "cancel")]
    [TestCase(true, "cancel")]
    [TestCase(false, "dispose")]
    [TestCase(true, "dispose")]
    public void LifecycleObservesFailuresAndDisposesOpenedResponses(bool useAsync, string phase)
    {
        using var cancellation = new CancellationTokenSource();
        var lifecycle = new ProbeLifecycle();
        var stream = new FaultStream(StreamContent, phase);
        ClientResult Send()
        {
            Assert.That(lifecycle.Depth, Is.EqualTo(1));

            if (phase == "send")
            {
                throw new IOException("send");
            }

            if (phase == "cancel")
            {
                cancellation.Cancel();
            }

            return ClientResult.FromResponse(new MockPipelineResponse(200) { ContentStream = stream });
        }
        IEnumerable<string> Deserialize(SseItem<byte[]> item)
        {
            Assert.That(lifecycle.Depth, Is.EqualTo(1));

            if (phase == "deserialize")
            {
                throw new IOException("deserialize");
            }

            return DeserializeEvent(item);
        }

        if (useAsync)
        {
            var collection = new AsyncSseUpdateCollection<string>(
                () => Task.FromResult(Send()), Deserialize, cancellation.Token)
            {
                LifecycleFactory = () => lifecycle,
            };
            Assert.That(async () =>
            {
                await foreach (var update in collection)
                {
                }
            }, Throws.Exception);
        }
        else
        {
            var collection = new SseUpdateCollection<string>(Send, Deserialize, cancellation.Token)
            {
                LifecycleFactory = () => lifecycle,
            };
            Assert.That(() => collection.ToList(), Throws.Exception);
        }

        Assert.That(lifecycle.Exception, Is.Not.Null);
        Assert.That(lifecycle.TypedResponses, Is.EqualTo(phase == "send" ? 0 : 1));
        Assert.That(lifecycle.Depth, Is.Zero);
        Assert.That(stream.Disposals, Is.EqualTo(phase == "send" ? 0 : 1), lifecycle.Exception?.ToString());
    }

    [TestCase(false)]
    [TestCase(true)]
    public async Task RawPageLifecycleDoesNotReadContent(bool useAsync)
    {
        var lifecycle = new ProbeLifecycle();
        var stream = new FaultStream(StreamContent, "read");
        var page = ClientResult.FromResponse(new MockPipelineResponse(200) { ContentStream = stream });

        if (useAsync)
        {
            var collection = new AsyncSseUpdateCollection<string>(
                () => Task.FromResult(page), DeserializeEvent, CancellationToken.None)
            {
                LifecycleFactory = () => lifecycle,
            };

            await foreach (var raw in collection.GetRawPagesAsync())
            {
                Assert.That(raw, Is.SameAs(page));
                Assert.That(lifecycle.Depth, Is.Zero);
            }
        }
        else
        {
            var collection = new SseUpdateCollection<string>(() => page, DeserializeEvent, CancellationToken.None)
            {
                LifecycleFactory = () => lifecycle,
            };

            foreach (var raw in collection.GetRawPages())
            {
                Assert.That(raw, Is.SameAs(page));
                Assert.That(lifecycle.Depth, Is.Zero);
            }
        }

        Assert.That(lifecycle.Events, Is.Zero);
        Assert.That(lifecycle.TypedResponses, Is.Zero);
        Assert.That(lifecycle.CompletionKind, Is.EqualTo(SseCompletionKind.RawResponse));
        Assert.That(stream.Disposals, Is.Zero);
        stream.Dispose();
    }

    [TestCase(false, false, false)]
    [TestCase(false, false, true)]
    [TestCase(false, true, false)]
    [TestCase(false, true, true)]
    [TestCase(true, false, false)]
    [TestCase(true, false, true)]
    [TestCase(true, true, false)]
    [TestCase(true, true, true)]
    public async Task RawPageHandoffPreservesCallerOwnership(bool useAsync, bool consumeBody, bool disposeEarly)
    {
        var lifecycle = new ProbeLifecycle();
        var stream = new FaultStream(StreamContent, "");
        var page = ClientResult.FromResponse(new MockPipelineResponse(200) { ContentStream = stream });

        if (useAsync)
        {
            var collection = new AsyncSseUpdateCollection<string>(
                () => Task.FromResult(page), DeserializeEvent, CancellationToken.None)
            {
                LifecycleFactory = () => lifecycle,
            };
            await using var enumerator = collection.GetRawPagesAsync().GetAsyncEnumerator();
            Assert.That(await enumerator.MoveNextAsync(), Is.True);
            Assert.That(enumerator.Current, Is.SameAs(page));

            if (consumeBody)
            {
                await stream.CopyToAsync(Stream.Null);
            }

            if (!disposeEarly)
            {
                Assert.That(await enumerator.MoveNextAsync(), Is.False);
            }
        }
        else
        {
            var collection = new SseUpdateCollection<string>(() => page, DeserializeEvent, CancellationToken.None)
            {
                LifecycleFactory = () => lifecycle,
            };
            using var enumerator = collection.GetRawPages().GetEnumerator();
            Assert.That(enumerator.MoveNext(), Is.True);
            Assert.That(enumerator.Current, Is.SameAs(page));

            if (consumeBody)
            {
                stream.CopyTo(Stream.Null);
            }

            if (!disposeEarly)
            {
                Assert.That(enumerator.MoveNext(), Is.False);
            }
        }

        Assert.That(lifecycle.CompletionKind, Is.EqualTo(SseCompletionKind.RawResponse));
        Assert.That(lifecycle.Exception, Is.Null);
        Assert.That(lifecycle.Events, Is.Zero);
        Assert.That(lifecycle.Updates, Is.Empty);
        Assert.That(lifecycle.TypedResponses, Is.Zero);
        Assert.That(lifecycle.Depth, Is.Zero);
        Assert.That(stream.Disposals, Is.Zero);

        if (!consumeBody)
        {
            Assert.That(stream.Position, Is.Zero);
            stream.CopyTo(Stream.Null);
        }

        Assert.That(stream.Position, Is.EqualTo(stream.Length));
        page.GetRawResponse().Dispose();
        Assert.That(stream.Disposals, Is.EqualTo(1));
    }

    [TestCase(false)]
    [TestCase(true)]
    public void RawPageSendFailureIsObserved(bool useAsync)
    {
        var lifecycle = new ProbeLifecycle();
        var exception = new IOException("send");
        ClientResult Send()
        {
            Assert.That(lifecycle.Depth, Is.EqualTo(1));

            throw exception;
        }

        if (useAsync)
        {
            var collection = new AsyncSseUpdateCollection<string>(
                () => Task.FromResult(Send()), DeserializeEvent, CancellationToken.None)
            {
                LifecycleFactory = () => lifecycle,
            };
            Assert.That(async () =>
            {
                await foreach (var page in collection.GetRawPagesAsync())
                {
                }
            }, Throws.Exception.SameAs(exception));
        }
        else
        {
            var collection = new SseUpdateCollection<string>(Send, DeserializeEvent, CancellationToken.None)
            {
                LifecycleFactory = () => lifecycle,
            };
            Assert.That(() => collection.GetRawPages().ToList(), Throws.Exception.SameAs(exception));
        }

        Assert.That(lifecycle.Exception, Is.SameAs(exception));
        Assert.That(lifecycle.Events, Is.Zero);
        Assert.That(lifecycle.Depth, Is.Zero);
    }

    private sealed class ProbeLifecycle : SseLifecycle<string>
    {
        public int Depth { get; private set; }

        public int TypedResponses { get; private set; }

        public int Events { get; private set; }

        public List<string> Updates { get; } = [];

        public SseCompletionKind? CompletionKind { get; private set; }

        public Exception Exception { get; private set; }

        public override IDisposable Enter()
        {
            Depth++;

            return new Activation(this);
        }

        public override void OnEvent()
        {
            Assert.That(Depth, Is.EqualTo(1));
            Events++;
        }

        public override void OnTypedResponse() => TypedResponses++;

        public override void OnUpdate(string update)
        {
            Assert.That(Depth, Is.EqualTo(1));
            Updates.Add(update);
        }

        public override void OnException(Exception exception) => Exception ??= exception;

        public override void Complete(SseCompletionKind completionKind) => CompletionKind ??= completionKind;

        private sealed class Activation(ProbeLifecycle lifecycle) : IDisposable
        {
            public void Dispose() => lifecycle.Depth--;
        }
    }

    private sealed class FaultStream(string content, string phase) : Stream
    {
        private readonly MemoryStream _inner = new(Encoding.UTF8.GetBytes(content));

        public int Disposals { get; private set; }

        public override int Read(byte[] buffer, int offset, int count)
            => phase == "read" ? throw new IOException("read") : _inner.Read(buffer, offset, count);

        public override int Read(Span<byte> buffer)
            => phase == "read" ? throw new IOException("read") : _inner.Read(buffer);

        public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
            => ReadAsync(buffer.AsMemory(offset, count), cancellationToken).AsTask();

        public override async ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
        {
            await Task.Yield();

            if (phase == "read")
            {
                throw new IOException("read");
            }

            return await _inner.ReadAsync(buffer, cancellationToken);
        }

        public override bool CanRead => true;

        public override bool CanSeek => false;

        public override bool CanWrite => false;

        public override long Length => _inner.Length;

        public override long Position
        {
            get => _inner.Position;
            set => throw new NotSupportedException();
        }

        public override void Flush() => throw new NotSupportedException();

        public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();

        public override void SetLength(long value) => throw new NotSupportedException();

        public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();

        protected override void Dispose(bool disposing)
        {
            Disposals++;
            _inner.Dispose();
            base.Dispose(disposing);

            if (phase == "dispose")
            {
                throw new IOException("dispose");
            }
        }
    }

    /// <summary>
    /// Serves <paramref name="content"/> once and then blocks until the gate is set.
    /// </summary>
    private sealed class BlockingStream(byte[] content, ManualResetEventSlim gate) : Stream
    {
        private int _position;

        public override int Read(byte[] buffer, int offset, int count)
        {
            if (_position < content.Length)
            {
                int copied = Math.Min(count, content.Length - _position);
                Array.Copy(content, _position, buffer, offset, copied);
                _position += copied;

                return copied;
            }

            gate.Wait();

            return 0;
        }

        public override bool CanRead => true;

        public override bool CanSeek => false;

        public override bool CanWrite => false;

        public override long Length => throw new NotSupportedException();

        public override long Position
        {
            get => _position;
            set => throw new NotSupportedException();
        }

        public override void Flush()
        {
        }

        public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();

        public override void SetLength(long value) => throw new NotSupportedException();

        public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();
    }

    private static IEnumerable<string> DeserializeEventAndCancelOnUnknown(
        SseItem<byte[]> item,
        CancellationTokenSource source)
    {
        if (item.EventType == UnknownEventType)
        {
            source.Cancel();
        }

        return DeserializeEvent(item);
    }

    private static IEnumerable<string> DeserializeEvent(SseItem<byte[]> item)
    {
        if (item.EventType == UnknownEventType)
        {
            return [];
        }

        using JsonDocument document = JsonDocument.Parse(item.Data);

        return [document.RootElement.GetProperty("value").GetString()!];
    }

    private static ClientResult CreatePage() => CreatePage(StreamContent);

    private static ClientResult CreatePage(string content)
    {
        MockPipelineResponse response = new(200, "OK")
        {
            ContentStream = new MemoryStream(Encoding.UTF8.GetBytes(content)),
        };

        return ClientResult.FromResponse(response);
    }
}
