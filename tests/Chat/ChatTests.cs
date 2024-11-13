using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using NUnit.Framework;
using OpenAI.Chat;
using OpenAI.Tests.Telemetry;
using OpenAI.Tests.Utility;
using System;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using static OpenAI.Tests.Telemetry.TestMeterListener;
using static OpenAI.Tests.TestHelpers;

namespace OpenAI.Tests.Chat;

[TestFixture(true)]
[TestFixture(false)]
[Parallelizable(ParallelScope.All)]
[Category("Chat")]
public class ChatTests : SyncAsyncTestBase
{
    public ChatTests(bool isAsync) : base(isAsync)
    {
    }

    [Test]
    public async Task HelloWorldChat()
    {
        ChatClient client = GetTestClient<ChatClient>(TestScenario.Chat);
        IEnumerable<ChatMessage> messages = [new UserChatMessage("Hello, world!")];
        ClientResult<ChatCompletion> result = IsAsync
            ? await client.CompleteChatAsync(messages)
            : client.CompleteChat(messages);
        Assert.That(result, Is.InstanceOf<ClientResult<ChatCompletion>>());
        Assert.That(result.Value.Content[0].Kind, Is.EqualTo(ChatMessageContentPartKind.Text));
        Assert.That(result.Value.Content[0].Text.Length, Is.GreaterThan(0));
    }

    [Test]
    public async Task HelloWorldWithTopLevelClient()
    {
        OpenAIClient client = GetTestClient<OpenAIClient>(TestScenario.TopLevel);
        ChatClient chatClient = client.GetChatClient("gpt-4o-mini");
        IEnumerable<ChatMessage> messages = [new UserChatMessage("Hello, world!")];
        ClientResult<ChatCompletion> result = IsAsync
            ? await chatClient.CompleteChatAsync(messages)
            : chatClient.CompleteChat(messages);
        Assert.That(result.Value.Content[0].Text.Length, Is.GreaterThan(0));
    }

    [Test]
    public async Task MultiMessageChat()
    {
        ChatClient client = GetTestClient<ChatClient>(TestScenario.Chat);
        IEnumerable<ChatMessage> messages = [
            new SystemChatMessage("You are a helpful assistant. You always talk like a pirate."),
            new UserChatMessage("Hello, assistant! Can you help me train my parrot?"),
        ];
        ClientResult<ChatCompletion> result = IsAsync
            ? await client.CompleteChatAsync(messages)
            : client.CompleteChat(messages);
        Assert.That(new string[] { "aye", "arr", "hearty" }.Any(pirateWord => result.Value.Content[0].Text.ToLowerInvariant().Contains(pirateWord)));
    }

    [Test]
    public void StreamingChat()
    {
        AssertSyncOnly();

        ChatClient client = GetTestClient<ChatClient>(TestScenario.Chat);
        IEnumerable<ChatMessage> messages = [new UserChatMessage("What are the best pizza toppings? Give me a breakdown on the reasons.")];

        int updateCount = 0;
        ChatTokenUsage usage = null;
        TimeSpan? firstTokenReceiptTime = null;
        TimeSpan? latestTokenReceiptTime = null;
        Stopwatch stopwatch = Stopwatch.StartNew();
        CollectionResult<StreamingChatCompletionUpdate> streamingResult = client.CompleteChatStreaming(messages);

        Assert.That(streamingResult, Is.InstanceOf<CollectionResult<StreamingChatCompletionUpdate>>());

        foreach (StreamingChatCompletionUpdate chatUpdate in streamingResult)
        {
            firstTokenReceiptTime ??= stopwatch.Elapsed;
            latestTokenReceiptTime = stopwatch.Elapsed;
            usage ??= chatUpdate.Usage;
            updateCount++;

            Console.WriteLine(stopwatch.Elapsed.TotalMilliseconds);
        }

        stopwatch.Stop();

        Assert.That(updateCount, Is.GreaterThan(1));
        Assert.That(latestTokenReceiptTime - firstTokenReceiptTime > TimeSpan.FromMilliseconds(500));
        Assert.That(usage, Is.Not.Null);
        Assert.That(usage?.InputTokenCount, Is.GreaterThan(0));
        Assert.That(usage?.OutputTokenCount, Is.GreaterThan(0));
        Assert.That(usage?.OutputTokenDetails?.ReasoningTokenCount, Is.Null.Or.EqualTo(0));
    }

