using System.Diagnostics.CodeAnalysis;

namespace OpenAI.VectorStores;

[CodeGenType("DotNetCombinedChunkingStrategyParam")]
public abstract partial class FileChunkingStrategy
{
    /// <summary>
    /// Gets a value representing the default, automatic selection for a file chunking strategy.
    /// </summary>
    /// <remarks>
    /// This value is only valid on vector store requests. <see cref="VectorStoreFileAssociation"/> response instances
    /// will report the concrete chunking strategy applied after automatic selection.
    /// </remarks>
    public static FileChunkingStrategy Auto => _autoValue ??= new();

    /// <summary>
    /// Gets a value representing the <c>other</c>, unknown strategy type.
    /// </summary>
    /// <remarks>
    /// This value is present on responses when no chunking strategy could be found. This is typically only true for
    /// vector stores created earlier than file chunking strategy availability.
    /// </remarks>
    public static FileChunkingStrategy Unknown => _otherValue ??= new();

    /// <inheritdoc cref="StaticFileChunkingStrategy.StaticFileChunkingStrategy(int,int)"/>
    public static FileChunkingStrategy CreateStaticStrategy(
        int maxTokensPerChunk,
        int overlappingTokenCount)
    {
        return new StaticFileChunkingStrategy(
                maxTokensPerChunk,
                overlappingTokenCount);
    }

    private static InternalDotNetCombinedAutoChunkingStrategyParam _autoValue;
    private static InternalDotNetCombinedOtherChunkingStrategyParam _otherValue;
}
