using Microsoft.TypeSpec.Generator.Customizations;
using System.Collections.Generic;

namespace OpenAI.Audio;

// CUSTOM: Added Experimental attribute.
[CodeGenType("DotNetTranscriptTextDoneEvent")]
public partial class StreamingAudioTranscriptionTextDoneUpdate
{
    // CUSTOM: Rename; make readonly; apply shared audio logprobs type
    [CodeGenMember("Logprobs")]
    public IReadOnlyList<AudioTokenLogProbabilityDetails> TranscriptionTokenLogProbabilities { get; }
}