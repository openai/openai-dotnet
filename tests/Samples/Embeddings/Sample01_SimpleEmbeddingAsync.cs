using NUnit.Framework;
using OpenAI.Embeddings;
using System;
using System.Threading.Tasks;

namespace OpenAI.Samples
{
    public partial class EmbeddingSamples
    {
        [Test]
        [Ignore("Compilation validation only")]
        public async Task Sample01_SimpleEmbeddingAsync()
        {
            EmbeddingClient client = new("text-embedding-3-small", Environment.GetEnvironmentVariable("OPENAI_API_KEY"));

            string description =
                "Best hotel in town if you like luxury hotels. They have an amazing infinity pool, a spa,"
                + " and a really helpful concierge. The location is perfect -- right downtown, close to all"
                + " the tourist attractions. We highly recommend this hotel.";

            Embedding embedding = await client.GenerateEmbeddingAsync(description);
            ReadOnlyMemory<float> vector = embedding.Vector;

            Console.WriteLine($"Dimension: {vector.Length}");
            Console.WriteLine($"Floats: ");
            for (int i = 0; i < vector.Length; i++)
            {
                Console.WriteLine($"  [{i}] = {vector.Span[i]}");
            }
        }
    }
}
