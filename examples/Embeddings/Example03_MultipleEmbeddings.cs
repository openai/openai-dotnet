using NUnit.Framework;
using OpenAI.Embeddings;
using System;
using System.Collections.Generic;

namespace OpenAI.Examples;

public partial class EmbeddingExamples
{
    [Test]
    public void Example03_MultipleEmbeddings()
    {
        EmbeddingClient client = new("text-embedding-3-small", Environment.GetEnvironmentVariable("OPENAI_API_KEY"));

        string category = "Luxury";
        string description = "Best hotel in town if you like luxury hotels. They have an amazing infinity pool, a spa,"
            + " and a really helpful concierge. The location is perfect -- right downtown, close to all the tourist"
            + " attractions. We highly recommend this hotel.";
        List<string> inputs = [category, description];

        OpenAIEmbeddingCollection collection = client.GenerateEmbeddings(inputs);

        foreach (OpenAIEmbedding embedding in collection)
        {
            ReadOnlyMemory<float> vector = embedding.ToFloats();

            Console.WriteLine($"Dimension: {vector.Length}");
            Console.WriteLine($"Floats: ");
            for (int i = 0; i < vector.Length; i++)
            {
                Console.WriteLine($"  [{i,4}] = {vector.Span[i]}");
            }

            Console.WriteLine();
        }
    }
}
