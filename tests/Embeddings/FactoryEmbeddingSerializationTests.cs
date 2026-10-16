using NUnit.Framework;
using OpenAI.Embeddings;
using System;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace OpenAI.Tests.Embeddings;

[Parallelizable(ParallelScope.All)]
[Category("Smoke")]
public class FactoryEmbeddingSerializationTests
{
    public static IEnumerable<TestCaseData> Vectors()
    {
        yield return new TestCaseData(Array.Empty<float>());
        yield return new TestCaseData(new float[] { 1f, -2.5f, 3f });
        yield return new TestCaseData(new float[] { -0f, float.Epsilon, float.MaxValue });
        yield return new TestCaseData(new float[] { float.NaN, float.PositiveInfinity, float.NegativeInfinity });
    }

    [TestCaseSource(nameof(Vectors))]
    public void FactoryVectorRoundtripsThroughModelReaderWriter(float[] vector)
    {
        OpenAIEmbedding original = OpenAIEmbeddingsModelFactory.OpenAIEmbedding(index: 7, vector: vector);
        BinaryData data = ModelReaderWriter.Write(original);
        using JsonDocument json = JsonDocument.Parse(data);
        Assert.That(json.RootElement.GetProperty("index").GetInt32(), Is.EqualTo(7));
        OpenAIEmbedding restored = ModelReaderWriter.Read<OpenAIEmbedding>(data);
        Assert.That(restored.Index, Is.EqualTo(7));
        AssertBitsEqual(restored.ToFloats().ToArray(), vector);
        AssertBitsEqual(original.ToFloats().ToArray(), vector);
    }

    [TestCaseSource(nameof(Vectors))]
    public void CollectionOfFactoryVectorsRoundtrips(float[] vector)
    {
        OpenAIEmbedding[] items = [
            OpenAIEmbeddingsModelFactory.OpenAIEmbedding(index: 0, vector: vector),
            OpenAIEmbeddingsModelFactory.OpenAIEmbedding(index: 1, vector: new float[] { 4f, 5f }),
        ];
        OpenAIEmbeddingCollection original = OpenAIEmbeddingsModelFactory.OpenAIEmbeddingCollection(
            items: items, model: "synthetic-model",
            usage: OpenAIEmbeddingsModelFactory.EmbeddingTokenUsage(inputTokenCount: 2, totalTokenCount: 2));
        OpenAIEmbeddingCollection restored = ModelReaderWriter.Read<OpenAIEmbeddingCollection>(ModelReaderWriter.Write(original));
        Assert.That(restored.Count, Is.EqualTo(2));
        Assert.That(restored.Model, Is.EqualTo("synthetic-model"));
        Assert.That(restored.Usage.InputTokenCount, Is.EqualTo(2));
        AssertBitsEqual(restored[0].ToFloats().ToArray(), vector);
        AssertBitsEqual(restored[1].ToFloats().ToArray(), [4f, 5f]);
    }

    [TestCase("[]")]
    [TestCase("\"\"")]
    [TestCase("[1,-2.5]")]
    [TestCase("\"AACAPwAAIMA=\"")]
    public void ExistingJsonAndBase64VectorsStillRoundtrip(string payload)
    {
        BinaryData data = BinaryData.FromString("{\"index\":3,\"object\":\"embedding\",\"embedding\":" + payload + "}");
        OpenAIEmbedding first = ModelReaderWriter.Read<OpenAIEmbedding>(data);
        OpenAIEmbedding second = ModelReaderWriter.Read<OpenAIEmbedding>(ModelReaderWriter.Write(first));
        Assert.That(second.Index, Is.EqualTo(3));
        AssertBitsEqual(second.ToFloats().ToArray(), first.ToFloats().ToArray());
    }

    [TestCase("\"not base64\"")]
    [TestCase("\"AQ==\"")]
    [TestCase("[\"text\"]")]
    public void MalformedVectorsAreStillRejected(string payload)
    {
        BinaryData data = BinaryData.FromString("{\"index\":0,\"embedding\":" + payload + "}");
        Assert.Throws<FormatException>(() => ModelReaderWriter.Read<OpenAIEmbedding>(data));
    }

    private static void AssertBitsEqual(float[] actual, float[] expected)
    {
        static int Bits(float value) => BitConverter.ToInt32(BitConverter.GetBytes(value), 0);
        Assert.That(actual.Select(Bits), Is.EqualTo(expected.Select(Bits)));
    }
}
