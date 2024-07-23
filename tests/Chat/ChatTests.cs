using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using NUnit.Framework;
using OpenAI.Chat;
using OpenAI.Tests.Utility;
using System;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using static OpenAI.Tests.TestHelpers;

namespace OpenAI.Tests.Chat;

[TestFixture(true)]
[TestFixture(false)]
[Parallelizable(ParallelScope.All)]
[Category("Chat")]
public partial class ChatTests : SyncAsyncTestBase
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
        ChatClient client = GetTestClient<ChatClient>(TestScenario.Chat);
        IEnumerable<ChatMessage> messages = [
            new UserChatMessage("What are the best pizza toppings? Give me a breakdown on the reasons.")
        ];

        TimeSpan? firstTokenReceiptTime = null;
        TimeSpan? latestTokenReceiptTime = null;
        Stopwatch stopwatch = Stopwatch.StartNew();

        ResultCollection<StreamingChatCompletionUpdate> streamingResult = client.CompleteChatStreaming(messages);
        Assert.That(streamingResult, Is.InstanceOf<ResultCollection<StreamingChatCompletionUpdate>>());
        int updateCount = 0;

        foreach (StreamingChatCompletionUpdate chatUpdate in streamingResult)
        {
            firstTokenReceiptTime ??= stopwatch.Elapsed;
            latestTokenReceiptTime = stopwatch.Elapsed;
            Console.WriteLine(stopwatch.Elapsed.TotalMilliseconds);
            updateCount++;
        }
        Assert.That(updateCount, Is.GreaterThan(1));
        Assert.That(latestTokenReceiptTime - firstTokenReceiptTime > TimeSpan.FromMilliseconds(500));

        // Validate that network stream was disposed - this will show up as the
        // the raw response holding an empty content stream.
        PipelineResponse response = streamingResult.GetRawResponse();
        Assert.That(response.ContentStream.Length, Is.EqualTo(0));
    }

    [Test]
    public async Task StreamingChatAsync()
    {
        ChatClient client = GetTestClient<ChatClient>(TestScenario.Chat);
        IEnumerable<ChatMessage> messages = [
            new UserChatMessage("What are the best pizza toppings? Give me a breakdown on the reasons.")
        ];

        TimeSpan? firstTokenReceiptTime = null;
        TimeSpan? latestTokenReceiptTime = null;
        Stopwatch stopwatch = Stopwatch.StartNew();

        AsyncResultCollection<StreamingChatCompletionUpdate> streamingResult = client.CompleteChatStreamingAsync(messages);
        Assert.That(streamingResult, Is.InstanceOf<AsyncResultCollection<StreamingChatCompletionUpdate>>());
        int updateCount = 0;
        ChatTokenUsage usage = null;

        await foreach (StreamingChatCompletionUpdate chatUpdate in streamingResult)
        {
            firstTokenReceiptTime ??= stopwatch.Elapsed;
            latestTokenReceiptTime = stopwatch.Elapsed;
            usage ??= chatUpdate.Usage;
            Console.WriteLine(stopwatch.Elapsed.TotalMilliseconds);
            updateCount++;
        }
        Assert.That(updateCount, Is.GreaterThan(1));
        Assert.That(latestTokenReceiptTime - firstTokenReceiptTime > TimeSpan.FromMilliseconds(500));
        Assert.That(usage, Is.Not.Null);
        Assert.That(usage?.InputTokens, Is.GreaterThan(0));
        Assert.That(usage?.OutputTokens, Is.GreaterThan(0));
        Assert.That(usage.InputTokens + usage.OutputTokens, Is.EqualTo(usage.TotalTokens));

        // Validate that network stream was disposed - this will show up as the
        // the raw response holding an empty content stream.
        PipelineResponse response = streamingResult.GetRawResponse();
        Assert.That(response.ContentStream.Length, Is.EqualTo(0));
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
                ChatMessageContentPart.CreateTextMessageContentPart("Describe this image for me."),
                ChatMessageContentPart.CreateImageMessageContentPart(imageData, mediaType)),
        ];
        ChatCompletionOptions options = new() { MaxTokens = 2048 };

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

        ChatCompletion chatCompletions = await client.CompleteChatAsync(messages, options);
        Assert.That(chatCompletions, Is.Not.Null);

        if (includeLogProbabilities)
        {
            IReadOnlyList<ChatTokenLogProbabilityInfo> chatTokenLogProbabilities = chatCompletions.ContentTokenLogProbabilities;
            Assert.That(chatTokenLogProbabilities, Is.Not.Null.Or.Empty);

            foreach (ChatTokenLogProbabilityInfo tokenLogProbs in chatTokenLogProbabilities)
            {
                Assert.That(tokenLogProbs.Token, Is.Not.Null.Or.Empty);
                Assert.That(tokenLogProbs.Utf8ByteValues, Is.Not.Null);
                Assert.That(tokenLogProbs.TopLogProbabilities, Is.Not.Null.Or.Empty);
                Assert.That(tokenLogProbs.TopLogProbabilities, Has.Count.EqualTo(topLogProbabilityCount));

                foreach (ChatTokenTopLogProbabilityInfo tokenTopLogProbs in tokenLogProbs.TopLogProbabilities)
                {
                    Assert.That(tokenTopLogProbs.Token, Is.Not.Null.Or.Empty);
                    Assert.That(tokenTopLogProbs.Utf8ByteValues, Is.Not.Null);
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

        AsyncResultCollection<StreamingChatCompletionUpdate> chatCompletionUpdates = client.CompleteChatStreamingAsync(messages, options);
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

                foreach (ChatTokenLogProbabilityInfo tokenLogProbs in chatCompletionUpdate.ContentTokenLogProbabilities)
                {
                    Assert.That(tokenLogProbs.Token, Is.Not.Null.Or.Empty);
                    Assert.That(tokenLogProbs.Utf8ByteValues, Is.Not.Null);
                    Assert.That(tokenLogProbs.TopLogProbabilities, Is.Not.Null.Or.Empty);
                    Assert.That(tokenLogProbs.TopLogProbabilities, Has.Count.EqualTo(topLogProbabilityCount));

                    foreach (ChatTokenTopLogProbabilityInfo tokenTopLogProbs in tokenLogProbs.TopLogProbabilities)
                    {
                        Assert.That(tokenTopLogProbs.Token, Is.Not.Null.Or.Empty);
                        Assert.That(tokenTopLogProbs.Utf8ByteValues, Is.Not.Null);
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
    public async Task JsonResult()
    {
        ChatClient client = GetTestClient<ChatClient>(TestScenario.Chat);
        IEnumerable<ChatMessage> messages = [
            new UserChatMessage("Give me a JSON object with the following properties: red, green, and blue. The value "
                + "of each property should be a string containing their RGB representation in hexadecimal.")
        ];
        ChatCompletionOptions options = new() { ResponseFormat = ChatResponseFormat.JsonObject };
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
}
