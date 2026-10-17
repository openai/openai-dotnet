using Microsoft.ClientModel.TestFramework;
using NUnit.Framework;
using OpenAI.Decisions;
using OpenAI.Tests.Utility;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace OpenAI.Tests.Decisions;

[Category("Decisions")]
public class DecisionsTests : OpenAIRecordedTestBase
{
    public DecisionsTests(bool isAsync) : base(isAsync)
    {
    }

    [RecordedTest]
    public async Task CreateTextPredicateDecision()
    {
        DecisionsClient client = GetProxiedDecisionsClient();
        DecisionCreationOptions options = CreateTextOptions("The customer says: Thank you! That solved my problem.");
        options.Questions.Add(new DecisionsPredicateQuestion
        {
            Instructions = "The customer is satisfied with the support they received.",
        });

        DecisionResource decision = await client.CreateDecisionAsync(options);

        AssertDecision(decision, options);
        Assert.That(decision.Answers[0], Is.TypeOf<DecisionsPredicateAnswer>());
        DecisionsPredicateAnswer answer = (DecisionsPredicateAnswer)decision.Answers[0];
        Assert.That(answer.Name, Is.Null);
        AssertProbability(answer.Probability);
        Assert.That(answer.Probability, Is.GreaterThan(0.5));
    }

    [RecordedTest]
    public async Task CreateStringChoiceDecision()
    {
        DecisionsClient client = GetProxiedDecisionsClient();
        DecisionCreationOptions options = CreateTextOptions("My order arrived with a broken screen. Please send a replacement.");
        options.Questions.Add(new DecisionsChoiceQuestion
        {
            Name = "request_category",
            Instructions = "Choose the category that best describes the customer's request.",
            Choices =
            {
                new DecisionsChoice { Value = new DecisionsChoiceValue("replacement"), Description = "Replace a damaged item." },
                new DecisionsChoice { Value = new DecisionsChoiceValue("refund"), Description = "Return the payment." },
                new DecisionsChoice { Value = new DecisionsChoiceValue("delivery"), Description = "Track a shipment." },
            },
        });

        DecisionResource decision = await client.CreateDecisionAsync(options);

        AssertDecision(decision, options);
        Assert.That(decision.Answers[0], Is.TypeOf<DecisionsChoiceAnswer>());
        DecisionsChoiceAnswer answer = (DecisionsChoiceAnswer)decision.Answers[0];
        Assert.That(answer.Name, Is.EqualTo("request_category"));
        Assert.That(answer.Choice.StringValue, Is.EqualTo("replacement"));
        Assert.That(answer.Choice.BooleanValue, Is.Null);
        AssertProbability(answer.Confidence);
        Assert.That(answer.Probabilities.Select(probability => probability.Value.StringValue),
            Is.EquivalentTo(new[] { "replacement", "refund", "delivery" }));
        Assert.That(answer.Probabilities.All(probability => probability.Value.BooleanValue is null), Is.True);
        foreach (DecisionsChoiceProbability probability in answer.Probabilities)
        {
            AssertProbability(probability.Probability);
        }
        Assert.That(answer.Probabilities.Sum(probability => probability.Probability), Is.EqualTo(1).Within(0.01));
    }

    [RecordedTest]
    public async Task CreateBooleanChoiceDecision()
    {
        DecisionsClient client = GetProxiedDecisionsClient();
        DecisionCreationOptions options = CreateTextOptions("Yes, please email me a copy of the receipt.");
        options.Questions.Add(new DecisionsChoiceQuestion
        {
            Name = "send_receipt",
            Instructions = "Decide whether the customer wants an emailed receipt.",
            Choices =
            {
                new DecisionsChoice { Value = new DecisionsChoiceValue(true), Description = "The customer requests a receipt." },
                new DecisionsChoice { Value = new DecisionsChoiceValue(false), Description = "The customer does not request a receipt." },
            },
        });

        DecisionResource decision = await client.CreateDecisionAsync(options);

        AssertDecision(decision, options);
        Assert.That(decision.Answers[0], Is.TypeOf<DecisionsChoiceAnswer>());
        DecisionsChoiceAnswer answer = (DecisionsChoiceAnswer)decision.Answers[0];
        Assert.That(answer.Name, Is.EqualTo("send_receipt"));
        Assert.That(answer.Choice.BooleanValue, Is.True);
        Assert.That(answer.Choice.StringValue, Is.Null);
        AssertProbability(answer.Confidence);
        Assert.That(answer.Probabilities.Select(probability => probability.Value.BooleanValue),
            Is.EquivalentTo(new bool?[] { true, false }));
        Assert.That(answer.Probabilities.All(probability => probability.Value.StringValue is null), Is.True);
        foreach (DecisionsChoiceProbability probability in answer.Probabilities)
        {
            AssertProbability(probability.Probability);
        }
        Assert.That(answer.Probabilities.Sum(probability => probability.Probability), Is.EqualTo(1).Within(0.01));
    }

