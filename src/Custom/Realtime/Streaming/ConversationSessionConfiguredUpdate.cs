using Microsoft.TypeSpec.Generator.Customizations;
using System.Collections.Generic;

namespace OpenAI.Realtime;

/// <summary>
/// The update (response command) of type <c>session.updated</c>, which is received when a preceding
/// <c>session.update</c> request command
/// (<see cref="RealtimeSessionClient.ConfigureConversationSessionAsync(OpenAI.Realtime.ConversationSessionOptions, System.Threading.CancellationToken)"/>)
/// has been applied to the session. New session configuration related to response generation will not take effect
/// until the next response; shared session configuration, such as input audio format, will apply immediately.
/// </summary>
[CodeGenType("RealtimeServerEventSessionUpdated")]
public partial class ConversationSessionConfiguredUpdate
{
    [CodeGenMember("Session")]
    internal readonly InternalRealtimeSessionGA _internalSession;

    public string SessionId => _internalSession.Id;

    public string Model => _internalSession.Model;

    public RealtimeContentModalities ContentModalities
        => RealtimeContentModalitiesExtensions.FromInternalModalities(_internalSession.OutputModalities);

    public string Instructions => _internalSession.Instructions;

    public ConversationVoice Voice
        => _internalSession.Audio?.Output?.Voice is { } voice ? new ConversationVoice(voice.ToString()) : default;

    public RealtimeAudioFormat InputAudioFormat
        => _internalSession.Audio?.Input?.Format ?? default;
    public RealtimeAudioFormat OutputAudioFormat
        => _internalSession.Audio?.Output?.Format ?? default;

    public InputTranscriptionOptions InputTranscriptionOptions => _internalSession.Audio?.Input?.Transcription;
    public TurnDetectionOptions TurnDetectionOptions => _internalSession.Audio?.Input?.TurnDetection;
    public IReadOnlyList<ConversationTool> Tools => [ .. _internalSession.Tools ];
    public ConversationToolChoice ToolChoice => ConversationToolChoice.FromBinaryData(_internalSession.ToolChoice);
    public float Temperature => _internalSession.Temperature ?? default;
    // Customization: API changed from max_response_output_tokens to max_output_tokens
    public ConversationMaxTokensChoice MaxOutputTokens => ConversationMaxTokensChoice.FromBinaryData(_internalSession.MaxOutputTokens);
}
