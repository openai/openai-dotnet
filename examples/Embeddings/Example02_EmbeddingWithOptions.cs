using NUnit.Framework;
using OpenAI.Embeddings;
using System;

namespace OpenAI.Examples;

public partial class EmbeddingExamples
{
    [Test]
    public void Example02_EmbeddingWithOptions()
    {
        EmbeddingClient client = new("text-embedding-3-small", Environment.GetEnvironmentVariable("OPENAI_API_KEY"));

        string description = "Best hotel in town if you like luxury hotels. They have an amazing infinity pool, a spa,"
            + " and a really helpful concierge. The location is perfect -- right downtown, close to all the tourist"
            + " attractions. We highly recommend this hotel.";

        EmbeddingGenerationOptions options = new() { Dimensions = 512 };

        OpenAIEmbedding embedding = client.GenerateEmbedding(description, options);
        ReadOnlyMemory<float> vector = embedding.ToFloats();

        Console.WriteLine($"Dimension: {vector.Length}");
        Console.WriteLine($"Floats: ");
        for (int i = 0; i < vector.Length; i++)
        {
            Console.WriteLine($"  [{i,3}] = {vector.Span[i]}");
        }
    }
}
