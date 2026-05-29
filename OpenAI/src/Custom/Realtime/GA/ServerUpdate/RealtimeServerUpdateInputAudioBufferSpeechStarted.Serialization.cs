using Microsoft.TypeSpec.Generator.Customizations;
using System;
using System.ClientModel.Primitives;
using System.Runtime.CompilerServices;
using System.Text.Json;

namespace OpenAI.Realtime;

// CUSTOM: Temporary fix to serialize AudioStartTime as an integer number of milliseconds instead of a double to match the REST API.
// Remove once generator bug https://github.com/microsoft/typespec/issues/10831 is addressed
[CodeGenSerialization(nameof(AudioStartTime), SerializationValueHook = nameof(SerializeAudioStartTimeValue))]
public partial class RealtimeServerUpdateInputAudioBufferSpeechStarted
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void SerializeAudioStartTimeValue(Utf8JsonWriter writer, ModelReaderWriterOptions options)
        => writer.WriteNumberValue((long)(AudioStartTime.Ticks / TimeSpan.TicksPerMillisecond));
}
