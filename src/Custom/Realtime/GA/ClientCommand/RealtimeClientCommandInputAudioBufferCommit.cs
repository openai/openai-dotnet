using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Realtime;

/// <summary>
/// Corresponds to the <c>input_audio_buffer.commit</c> client event.
/// Send this event to commit the user input audio buffer, which will create a new user message item in the conversation. This event will produce an error if the input audio buffer is empty. When in Server VAD mode, the client does not need to send this event, the server will commit the audio buffer automatically.
/// Committing the input audio buffer will trigger input audio transcription (if enabled in session configuration), but it will not create a response from the model. The server will respond with an <c>input_audio_buffer.committed</c> event.
/// </summary>
// CUSTOM: Renamed.
[CodeGenType("RealtimeClientEventInputAudioBufferCommitGA")]
public partial class GARealtimeClientCommandInputAudioBufferCommit
{
}