namespace OpenAI.Chat;

/// <summary>
/// Represents computed token consumption statistics for a chat completion request.
/// </summary>
[CodeGenModel("CompletionUsage")]
public partial class ChatTokenUsage
{
    // CUSTOM: Renamed.
    /// <summary> Number of tokens in the generated completion. </summary>
    [CodeGenMember("CompletionTokens")]
    public int OutputTokens { get; }

    // CUSOTM: Renamed.
    /// <summary> Number of tokens in the prompt. </summary>
    [CodeGenMember("PromptTokens")]
    public int InputTokens { get; }
}