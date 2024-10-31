using NUnit.Framework;
using OpenAI.Moderations;
using OpenAI.Tests.Utility;
using System.ClientModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.ClientModel.Primitives;
using System.Text.Json;

namespace OpenAI.Tests.Moderations;

[TestFixture(true)]
[TestFixture(false)]
[Parallelizable(ParallelScope.All)]
[Category("Moderations")]
[Category("Smoke")]
public partial class ModerationSmokeTests : SyncAsyncTestBase
{
    public ModerationSmokeTests(bool isAsync) : base(isAsync)
    {
    }

    [Test]
    public async Task ClassifySingleInputSmokeTest()
    {
        BinaryData mockRequest = BinaryData.FromString($$"""
        {
            "input": "I am killing all my houseplants!"
        }
        """);
        BinaryData mockResponse = BinaryData.FromString($$"""
        {
            "results": [
                {   
                    "flagged": true,
                    "categories": {
                        "violence": true
                    },
                    "category_scores": {
                        "violence": 0.5
                    }
                }
            ]
        }
        """);
        MockPipelineTransport mockTransport = new(mockRequest, mockResponse);

        OpenAIClientOptions options = new()
        {
            Transport = mockTransport
        };

        ModerationClient client = new ModerationClient("model", new ApiKeyCredential("sk-not-a-real-key"), options);
        ModerationResult moderation = IsAsync
            ? await client.ClassifyTextAsync("Mock me!")
            : client.ClassifyText("Mock me!");

        Assert.That(moderation, Is.Not.Null);
        Assert.That(moderation.Flagged, Is.True);
        Assert.That(moderation.Violence.Flagged, Is.True);
        Assert.That(moderation.Violence.Score, Is.EqualTo(0.5f));
    }

    [Test]
    public async Task ClassifyMultipleInputsSmokeTest()
    {
        BinaryData mockRequest = BinaryData.FromString($$"""
        {
            "input": [
                "I forgot to water my houseplants!",
                "I am killing all my houseplants!"
            ]
        }
        """);
        BinaryData mockResponse = BinaryData.FromString("""
        {
            "results": [
                {   
                    "flagged": false,
                    "categories": {
                        "violence": false
                    },
                    "category_scores": {
                        "violence": 0.0
                     }
                },
                {   
                    "flagged": true,
                    "categories": {
                        "violence": true
                    },
                    "category_scores": {
                        "violence": 0.5
                    },
                    "category_applied_input_types": {
                        "violence": ["text"]
                    }
                },
                {
                    "flagged": true,
                    "categories": {
                        "illicit": true
                    },
                    "category_scores": {
                        "illicit": 0.42
                    },
                    "category_applied_input_types": {
                        "illicit": ["image","potato"]
                    }
                }
            ]
        }
        """);

        MockPipelineTransport mockTransport = new(mockRequest, mockResponse);

        OpenAIClientOptions options = new()
        {
            Transport = mockTransport
        };

        ModerationClient client = new ModerationClient("model", new ApiKeyCredential("sk-not-a-real-key"), options);
        ModerationResultCollection moderations = IsAsync
            ? await client.ClassifyTextAsync(new List<string> { "Mock me 1!", "Mock me 2!" })
            : client.ClassifyText(new List<string> { "Mock me 1!", "Mock me 2!" });

        Assert.That(moderations, Is.Not.Null);
        Assert.That(moderations.Count, Is.EqualTo(3));

        Assert.That(moderations[0], Is.Not.Null);
        Assert.That(moderations[0].Flagged, Is.False);
        Assert.That(moderations[0].Violence.ApplicableInputKinds.HasFlag(ModerationApplicableInputKinds.Text), Is.False);

        Assert.That(moderations[1], Is.Not.Null);
        Assert.That(moderations[1].Flagged, Is.True);
        Assert.That(moderations[1].Violence.Flagged, Is.True);
        Assert.That(moderations[1].Violence.Score, Is.EqualTo(0.5f));
        Assert.That(moderations[1].Violence.ApplicableInputKinds.HasFlag(ModerationApplicableInputKinds.Text), Is.True);
        Assert.That(moderations[1].Violence.ApplicableInputKinds.HasFlag(ModerationApplicableInputKinds.Image), Is.False);
        Assert.That(moderations[1].Violence.ApplicableInputKinds.HasFlag(ModerationApplicableInputKinds.Other), Is.False);

        Assert.That(moderations[2], Is.Not.Null);
        Assert.That(moderations[2].Flagged, Is.True);
        Assert.That(moderations[2].Illicit.Flagged, Is.True);
        Assert.That(moderations[2].Illicit.Score, Is.EqualTo(0.42f));
        Assert.That(moderations[2].Illicit.ApplicableInputKinds.HasFlag(ModerationApplicableInputKinds.Text), Is.False);
        Assert.That(moderations[2].Illicit.ApplicableInputKinds.HasFlag(ModerationApplicableInputKinds.Image), Is.True);
        Assert.That(moderations[2].Illicit.ApplicableInputKinds.HasFlag(ModerationApplicableInputKinds.Other), Is.True);
    }

