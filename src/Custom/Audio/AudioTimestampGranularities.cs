using System;

namespace OpenAI.Audio;

/// <summary>
/// Specifies the timestamp granularities to populate for a transcription.
/// </summary>
[Flags]
public enum AudioTimestampGranularities
{
    /// <summary>
    /// The default value that, when equivalent to a request's flags, specifies no specific audio timestamp granularity
    /// and defers to the default timestamp behavior.
    /// </summary>
    Default = 0,

    /// <summary>
    /// The value that, when present in the request's flags, specifies that audio information should include word-level
    /// timestamp information.
    /// </summary>
    Word = 1,

    /// <summary>
    /// The value that, when present in the request's flags, specifies that audio information should include
    /// segment-level timestamp information.
    /// </summary>
    Segment = 2,
}