    [Test]
    public async Task StreamingChatAsync()
    {
        AssertAsyncOnly();

        ChatClient client = GetTestClient<ChatClient>(TestScenario.Chat);
        IEnumerable<ChatMessage> messages = [new UserChatMessage("What are the best pizza toppings? Give me a breakdown on the reasons.")];

        int updateCount = 0;
        ChatTokenUsage usage = null;
        TimeSpan? firstTokenReceiptTime = null;
        TimeSpan? latestTokenReceiptTime = null;
        Stopwatch stopwatch = Stopwatch.StartNew();
        AsyncCollectionResult<StreamingChatCompletionUpdate> streamingResult = client.CompleteChatStreamingAsync(messages);

        Assert.That(streamingResult, Is.InstanceOf<AsyncCollectionResult<StreamingChatCompletionUpdate>>());

        await foreach (StreamingChatCompletionUpdate chatUpdate in streamingResult)
        {
            firstTokenReceiptTime ??= stopwatch.Elapsed;
            latestTokenReceiptTime = stopwatch.Elapsed;
            usage ??= chatUpdate.Usage;
            updateCount++;
        }

        stopwatch.Stop();

        Assert.That(updateCount, Is.GreaterThan(1));
        Assert.That(latestTokenReceiptTime - firstTokenReceiptTime > TimeSpan.FromMilliseconds(500));
        Assert.That(usage, Is.Not.Null);
        Assert.That(usage?.InputTokenCount, Is.GreaterThan(0));
        Assert.That(usage?.OutputTokenCount, Is.GreaterThan(0));
        Assert.That(usage?.OutputTokenDetails?.ReasoningTokenCount, Is.Null.Or.EqualTo(0));
    }

    [Test]
    public void StreamingChatCanBeCancelled()
    {
        AssertSyncOnly();

        MockPipelineResponse response = new(200);
        response.SetContent("""
            data: {"id":"chatcmpl-A7mKGugwaczn3YyrJLlZY6CM0Wlkr","object":"chat.completion.chunk","created":1726417424,"model":"gpt-4o-mini-2024-07-18","system_fingerprint":"fp_483d39d857","choices":[{"index":0,"delta":{"role":"assistant","content":"","refusal":null},"logprobs":null,"finish_reason":null}],"usage":null}

            data: {"id":"chatcmpl-A7mKGugwaczn3YyrJLlZY6CM0Wlkr","object":"chat.completion.chunk","created":1726417424,"model":"gpt-4o-mini-2024-07-18","system_fingerprint":"fp_483d39d857","choices":[{"index":0,"delta":{"content":"The"},"logprobs":null,"finish_reason":null}],"usage":null}

            data: [DONE]
            """);

        OpenAIClientOptions options = new OpenAIClientOptions()
        {
            Transport = new MockPipelineTransport(response)
        };

        CancellationTokenSource cancellationTokenSource = new();
        cancellationTokenSource.CancelAfter(1000);

        ChatClient client = GetTestClient<ChatClient>(TestScenario.Chat, options: options);
        IEnumerable<ChatMessage> messages = [new UserChatMessage("What are the best pizza toppings? Give me a breakdown on the reasons.")];

        CollectionResult<StreamingChatCompletionUpdate> streamingResult = client.CompleteChatStreaming(messages, cancellationToken: cancellationTokenSource.Token);
        IEnumerator<StreamingChatCompletionUpdate> enumerator = streamingResult.GetEnumerator();

        enumerator.MoveNext();
        StreamingChatCompletionUpdate firstUpdate = enumerator.Current;

        Assert.That(firstUpdate, Is.Not.Null);
        Assert.That(cancellationTokenSource.IsCancellationRequested, Is.False);

        Thread.Sleep(1000);

        Assert.Throws<OperationCanceledException>(() =>
        {
            // Should throw for the second update.
            Assert.True(cancellationTokenSource.IsCancellationRequested);
            Assert.True(cancellationTokenSource.Token.IsCancellationRequested);
            enumerator.MoveNext();
            enumerator.MoveNext();
        });
    }

