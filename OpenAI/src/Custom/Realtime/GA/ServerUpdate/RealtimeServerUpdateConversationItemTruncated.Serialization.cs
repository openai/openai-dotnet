using Microsoft.TypeSpec.Generator.Customizations;
using System;
using System.ClientModel.Primitives;
using System.Runtime.CompilerServices;
using System.Text.Json;

namespace OpenAI.Realtime;

// CUSTOM: Temporary fix to serialize AudioEndTime as an integer number of milliseconds instead of a double to match the REST API.
// Remove once generator bug https://github.com/microsoft/typespec/issues/10831 is addressed
[CodeGenSerialization(nameof(AudioEndTime), SerializationValueHook = nameof(SerializeAudioEndTimeValue))]
public partial class RealtimeServerUpdateConversationItemTruncated
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void SerializeAudioEndTimeValue(Utf8JsonWriter writer, ModelReaderWriterOptions options)
        => writer.WriteNumberValue((long)(AudioEndTime.Ticks / TimeSpan.TicksPerMillisecond));
}
