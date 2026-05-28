using NUnit.Framework;
using OpenAI.Realtime;
using System;
using System.ClientModel.Primitives;
using System.Text.Json;

namespace OpenAI.Tests.Realtime;

#pragma warning disable OPENAI002

[Category("Smoke")]
public class RealtimeSerializationSmokeTests
{
    [Test]
    public void ConversationItemTruncateSerializesAudioEndMillisecondsAsInteger()
    {
        RealtimeClientCommandConversationItemTruncate command = new(
            "item_test",
            contentIndex: 0,
            audioEndTime: TimeSpan.FromTicks(12_345_678));

        BinaryData serializedCommand = ModelReaderWriter.Write(command);

        using JsonDocument document = JsonDocument.Parse(serializedCommand);
        JsonElement audioEndMilliseconds = document.RootElement.GetProperty("audio_end_ms");

        Assert.That(audioEndMilliseconds.TryGetInt64(out long value), Is.True);
        Assert.That(value, Is.EqualTo(1234));
        Assert.That(audioEndMilliseconds.GetRawText(), Does.Not.Contain("."));
    }
}