    [RecordedTest]
    public async Task CreateScoreDecision()
    {
        DecisionsClient client = GetProxiedDecisionsClient();
        DecisionCreationOptions options = CreateTextOptions("The service was excellent: fast, friendly, and helpful.");
        options.Questions.Add(CreateSentimentQuestion("sentiment"));

        DecisionResource decision = await client.CreateDecisionAsync(options);

        AssertDecision(decision, options);
        Assert.That(decision.Answers[0], Is.TypeOf<DecisionsScoreAnswer>());
        AssertSentimentAnswer((DecisionsScoreAnswer)decision.Answers[0], "sentiment");
    }

    [RecordedTest]
    public async Task CreateMultipleNamedQuestionsDecision()
    {
        DecisionsClient client = GetProxiedDecisionsClient();
        DecisionCreationOptions options = CreateTextOptions("Thank you for the excellent support! Please email me the receipt.");
        options.Questions.Add(new DecisionsPredicateQuestion
        {
            Name = "satisfied",
            Instructions = "The customer is satisfied with the support they received.",
        });
        options.Questions.Add(new DecisionsChoiceQuestion
        {
            Name = "requested_action",
            Instructions = "Choose the action requested by the customer.",
            Choices =
            {
                new DecisionsChoice { Value = "email_receipt", Description = "Email the receipt." },
                new DecisionsChoice { Value = "cancel_order", Description = "Cancel the order." },
            },
        });
        options.Questions.Add(CreateSentimentQuestion("sentiment"));

        DecisionResource decision = await client.CreateDecisionAsync(options);

        AssertDecision(decision, options);
        Assert.That(decision.Answers[0], Is.TypeOf<DecisionsPredicateAnswer>());
        DecisionsPredicateAnswer predicateAnswer = (DecisionsPredicateAnswer)decision.Answers[0];
        Assert.That(predicateAnswer.Name, Is.EqualTo("satisfied"));
        AssertProbability(predicateAnswer.Probability);
        Assert.That(predicateAnswer.Probability, Is.GreaterThan(0.5));

        Assert.That(decision.Answers[1], Is.TypeOf<DecisionsChoiceAnswer>());
        DecisionsChoiceAnswer choiceAnswer = (DecisionsChoiceAnswer)decision.Answers[1];
        Assert.That(choiceAnswer.Name, Is.EqualTo("requested_action"));
        Assert.That(choiceAnswer.Choice.StringValue, Is.EqualTo("email_receipt"));
        Assert.That(choiceAnswer.Choice.BooleanValue, Is.Null);
        AssertProbability(choiceAnswer.Confidence);

        Assert.That(decision.Answers[2], Is.TypeOf<DecisionsScoreAnswer>());
        AssertSentimentAnswer((DecisionsScoreAnswer)decision.Answers[2], "sentiment");
    }

    [RecordedTest]
    public async Task CreateImageUrlDecision()
    {
        DecisionsClient client = GetProxiedDecisionsClient();
        DecisionCreationOptions options = CreateImageOptions(
            "https://raw.githubusercontent.com/openai/openai-dotnet/db6328accdd7927f19915cdc5412eb841f2447c1/tests/Assets/images_empty_room.png",
            "The image shows an indoor room.");

        DecisionResource decision = await client.CreateDecisionAsync(options);

        AssertImageDecision(decision, options);
    }

    [RecordedTest]
    public async Task CreateImageDataUrlDecision()
    {
        DecisionsClient client = GetProxiedDecisionsClient();
        byte[] imageBytes = File.ReadAllBytes(Path.Combine("Assets", "images_dog_and_cat.png"));
        DecisionCreationOptions options = CreateImageOptions(
            $"data:image/png;base64,{Convert.ToBase64String(imageBytes)}",
            "The image contains a dog and a cat.");

        DecisionResource decision = await client.CreateDecisionAsync(options);

        AssertImageDecision(decision, options);
    }

