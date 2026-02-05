using Microsoft.TypeSpec.Generator.Customizations;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace OpenAI.Audio;

[CodeGenType("DotNetCombinedJsonTranscriptionResponse")]
public partial class AudioTranscription
{
    // CUSTOM: Made nullable because this is an optional property.
    /// <summary> The duration of the input audio. </summary>
    public TimeSpan? Duration { get; }

    // CUSTOM:
    // - Added Experimental attribute.
    // - Reused common logprob type.
    // - Made readonly.
    [Experimental("OPENAI001")]
    [CodeGenMember("Logprobs")]
    public IReadOnlyList<AudioTokenLogProbabilityDetails> TranscriptionTokenLogProbabilities { get; }
}