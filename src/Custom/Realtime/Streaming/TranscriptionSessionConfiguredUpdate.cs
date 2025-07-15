using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace OpenAI.Realtime;

/// <summary>
/// The update (response command) of type <c>session.updated</c>, which is received when a preceding
/// <c>session.update</c> request command
/// (<see cref="RealtimeSession.ConfigureConversationSessionAsync(OpenAI.Realtime.ConversationSessionOptions, System.Threading.CancellationToken)"/>)
/// has been applied to the session. New session configuration related to response generation will not take effect
/// until the next response; shared session configuration, such as input audio format, will apply immediately.
/// </summary>
[CodeGenType("RealtimeServerEventTranscriptionSessionUpdated")]
public partial class TranscriptionSessionConfiguredUpdate
{
    [CodeGenMember("Session")]
    internal readonly InternalRealtimeTranscriptionSessionCreateResponse _internalSession;

    private InternalRealtimeTranscriptionSessionCreateResponseClientSecret ClientSecret { get; }

    public RealtimeContentModalities ContentModalities
        => RealtimeContentModalitiesExtensions.FromInternalModalities(_internalSession?.Modalities);

    public RealtimeAudioFormat InputAudioFormat => _internalSession?.InputAudioFormat ?? default;

    public InputTranscriptionOptions InputAudioTranscription => _internalSession?.InputAudioTranscription;

    public TurnDetectionOptions TurnDetection => _internalSession?.TurnDetection;
}
