using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Realtime;

/// <summary>
/// Corresponds to the <c>response.create</c> client event.
/// This event instructs the server to create a Response, which means triggering
/// model inference. When in Server VAD mode, the server will create Responses
/// automatically.
/// <para>
/// A Response will include at least one Item, and may have two, in which case
/// the second will be a function call. These Items will be appended to the
/// conversation history by default.
/// </para>
/// <para>
/// The server will respond with a <c>response.created</c> event, events for Items
/// and content created, and finally a <c>response.done</c> event to indicate the
/// Response is complete.
/// </para>
/// <para>
/// The <c>response.create</c> event includes inference configuration like
/// <c>instructions</c> and <c>tools</c>. If these are set, they will override the Session's
/// configuration for this Response only.
/// </para>
/// <para>
/// Responses can be created out-of-band of the default Conversation, meaning that they can
/// have arbitrary input, and it's possible to disable writing the output to the Conversation.
/// Only one Response can write to the default Conversation at a time, but otherwise multiple
/// Responses can be created in parallel. The <c>metadata</c> field is a good way to disambiguate
/// multiple simultaneous Responses.
/// </para>
/// <para>
/// Clients can set <c>conversation</c> to <c>none</c> to create a Response that does not write to the default
/// Conversation. Arbitrary input can be provided with the <c>input</c> field, which is an array accepting
/// raw Items and references to existing Items.
/// </para>
/// </summary>
// CUSTOM: Renamed.
[CodeGenType("RealtimeClientEventResponseCreateGA")]
public partial class GARealtimeClientCommandResponseCreate
{
    // CUSTOM: Renamed.
    [CodeGenMember("Response")]
    public GARealtimeResponseOptions ResponseOptions { get; set; }
}