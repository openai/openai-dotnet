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

        Assert.ThrowsAsync<OperationCanceledException>(async () =>
        {
            await foreach (string _ in collection)
            {
            }
        });
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
        Assert.ThrowsAsync<OperationCanceledException>(async () =>
        {
            await foreach (string _ in collection)
            {
            }
        });
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
        public override void Flush() { }
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
