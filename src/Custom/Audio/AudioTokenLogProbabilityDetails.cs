using System;
using System.Diagnostics.CodeAnalysis;

namespace OpenAI.Audio;

// CUSTOM: Added Experimental attribute.
[CodeGenType("DotNetAudioLogProbsProperties")]
public partial class AudioTokenLogProbabilityDetails
{
    // CUSTOM: Renamed.
    [CodeGenMember("Logprob")]
    public float LogProbability { get; }

    // CUSTOM: Renamed, type changed to ReadOnlyMemory<byte>
    [CodeGenMember("Bytes")]
    public ReadOnlyMemory<byte> Utf8Bytes { get; }
}