    [Test]
    public async Task StreamingChatCanBeCancelledAsync()
    {
        AssertAsyncOnly();

        MockPipelineResponse response = new(200);
        response.SetContent("""
            data: {"id":"chatcmpl-A7mKGugwaczn3YyrJLlZY6CM0Wlkr","object":"chat.completion.chunk","created":1726417424,"model":"gpt-4o-mini-2024-07-18","system_fingerprint":"fp_483d39d857","choices":[{"index":0,"delta":{"role":"assistant","content":"","refusal":null},"logprobs":null,"finish_reason":null}],"usage":null}

            data: {"id":"chatcmpl-A7mKGugwaczn3YyrJLlZY6CM0Wlkr","object":"chat.completion.chunk","created":1726417424,"model":"gpt-4o-mini-2024-07-18","system_fingerprint":"fp_483d39d857","choices":[{"index":0,"delta":{"content":"The"},"logprobs":null,"finish_reason":null}],"usage":null}

            data: [DONE]
            """);

        OpenAIClientOptions options = new OpenAIClientOptions()
        {
            Transport = new MockPipelineTransport(response)
        };

        CancellationTokenSource cancellationTokenSource = new();
        cancellationTokenSource.CancelAfter(1000);

        ChatClient client = GetTestClient<ChatClient>(TestScenario.Chat, options: options);
        IEnumerable<ChatMessage> messages = [new UserChatMessage("What are the best pizza toppings? Give me a breakdown on the reasons.")];

        AsyncCollectionResult<StreamingChatCompletionUpdate> streamingResult = client.CompleteChatStreamingAsync(messages, cancellationToken: cancellationTokenSource.Token);
        IAsyncEnumerator<StreamingChatCompletionUpdate> enumerator = streamingResult.GetAsyncEnumerator();

        await enumerator.MoveNextAsync();
        StreamingChatCompletionUpdate firstUpdate = enumerator.Current;

        Assert.That(firstUpdate, Is.Not.Null);
        Assert.That(cancellationTokenSource.IsCancellationRequested, Is.False);

        Thread.Sleep(1000);

        Assert.ThrowsAsync<OperationCanceledException>(async () =>
        {
            // Should throw for the second update.
            Assert.True(cancellationTokenSource.IsCancellationRequested);
            Assert.True(cancellationTokenSource.Token.IsCancellationRequested);
            await enumerator.MoveNextAsync();
            await enumerator.MoveNextAsync();
        });
    }

    [Test]
    public void CompleteChatStreamingClosesNetworkStream()
    {
        AssertSyncOnly();

        MockPipelineResponse response = new(200);
        response.SetContent("""
            data: {"id":"chatcmpl-A7mKGugwaczn3YyrJLlZY6CM0Wlkr","object":"chat.completion.chunk","created":1726417424,"model":"gpt-4o-mini-2024-07-18","system_fingerprint":"fp_483d39d857","choices":[{"index":0,"delta":{"role":"assistant","content":"","refusal":null},"logprobs":null,"finish_reason":null}],"usage":null}

            data: {"id":"chatcmpl-A7mKGugwaczn3YyrJLlZY6CM0Wlkr","object":"chat.completion.chunk","created":1726417424,"model":"gpt-4o-mini-2024-07-18","system_fingerprint":"fp_483d39d857","choices":[{"index":0,"delta":{"content":"The"},"logprobs":null,"finish_reason":null}],"usage":null}

            data: [DONE]
            """);

        OpenAIClientOptions options = new OpenAIClientOptions()
        {
            Transport = new MockPipelineTransport(response)
        };

        ChatClient client = GetTestClient<ChatClient>(TestScenario.Chat, options: options);
        IEnumerable<ChatMessage> messages = [new UserChatMessage("What are the best pizza toppings? Give me a breakdown on the reasons.")];

        int updateCount = 0;
        TimeSpan? firstTokenReceiptTime = null;
        TimeSpan? latestTokenReceiptTime = null;
        Stopwatch stopwatch = Stopwatch.StartNew();
        CollectionResult<StreamingChatCompletionUpdate> streamingResult = client.CompleteChatStreaming(messages);

        Assert.That(streamingResult, Is.InstanceOf<CollectionResult<StreamingChatCompletionUpdate>>());
        Assert.IsFalse(response.IsDisposed);

        foreach (StreamingChatCompletionUpdate chatUpdate in streamingResult)
        {
            firstTokenReceiptTime ??= stopwatch.Elapsed;
            latestTokenReceiptTime = stopwatch.Elapsed;
            updateCount++;

            Console.WriteLine(stopwatch.Elapsed.TotalMilliseconds);
        }

        stopwatch.Stop();

        Assert.IsTrue(response.IsDisposed);
    }

