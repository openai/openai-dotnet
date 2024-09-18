using System;
using System.ClientModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using OpenAI.Moderations;
using OpenAI.Tests.Utility;

namespace OpenAI.Tests.Moderations;

[TestFixture(true)]
[TestFixture(false)]
[Parallelizable(ParallelScope.All)]
[Category("Smoke")]
public partial class ModerationsMockTests : SyncAsyncTestBase
{
    private static readonly ApiKeyCredential s_fakeCredential = new ApiKeyCredential("key");

    public ModerationsMockTests(bool isAsync)
        : base(isAsync)
    {
    }

    [Test]
    public async Task ClassifyTextInputDeserializesHateCategory()
    {
        OpenAIClientOptions clientOptions = GetClientOptionsWithMockResponse(200, """
        {
            "results": [
                {
                    "categories": {
                        "hate": true
                    },
                    "category_scores": {
                        "hate": 0.895
                    }
                }
            ]
        }
        """);
        ModerationClient client = new ModerationClient("model", s_fakeCredential, clientOptions);

        ModerationResult moderation = IsAsync
            ? await client.ClassifyTextAsync("input")
            : client.ClassifyText("input");

        AssertModerationCategories(moderation, hate: (true, 0.895f));
    }

    [Test]
    public async Task ClassifyTextInputDeserializesHateThreateningCategory()
    {
        OpenAIClientOptions clientOptions = GetClientOptionsWithMockResponse(200, """
        {
            "results": [
                {
                    "categories": {
                        "hate/threatening": true
                    },
                    "category_scores": {
                        "hate/threatening": 0.895
                    }
                }
            ]
        }
        """);
        ModerationClient client = new ModerationClient("model", s_fakeCredential, clientOptions);

        ModerationResult moderation = IsAsync
            ? await client.ClassifyTextAsync("input")
            : client.ClassifyText("input");

        AssertModerationCategories(moderation, hateThreatening: (true, 0.895f));
    }

    [Test]
    public async Task ClassifyTextInputDeserializesHarassmentCategory()
    {
        OpenAIClientOptions clientOptions = GetClientOptionsWithMockResponse(200, """
        {
            "results": [
                {
                    "categories": {
                        "harassment": true
                    },
                    "category_scores": {
                        "harassment": 0.895
                    }
                }
            ]
        }
        """);
        ModerationClient client = new ModerationClient("model", s_fakeCredential, clientOptions);

        ModerationResult moderation = IsAsync
            ? await client.ClassifyTextAsync("input")
            : client.ClassifyText("input");

        AssertModerationCategories(moderation, harassment: (true, 0.895f));
    }

    [Test]
    public async Task ClassifyTextInputDeserializesHarassmentThreateningCategory()
    {
        OpenAIClientOptions clientOptions = GetClientOptionsWithMockResponse(200, """
        {
            "results": [
                {
                    "categories": {
                        "harassment/threatening": true
                    },
                    "category_scores": {
                        "harassment/threatening": 0.895
                    }
                }
            ]
        }
        """);
        ModerationClient client = new ModerationClient("model", s_fakeCredential, clientOptions);

        ModerationResult moderation = IsAsync
            ? await client.ClassifyTextAsync("input")
            : client.ClassifyText("input");

        AssertModerationCategories(moderation, harassmentThreatening: (true, 0.895f));
    }

    [Test]
    public async Task ClassifyTextInputDeserializesSelfHarmCategory()
    {
        OpenAIClientOptions clientOptions = GetClientOptionsWithMockResponse(200, """
        {
            "results": [
                {
                    "categories": {
                        "self-harm": true
                    },
                    "category_scores": {
                        "self-harm": 0.895
                    }
                }
            ]
        }
        """);
        ModerationClient client = new ModerationClient("model", s_fakeCredential, clientOptions);

        ModerationResult moderation = IsAsync
            ? await client.ClassifyTextAsync("input")
            : client.ClassifyText("input");

        AssertModerationCategories(moderation, selfHarm: (true, 0.895f));
    }

