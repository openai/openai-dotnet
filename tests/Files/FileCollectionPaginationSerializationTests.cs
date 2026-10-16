using NUnit.Framework;
using OpenAI.Files;
using System;
using System.ClientModel.Primitives;
using System.Linq;
using System.Text.Json;

namespace OpenAI.Tests.Files;

[Parallelizable(ParallelScope.All)]
[Category("Smoke")]
[Category("Files")]
public class FileCollectionPaginationSerializationTests
{
    [TestCase(false, "J")]
    [TestCase(true, "J")]
    [TestCase(false, "W")]
    [TestCase(true, "W")]
    public void PaginationSurvivesRepeatedRoundtrips(bool hasMore, string format)
    {
        BinaryData original = BinaryData.FromString($$"""
        {
          "object": "list",
          "data": [{"id":"file-first"}, {"id":"file-last"}],
          "first_id": "file-first",
          "last_id": "file-last",
          "has_more": {{hasMore.ToString().ToLowerInvariant()}},
          "synthetic_extension": {"retained":true}
        }
        """);
        OpenAIFileCollection collection = ModelReaderWriter.Read<OpenAIFileCollection>(original);
        for (int iteration = 0; iteration < 2; iteration++)
        {
            BinaryData serialized = ModelReaderWriter.Write(collection, new ModelReaderWriterOptions(format));
            using JsonDocument document = JsonDocument.Parse(serialized);
            JsonElement value = document.RootElement;
            Assert.That(value.GetProperty("first_id").GetString(), Is.EqualTo("file-first"));
            Assert.That(value.GetProperty("last_id").GetString(), Is.EqualTo("file-last"));
            Assert.That(value.GetProperty("has_more").GetBoolean(), Is.EqualTo(hasMore));
            Assert.That(value.GetProperty("synthetic_extension").GetProperty("retained").GetBoolean(), Is.True);
            Assert.That(value.EnumerateObject().Select(property => property.Name).Distinct().Count(), Is.EqualTo(value.EnumerateObject().Count()));
            collection = ModelReaderWriter.Read<OpenAIFileCollection>(serialized);
            Assert.That(collection.Select(file => file.Id), Is.EqualTo(new[] { "file-first", "file-last" }));
        }
    }

    [TestCase("{\"data\":[],\"object\":\"list\"}")]
    [TestCase("{\"data\":[],\"object\":\"list\",\"first_id\":null,\"last_id\":null,\"has_more\":false}")]
    public void EmptyPagesKeepTheirDefaultPagination(string json)
    {
        OpenAIFileCollection collection = ModelReaderWriter.Read<OpenAIFileCollection>(BinaryData.FromString(json));
        using JsonDocument document = JsonDocument.Parse(ModelReaderWriter.Write(collection));
        Assert.That(document.RootElement.GetProperty("first_id").ValueKind, Is.EqualTo(JsonValueKind.Null));
        Assert.That(document.RootElement.GetProperty("last_id").ValueKind, Is.EqualTo(JsonValueKind.Null));
        Assert.That(document.RootElement.GetProperty("has_more").GetBoolean(), Is.False);
        Assert.That(document.RootElement.GetProperty("data").GetArrayLength(), Is.Zero);
    }

    [Test]
    public void FactoryCollectionStillSerializesWithDefaultPagination()
    {
        OpenAIFileCollection collection = OpenAIFilesModelFactory.OpenAIFileCollection();
        BinaryData serialized = ModelReaderWriter.Write(collection);
        OpenAIFileCollection roundtrip = ModelReaderWriter.Read<OpenAIFileCollection>(serialized);
        Assert.That(roundtrip, Is.Empty);
        using JsonDocument document = JsonDocument.Parse(serialized);
        Assert.That(document.RootElement.GetProperty("has_more").GetBoolean(), Is.False);
    }
}
