using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Realtime;

// CUSTOM: Renamed.
[CodeGenType("IncludableGA")]
public readonly partial struct RealtimeIncludedProperty
{
    // CUSTOM: Renamed.
    [CodeGenMember("ItemInputAudioTranscriptionLogprobs")]
    public static RealtimeIncludedProperty ItemInputAudioTranscriptionLogProbabilities { get; } = new RealtimeIncludedProperty(ItemInputAudioTranscriptionLogprobsValue);
}