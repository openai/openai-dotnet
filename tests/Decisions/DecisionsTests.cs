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

    // Evaluates a text predicate and verifies an unnamed predicate answer, its probability, and token usage.
    [RecordedTest]
    public async Task CreateTextPredicateDecision()
    {
        DecisionsClient client = GetProxiedDecisionsClient();
        DecisionCreationOptions options = CreateTextOptions("The customer says: Thank you! That solved my problem.");
        options.Questions.Add(new DecisionsPredicateQuestion
        {
            Instructions = { new DecisionsInstructionsInputTextPart { Text = "The customer is satisfied with the support they received." } },
        });

        DecisionResource decision = await client.CreateDecisionAsync(options);

        AssertDecision(decision, options);
        Assert.That(decision.Answers[0], Is.TypeOf<DecisionsPredicateAnswer>());
        DecisionsPredicateAnswer answer = (DecisionsPredicateAnswer)decision.Answers[0];
        Assert.That(answer.Name, Is.Null);
        Assert.That(answer.Probability, Is.InRange(0, 1));
        Assert.That(answer.Probability, Is.GreaterThan(0.5));
    }

    // Classifies a request using string choices and verifies the selected value, confidence, and probability distribution.
    [RecordedTest]
    public async Task CreateStringChoiceDecision()
    {
        DecisionsClient client = GetProxiedDecisionsClient();
        DecisionCreationOptions options = CreateTextOptions("My order arrived with a broken screen. Please send a replacement.");
        options.Questions.Add(new DecisionsChoiceQuestion
        {
            Name = "request_category",
            Instructions = { new DecisionsInstructionsInputTextPart { Text = "Choose the category that best describes the customer's request." } },
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
        Assert.That(answer.Confidence, Is.InRange(0, 1));
        Assert.That(answer.Probabilities.Select(probability => probability.Value.StringValue),
            Is.EquivalentTo(new[] { "replacement", "refund", "delivery" }));
        Assert.That(answer.Probabilities.All(probability => probability.Value.BooleanValue is null), Is.True);
        foreach (DecisionsChoiceProbability probability in answer.Probabilities)
        {
            Assert.That(probability.Probability, Is.InRange(0, 1));
        }
        Assert.That(answer.Probabilities.Sum(probability => probability.Probability), Is.EqualTo(1).Within(0.01));
    }

    // Evaluates boolean choices and verifies the active boolean component and normalized choice probabilities.
    [RecordedTest]
    public async Task CreateBooleanChoiceDecision()
    {
        DecisionsClient client = GetProxiedDecisionsClient();
        DecisionCreationOptions options = CreateTextOptions("Yes, please email me a copy of the receipt.");
        options.Questions.Add(new DecisionsChoiceQuestion
        {
            Name = "send_receipt",
            Instructions = { new DecisionsInstructionsInputTextPart { Text = "Decide whether the customer wants an emailed receipt." } },
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
        Assert.That(answer.Confidence, Is.InRange(0, 1));
        Assert.That(answer.Probabilities.Select(probability => probability.Value.BooleanValue),
            Is.EquivalentTo(new bool?[] { true, false }));
        Assert.That(answer.Probabilities.All(probability => probability.Value.StringValue is null), Is.True);
        foreach (DecisionsChoiceProbability probability in answer.Probabilities)
        {
            Assert.That(probability.Probability, Is.InRange(0, 1));
        }
        Assert.That(answer.Probabilities.Sum(probability => probability.Probability), Is.EqualTo(1).Within(0.01));
    }

    // Scores text sentiment and verifies the answer name, level probabilities, and probability-weighted score.
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

    // Submits predicate, choice, and score questions together and verifies answer order, names, and concrete types.
    [RecordedTest]
    public async Task CreateMultipleNamedQuestionsDecision()
    {
        DecisionsClient client = GetProxiedDecisionsClient();
        DecisionCreationOptions options = CreateTextOptions("Thank you for the excellent support! Please email me the receipt.");
        options.Questions.Add(new DecisionsPredicateQuestion
        {
            Name = "satisfied",
            Instructions = { new DecisionsInstructionsInputTextPart { Text = "The customer is satisfied with the support they received." } },
        });
        options.Questions.Add(new DecisionsChoiceQuestion
        {
            Name = "requested_action",
            Instructions = { new DecisionsInstructionsInputTextPart { Text = "Choose the action requested by the customer." } },
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
        Assert.That(predicateAnswer.Probability, Is.InRange(0, 1));
        Assert.That(predicateAnswer.Probability, Is.GreaterThan(0.5));

        Assert.That(decision.Answers[1], Is.TypeOf<DecisionsChoiceAnswer>());
        DecisionsChoiceAnswer choiceAnswer = (DecisionsChoiceAnswer)decision.Answers[1];
        Assert.That(choiceAnswer.Name, Is.EqualTo("requested_action"));
        Assert.That(choiceAnswer.Choice.StringValue, Is.EqualTo("email_receipt"));
        Assert.That(choiceAnswer.Choice.BooleanValue, Is.Null);
        Assert.That(choiceAnswer.Confidence, Is.InRange(0, 1));

        Assert.That(decision.Answers[2], Is.TypeOf<DecisionsScoreAnswer>());
        AssertSentimentAnswer((DecisionsScoreAnswer)decision.Answers[2], "sentiment");
    }

    // Evaluates an image supplied by HTTPS URI and verifies the image predicate answer and usage.
    [RecordedTest]
    public async Task CreateImageUriDecision()
    {
        DecisionsClient client = GetProxiedDecisionsClient();
        DecisionCreationOptions options = CreateImageOptions(
            "https://raw.githubusercontent.com/openai/openai-dotnet/db6328accdd7927f19915cdc5412eb841f2447c1/tests/Assets/images_empty_room.png",
            "The image shows an indoor room.");

        DecisionResource decision = await client.CreateDecisionAsync(options);

        AssertImageDecision(decision, options);
    }

    // Evaluates a local image encoded as a data URI and verifies the image predicate answer and usage.
    [RecordedTest]
    public async Task CreateImageDataUriDecision()
    {
        DecisionsClient client = GetProxiedDecisionsClient();
        byte[] imageBytes = File.ReadAllBytes(Path.Combine("Assets", "images_dog_and_cat.png"));
        DecisionCreationOptions options = CreateImageOptions(
            $"data:image/png;base64,{Convert.ToBase64String(imageBytes)}",
            "The image contains a dog and a cat.");

        DecisionResource decision = await client.CreateDecisionAsync(options);

        AssertImageDecision(decision, options);
    }

    [RecordedTest]
    public async Task CreateStructuredJsonDecision()
    {
        DecisionsClient client = GetProxiedDecisionsClient();
        DecisionCreationOptions options = CreateTextOptions("Evaluate these order records.");
        ((DecisionsUserMessageItem)options.InputItems[0]).Content.Add(new DecisionsContentInputJsonPart(
        [
            BinaryData.FromString("""{"order_id":"first","total":42}"""),
            BinaryData.FromString("""{"order_id":"second","total":7}"""),
        ]));
        options.Questions.Add(new DecisionsPredicateQuestion
        {
            Name = "positive_totals",
            Instructions =
            {
                new DecisionsInstructionsInputTextPart { Text = "Every order's total is greater than the minimum total in the reference JSON." },
                new DecisionsInstructionsInputJsonPart([BinaryData.FromString("""{"minimum_total":0}""")]),
            },
        });

        DecisionResource decision = await client.CreateDecisionAsync(options);

        AssertDecision(decision, options);
        DecisionsPredicateAnswer answer = decision.Answers[0] as DecisionsPredicateAnswer;
        Assert.That(answer, Is.Not.Null);
        Assert.That(answer.Name, Is.EqualTo("positive_totals"));
        Assert.That(answer.Probability, Is.InRange(0.5, 1));
    }

    [RecordedTest]
    public async Task CreateMultimodalInstructionsDecision()
    {
        DecisionsClient client = GetProxiedDecisionsClient();
        DecisionCreationOptions options = CreateTextOptions("The reference image shows an indoor room.");
        options.Questions.Add(new DecisionsPredicateQuestion
        {
            Name = "matches_reference",
            Instructions =
            {
                new DecisionsInstructionsInputTextPart { Text = "The statement in the input correctly describes the reference image." },
                new DecisionsInstructionsInputImagePart
                {
                    ImageUri = "https://raw.githubusercontent.com/openai/openai-dotnet/db6328accdd7927f19915cdc5412eb841f2447c1/tests/Assets/images_empty_room.png",
                    ImageDetailLevel = DecisionsImageDetailLevel.Low,
                },
            },
        });

        DecisionResource decision = await client.CreateDecisionAsync(options);

        AssertDecision(decision, options);
        DecisionsPredicateAnswer answer = decision.Answers[0] as DecisionsPredicateAnswer;
        Assert.That(answer, Is.Not.Null);
        Assert.That(answer.Name, Is.EqualTo("matches_reference"));
        Assert.That(answer.Probability, Is.InRange(0.5, 1));
    }

    [RecordedTest]
    public async Task CreateConversationAndToolHistoryDecision()
    {
        DecisionsClient client = GetProxiedDecisionsClient();
        DecisionCreationOptions options = new()
        {
            Model = TestModel.Decisions,
            InputItems =
            {
                new DecisionsSystemMessageItem
                {
                    Content = { new DecisionsInstructionMessageTextContentPart { Text = "You are a shopping assistant." } },
                },
                new DecisionsDeveloperMessageItem
                {
                    Content = { new DecisionsInstructionMessageTextContentPart { Text = "Use tool results to answer product questions." } },
                },
                new DecisionsUserMessageItem
                {
                    Content = { new DecisionsContentInputTextPart { Text = "Is the widget in stock, and what does it cost?" } },
                },
                new DecisionsAssistantMessageItem
                {
                    Content = { new DecisionsContentOutputTextPart { Text = "I will check the price and availability." } },
                    Phase = DecisionsAssistantMessagePhase.Commentary,
                },
                new DecisionsFunctionCallItem
                {
                    CallId = "call_price",
                    Name = "lookup_price",
                    Arguments = """{"product":"widget"}""",
                    Status = DecisionsToolCallStatus.Completed,
                },
                new DecisionsFunctionCallOutputItem
                {
                    CallId = "call_price",
                    Output = { new DecisionsToolOutputTextContentPart { Text = """{"price":42,"currency":"USD"}""" } },
                    Status = DecisionsToolCallStatus.Completed,
                },
                new DecisionsCustomToolCallItem
                {
                    CallId = "call_stock",
                    Name = "lookup_inventory",
                    Input = "widget",
                    Status = DecisionsToolCallStatus.Completed,
                },
                new DecisionsCustomToolCallOutputItem
                {
                    CallId = "call_stock",
                    Output = { new DecisionsToolOutputTextContentPart { Text = "The widget is in stock." } },
                    Status = DecisionsToolCallStatus.Completed,
                },
                new DecisionsAssistantMessageItem
                {
                    Content = { new DecisionsContentOutputTextPart { Text = "The widget is in stock and costs $42 USD." } },
                    Phase = DecisionsAssistantMessagePhase.FinalAnswer,
                },
            },
            Questions =
            {
                new DecisionsPredicateQuestion
                {
                    Name = "grounded_answer",
                    Instructions = { new DecisionsInstructionsInputTextPart { Text = "The assistant's final answer accurately states the price and availability from both tool results." } },
                },
            },
        };

        DecisionResource decision = await client.CreateDecisionAsync(options);

        AssertDecision(decision, options);
        DecisionsPredicateAnswer answer = decision.Answers[0] as DecisionsPredicateAnswer;
        Assert.That(answer, Is.Not.Null);
        Assert.That(answer.Name, Is.EqualTo("grounded_answer"));
        Assert.That(answer.Probability, Is.InRange(0.5, 1));
    }

    [RecordedTest]
    public async Task CreatePromptCachedDecision()
    {
        DecisionsClient client = GetProxiedDecisionsClient();
        DecisionCreationOptions options = CreateTextOptions(
            string.Concat(Enumerable.Repeat("Reference fact: the product is in stock.\n", 1024)));
        DecisionsContentInputTextPart text = (DecisionsContentInputTextPart)((DecisionsUserMessageItem)options.InputItems[0]).Content[0];
        text.PromptCacheBreakpoint = new() { Mode = DecisionsPromptCacheBreakpointMode.Explicit };
        options.PromptCacheKey = "openai-dotnet-decisions-cache-test";
        options.PromptCacheOptions = new()
        {
            Mode = DecisionsPromptCacheMode.Explicit,
            TimeToLive = DecisionsPromptCacheTimeToLive.ThirtyMinutes,
        };
        options.Questions.Add(new DecisionsPredicateQuestion
        {
            Name = "in_stock",
            Instructions = { new DecisionsInstructionsInputTextPart { Text = "The reference says that the product is in stock." } },
        });

        DecisionResource decision = await client.CreateDecisionAsync(options);

        AssertDecision(decision, options);
        DecisionsPredicateAnswer answer = decision.Answers[0] as DecisionsPredicateAnswer;
        Assert.That(answer, Is.Not.Null);
        Assert.That(answer.Name, Is.EqualTo("in_stock"));
        Assert.That(answer.Probability, Is.InRange(0.5, 1));
    }

    private DecisionCreationOptions CreateTextOptions(string text)
    {
        return new DecisionCreationOptions
        {
            Model = TestModel.Decisions,
            InputItems =
            {
                new DecisionsUserMessageItem
                {
                    Content = { new DecisionsContentInputTextPart { Text = text } },
                },
            },
        };
    }

    private DecisionCreationOptions CreateImageOptions(string imageUri, string instructions)
    {
        DecisionCreationOptions options = CreateTextOptions("Evaluate the supplied image.");
        DecisionsUserMessageItem message = (DecisionsUserMessageItem)options.InputItems[0];
        message.Content.Add(new DecisionsContentInputImagePart
        {
            ImageUri = imageUri,
            ImageDetailLevel = DecisionsImageDetailLevel.Low,
        });
        options.Questions.Add(new DecisionsPredicateQuestion
        {
            Name = "image_content",
            Instructions = { new DecisionsInstructionsInputTextPart { Text = instructions } },
        });
        return options;
    }

    private static DecisionsScoreQuestion CreateSentimentQuestion(string name)
    {
        return new DecisionsScoreQuestion
        {
            Name = name,
            Instructions = { new DecisionsInstructionsInputTextPart { Text = "Rate the customer's sentiment from negative to positive." } },
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
        Assert.That(decision.Usage.InputTokenCount, Is.GreaterThan(0));
        Assert.That(decision.Usage.OutputTokenCount, Is.GreaterThanOrEqualTo(0));
        Assert.That(decision.Usage.TotalTokenCount,
            Is.EqualTo(decision.Usage.InputTokenCount + decision.Usage.OutputTokenCount));
        Assert.That(decision.Usage.InputTokenDetails, Is.Not.Null);
        Assert.That(decision.Usage.InputTokenDetails.CachedTokenCount, Is.GreaterThanOrEqualTo(0));
        Assert.That(decision.Usage.InputTokenDetails.CacheWriteTokenCount, Is.GreaterThanOrEqualTo(0));
        Assert.That(decision.Usage.OutputTokenDetails, Is.Not.Null);
        Assert.That(decision.Usage.OutputTokenDetails.ReasoningTokenCount, Is.GreaterThanOrEqualTo(0));
    }

    private static void AssertSentimentAnswer(DecisionsScoreAnswer answer, string name)
    {
        string[] labels = ["negative", "neutral", "positive"];
        Assert.That(answer.Name, Is.EqualTo(name));
        Assert.That(answer.Confidence, Is.InRange(0, 1));
        Assert.That(answer.Score, Is.InRange(0, labels.Length - 1));
        Assert.That(answer.Probabilities, Has.Count.EqualTo(labels.Length));
        Assert.That(answer.Probabilities.Select(probability => probability.Label), Is.EquivalentTo(labels));
        foreach (DecisionsScoreProbability probability in answer.Probabilities)
        {
            Assert.That(probability.Probability, Is.InRange(0, 1));
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
        Assert.That(answer.Probability, Is.InRange(0, 1));
        Assert.That(answer.Probability, Is.GreaterThan(0.5));
    }
}
