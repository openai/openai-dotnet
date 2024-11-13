namespace OpenAI.Chat;

/// <summary>
/// A collection of additional information about the value reported in <see cref="ChatTokenUsage.OutputTokenCount"/>.
/// </summary>
[CodeGenModel("CompletionUsageCompletionTokensDetails")]
public partial class ChatOutputTokenUsageDetails
{
    // CUSTOM: Renamed.
    /// <summary>
    /// The number of internally-consumed output tokens used for integrated reasoning with a supported model.
    /// <summary>
    /// <remarks>
    /// <para>
    /// This is currently only applicable to <c>o1</c> models.
    /// </para>
    /// <para>
    /// <see cref="ReasoningTokenCount"/> is part of the total <see cref="ChatTokenUsage.OutputTokenCount"/> and will
    /// thus always be less than or equal to this parent number.
    /// </para>
    /// </summary>
    [CodeGenMember("ReasoningTokens")]
    public int ReasoningTokenCount { get; }

    [CodeGenMember("AudioTokens")]
    public int? AudioTokenCount { get; }
}