namespace OpenAI.Chat;

/// <summary>
/// Represents computed token consumption statistics for a chat completion request.
/// </summary>
[CodeGenModel("CompletionUsage")]
public partial class ChatTokenUsage
{
    // CUSTOM: Renamed.
    /// <summary>
    /// The combined number of output tokens in the generated completion, as consumed by the model.
    /// </summary>
    /// <remarks>
    /// When using a model that supports <see cref="ReasoningTokens"/> such as <c>o1-mini</c>, this value represents
    /// the sum of those reasoning tokens and conventional, displayed output tokens.
    /// </remarks>
    [CodeGenMember("CompletionTokens")]
    public int OutputTokenCount { get; }

    // CUSTOM: Renamed.
    /// <summary>
    /// The number of tokens in the request message input, spanning all message content items.
    /// </summary>
    [CodeGenMember("PromptTokens")]
    public int InputTokenCount { get; }

    // CUSTOM: Renamed.
    /// <summary>
    /// The total number of combined input (prompt) and output (completion) tokens used by a chat completion operation.
    /// </summary>
    [CodeGenMember("TotalTokens")]
    public int TotalTokenCount { get; }

    // CUSTOM: Renamed.
    /// <summary>
    /// Additional information about the tokens represented by <see cref="OutputTokenCount"/>, including the count of
    /// consumed reasoning tokens by supported models.
    /// </summary>
    [CodeGenMember("CompletionTokensDetails")]
    public ChatOutputTokenUsageDetails OutputTokenDetails { get; }

    // CUSTOM: Renamed.
    /// <summary>
    /// Additional information about the tokens represented by <see cref="InputTokenCount"/>, including the count of
    /// audio tokens, if applicable to the model.
    /// </summary>
    [CodeGenMember("PromptTokensDetails")]
    public ChatInputTokenUsageDetails InputTokenDetails { get; }
}