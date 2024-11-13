using System;
using System.Collections.Generic;

namespace OpenAI.Chat;

/// <summary>
/// Represents a single item of log probability information as requested via
/// <see cref="ChatCompletionOptions.IncludeLogProbabilities"/> and
/// <see cref="ChatCompletionOptions.TopLogProbabilityCount"/>.
/// </summary>
[CodeGenModel("ChatCompletionTokenLogprobTopLogprob")]
public partial class ChatTokenTopLogProbabilityDetails
{
    // CUSTOM: Renamed.
    /// <summary> The log probability of this token, if it is within the top 20 most likely tokens. Otherwise, the value `-9999.0` is used to signify that the token is very unlikely. </summary>
    [CodeGenMember("Logprob")]
    public float LogProbability { get; }

    // CUSTOM:
    // - Renamed.
    // - Changed type from IReadOnlyList<int> to ReadOnlyMemory<byte>?.
    /// <summary>
    /// A list of integers representing the UTF-8 bytes representation of the token. Useful in instances where
    /// characters are represented by multiple tokens and their byte representations must be combined to generate
    /// the correct text representation. Can be null if there is no bytes representation for the token.
    /// </summary>
    [CodeGenMember("Bytes")]
    public ReadOnlyMemory<byte>? Utf8Bytes { get; }
}