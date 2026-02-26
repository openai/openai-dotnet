using Microsoft.TypeSpec.Generator.Customizations;
using System;

namespace OpenAI.Realtime;

/// <summary>
/// Corresponds to the <c>input_audio_buffer.append</c> client event.
/// Send this event to append audio bytes to the input audio buffer. The audio
/// buffer is temporary storage you can write to and later commit. A "commit" will create a new
/// user message item in the conversation history from the buffer content and clear the buffer.
/// Input audio transcription (if enabled) will be generated when the buffer is committed.
/// If VAD is enabled the audio buffer is used to detect speech and the server will decide
/// when to commit. When Server VAD is disabled, you must commit the audio buffer
/// manually. Input audio noise reduction operates on writes to the audio buffer.
/// <para>
/// The client may choose how much audio to place in each event up to a maximum
/// of 15 MiB, for example streaming smaller chunks from the client may allow the
/// VAD to be more responsive. Unlike most other client events, the server will
/// not send a confirmation response to this event.
/// </para>
/// </summary>
// CUSTOM: Renamed.
[CodeGenType("RealtimeClientEventInputAudioBufferAppendGA")]
public partial class GARealtimeClientCommandInputAudioBufferAppend
{
    // CUSTOM: Renamed.
    [CodeGenMember("Audio")]
    public BinaryData AudioBytes { get; }
}