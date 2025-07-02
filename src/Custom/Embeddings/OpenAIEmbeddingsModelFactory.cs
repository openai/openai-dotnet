using System.Collections.Generic;
using System.Linq;

namespace OpenAI.Embeddings;

/// <summary> Model factory for models. </summary>
public static partial class OpenAIEmbeddingsModelFactory
{
    /// <summary> Initializes a new instance of <see cref="OpenAI.Embeddings.OpenAIEmbedding"/>. </summary>
    /// <returns> A new <see cref="OpenAI.Embeddings.Embeddings"/> instance for mocking. </returns>
    public static OpenAIEmbedding OpenAIEmbedding(int index = default, IEnumerable<float> vector = null)
    {
        vector ??= new List<float>();

        return new OpenAIEmbedding(
            index,
            vector.ToArray());
    }

    /// <summary> Initializes a new instance of <see cref="OpenAI.Embeddings.OpenAIEmbeddingCollection"/>. </summary>
    /// <returns> A new <see cref="OpenAI.Embeddings.OpenAIEmbeddingCollection"/> instance for mocking. </returns>
    public static OpenAIEmbeddingCollection OpenAIEmbeddingCollection(IEnumerable<OpenAIEmbedding> items = null, string model = null, EmbeddingTokenUsage usage = null)
    {
        items ??= new List<OpenAIEmbedding>();

        return new OpenAIEmbeddingCollection(
            items.ToList(),
            model,
            "list",
            usage,
            serializedAdditionalRawData: null);
    }

    /// <summary> Initializes a new instance of <see cref="OpenAI.Embeddings.EmbeddingTokenUsage"/>. </summary>
    /// <returns> A new <see cref="OpenAI.Embeddings.EmbeddingTokenUsage"/> instance for mocking. </returns>
    public static EmbeddingTokenUsage EmbeddingTokenUsage(int inputTokenCount = default, int totalTokenCount = default)
    {
        return new EmbeddingTokenUsage(
            inputTokenCount,
            totalTokenCount,
            additionalBinaryDataProperties: null);
    }
}
