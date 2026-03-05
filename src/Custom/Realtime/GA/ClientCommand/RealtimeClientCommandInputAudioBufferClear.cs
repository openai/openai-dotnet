using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Realtime;

/// <summary>
/// Corresponds to the <c>input_audio_buffer.clear</c> client event.
/// Send this event to clear the audio bytes in the buffer. The server will
/// respond with an <c>input_audio_buffer.cleared</c> event.
/// </summary>
// CUSTOM: Renamed.
[CodeGenType("RealtimeClientEventInputAudioBufferClearGA")]
public partial class RealtimeClientCommandInputAudioBufferClear
{
}