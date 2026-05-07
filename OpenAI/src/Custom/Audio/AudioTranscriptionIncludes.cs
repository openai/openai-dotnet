using System;
using System.Diagnostics.CodeAnalysis;

namespace OpenAI.Audio;

// CUSTOM: Added Experimental attribute.
/// <summary>
/// Specifies the timestamp granularities to populate for a transcription.
/// </summary>
[Experimental("OPENAI001")]
[Flags]
public enum AudioTranscriptionIncludes
{
    /// <summary>
    /// The default value that, when equivalent to a request's flags, specifies no additional include[] values to provide.
    /// </summary>
    Default = 0,

    /// <summary>
    /// The value that, when present in the request's flags, specifies that audio information should include token log
    /// probabilities.
    /// </summary>
    Logprobs = 1,
}