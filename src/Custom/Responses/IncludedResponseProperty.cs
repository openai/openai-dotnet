using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Responses;

// CUSTOM: Renamed.
[CodeGenType("Includable")]
public readonly partial struct IncludedResponseProperty
{
    // CUSTOM: Renamed.
    [CodeGenMember("MessageInputImageImageUrl")]
    public static IncludedResponseProperty MessageInputImageUri { get; } = new IncludedResponseProperty(MessageInputImageImageUrlValue);

    // CUSTOM: Renamed.
    [CodeGenMember("ComputerCallOutputOutputImageUrl")]
    public static IncludedResponseProperty ComputerCallOutputImageUri { get; } = new IncludedResponseProperty(ComputerCallOutputOutputImageUrlValue);

    // CUSTOM: Renamed.
    [CodeGenMember("MessageOutputTextLogprobs")]
    public static IncludedResponseProperty MessageOutputTextLogProbabilities { get; } = new IncludedResponseProperty(MessageOutputTextLogprobsValue);
}