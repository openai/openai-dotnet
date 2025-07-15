using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace OpenAI.Audio;

// CUSTOM: Added Experimental attribute.
[CodeGenType("TranscriptTextDeltaEvent")]
public partial class StreamingAudioTranscriptionTextDeltaUpdate
{
    // CUSTOM: Rename; make readonly
    [CodeGenMember("Logprobs")]
    public IReadOnlyList<AudioTokenLogProbabilityDetails> TranscriptionTokenLogProbabilities { get; }
}