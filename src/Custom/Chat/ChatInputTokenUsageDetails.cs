namespace OpenAI.Chat;

/// <summary>
/// A collection of additional information about the value reported in <see cref="ChatTokenUsage.InputTokenCount"/>.
/// </summary>
[CodeGenModel("CompletionUsagePromptTokensDetails")]
public partial class ChatInputTokenUsageDetails
{
    // CUSTOM: Renamed.
    [CodeGenMember("AudioTokens")]
    public int? AudioTokenCount { get; }

    // CUSTOM: Renamed.
    [CodeGenMember("CachedTokens")]
    public int? CachedTokenCount { get; }
}