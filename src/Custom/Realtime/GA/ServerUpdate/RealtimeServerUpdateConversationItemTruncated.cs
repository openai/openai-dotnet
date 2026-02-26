using Microsoft.TypeSpec.Generator.Customizations;
using System;

namespace OpenAI.Realtime;

/// <summary>
/// Corresponds to the <c>conversation.item.truncated</c> server event.
/// Returned when an earlier assistant audio message item is truncated by the
/// client with a <c>conversation.item.truncate</c> event. This event is used to
/// synchronize the server's understanding of the audio with the client's playback.
/// This action will truncate the audio and remove the server-side text transcript
/// to ensure there is no text in the context that hasn't been heard by the user.
/// </summary>
// CUSTOM: Renamed.
[CodeGenType("RealtimeServerEventConversationItemTruncatedGA")]
public partial class GARealtimeServerUpdateConversationItemTruncated
{
    // CUSTOM: Renamed.
    [CodeGenMember("AudioEndMs")]
    public TimeSpan AudioEndTime { get; }
}