    [Test]
    public async Task ClassifyTextInputDeserializesSelfHarmIntentCategory()
    {
        OpenAIClientOptions clientOptions = GetClientOptionsWithMockResponse(200, """
        {
            "results": [
                {
                    "categories": {
                        "self-harm/intent": true
                    },
                    "category_scores": {
                        "self-harm/intent": 0.895
                    }
                }
            ]
        }
        """);
        ModerationClient client = new ModerationClient("model", s_fakeCredential, clientOptions);

        ModerationResult moderation = IsAsync
            ? await client.ClassifyTextAsync("input")
            : client.ClassifyText("input");

        AssertModerationCategories(moderation, selfHarmIntent: (true, 0.895f));
    }

    [Test]
    public async Task ClassifyTextInputDeserializesSelfHarmInstructionsCategory()
    {
        OpenAIClientOptions clientOptions = GetClientOptionsWithMockResponse(200, """
        {
            "results": [
                {
                    "categories": {
                        "self-harm/instructions": true
                    },
                    "category_scores": {
                        "self-harm/instructions": 0.895
                    }
                }
            ]
        }
        """);
        ModerationClient client = new ModerationClient("model", s_fakeCredential, clientOptions);

        ModerationResult moderation = IsAsync
            ? await client.ClassifyTextAsync("input")
            : client.ClassifyText("input");

        AssertModerationCategories(moderation, selfHarmInstructions: (true, 0.895f));
    }

    [Test]
    public async Task ClassifyTextInputDeserializesSexualCategory()
    {
        OpenAIClientOptions clientOptions = GetClientOptionsWithMockResponse(200, """
        {
            "results": [
                {
                    "categories": {
                        "sexual": true
                    },
                    "category_scores": {
                        "sexual": 0.895
                    }
                }
            ]
        }
        """);
        ModerationClient client = new ModerationClient("model", s_fakeCredential, clientOptions);

        ModerationResult moderation = IsAsync
            ? await client.ClassifyTextAsync("input")
            : client.ClassifyText("input");

        AssertModerationCategories(moderation, sexual: (true, 0.895f));
    }

    [Test]
    public async Task ClassifyTextInputDeserializesSexualMinorsCategory()
    {
        OpenAIClientOptions clientOptions = GetClientOptionsWithMockResponse(200, """
        {
            "results": [
                {
                    "categories": {
                        "sexual/minors": true
                    },
                    "category_scores": {
                        "sexual/minors": 0.895
                    }
                }
            ]
        }
        """);
        ModerationClient client = new ModerationClient("model", s_fakeCredential, clientOptions);

        ModerationResult moderation = IsAsync
            ? await client.ClassifyTextAsync("input")
            : client.ClassifyText("input");

        AssertModerationCategories(moderation, sexualMinors: (true, 0.895f));
    }

    [Test]
    public async Task ClassifyTextInputDeserializesViolenceCategory()
    {
        OpenAIClientOptions clientOptions = GetClientOptionsWithMockResponse(200, """
        {
            "results": [
                {
                    "categories": {
                        "violence": true
                    },
                    "category_scores": {
                        "violence": 0.895
                    }
                }
            ]
        }
        """);
        ModerationClient client = new ModerationClient("model", s_fakeCredential, clientOptions);

        ModerationResult moderation = IsAsync
            ? await client.ClassifyTextAsync("input")
            : client.ClassifyText("input");

        AssertModerationCategories(moderation, violence: (true, 0.895f));
    }

    [Test]
    public async Task ClassifyTextInputDeserializesViolenceGraphicCategory()
    {
        OpenAIClientOptions clientOptions = GetClientOptionsWithMockResponse(200, """
        {
            "results": [
                {
                    "categories": {
                        "violence/graphic": true
                    },
                    "category_scores": {
                        "violence/graphic": 0.895
                    }
                }
            ]
        }
        """);
        ModerationClient client = new ModerationClient("model", s_fakeCredential, clientOptions);

        ModerationResult moderation = IsAsync
            ? await client.ClassifyTextAsync("input")
            : client.ClassifyText("input");

        AssertModerationCategories(moderation, violenceGraphic: (true, 0.895f));
    }

