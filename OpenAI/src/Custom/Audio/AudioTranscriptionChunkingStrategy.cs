using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Audio;

// CUSTOM: Added to represent a non-discriminated union.
[CodeGenType("DotNetTranscriptionChunkingStrategy")]
[CodeGenVisibility(nameof(AudioTranscriptionChunkingStrategy), CodeGenVisibility.Internal)]
public partial class AudioTranscriptionChunkingStrategy
{
    // CUSTOM: Added to support the corresponding component of the union.
    public AudioTranscriptionChunkingStrategy(AudioTranscriptionDefaultChunkingStrategy defaultChunkingStrategy)
    {
        DefaultChunkingStrategy = defaultChunkingStrategy;
    }

    // CUSTOM: Added to support the corresponding component of the union.
    public AudioTranscriptionChunkingStrategy(AudioTranscriptionCustomChunkingStrategy customChunkingStrategy)
    {
        Argument.AssertNotNull(customChunkingStrategy, nameof(customChunkingStrategy));

        CustomChunkingStrategy = customChunkingStrategy;
    }

    // CUSTOM: Removed setter.
    [CodeGenMember("DefaultChunkingStrategy")]
    public AudioTranscriptionDefaultChunkingStrategy? DefaultChunkingStrategy { get; }

    // CUSTOM: Removed setter.
    [CodeGenMember("CustomChunkingStrategy")]
    public AudioTranscriptionCustomChunkingStrategy CustomChunkingStrategy { get; }

    // CUSTOM: Added for convenience.
    public static implicit operator AudioTranscriptionChunkingStrategy(AudioTranscriptionDefaultChunkingStrategy defaultChunkingStrategy) => new(defaultChunkingStrategy);

    // CUSTOM: Added for convenience.
    public static implicit operator AudioTranscriptionChunkingStrategy(AudioTranscriptionCustomChunkingStrategy customChunkingStrategy) => customChunkingStrategy is null ? null : new(customChunkingStrategy);
}
