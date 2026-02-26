using Microsoft.TypeSpec.Generator.Customizations;
using System;

namespace OpenAI.Realtime;

/// <summary>
/// Corresponds to the <c>conversation.item.truncate</c> client event.
/// Send this event to truncate a previous assistant message's audio. The server
/// will produce audio faster than realtime, so this event is useful when the user
/// interrupts to truncate audio that has already been sent to the client but not
/// yet played. This will synchronize the server's understanding of the audio with
/// the client's playback.
/// Truncating audio will delete the server-side text transcript to ensure there
/// is not text in the context that hasn't been heard by the user.
/// If successful, the server will respond with a <c>conversation.item.truncated</c>
/// event.
/// </summary>
// CUSTOM: Renamed.
[CodeGenType("RealtimeClientEventConversationItemTruncateGA")]
public partial class GARealtimeClientCommandConversationItemTruncate
{
    // CUSTOM: Renamed.
    [CodeGenMember("AudioEndMs")]
    public TimeSpan AudioEndTime { get; }
}