    [Test]
    public void ClassifyTextInputRespectsTheCancellationToken()
    {
        ModerationClient client = new ModerationClient("model", s_fakeCredential);
        using CancellationTokenSource cancellationSource = new();
        cancellationSource.Cancel();

        if (IsAsync)
        {
            Assert.That(async () => await client.ClassifyTextAsync("input", cancellationSource.Token),
                Throws.InstanceOf<OperationCanceledException>());
        }
        else
        {
            Assert.That(() => client.ClassifyText("input", cancellationSource.Token),
                Throws.InstanceOf<OperationCanceledException>());
        }
    }

    [Test]
    public async Task ClassifyTextInputsDeserializesHateCategory()
    {
        OpenAIClientOptions clientOptions = GetClientOptionsWithMockResponse(200, """
        {
            "results": [
                {
                    "categories": {
                        "hate": true
                    },
                    "category_scores": {
                        "hate": 0.895
                    }
                }
            ]
        }
        """);
        ModerationClient client = new ModerationClient("model", s_fakeCredential, clientOptions);

        ModerationCollection resultCollection = IsAsync
            ? await client.ClassifyTextAsync(["input"])
            : client.ClassifyText(["input"]);
        ModerationResult moderation = resultCollection.Single();

        AssertModerationCategories(moderation, hate: (true, 0.895f));
    }

    [Test]
    public async Task ClassifyTextInputsDeserializesHateThreateningCategory()
    {
        OpenAIClientOptions clientOptions = GetClientOptionsWithMockResponse(200, """
        {
            "results": [
                {
                    "categories": {
                        "hate/threatening": true
                    },
                    "category_scores": {
                        "hate/threatening": 0.895
                    }
                }
            ]
        }
        """);
        ModerationClient client = new ModerationClient("model", s_fakeCredential, clientOptions);

        ModerationCollection resultCollection = IsAsync
            ? await client.ClassifyTextAsync(["input"])
            : client.ClassifyText(["input"]);
        ModerationResult moderation = resultCollection.Single();

        AssertModerationCategories(moderation, hateThreatening: (true, 0.895f));
    }

    [Test]
    public async Task ClassifyTextInputsDeserializesHarassmentCategory()
    {
        OpenAIClientOptions clientOptions = GetClientOptionsWithMockResponse(200, """
        {
            "results": [
                {
                    "categories": {
                        "harassment": true
                    },
                    "category_scores": {
                        "harassment": 0.895
                    }
                }
            ]
        }
        """);
        ModerationClient client = new ModerationClient("model", s_fakeCredential, clientOptions);

        ModerationCollection resultCollection = IsAsync
            ? await client.ClassifyTextAsync(["input"])
            : client.ClassifyText(["input"]);
        ModerationResult moderation = resultCollection.Single();

        AssertModerationCategories(moderation, harassment: (true, 0.895f));
    }

    [Test]
    public async Task ClassifyTextInputsDeserializesHarassmentThreateningCategory()
    {
        OpenAIClientOptions clientOptions = GetClientOptionsWithMockResponse(200, """
        {
            "results": [
                {
                    "categories": {
                        "harassment/threatening": true
                    },
                    "category_scores": {
                        "harassment/threatening": 0.895
                    }
                }
            ]
        }
        """);
        ModerationClient client = new ModerationClient("model", s_fakeCredential, clientOptions);

        ModerationCollection resultCollection = IsAsync
            ? await client.ClassifyTextAsync(["input"])
            : client.ClassifyText(["input"]);
        ModerationResult moderation = resultCollection.Single();

        AssertModerationCategories(moderation, harassmentThreatening: (true, 0.895f));
    }

    [Test]
    public async Task ClassifyTextInputsDeserializesSelfHarmCategory()
    {
        OpenAIClientOptions clientOptions = GetClientOptionsWithMockResponse(200, """
        {
            "results": [
                {
                    "categories": {
                        "self-harm": true
                    },
                    "category_scores": {
                        "self-harm": 0.895
                    }
                }
            ]
        }
        """);
        ModerationClient client = new ModerationClient("model", s_fakeCredential, clientOptions);

        ModerationCollection resultCollection = IsAsync
            ? await client.ClassifyTextAsync(["input"])
            : client.ClassifyText(["input"]);
        ModerationResult moderation = resultCollection.Single();

        AssertModerationCategories(moderation, selfHarm: (true, 0.895f));
    }

