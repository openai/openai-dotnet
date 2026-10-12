using Microsoft.TypeSpec.Generator.Customizations;
using System;

namespace OpenAI.Realtime;

// CUSTOM: Renamed.
[CodeGenType("DotNetRealtimeLogProbsProperties")]
public partial class RealtimeTokenLogProbabilityDetails
{
    // CUSTOM: Renamed.
    [CodeGenMember("Logprob")]
    public float LogProbability { get; set; }

    // CUSTOM:
    // - Renamed.
    // - Changed type.
    [CodeGenMember("Bytes")]
    public ReadOnlyMemory<byte> Utf8Bytes { get; set; }
}