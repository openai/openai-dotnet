using Microsoft.TypeSpec.Generator.Customizations;
using System;
using System.Collections.Generic;

namespace OpenAI.Responses;

// CUSTOM: Renamed.
[CodeGenType("LogProb")]
public partial class ResponseTokenLogProbabilityDetails
{
    // CUSTOM: Renamed.
    [CodeGenMember("Logprob")]
    public float LogProbability { get; set; }

    // CUSTOM:
    // - Renamed.
    // - Changed type from IReadOnlyList<int> to ReadOnlyMemory<byte>?.
    [CodeGenMember("Bytes")]
    public ReadOnlyMemory<byte>? Utf8Bytes { get; set; }

    // CUSTOM: Renamed.
    [CodeGenMember("TopLogprobs")]
    public IReadOnlyList<ResponseTokenTopLogProbabilityDetails> TopLogProbabilities { get; }
}