    [Test]
    public void SerializeModerationResult()
    {
        BinaryData data = BinaryData.FromString($$"""
        {
            "flagged": true,
            "categories": {
                "violence": true
            },
            "category_scores": {
                "violence": 0.5
            }
        }
        """);

        // Deserialize the raw JSON and then serialize it back to confirm nothing was lost.
        ModerationResult moderationResult = ModelReaderWriter.Read<ModerationResult>(data);
        Assert.That(moderationResult.Flagged, Is.EqualTo(true));
        Assert.That(moderationResult.Violence.Flagged, Is.EqualTo(true));
        Assert.That(moderationResult.Violence.Score, Is.EqualTo(0.5f));

        BinaryData serializedPart = ModelReaderWriter.Write(moderationResult);
        using JsonDocument partAsJson = JsonDocument.Parse(serializedPart);
        Assert.That(partAsJson.RootElement, Is.Not.Null);
        Assert.That(partAsJson.RootElement.ValueKind, Is.EqualTo(JsonValueKind.Object));

        Assert.That(partAsJson.RootElement.TryGetProperty("flagged", out JsonElement flagged), Is.True);
        Assert.That(flagged, Is.Not.Null);
        Assert.That(flagged.ValueKind, Is.EqualTo(JsonValueKind.True));

        // Access and assert the "categories" object and its "violence" value
        Assert.That(partAsJson.RootElement.TryGetProperty("categories", out JsonElement categories), Is.True);
        Assert.That(categories, Is.Not.Null);
        Assert.That(categories.ValueKind, Is.EqualTo(JsonValueKind.Object));

        Assert.That(categories.TryGetProperty("violence", out JsonElement violenceCategory), Is.True);
        Assert.That(violenceCategory, Is.Not.Null);
        Assert.That(violenceCategory.ValueKind, Is.EqualTo(JsonValueKind.True));

        // Access and assert the "category_scores" object and its "violence" value
        Assert.That(partAsJson.RootElement.TryGetProperty("category_scores", out JsonElement categoryScores), Is.True);
        Assert.That(categoryScores, Is.Not.Null);
        Assert.That(categoryScores.ValueKind, Is.EqualTo(JsonValueKind.Object));

        Assert.That(categoryScores.TryGetProperty("violence", out JsonElement violenceScore), Is.True);
        Assert.That(violenceScore, Is.Not.Null);
        Assert.That(violenceScore.ValueKind, Is.EqualTo(JsonValueKind.Number));
        Assert.That(violenceScore.GetSingle(), Is.EqualTo(0.5f));
    }