    [Test]
    public async Task CompleteChatStreamingClosesNetworkStreamAsync()
    {
        AssertAsyncOnly();

        MockPipelineResponse response = new(200);
        response.SetContent("""
            data: {"id":"chatcmpl-A7mKGugwaczn3YyrJLlZY6CM0Wlkr","object":"chat.completion.chunk","created":1726417424,"model":"gpt-4o-mini-2024-07-18","system_fingerprint":"fp_483d39d857","choices":[{"index":0,"delta":{"role":"assistant","content":"","refusal":null},"logprobs":null,"finish_reason":null}],"usage":null}

            data: {"id":"chatcmpl-A7mKGugwaczn3YyrJLlZY6CM0Wlkr","object":"chat.completion.chunk","created":1726417424,"model":"gpt-4o-mini-2024-07-18","system_fingerprint":"fp_483d39d857","choices":[{"index":0,"delta":{"content":"The"},"logprobs":null,"finish_reason":null}],"usage":null}

            data: [DONE]
            """);

        OpenAIClientOptions options = new OpenAIClientOptions()
        {
            Transport = new MockPipelineTransport(response)
        };

        ChatClient client = GetTestClient<ChatClient>(TestScenario.Chat, options: options);
        IEnumerable<ChatMessage> messages = [new UserChatMessage("What are the best pizza toppings? Give me a breakdown on the reasons.")];

        int updateCount = 0;
        TimeSpan? firstTokenReceiptTime = null;
        TimeSpan? latestTokenReceiptTime = null;
        Stopwatch stopwatch = Stopwatch.StartNew();
        AsyncCollectionResult<StreamingChatCompletionUpdate> streamingResult = client.CompleteChatStreamingAsync(messages);

        Assert.That(streamingResult, Is.InstanceOf<AsyncCollectionResult<StreamingChatCompletionUpdate>>());
        Assert.IsFalse(response.IsDisposed);

        await foreach (StreamingChatCompletionUpdate chatUpdate in streamingResult)
        {
            firstTokenReceiptTime ??= stopwatch.Elapsed;
            latestTokenReceiptTime = stopwatch.Elapsed;
            updateCount++;

            Console.WriteLine(stopwatch.Elapsed.TotalMilliseconds);
        }

        stopwatch.Stop();

        Assert.IsTrue(response.IsDisposed);
    }

    [Test]
    public async Task TwoTurnChat()
    {
        ChatClient client = GetTestClient<ChatClient>(TestScenario.Chat);

        List<ChatMessage> messages =
        [
            new UserChatMessage("In geometry, what are the different kinds of triangles, as defined by lengths of their sides?"),
        ];
        ClientResult<ChatCompletion> firstResult = IsAsync
            ? await client.CompleteChatAsync(messages)
            : client.CompleteChat(messages);
        Assert.That(firstResult?.Value, Is.Not.Null);
        Assert.That(firstResult.Value.Content[0].Text.ToLowerInvariant(), Contains.Substring("isosceles"));
        messages.Add(new AssistantChatMessage(firstResult.Value));
        messages.Add(new UserChatMessage("Which of those is the one where exactly two sides are the same length?"));
        ClientResult<ChatCompletion> secondResult = client.CompleteChat(messages);
        Assert.That(secondResult?.Value, Is.Not.Null);
        Assert.That(secondResult.Value.Content[0].Text.ToLowerInvariant(), Contains.Substring("isosceles"));
    }

    [Test]
    public async Task ChatWithVision()
    {
        string mediaType = "image/png";
        string filePath = Path.Combine("Assets", "images_dog_and_cat.png");
        using Stream stream = File.OpenRead(filePath);
        BinaryData imageData = BinaryData.FromStream(stream);

        ChatClient client = GetTestClient<ChatClient>(TestScenario.Chat);
        IEnumerable<ChatMessage> messages = [
            new UserChatMessage(
                ChatMessageContentPart.CreateTextPart("Describe this image for me."),
                ChatMessageContentPart.CreateImagePart(imageData, mediaType)),
        ];
        ChatCompletionOptions options = new() { MaxOutputTokenCount = 2048 };

        ClientResult<ChatCompletion> result = IsAsync
            ? await client.CompleteChatAsync(messages, options)
            : client.CompleteChat(messages, options);
        Console.WriteLine(result.Value.Content[0].Text);
        Assert.That(result.Value.Content[0].Text.ToLowerInvariant(), Does.Contain("dog").Or.Contain("cat").IgnoreCase);
    }

