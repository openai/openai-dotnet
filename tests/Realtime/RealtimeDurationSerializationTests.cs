using NUnit.Framework;
using OpenAI.Realtime;
using System;
using System.ClientModel.Primitives;
using System.Text.Json;

namespace OpenAI.Tests.Realtime;

#pragma warning disable OPENAI002

[Category("Smoke")]
public class RealtimeDurationSerializationTests
{
    [Test]
    public void AudioEndMs_SerializesAsInteger()
    {
        // A TimeSpan with sub-millisecond precision that would produce a fractional double
        var truncate = new RealtimeClientCommandConversationItemTruncate(
            itemId: "item_abc",
            contentIndex: 0,
            audioEndTime: TimeSpan.FromTicks(12345678)); // 1234.5678 ms

        BinaryData json = ModelReaderWriter.Write(truncate);
        using JsonDocument doc = JsonDocument.Parse(json);
        JsonElement root = doc.RootElement;

        // Should be 1234 (integer), not 1234.5678
        var audioEndMs = root.GetProperty("audio_end_ms");
        Assert.That(audioEndMs.TryGetInt64(out long value), Is.True);
        Assert.That(value, Is.EqualTo(1234));
    }
}
