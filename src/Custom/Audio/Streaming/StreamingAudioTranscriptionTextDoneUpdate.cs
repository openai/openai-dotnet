using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace OpenAI.Audio;

// CUSTOM: Added Experimental attribute.
[CodeGenType("TranscriptTextDoneEvent")]
public partial class StreamingAudioTranscriptionTextDoneUpdate
{
    // CUSTOM: Rename; make readonly; apply shared audio logprobs type
    [CodeGenMember("Logprobs")]
    public IReadOnlyList<AudioTokenLogProbabilityDetails> TranscriptionTokenLogProbabilities { get; }
}