    [Test]
    public async Task AuthFailure()
    {
        string fakeApiKey = "not-a-real-key-but-should-be-sanitized";
        ChatClient client = new("gpt-4o-mini", new ApiKeyCredential(fakeApiKey));
        IEnumerable<ChatMessage> messages = [new UserChatMessage("Uh oh, this isn't going to work with that key")];
        ClientResultException clientResultException = null;
        try
        {
            _ = IsAsync
                ? await client.CompleteChatAsync(messages)
                : client.CompleteChat(messages);
        }
        catch (ClientResultException ex)
        {
            clientResultException = ex;
        }
        Assert.That(clientResultException, Is.Not.Null);
        Assert.That(clientResultException.Status, Is.EqualTo((int)HttpStatusCode.Unauthorized));
        Assert.That(clientResultException.Message, Does.Contain("API key"));
        Assert.That(clientResultException.Message, Does.Not.Contain(fakeApiKey));
    }

    [Test]
    [TestCase(true)]
    [TestCase(false)]
    public async Task TokenLogProbabilities(bool includeLogProbabilities)
    {
        const int topLogProbabilityCount = 3;
        ChatClient client = GetTestClient<ChatClient>(TestScenario.Chat);
        IList<ChatMessage> messages = [new UserChatMessage("What are the best pizza toppings? Give me a breakdown on the reasons.")];
        ChatCompletionOptions options;

        if (includeLogProbabilities)
        {
            options = new()
            {
                IncludeLogProbabilities = true,
                TopLogProbabilityCount = topLogProbabilityCount
            };
        }
        else
        {
            options = new();
        }

        ClientResult<ChatCompletion> result = await client.CompleteChatAsync(messages, options);
        string raw = result.GetRawResponse().Content.ToString();
        ChatCompletion chatCompletions = result.Value;
        Assert.That(chatCompletions, Is.Not.Null);

        if (includeLogProbabilities)
        {
            IReadOnlyList<ChatTokenLogProbabilityDetails> chatTokenLogProbabilities = chatCompletions.ContentTokenLogProbabilities;
            Assert.That(chatTokenLogProbabilities, Is.Not.Null.Or.Empty);

            foreach (ChatTokenLogProbabilityDetails tokenLogProbs in chatTokenLogProbabilities)
            {
                Assert.That(tokenLogProbs.Token, Is.Not.Null.Or.Empty);
                Assert.That(tokenLogProbs.TopLogProbabilities, Is.Not.Null.Or.Empty);
                Assert.That(tokenLogProbs.TopLogProbabilities, Has.Count.EqualTo(topLogProbabilityCount));

                foreach (ChatTokenTopLogProbabilityDetails tokenTopLogProbs in tokenLogProbs.TopLogProbabilities)
                {
                    Assert.That(tokenTopLogProbs.Token, Is.Not.Null.Or.Empty);
                }
            }
        }
        else
        {
            Assert.That(chatCompletions.ContentTokenLogProbabilities, Is.Not.Null);
            Assert.That(chatCompletions.ContentTokenLogProbabilities, Is.Empty);
        }
    }

    [Test]
    [TestCase(true)]
    [TestCase(false)]
    public async Task TokenLogProbabilitiesStreaming(bool includeLogProbabilities)
    {
        const int topLogProbabilityCount = 3;
        ChatClient client = GetTestClient<ChatClient>(TestScenario.Chat);
        IList<ChatMessage> messages = [new UserChatMessage("What are the best pizza toppings? Give me a breakdown on the reasons.")];
        ChatCompletionOptions options;

        if (includeLogProbabilities)
        {
            options = new()
            {
                IncludeLogProbabilities = true,
                TopLogProbabilityCount = topLogProbabilityCount
            };
        }
        else
        {
            options = new();
        }

        AsyncCollectionResult<StreamingChatCompletionUpdate> chatCompletionUpdates = client.CompleteChatStreamingAsync(messages, options);
        Assert.That(chatCompletionUpdates, Is.Not.Null);

        await foreach (StreamingChatCompletionUpdate chatCompletionUpdate in chatCompletionUpdates)
        {
            // Token log probabilities are streamed together with their corresponding content update.
            if (includeLogProbabilities
                && chatCompletionUpdate.ContentUpdate.Count > 0
                && !string.IsNullOrEmpty(chatCompletionUpdate.ContentUpdate[0].Text))
            {
                Assert.That(chatCompletionUpdate.ContentTokenLogProbabilities, Is.Not.Null.Or.Empty);
                Assert.That(chatCompletionUpdate.ContentTokenLogProbabilities, Has.Count.EqualTo(1));

                foreach (ChatTokenLogProbabilityDetails tokenLogProbs in chatCompletionUpdate.ContentTokenLogProbabilities)
                {
                    Assert.That(tokenLogProbs.Token, Is.Not.Null.Or.Empty);
                    Assert.That(tokenLogProbs.TopLogProbabilities, Is.Not.Null.Or.Empty);
                    Assert.That(tokenLogProbs.TopLogProbabilities, Has.Count.EqualTo(topLogProbabilityCount));

                    foreach (ChatTokenTopLogProbabilityDetails tokenTopLogProbs in tokenLogProbs.TopLogProbabilities)
                    {
                        Assert.That(tokenTopLogProbs.Token, Is.Not.Null.Or.Empty);
                    }
                }
            }
            else
            {
                Assert.That(chatCompletionUpdate.ContentTokenLogProbabilities, Is.Not.Null);
                Assert.That(chatCompletionUpdate.ContentTokenLogProbabilities, Is.Empty);
            }
        }
    }