    private DecisionCreationOptions CreateTextOptions(string text)
    {
        return new DecisionCreationOptions
        {
            Model = TestEnvironment.DecisionsModel,
            Input =
            {
                new DecisionsMessageItem
                {
                    Content = { new DecisionsInputTextContentPart { Text = text } },
                },
            },
        };
    }

    private DecisionCreationOptions CreateImageOptions(string imageUrl, string instructions)
    {
        DecisionCreationOptions options = CreateTextOptions("Evaluate the supplied image.");
        DecisionsMessageItem message = (DecisionsMessageItem)options.Input[0];
        message.Content.Add(new DecisionsInputImageContentPart
        {
            ImageUrl = imageUrl,
            Detail = DecisionsImageDetailLevel.Low,
        });
        options.Questions.Add(new DecisionsPredicateQuestion
        {
            Name = "image_content",
            Instructions = instructions,
        });
        return options;
    }

    private static DecisionsScoreQuestion CreateSentimentQuestion(string name)
    {
        return new DecisionsScoreQuestion
        {
            Name = name,
            Instructions = "Rate the customer's sentiment from negative to positive.",
            Levels =
            {
                new DecisionsScoreLevel { Label = "negative", Description = "The customer is dissatisfied." },
                new DecisionsScoreLevel { Label = "neutral", Description = "The customer is neither satisfied nor dissatisfied." },
                new DecisionsScoreLevel { Label = "positive", Description = "The customer is satisfied." },
            },
        };
    }

    private static void AssertDecision(DecisionResource decision, DecisionCreationOptions options)
    {
        Assert.That(decision, Is.Not.Null);
        Assert.That(decision.Model, Does.StartWith(options.Model));
        Assert.That(decision.Answers, Has.Count.EqualTo(options.Questions.Count));
        Assert.That(decision.Usage, Is.Not.Null);
        Assert.That(decision.Usage.InputTokens, Is.GreaterThan(0));
        Assert.That(decision.Usage.OutputTokens, Is.GreaterThanOrEqualTo(0));
        Assert.That(decision.Usage.TotalTokens,
            Is.EqualTo(decision.Usage.InputTokens + decision.Usage.OutputTokens));
        Assert.That(decision.Usage.InputTokensDetails, Is.Not.Null);
        Assert.That(decision.Usage.InputTokensDetails.CachedTokens, Is.GreaterThanOrEqualTo(0));
        Assert.That(decision.Usage.InputTokensDetails.CacheWriteTokens, Is.GreaterThanOrEqualTo(0));
        Assert.That(decision.Usage.OutputTokensDetails, Is.Not.Null);
        Assert.That(decision.Usage.OutputTokensDetails.ReasoningTokens, Is.GreaterThanOrEqualTo(0));
    }

    private static void AssertSentimentAnswer(DecisionsScoreAnswer answer, string name)
    {
        string[] labels = ["negative", "neutral", "positive"];
        Assert.That(answer.Name, Is.EqualTo(name));
        AssertProbability(answer.Confidence);
        Assert.That(answer.Score, Is.InRange(0, labels.Length - 1));
        Assert.That(answer.Probabilities, Has.Count.EqualTo(labels.Length));
        Assert.That(answer.Probabilities.Select(probability => probability.Label), Is.EquivalentTo(labels));
        foreach (DecisionsScoreProbability probability in answer.Probabilities)
        {
            AssertProbability(probability.Probability);
            Assert.That(probability.Value, Is.InRange(0, labels.Length - 1));
            Assert.That(probability.Label, Is.EqualTo(labels[probability.Value]));
        }
        Assert.That(answer.Probabilities.Sum(probability => probability.Probability), Is.EqualTo(1).Within(0.01));
        Assert.That(answer.Score,
            Is.EqualTo(answer.Probabilities.Sum(probability => probability.Value * probability.Probability)).Within(0.01));
        Assert.That(answer.Score, Is.GreaterThan(1));
    }

    private static void AssertImageDecision(DecisionResource decision, DecisionCreationOptions options)
    {
        AssertDecision(decision, options);
        Assert.That(decision.Answers[0], Is.TypeOf<DecisionsPredicateAnswer>());
        DecisionsPredicateAnswer answer = (DecisionsPredicateAnswer)decision.Answers[0];
        Assert.That(answer.Name, Is.EqualTo("image_content"));
        AssertProbability(answer.Probability);
        Assert.That(answer.Probability, Is.GreaterThan(0.5));
    }

    private static void AssertProbability(float probability)
    {
        Assert.That(probability, Is.InRange(0, 1));
    }
}
