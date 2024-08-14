using System.Collections.Generic;
using System.Linq;

namespace OpenAI.Embeddings;

/// <summary> Model factory for models. </summary>
public static partial class OpenAIEmbeddingsModelFactory
{
    /// <summary> Initializes a new instance of <see cref="OpenAI.Embeddings.Embedding"/>. </summary>
    /// <returns> A new <see cref="OpenAI.Embeddings.Embeddings"/> instance for mocking. </returns>
    public static Embedding Embedding(int index = default, IEnumerable<float> vector = null)
    {
        vector ??= new List<float>();

        return new Embedding(
            index,
            vector.ToArray());
    }

    /// <summary> Initializes a new instance of <see cref="OpenAI.Embeddings.EmbeddingCollection"/>. </summary>
    /// <returns> A new <see cref="OpenAI.Embeddings.EmbeddingCollection"/> instance for mocking. </returns>
    public static EmbeddingCollection EmbeddingCollection(IEnumerable<Embedding> items = null, string model = null, EmbeddingTokenUsage usage = null)
    {
        items ??= new List<Embedding>();

        return new EmbeddingCollection(
            items.ToList(),
            model,
            InternalCreateEmbeddingResponseObject.List,
            usage,
            serializedAdditionalRawData: null);
    }

    /// <summary> Initializes a new instance of <see cref="OpenAI.Embeddings.EmbeddingTokenUsage"/>. </summary>
    /// <returns> A new <see cref="OpenAI.Embeddings.EmbeddingTokenUsage"/> instance for mocking. </returns>
    public static EmbeddingTokenUsage EmbeddingTokenUsage(int inputTokens = default, int totalTokens = default)
    {
        return new EmbeddingTokenUsage(
            inputTokens,
            totalTokens,
            serializedAdditionalRawData: null);
    }
}
