using NUnit.Framework;
using OpenAI.Embeddings;
using System;
using System.ClientModel;
using System.Text.Json;

namespace OpenAI.Examples;

public partial class EmbeddingExamples
{
    [Test]
    public void Example04_SimpleEmbeddingProtocol()
    {
        EmbeddingClient client = new("text-embedding-3-small", Environment.GetEnvironmentVariable("OPENAI_API_KEY"));

        string description = "Best hotel in town if you like luxury hotels. They have an amazing infinity pool, a spa,"
            + " and a really helpful concierge. The location is perfect -- right downtown, close to all the tourist"
            + " attractions. We highly recommend this hotel.";

        BinaryData input = BinaryData.FromObjectAsJson(new
        {
            model = "text-embedding-3-small",
            input = description,
            encoding_format = "float"
        });

        using BinaryContent content = BinaryContent.Create(input);
        ClientResult result = client.GenerateEmbeddings(content);
        BinaryData output = result.GetRawResponse().Content;

        using JsonDocument outputAsJson = JsonDocument.Parse(output.ToString());
        JsonElement vector = outputAsJson.RootElement
            .GetProperty("data"u8)[0]
            .GetProperty("embedding"u8);

        Console.WriteLine($"Dimension: {vector.GetArrayLength()}");
        Console.WriteLine($"Floats: ");
        int i = 0;
        foreach (JsonElement element in vector.EnumerateArray())
        {
            Console.WriteLine($"  [{i++,4}] = {element.GetDouble()}");
        }
    }
}
