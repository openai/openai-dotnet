using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Realtime;

// CUSTOM: Renamed.
[CodeGenType("IncludableGA")]
public readonly partial struct GARealtimeIncludedProperty
{
    // CUSTOM: Renamed.
    [CodeGenMember("ItemInputAudioTranscriptionLogprobs")]
    public static GARealtimeIncludedProperty ItemInputAudioTranscriptionLogProbabilities { get; } = new GARealtimeIncludedProperty(ItemInputAudioTranscriptionLogprobsValue);
}