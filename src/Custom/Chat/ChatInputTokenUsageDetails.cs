namespace OpenAI.Chat;

/// <summary> A breakdown of the number of tokens used in the input as reported in <see cref="ChatTokenUsage.InputTokenCount"/>. </summary>
[CodeGenType("CompletionUsagePromptTokensDetails")]
public partial class ChatInputTokenUsageDetails
{
    // CUSTOM: Renamed.
    /// <summary> The number of audio tokens in the input. </summary>
    [CodeGenMember("AudioTokens")]
    public int AudioTokenCount { get; }

    // CUSTOM: Renamed.
    /// <summary> The number of cached tokens in the input. </summary>
    [CodeGenMember("CachedTokens")]
    public int CachedTokenCount { get; }
}