    [Test]
    public async Task ClassifyTextInputsDeserializesSelfHarmIntentCategory()
    {
        OpenAIClientOptions clientOptions = GetClientOptionsWithMockResponse(200, """
        {
            "results": [
                {
                    "categories": {
                        "self-harm/intent": true
                    },
                    "category_scores": {
                        "self-harm/intent": 0.895
                    }
                }
            ]
        }
        """);
        ModerationClient client = new ModerationClient("model", s_fakeCredential, clientOptions);

        ModerationCollection resultCollection = IsAsync
            ? await client.ClassifyTextAsync(["input"])
            : client.ClassifyText(["input"]);
        ModerationResult moderation = resultCollection.Single();

        AssertModerationCategories(moderation, selfHarmIntent: (true, 0.895f));
    }

    [Test]
    public async Task ClassifyTextInputsDeserializesSelfHarmInstructionsCategory()
    {
        OpenAIClientOptions clientOptions = GetClientOptionsWithMockResponse(200, """
        {
            "results": [
                {
                    "categories": {
                        "self-harm/instructions": true
                    },
                    "category_scores": {
                        "self-harm/instructions": 0.895
                    }
                }
            ]
        }
        """);
        ModerationClient client = new ModerationClient("model", s_fakeCredential, clientOptions);

        ModerationCollection resultCollection = IsAsync
            ? await client.ClassifyTextAsync(["input"])
            : client.ClassifyText(["input"]);
        ModerationResult moderation = resultCollection.Single();

        AssertModerationCategories(moderation, selfHarmInstructions: (true, 0.895f));
    }

    [Test]
    public async Task ClassifyTextInputsDeserializesSexualCategory()
    {
        OpenAIClientOptions clientOptions = GetClientOptionsWithMockResponse(200, """
        {
            "results": [
                {
                    "categories": {
                        "sexual": true
                    },
                    "category_scores": {
                        "sexual": 0.895
                    }
                }
            ]
        }
        """);
        ModerationClient client = new ModerationClient("model", s_fakeCredential, clientOptions);

        ModerationCollection resultCollection = IsAsync
            ? await client.ClassifyTextAsync(["input"])
            : client.ClassifyText(["input"]);
        ModerationResult moderation = resultCollection.Single();

        AssertModerationCategories(moderation, sexual: (true, 0.895f));
    }

    [Test]
    public async Task ClassifyTextInputsDeserializesSexualMinorsCategory()
    {
        OpenAIClientOptions clientOptions = GetClientOptionsWithMockResponse(200, """
        {
            "results": [
                {
                    "categories": {
                        "sexual/minors": true
                    },
                    "category_scores": {
                        "sexual/minors": 0.895
                    }
                }
            ]
        }
        """);
        ModerationClient client = new ModerationClient("model", s_fakeCredential, clientOptions);

        ModerationCollection resultCollection = IsAsync
            ? await client.ClassifyTextAsync(["input"])
            : client.ClassifyText(["input"]);
        ModerationResult moderation = resultCollection.Single();

        AssertModerationCategories(moderation, sexualMinors: (true, 0.895f));
    }

    [Test]
    public async Task ClassifyTextInputsDeserializesViolenceCategory()
    {
        OpenAIClientOptions clientOptions = GetClientOptionsWithMockResponse(200, """
        {
            "results": [
                {
                    "categories": {
                        "violence": true
                    },
                    "category_scores": {
                        "violence": 0.895
                    }
                }
            ]
        }
        """);
        ModerationClient client = new ModerationClient("model", s_fakeCredential, clientOptions);

        ModerationCollection resultCollection = IsAsync
            ? await client.ClassifyTextAsync(["input"])
            : client.ClassifyText(["input"]);
        ModerationResult moderation = resultCollection.Single();

        AssertModerationCategories(moderation, violence: (true, 0.895f));
    }

    [Test]
    public async Task ClassifyTextInputsDeserializesViolenceGraphicCategory()
    {
        OpenAIClientOptions clientOptions = GetClientOptionsWithMockResponse(200, """
        {
            "results": [
                {
                    "categories": {
                        "violence/graphic": true
                    },
                    "category_scores": {
                        "violence/graphic": 0.895
                    }
                }
            ]
        }
        """);
        ModerationClient client = new ModerationClient("model", s_fakeCredential, clientOptions);

        ModerationCollection resultCollection = IsAsync
            ? await client.ClassifyTextAsync(["input"])
            : client.ClassifyText(["input"]);
        ModerationResult moderation = resultCollection.Single();

        AssertModerationCategories(moderation, violenceGraphic: (true, 0.895f));
    }

