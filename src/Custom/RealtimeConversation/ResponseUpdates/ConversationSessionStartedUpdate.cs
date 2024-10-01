using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace OpenAI.RealtimeConversation;

[Experimental("OPENAI002")]
[CodeGenModel("RealtimeResponseSessionCreatedCommand")]
public partial class ConversationSessionStartedUpdate
{
    [CodeGenMember("Session")]
    internal readonly InternalRealtimeResponseSession _internalSession;

    public string SessionId => _internalSession.Id;

    public string Model => _internalSession.Model;

    public ConversationContentModalities ContentModalities
        => ConversationContentModalitiesExtensions.FromInternalModalities(_internalSession.Modalities);

    public string Instructions => _internalSession.Instructions;

    public ConversationVoice Voice => _internalSession.Voice;

    public ConversationAudioFormat InputAudioFormat => _internalSession.InputAudioFormat;
    public ConversationAudioFormat OutputAudioFormat => _internalSession.OutputAudioFormat;

    public ConversationInputTranscriptionOptions TranscriptionSettings => _internalSession.InputAudioTranscription;
    public ConversationTurnDetectionOptions TurnDetectionSettings => _internalSession.TurnDetection;
    public IReadOnlyList<ConversationTool> Tools => _internalSession.Tools;
    public ConversationToolChoice ToolChoice => ConversationToolChoice.FromBinaryData(_internalSession.ToolChoice);
    public float Temperature => _internalSession.Temperature;
    public ConversationMaxTokensChoice MaxOutputTokens => _internalSession.MaxResponseOutputTokens;
}
