namespace OpenAI.Embeddings;

[CodeGenModel("CreateEmbeddingResponseUsage")]
public partial class EmbeddingTokenUsage
{
    // CUSTOM: Renamed.
    /// <summary> The number of tokens used by the input prompts. </summary>
    [CodeGenMember("PromptTokens")]
    public int InputTokenCount { get; }

    // CUSTOM: Renamed.
    [CodeGenMember("TotalTokens")]
    public int TotalTokenCount { get; }
}