    [Test]
    public void ClassifyTextInputsRespectsTheCancellationToken()
    {
        ModerationClient client = new ModerationClient("model", s_fakeCredential);
        using CancellationTokenSource cancellationSource = new();
        cancellationSource.Cancel();

        if (IsAsync)
        {
            Assert.That(async () => await client.ClassifyTextAsync(["input"], cancellationSource.Token),
                Throws.InstanceOf<OperationCanceledException>());
        }
        else
        {
            Assert.That(() => client.ClassifyText(["input"], cancellationSource.Token),
                Throws.InstanceOf<OperationCanceledException>());
        }
    }

    private void AssertModerationCategories(
        ModerationResult moderation,
        (bool flagged, float score) hate = default,
        (bool flagged, float score) hateThreatening = default,
        (bool flagged, float score) harassment = default,
        (bool flagged, float score) harassmentThreatening = default,
        (bool flagged, float score) selfHarm = default,
        (bool flagged, float score) selfHarmIntent = default,
        (bool flagged, float score) selfHarmInstructions = default,
        (bool flagged, float score) sexual = default,
        (bool flagged, float score) sexualMinors = default,
        (bool flagged, float score) violence = default,
        (bool flagged, float score) violenceGraphic = default)
    {
        Assert.That(moderation.Categories.Hate, Is.EqualTo(hate.flagged));
        Assert.That(moderation.Categories.HateThreatening, Is.EqualTo(hateThreatening.flagged));
        Assert.That(moderation.Categories.Harassment, Is.EqualTo(harassment.flagged));
        Assert.That(moderation.Categories.HarassmentThreatening, Is.EqualTo(harassmentThreatening.flagged));
        Assert.That(moderation.Categories.SelfHarm, Is.EqualTo(selfHarm.flagged));
        Assert.That(moderation.Categories.SelfHarmIntent, Is.EqualTo(selfHarmIntent.flagged));
        Assert.That(moderation.Categories.SelfHarmInstructions, Is.EqualTo(selfHarmInstructions.flagged));
        Assert.That(moderation.Categories.Sexual, Is.EqualTo(sexual.flagged));
        Assert.That(moderation.Categories.SexualMinors, Is.EqualTo(sexualMinors.flagged));
        Assert.That(moderation.Categories.Violence, Is.EqualTo(violence.flagged));
        Assert.That(moderation.Categories.ViolenceGraphic, Is.EqualTo(violenceGraphic.flagged));

        Assert.That(moderation.CategoryScores.Hate, Is.EqualTo(hate.score));
        Assert.That(moderation.CategoryScores.HateThreatening, Is.EqualTo(hateThreatening.score));
        Assert.That(moderation.CategoryScores.Harassment, Is.EqualTo(harassment.score));
        Assert.That(moderation.CategoryScores.HarassmentThreatening, Is.EqualTo(harassmentThreatening.score));
        Assert.That(moderation.CategoryScores.SelfHarm, Is.EqualTo(selfHarm.score));
        Assert.That(moderation.CategoryScores.SelfHarmIntent, Is.EqualTo(selfHarmIntent.score));
        Assert.That(moderation.CategoryScores.SelfHarmInstructions, Is.EqualTo(selfHarmInstructions.score));
        Assert.That(moderation.CategoryScores.Sexual, Is.EqualTo(sexual.score));
        Assert.That(moderation.CategoryScores.SexualMinors, Is.EqualTo(sexualMinors.score));
        Assert.That(moderation.CategoryScores.Violence, Is.EqualTo(violence.score));
        Assert.That(moderation.CategoryScores.ViolenceGraphic, Is.EqualTo(violenceGraphic.score));
    }

    private OpenAIClientOptions GetClientOptionsWithMockResponse(int status, string content)
    {
        MockPipelineResponse response = new MockPipelineResponse(status);
        response.SetContent(content);

        return new OpenAIClientOptions()
        {
            Transport = new MockPipelineTransport(response)
        };
    }
}
