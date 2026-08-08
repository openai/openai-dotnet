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

    private static IEnumerable<string> DeserializeEvent(SseItem<byte[]> item)
    {
        if (item.EventType == UnknownEventType)
        {
            return [];
        }

        using JsonDocument document = JsonDocument.Parse(item.Data);
        return [document.RootElement.GetProperty("value").GetString()!];
    }

    private static ClientResult CreatePage()
    {
        MockPipelineResponse response = new(200, "OK")
        {
            ContentStream = new MemoryStream(Encoding.UTF8.GetBytes(StreamContent)),
        };

        return ClientResult.FromResponse(response);
    }
}
