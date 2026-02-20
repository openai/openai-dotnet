using Microsoft.TypeSpec.Generator.Customizations;
using System;
using System.Collections.Generic;

namespace OpenAI.Realtime;

// CUSTOM: Renamed.
[CodeGenType("LogProbPropertiesGA")]
public partial class GARealtimeLogProbabilityDetails
{
    // CUSTOM: Renamed.
    [CodeGenMember("Logprob")]
    public float LogProbability { get; }

    // CUSTOM:
    // - Renamed.
    // - Changed type.
    [CodeGenMember("Bytes")]
    public ReadOnlyMemory<byte> Utf8Bytes { get; }
}