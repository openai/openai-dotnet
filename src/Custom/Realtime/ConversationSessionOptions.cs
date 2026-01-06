using Microsoft.TypeSpec.Generator.Customizations;
using System;
using System.ClientModel.Primitives;
using System.Collections.Generic;

namespace OpenAI.Realtime;

[CodeGenType("RealtimeRequestSession")]
public partial class ConversationSessionOptions
{
    // CUSTOM: Use a scenario-specific copy of the shared voice ID collection
    public ConversationVoice? Voice { get; set; }

    [CodeGenMember("Model")]
    internal InternalRealtimeRequestSessionModel? Model { get; set; }

    [CodeGenMember("Modalities")]
    private IList<InternalRealtimeRequestSessionModality> _internalModalities;

    public RealtimeContentModalities ContentModalities
    {
        get => RealtimeContentModalitiesExtensions.FromInternalModalities(_internalModalities);
        set => _internalModalities = value.ToInternalModalities();
    }

    [CodeGenMember("ToolChoice")]
    private BinaryData _internalToolChoice;

    public ConversationToolChoice ToolChoice
    {
        get => ConversationToolChoice.FromBinaryData(_internalToolChoice);
        set
        {
            _internalToolChoice = value is not null
                ? ModelReaderWriter.Write(value, ModelReaderWriterOptions.Json, OpenAIContext.Default)
                : null;
        }
    }

    [CodeGenMember("MaxResponseOutputTokens")]
    private BinaryData _maxResponseOutputTokens;

    public ConversationMaxTokensChoice MaxOutputTokens
    {
        get => ConversationMaxTokensChoice.FromBinaryData(_maxResponseOutputTokens);
        set
        {
            _maxResponseOutputTokens = value == null ? null : ModelReaderWriter.Write(value, ModelReaderWriterOptions.Json, OpenAIContext.Default);
        }
    }

    [CodeGenMember("TurnDetection")]
    public TurnDetectionOptions TurnDetectionOptions { get; set; }

    [CodeGenMember("InputAudioTranscription")]
    public InputTranscriptionOptions InputTranscriptionOptions { get; set; }

    [CodeGenMember("InputAudioNoiseReduction")]
    public InputNoiseReductionOptions InputNoiseReductionOptions { get; set; }
}