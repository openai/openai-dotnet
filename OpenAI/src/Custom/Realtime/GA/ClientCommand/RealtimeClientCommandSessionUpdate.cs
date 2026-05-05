using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Realtime;

/// <summary>
/// Corresponds to the <c>session.update</c> client event.
/// Send this event to update the session's configuration.
/// The client may send this event at any time to update any field
/// except for <c>voice</c> and <c>model</c>. <c>voice</c> can be updated only if there have been no other audio outputs yet.
/// When the server receives a <c>session.update</c>, it will respond
/// with a <c>session.updated</c> event showing the full, effective configuration.
/// Only the fields that are present in the <c>session.update</c> are updated. To clear a field like
/// <c>instructions</c>, pass an empty string. To clear a field like <c>tools</c>, pass an empty array.
/// To clear a field like <c>turn_detection</c>, pass <c>null</c>.
/// </summary>
// CUSTOM: Renamed.
[CodeGenType("RealtimeClientEventSessionUpdateGA")]
public partial class RealtimeClientCommandSessionUpdate
{
    // CUSTOM: Renamed.
    [CodeGenMember("Session")]
    public RealtimeSessionOptions SessionOptions { get; }
}