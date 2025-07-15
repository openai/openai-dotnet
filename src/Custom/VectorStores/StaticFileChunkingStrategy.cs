using System.Diagnostics.CodeAnalysis;

namespace OpenAI.VectorStores;

[CodeGenType("DotNetCombinedStaticChunkingStrategyParam")]
public partial class StaticFileChunkingStrategy : FileChunkingStrategy
{
    [CodeGenMember("Static")]
    private InternalStaticChunkingStrategy _internalDetails;


    /// <summary>
    /// Creates a new instance of <see cref="StaticFileChunkingStrategy"/>, which allows for direct specification of
    /// file chunk size and chunk overlap windows.
    /// </summary>
    /// <param name="maxTokensPerChunk"></param>
    /// <param name="overlappingTokenCount"></param>
    public StaticFileChunkingStrategy(int maxTokensPerChunk, int overlappingTokenCount)
        : this(new InternalStaticChunkingStrategy(maxTokensPerChunk, overlappingTokenCount))
    {
    }

    /// <summary>
    /// The maximum size of a file chunk, in tokens.
    /// </summary>
    /// <remarks>
    /// If not otherwise specified, a default of <c>800</c> will be used.
    /// </remarks>
    public int MaxTokensPerChunk => _internalDetails.MaxChunkSizeTokens;

    /// <summary>
    /// The number of shared, overlapping tokens allowed between chunks.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This value may not exceed half of <see cref="MaxTokensPerChunk"/>.
    /// </para>
    /// If not otherwise specified, a default of <c>400</c> will be used.
    /// </remarks>
    public int OverlappingTokenCount => _internalDetails.ChunkOverlapTokens;
}
