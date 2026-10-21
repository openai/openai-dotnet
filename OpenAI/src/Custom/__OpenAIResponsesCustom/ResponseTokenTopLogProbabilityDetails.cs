using Microsoft.TypeSpec.Generator.Customizations;
using System;

namespace OpenAI.Responses;

// CUSTOM: Renamed.
[CodeGenType("TopLogProb")]
public partial class ResponseTokenTopLogProbabilityDetails
{
    // CUSTOM: Renamed.
    [CodeGenMember("Logprob")]
    public float LogProbability { get; set; }

    // CUSTOM:
    // - Renamed.
    // - Changed type from IReadOnlyList<int> to ReadOnlyMemory<byte>?.
    [CodeGenMember("Bytes")]
    public ReadOnlyMemory<byte>? Utf8Bytes { get; set; }
}
