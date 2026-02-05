using Microsoft.TypeSpec.Generator.Customizations;
using System.Collections.Generic;

namespace OpenAI.Audio;

// CUSTOM: Added Experimental attribute.
[CodeGenType("DotNetTranscriptTextDeltaEvent")]
public partial class StreamingAudioTranscriptionTextDeltaUpdate
{
    // CUSTOM: Rename; make readonly
    [CodeGenMember("Logprobs")]
    public IReadOnlyList<AudioTokenLogProbabilityDetails> TranscriptionTokenLogProbabilities { get; }
}