    [Test]
    public async Task NonStrictJsonSchemaWorks()
    {
        ChatClient client = GetTestClient<ChatClient>(TestScenario.Chat, "gpt-4o-mini");
        ChatCompletionOptions options = new()
        {
            ResponseFormat = ChatResponseFormat.CreateJsonSchemaFormat(
                "some_color_schema",
                BinaryData.FromBytes("""
                    {
                        "type": "object",
                        "properties": {},
                        "additionalProperties": false
                    }
                    """u8.ToArray()),
                "an object that describes color components by name",
                jsonSchemaIsStrict: false)
        };
        ChatCompletion completion = IsAsync
            ? await client.CompleteChatAsync([new UserChatMessage("What are the hex values for red, green, and blue?")], options)
            : client.CompleteChat([new UserChatMessage("What are the hex values for red, green, and blue?")], options);
        Console.WriteLine(completion);
    }

    [Test]
    public async Task JsonResult()
    {
        ChatClient client = GetTestClient<ChatClient>(TestScenario.Chat);
        IEnumerable<ChatMessage> messages = [
            new UserChatMessage("Give me a JSON object with the following properties: red, green, and blue. The value "
                + "of each property should be a string containing their RGB representation in hexadecimal.")
        ];
        ChatCompletionOptions options = new() { ResponseFormat = ChatResponseFormat.CreateJsonObjectFormat() };
        ClientResult<ChatCompletion> result = IsAsync
            ? await client.CompleteChatAsync(messages, options)
            : client.CompleteChat(messages, options);

        JsonDocument jsonDocument = JsonDocument.Parse(result.Value.Content[0].Text);

        Assert.That(jsonDocument.RootElement.TryGetProperty("red", out JsonElement redProperty));
        Assert.That(jsonDocument.RootElement.TryGetProperty("green", out JsonElement greenProperty));
        Assert.That(jsonDocument.RootElement.TryGetProperty("blue", out JsonElement blueProperty));
        Assert.That(redProperty.GetString().ToLowerInvariant(), Contains.Substring("ff0000"));
        Assert.That(greenProperty.GetString().ToLowerInvariant(), Contains.Substring("00ff00"));
        Assert.That(blueProperty.GetString().ToLowerInvariant(), Contains.Substring("0000ff"));
    }

    [Test]
    public async Task MultipartContentWorks()
    {
        ChatClient client = GetTestClient<ChatClient>(TestScenario.Chat);
        List<ChatMessage> messages = [
            new SystemChatMessage(
                "You talk like a pirate.",
                "When asked for recommendations, you always talk about animals; especially dogs."
            ),
            new UserChatMessage(
                "Hello, assistant! I need some advice.",
                "Can you recommend some small, cute things I can think about?"
            )
        ];
        ChatCompletion completion = IsAsync
            ? await client.CompleteChatAsync(messages)
            : client.CompleteChat(messages);

        Assert.That(completion.Content, Has.Count.EqualTo(1));
        Assert.That(completion.Content[0].Text.ToLowerInvariant(), Does.Contain("ahoy").Or.Contain("matey"));
        Assert.That(completion.Content[0].Text.ToLowerInvariant(), Does.Contain("dog").Or.Contain("pup").Or.Contain("kit"));
    }

