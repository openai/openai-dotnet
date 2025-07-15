using System;
using System.Collections.Generic;
using System.ClientModel.Primitives;
using System.Diagnostics.CodeAnalysis;

namespace OpenAI.Realtime;

[CodeGenType("RealtimeTranscriptionSessionCreateRequest")]
public partial class TranscriptionSessionOptions
{
    [CodeGenMember("Modalities")]
    private IList<InternalRealtimeRequestSessionModality> _internalModalities;

    public RealtimeContentModalities ContentModalities
    {
        get => RealtimeContentModalitiesExtensions.FromInternalModalities(_internalModalities);
        set => _internalModalities = value.ToInternalModalities();
    }

    [CodeGenMember("InputAudioFormat")]
    public RealtimeAudioFormat? InputAudioFormat { get; set; }

    [CodeGenMember("InputAudioTranscription")]
    public InputTranscriptionOptions InputTranscriptionOptions { get; set; }

    [CodeGenMember("TurnDetection")]
    public TurnDetectionOptions TurnDetectionOptions { get; set; }

    [CodeGenMember("InputAudioNoiseReduction")]
    public InputNoiseReductionOptions InputNoiseReductionOptions { get; set; }

    [CodeGenMember("Include")]
    public IList<string> Include { get; } = ["item.input_audio_transcription.logprobs"];
}