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
[CodeGenType("RealtimeServerEventSessionUpdated")]
public partial class ConversationSessionConfiguredUpdate
{
    [CodeGenMember("Session")]
    internal readonly InternalRealtimeResponseSession _internalSession;

    public string SessionId => _internalSession.Id;

    public string Model => _internalSession.Model;

    public RealtimeContentModalities ContentModalities
        => RealtimeContentModalitiesExtensions.FromInternalModalities(_internalSession.Modalities);

    public string Instructions => _internalSession.Instructions;

    public ConversationVoice Voice => _internalSession.Voice;

    public RealtimeAudioFormat InputAudioFormat => _internalSession.InputAudioFormat;
    public RealtimeAudioFormat OutputAudioFormat => _internalSession.OutputAudioFormat;

    public InputTranscriptionOptions InputTranscriptionOptions => _internalSession.InputAudioTranscription;
    public TurnDetectionOptions TurnDetectionOptions => _internalSession.TurnDetection;
    public IReadOnlyList<ConversationTool> Tools => [ .. _internalSession.Tools ];
    public ConversationToolChoice ToolChoice => ConversationToolChoice.FromBinaryData(_internalSession.ToolChoice);
    public float Temperature => _internalSession.Temperature;
    public ConversationMaxTokensChoice MaxOutputTokens => _internalSession.MaxResponseOutputTokens;
}