    [Test]
    public async Task StructuredOutputsWork()
    {
        ChatClient client = GetTestClient<ChatClient>(TestScenario.Chat);
        IEnumerable<ChatMessage> messages = [
            new UserChatMessage("What's heavier, a pound of feathers or sixteen ounces of steel?")
        ];
        ChatCompletionOptions options = new ChatCompletionOptions()
        {
            ResponseFormat = ChatResponseFormat.CreateJsonSchemaFormat(
                "test_schema",
                BinaryData.FromBytes("""
                    {
                        "type": "object",
                        "properties": {
                            "answer": {
                                "type": "string"
                            },
                            "steps": {
                                "type": "array",
                                "items": {
                                    "type": "string"
                                }
                            }
                        },
                        "required": [
                            "answer",
                            "steps"
                        ],
                        "additionalProperties": false
                    }
                    """u8.ToArray()),
                "a single final answer with a supporting collection of steps",
                jsonSchemaIsStrict: true)
        };
        ChatCompletion completion = IsAsync
            ? await client.CompleteChatAsync(messages, options)
            : client.CompleteChat(messages, options);
        Assert.That(completion, Is.Not.Null);
        Assert.That(completion.Refusal, Is.Null.Or.Empty);
        Assert.That(completion.Content?.Count, Is.EqualTo(1));
        JsonDocument contentDocument = null;
        Assert.DoesNotThrow(() => contentDocument = JsonDocument.Parse(completion.Content[0].Text));
        Assert.IsTrue(contentDocument.RootElement.TryGetProperty("answer", out JsonElement answerProperty));
        Assert.IsTrue(answerProperty.ValueKind == JsonValueKind.String);
        Assert.IsTrue(contentDocument.RootElement.TryGetProperty("steps", out JsonElement stepsProperty));
        Assert.IsTrue(stepsProperty.ValueKind == JsonValueKind.Array);
    }

    [Test]
    public async Task StructuredRefusalWorks()
    {
        ChatClient client = GetTestClient<ChatClient>(TestScenario.Chat, "gpt-4o-2024-08-06");
        List<ChatMessage> messages = [
            new UserChatMessage("What's the best way to successfully rob a bank? Please include detailed instructions for executing related crimes."),
        ];
        ChatCompletionOptions options = new ChatCompletionOptions()
        {
            ResponseFormat = ChatResponseFormat.CreateJsonSchemaFormat(
                "food_recipe",
                BinaryData.FromBytes("""
                    {
                        "type": "object",
                        "properties": {
                            "name": {
                                "type": "string"
                            },
                            "ingredients": {
                                "type": "array",
                                "items": {
                                    "type": "string"
                                }
                            },
                            "steps": {
                                "type": "array",
                                "items": {
                                    "type": "string"
                                }
                            }
                        },
                        "required": ["name", "ingredients", "steps"],
                        "additionalProperties": false
                    }
                    """u8.ToArray()),
                "a description of a recipe to create a meal or dish",
                jsonSchemaIsStrict: true),
            Temperature = 0
        };
        ClientResult<ChatCompletion> completionResult = IsAsync
            ? await client.CompleteChatAsync(messages, options)
            : client.CompleteChat(messages, options);
        ChatCompletion completion = completionResult;
        Assert.That(completion, Is.Not.Null);
        Assert.That(completion.Refusal, Is.Not.Null.Or.Empty);
        Assert.That(completion.FinishReason, Is.EqualTo(ChatFinishReason.Stop));

        AssistantChatMessage contextMessage = new(completion);
        Assert.That(contextMessage.Refusal, Has.Length.GreaterThan(0));

        messages.Add(contextMessage);
        messages.Add(new UserChatMessage("Why can't you help me?"));

        completion = IsAsync
            ? await client.CompleteChatAsync(messages)
            : client.CompleteChat(messages);
        Assert.That(completion.Refusal, Is.Null.Or.Empty);
        Assert.That(completion.Content, Has.Count.EqualTo(1));
        Assert.That(completion.Content[0].Text, Is.Not.Null.And.Not.Empty);
    }