    [Test]
    public void SerializeModerationResultCollection()
    {
        BinaryData data = BinaryData.FromString($$"""
        {
            "results": [
                {   
                    "flagged": false,
                    "categories": {
                        "violence": false
                    },
                    "category_scores": {
                        "violence": 0.0
                     }
                },
                {   
                    "flagged": true,
                    "categories": {
                        "violence": true
                    },
                    "category_scores": {
                        "violence": 0.5
                    }
                }
            ]
        }
        """);

        // Deserialize the raw JSON and then serialize it back to confirm nothing was lost.
        ModerationResultCollection moderations = ModelReaderWriter.Read<ModerationResultCollection>(data);
        Assert.That(moderations[0], Is.Not.Null);
        Assert.That(moderations[0].Flagged, Is.False);

        Assert.That(moderations[1], Is.Not.Null);
        Assert.That(moderations[1].Flagged, Is.True);
        Assert.That(moderations[1].Violence.Flagged, Is.True);
        Assert.That(moderations[1].Violence.Score, Is.EqualTo(0.5f));

        BinaryData serializedPart = ModelReaderWriter.Write(moderations);
        using JsonDocument partAsJson = JsonDocument.Parse(serializedPart);
        Assert.That(partAsJson.RootElement, Is.Not.Null);
        Assert.That(partAsJson.RootElement.ValueKind, Is.EqualTo(JsonValueKind.Object));

        // Access the "results" array
        Assert.That(partAsJson.RootElement.TryGetProperty("results", out JsonElement results), Is.True);
        Assert.That(results, Is.Not.Null);
        Assert.That(results.ValueKind, Is.EqualTo(JsonValueKind.Array));

        // Access the first object in the results array and verify its properties
        JsonElement firstResult = results[0];
        Assert.That(firstResult.TryGetProperty("flagged", out JsonElement flagged1), Is.True);
        Assert.That(flagged1, Is.Not.Null);
        Assert.That(flagged1.ValueKind, Is.EqualTo(JsonValueKind.False));

        Assert.That(firstResult.TryGetProperty("categories", out JsonElement categories1), Is.True);
        Assert.That(categories1, Is.Not.Null);
        Assert.That(categories1.ValueKind, Is.EqualTo(JsonValueKind.Object));

        Assert.That(categories1.TryGetProperty("violence", out JsonElement violenceCategory1), Is.True);
        Assert.That(violenceCategory1, Is.Not.Null);
        Assert.That(violenceCategory1.ValueKind, Is.EqualTo(JsonValueKind.False)); // Assert violence is false

        Assert.That(firstResult.TryGetProperty("category_scores", out JsonElement categoryScores1), Is.True);
        Assert.That(categoryScores1, Is.Not.Null);
        Assert.That(categoryScores1.ValueKind, Is.EqualTo(JsonValueKind.Object));

        Assert.That(categoryScores1.TryGetProperty("violence", out JsonElement violenceScore1), Is.True);
        Assert.That(violenceScore1, Is.Not.Null);
        Assert.That(violenceScore1.ValueKind, Is.EqualTo(JsonValueKind.Number));
        Assert.That(violenceScore1.GetDouble(), Is.EqualTo(0.0)); // Assert violence score is 0.0

        // Access the second object in the results array and verify its properties
        JsonElement secondResult = results[1];
        Assert.That(secondResult.TryGetProperty("flagged", out JsonElement flagged2), Is.True);
        Assert.That(flagged2, Is.Not.Null);
        Assert.That(flagged2.ValueKind, Is.EqualTo(JsonValueKind.True));

        Assert.That(secondResult.TryGetProperty("categories", out JsonElement categories2), Is.True);
        Assert.That(categories2, Is.Not.Null);
        Assert.That(categories2.ValueKind, Is.EqualTo(JsonValueKind.Object));

        Assert.That(categories2.TryGetProperty("violence", out JsonElement violenceCategory2), Is.True);
        Assert.That(violenceCategory2, Is.Not.Null);
        Assert.That(violenceCategory2.ValueKind, Is.EqualTo(JsonValueKind.True)); // Assert violence is true

        Assert.That(secondResult.TryGetProperty("category_scores", out JsonElement categoryScores2), Is.True);
        Assert.That(categoryScores2, Is.Not.Null);
        Assert.That(categoryScores2.ValueKind, Is.EqualTo(JsonValueKind.Object));

        Assert.That(categoryScores2.TryGetProperty("violence", out JsonElement violenceScore2), Is.True);
        Assert.That(violenceScore2, Is.Not.Null);
        Assert.That(violenceScore2.ValueKind, Is.EqualTo(JsonValueKind.Number));
        Assert.That(violenceScore2.GetDouble(), Is.EqualTo(0.5)); // Assert violence score is 0.5
    }
}
