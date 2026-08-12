#pragma warning disable SCME0005
using Microsoft.ClientModel.TestFramework.Mocks;
using NUnit.Framework;
using System;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.IO;
using System.Net.ServerSentEvents;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace OpenAI.Tests.Utility;

/// <summary>
/// Tests for <see cref="SseStreamingClientResult"/> to verify that events
/// yielding zero updates do not terminate the stream and that cancellation
/// is observed promptly.
/// </summary>
[Parallelizable(ParallelScope.All)]
[Category("Smoke")]
public class SseStreamingClientResultTests
{
    private const string UnknownEventType = "unknown";

    private const string StreamContent =
        "event: modeled\ndata: {\"value\":\"A\"}\n\n" +
        "event: " + UnknownEventType + "\ndata: {\"value\":\"skipped\"}\n\n" +
        "event: modeled\ndata: {\"value\":\"B\"}\n\n" +
        "data: [DONE]\n\n";

    [Test]
    public async Task EventWithNoUpdatesDoesNotEndTheStream()
    {
        AsyncStreamingClientResult<string> result = CreateResult(StreamContent);

        List<string> updates = [];
        await foreach (string update in result)
        {
            updates.Add(update);
        }

        Assert.That(updates, Is.EqualTo(new[] { "A", "B" }));
    }

    private const string StreamContentWithConsecutiveUnknownEvents =
        "event: modeled\ndata: {\"value\":\"A\"}\n\n" +
        "event: " + UnknownEventType + "\ndata: {\"value\":\"skipped\"}\n\n" +
        "event: " + UnknownEventType + "\ndata: {\"value\":\"skipped\"}\n\n" +
        "event: modeled\ndata: {\"value\":\"B\"}\n\n" +
        "data: [DONE]\n\n";

    [Test]
    public async Task SkippingEventsWithNoUpdatesObservesCancellation()
    {
        using CancellationTokenSource source = new();

        await using AsyncStreamingClientResult<string> result = SseStreamingClientResult.Create(
            CreateResponse(StreamContentWithConsecutiveUnknownEvents),
            item => DeserializeEventAndCancelOnUnknown(item, source),
            source.Token);

        IAsyncEnumerator<string> enumerator = ((IAsyncEnumerable<string>)result).GetAsyncEnumerator();

        Assert.That(await enumerator.MoveNextAsync(), Is.True);
        Assert.That(enumerator.Current, Is.EqualTo("A"));

        Assert.ThrowsAsync<OperationCanceledException>(async () => await enumerator.MoveNextAsync());
    }

    private const string StreamContentUnknownThenTerminal =
        "event: modeled\ndata: {\"value\":\"A\"}\n\n" +
        "event: " + UnknownEventType + "\ndata: {\"value\":\"skipped\"}\n\n" +
        "data: [DONE]\n\n";

    [Test]
    public async Task CancellationIsNotOvertakenByABufferedTerminalEvent()
    {
        using CancellationTokenSource source = new();

        await using AsyncStreamingClientResult<string> result = SseStreamingClientResult.Create(
            CreateResponse(StreamContentUnknownThenTerminal),
            item => DeserializeEventAndCancelOnUnknown(item, source),
            source.Token);

        IAsyncEnumerator<string> enumerator = ((IAsyncEnumerable<string>)result).GetAsyncEnumerator();

        Assert.That(await enumerator.MoveNextAsync(), Is.True);
        Assert.That(enumerator.Current, Is.EqualTo("A"));

        Assert.ThrowsAsync<OperationCanceledException>(async () => await enumerator.MoveNextAsync());
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

    private static AsyncStreamingClientResult<string> CreateResult(string content)
    {
        return SseStreamingClientResult.Create(
            CreateResponse(content),
            DeserializeEvent,
            CancellationToken.None);
    }

    private static PipelineResponse CreateResponse(string content)
    {
        return new MockPipelineResponse(200)
        {
            ContentStream = new NonClosingMemoryStream(Encoding.UTF8.GetBytes(content)),
        };
    }

    private sealed class NonClosingMemoryStream(byte[] buffer) : MemoryStream(buffer)
    {
        protected override void Dispose(bool disposing) { }
    }
}