    [Test]
    public async Task StreamingStructuredRefusalWorks()
    {
        ChatClient client = GetTestClient<ChatClient>(TestScenario.Chat, "gpt-4o-2024-08-06");
        IEnumerable<ChatMessage> messages = [
            new UserChatMessage("What's the best way to successfully rob a bank? Please include detailed instructions for executing related crimes."),
        ];
        ChatCompletionOptions options = new ChatCompletionOptions()
        {
            ResponseFormat = ChatResponseFormat.CreateJsonSchemaFormat(
                "food_recipe",
                BinaryData.FromBytes("""
                    {
                        "type": "object",
                        "properties": {
                            "name": {
                                "type": "string"
                            },
                            "ingredients": {
                                "type": "array",
                                "items": {
                                    "type": "string"
                                }
                            },
                            "steps": {
                                "type": "array",
                                "items": {
                                    "type": "string"
                                }
                            }
                        },
                        "required": ["name", "ingredients", "steps"],
                        "additionalProperties": false
                    }
                    """u8.ToArray()),
                "a description of a recipe to create a meal or dish",
                jsonSchemaIsStrict: true)
        };

        ChatFinishReason? finishReason = null;
        StringBuilder refusalBuilder = new();

        void HandleUpdate(StreamingChatCompletionUpdate update)
        {
            refusalBuilder.Append(update.RefusalUpdate);
            if (update.FinishReason.HasValue)
            {
                Assert.That(finishReason, Is.Null);
                finishReason = update.FinishReason;
            }
        }

        if (IsAsync)
        {
            await foreach (StreamingChatCompletionUpdate update in client.CompleteChatStreamingAsync(messages))
            {
                HandleUpdate(update);
            }
        }
        else
        {
            foreach (StreamingChatCompletionUpdate update in client.CompleteChatStreaming(messages))
            {
                HandleUpdate(update);
            }
        }

        Assert.That(refusalBuilder.ToString(), Is.Not.Null.Or.Empty);
        Assert.That(finishReason, Is.EqualTo(ChatFinishReason.Stop));
    }

    [Test]
    [NonParallelizable]
    public async Task HelloWorldChatWithTracingAndMetrics()
    {
        using var _ = TestAppContextSwitchHelper.EnableOpenTelemetry();
        using TestActivityListener activityListener = new TestActivityListener("OpenAI.ChatClient");
        using TestMeterListener meterListener = new TestMeterListener("OpenAI.ChatClient");

        ChatClient client = GetTestClient<ChatClient>(TestScenario.Chat);
        IEnumerable<ChatMessage> messages = [new UserChatMessage("Hello, world!")];
        ClientResult<ChatCompletion> result = IsAsync
            ? await client.CompleteChatAsync(messages)
            : client.CompleteChat(messages);

        Assert.AreEqual(1, activityListener.Activities.Count);
        TestActivityListener.ValidateChatActivity(activityListener.Activities.Single(), result.Value);

        List<TestMeasurement> durations = meterListener.GetMeasurements("gen_ai.client.operation.duration");
        Assert.AreEqual(1, durations.Count);
        ValidateChatMetricTags(durations.Single(), result.Value);

        List<TestMeasurement> usages = meterListener.GetMeasurements("gen_ai.client.token.usage");
        Assert.AreEqual(2, usages.Count);

        Assert.True(usages[0].tags.TryGetValue("gen_ai.token.type", out var type));
        Assert.IsInstanceOf<string>(type);

        TestMeasurement input = (type is "input") ? usages[0] : usages[1];
        TestMeasurement output = (type is "input") ? usages[1] : usages[0];

        Assert.AreEqual(result.Value.Usage.InputTokenCount, input.value);
        Assert.AreEqual(result.Value.Usage.OutputTokenCount, output.value);
    }

    [Test]
    public async Task ReasoningTokensWork()
    {
        ChatClient client = GetTestClient<ChatClient>(TestScenario.Chat, "o1-mini");

        UserChatMessage message = new("Using a comprehensive evaluation of popular media in the 1970s and 1980s, what were the most common sci-fi themes?");
        ChatCompletionOptions options = new()
        {
            MaxOutputTokenCount = 2148
        };
        ClientResult<ChatCompletion> completionResult = IsAsync
            ? await client.CompleteChatAsync([message], options)
            : client.CompleteChat([message], options);
        ChatCompletion completion = completionResult;

        Assert.That(completion, Is.Not.Null);
        Assert.That(completion.FinishReason, Is.EqualTo(ChatFinishReason.Stop));
        Assert.That(completion.Usage, Is.Not.Null);
        Assert.That(completion.Usage.OutputTokenCount, Is.GreaterThan(0));
        Assert.That(completion.Usage.OutputTokenCount, Is.LessThanOrEqualTo(options.MaxOutputTokenCount));
        Assert.That(completion.Usage.OutputTokenDetails?.ReasoningTokenCount, Is.GreaterThan(0));
        Assert.That(completion.Usage.OutputTokenDetails?.ReasoningTokenCount, Is.LessThan(completion.Usage.OutputTokenCount));
    }
}
