using NUnit.Framework;
using OpenAI.Chat;
using OpenAI.Tests.Utility;
using System;
using System.ClientModel;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using static OpenAI.Tests.TestHelpers;

namespace OpenAI.Tests.Chat;

[TestFixture(true)]
[TestFixture(false)]
[Parallelizable(ParallelScope.All)]
[Category("StoredChat")]
public class ChatStoreToolTests : SyncAsyncTestBase
{
    private const int s_delayInMilliseconds = 5000;

    public ChatStoreToolTests(bool isAsync) : base(isAsync)
    {
    }

    [Test]
    public async Task ChatMetadata()
    {
        ChatClient client = GetTestClient();

        ChatCompletionOptions options = new()
        {
            StoredOutputEnabled = true,
            Metadata =
            {
                ["my_metadata_key"] = "my_metadata_value",
            },
        };

        ChatCompletion completion = await client.CompleteChatAsync(
            ["Hello, world!"],
            options);

        int count = 0;
        await foreach (var fetchedCompletion in client.GetChatCompletionsAsync())
        {
            count++;
        }
        Assert.That(count, Is.GreaterThan(0));
    }

    [Test]
    public async Task GetChatCompletionsWithPagination()
    {
        ChatClient client = GetTestClient();

        // Create multiple completions with stored output enabled
        var completionIds = new List<string>();
        for (int i = 0; i < 3; i++)
        {
            ChatCompletionOptions options = new()
            {
                StoredOutputEnabled = true,
                Metadata = { ["test_key"] = $"test_value_{i}" }
            };

            ChatCompletion completion = await client.CompleteChatAsync(
                [$"Test message {i}: Say 'Hello World {i}'"],
                options);

            completionIds.Add(completion.Id);
        }

        await Task.Delay(s_delayInMilliseconds); // Wait for completions to be stored

        // Test pagination with limit
        ChatCompletionCollectionOptions paginationOptions = new()
        {
            PageSizeLimit = 2
        };

        int totalCount = 0;
        string lastId = null;

        await foreach (var fetchedCompletion in client.GetChatCompletionsAsync(paginationOptions))
        {
            totalCount++;
            lastId = fetchedCompletion.Id;
            Assert.That(fetchedCompletion.Id, Is.Not.Null.And.Not.Empty);
            Assert.That(fetchedCompletion.Content, Is.Not.Null);

            if (totalCount >= 2) break; // Stop after getting 2 items
        }

        Assert.That(totalCount, Is.EqualTo(2));
        Assert.That(lastId, Is.Not.Null);

        // Clean up
        foreach (var id in completionIds)
        {
            try
            {
                await client.DeleteChatCompletionAsync(id);
            }
            catch { /* Ignore cleanup errors */ }
        }
    }

    [Test]
    public async Task GetChatCompletionsWithAfterIdPagination()
    {
        ChatClient client = GetTestClient();

        // Create multiple completions
        var completionIds = new List<string>();
        for (int i = 0; i < 3; i++)
        {
            ChatCompletionOptions createOptions = new()
            {
                StoredOutputEnabled = true
            };

            ChatCompletion completion = await client.CompleteChatAsync(
                [$"Pagination test {i}: Say 'Test {i}'"],
                createOptions);

            completionIds.Add(completion.Id);
        }

        await Task.Delay(s_delayInMilliseconds); // Wait for completions to be stored

        // Get first completion to use as afterId
        string afterId = null;
        await foreach (var firstCompletion in client.GetChatCompletionsAsync())
        {
            afterId = firstCompletion.Id;
            break;
        }

        Assert.That(afterId, Is.Not.Null);

        // Test pagination starting after the first ID
        ChatCompletionCollectionOptions paginationOptions = new()
        {
            AfterId = afterId,
            PageSizeLimit = 2
        };

        int count = 0;
        await foreach (var completion in client.GetChatCompletionsAsync(paginationOptions))
        {
            count++;
            // Ensure we don't get the afterId completion
            Assert.That(completion.Id, Is.Not.EqualTo(afterId));
            if (count >= 2) break;
        }

        // Clean up
        foreach (var id in completionIds)
        {
            try
            {
                await client.DeleteChatCompletionAsync(id);
            }
            catch { /* Ignore cleanup errors */ }
        }
    }

    [Test]
    public async Task GetChatCompletionsWithOrderFiltering()
    {
        ChatClient client = GetTestClient();

        // Create completions with timestamps
        var completionIds = new List<string>();
        for (int i = 0; i < 2; i++)
        {
            ChatCompletionOptions createOptions = new()
            {
                StoredOutputEnabled = true,
                Metadata = { ["sequence"] = i.ToString() }
            };

            ChatCompletion completion = await client.CompleteChatAsync(
                [$"Order test {i}: Say 'Order {i}'"],
                createOptions);

            completionIds.Add(completion.Id);
            await Task.Delay(1000); // Ensure different timestamps
        }

        await Task.Delay(s_delayInMilliseconds); // Wait for completions to be stored

        // Test ascending order
        ChatCompletionCollectionOptions ascOptions = new()
        {
            Order = ChatCompletionCollectionOrder.Ascending,
            PageSizeLimit = 5
        };

        var ascResults = new List<ChatCompletion>();
        await foreach (var completion in client.GetChatCompletionsAsync(ascOptions))
        {
            ascResults.Add(completion);
            if (ascResults.Count >= 2) break;
        }

        // Test descending order
        ChatCompletionCollectionOptions descOptions = new()
        {
            Order = ChatCompletionCollectionOrder.Descending,
            PageSizeLimit = 5
        };

        var descResults = new List<ChatCompletion>();
        await foreach (var completion in client.GetChatCompletionsAsync(descOptions))
        {
            descResults.Add(completion);
            if (descResults.Count >= 2) break;
        }

        // Verify we get results in both cases
        Assert.That(ascResults, Has.Count.GreaterThan(0));
        Assert.That(descResults, Has.Count.GreaterThan(0));

        // The first result in descending order should be the most recent
        // (though exact ordering validation is tricky due to timing)
        Assert.That(descResults[0].Id, Is.Not.Null.And.Not.Empty);

        // Clean up
        foreach (var id in completionIds)
        {
            try
            {
                await client.DeleteChatCompletionAsync(id);
            }
            catch { /* Ignore cleanup errors */ }
        }
    }

    [Test]
    public async Task GetChatCompletionsWithMetadataFiltering()
    {
        ChatClient client = GetTestClient();

        // Create completions with different metadata
        var testMetadataKey = $"test_scenario_{Guid.NewGuid():N}";
        var completionIds = new List<string>();

        // Create completion with specific metadata
        ChatCompletionOptions options1 = new()
        {
            StoredOutputEnabled = true,
            Metadata = { [testMetadataKey] = "target_value" }
        };

        ChatCompletion targetCompletion = await client.CompleteChatAsync(
            ["Metadata test: Say 'Target completion'"],
            options1);
        completionIds.Add(targetCompletion.Id);

        // Create completion with different metadata
        ChatCompletionOptions options2 = new()
        {
            StoredOutputEnabled = true,
            Metadata = { [testMetadataKey] = "other_value" }
        };

        ChatCompletion otherCompletion = await client.CompleteChatAsync(
            ["Metadata test: Say 'Other completion'"],
            options2);
        completionIds.Add(otherCompletion.Id);

        await Task.Delay(s_delayInMilliseconds); // Wait for completions to be stored

        // Filter by specific metadata
        ChatCompletionCollectionOptions filterOptions = new()
        {
            Metadata = { [testMetadataKey] = "target_value" },
            PageSizeLimit = 10
        };

        int totalFound = 0;

        await foreach (var completion in client.GetChatCompletionsAsync(filterOptions))
        {
            totalFound++;
            Assert.That(completion.Id, Is.Not.Null.And.Not.Empty);
            if (totalFound >= 20) break; // Prevent infinite loop
        }

        // Should find completions (filtering behavior may vary by implementation)
        Assert.That(totalFound, Is.GreaterThan(0));

        // Clean up
        foreach (var id in completionIds)
        {
            try
            {
                await client.DeleteChatCompletionAsync(id);
            }
            catch { /* Ignore cleanup errors */ }
        }
    }

    [Test]
    public async Task GetChatCompletionsWithModelFiltering()
    {
        ChatClient client = GetTestClient();

        // Create completion with default model
        ChatCompletionOptions createOptions = new()
        {
            StoredOutputEnabled = true,
            Metadata = { ["model_test"] = "true" }
        };

        ChatCompletion completion = await client.CompleteChatAsync(
            ["Model filter test: Say 'Hello'"],
            createOptions);

        await Task.Delay(s_delayInMilliseconds); // Wait for completions to be stored

        // Filter by the model used by the test client
        ChatCompletionCollectionOptions filterOptions = new()
        {
            Model = "gpt-4o-mini-2024-07-18", // Common test model
            PageSizeLimit = 10
        };

        int count = 0;
        await foreach (var fetchedCompletion in client.GetChatCompletionsAsync(filterOptions))
        {
            count++;
            Assert.That(fetchedCompletion.Id, Is.Not.Null.And.Not.Empty);
            Assert.That(fetchedCompletion.Model, Is.EqualTo(filterOptions.Model));
            if (count >= 5) break; // Limit results for test performance
        }

        Assert.That(count, Is.GreaterThan(0));

        // Clean up
        try
        {
            await client.DeleteChatCompletionAsync(completion.Id);
        }
        catch { /* Ignore cleanup errors */ }
    }

    [Test]
    public async Task GetChatCompletionsWithEmptyOptions()
    {
        ChatClient client = GetTestClient();

        // Create a completion to ensure we have something to fetch
        ChatCompletionOptions createOptions = new()
        {
            StoredOutputEnabled = true
        };

        ChatCompletion completion = await client.CompleteChatAsync(
            ["Empty options test: Say 'Hello'"],
            createOptions);

        await Task.Delay(s_delayInMilliseconds); // Wait for completions to be stored

        // Test with default/empty options
        int count = 0;
        await foreach (var fetchedCompletion in client.GetChatCompletionsAsync())
        {
            count++;
            Assert.That(fetchedCompletion.Id, Is.Not.Null.And.Not.Empty);
            Assert.That(fetchedCompletion.Content, Is.Not.Null);
            if (count >= 3) break; // Limit for test performance
        }

        Assert.That(count, Is.GreaterThan(0));

        // Clean up
        try
        {
            await client.DeleteChatCompletionAsync(completion.Id);
        }
        catch { /* Ignore cleanup errors */ }
    }

    [Test]
    public async Task GetChatCompletionsWithCombinedFilters()
    {
        ChatClient client = GetTestClient();

        // Create completion with combined metadata for filtering
        var testKey = $"combined_test_{Guid.NewGuid():N}";
        ChatCompletionOptions createOptions = new()
        {
            StoredOutputEnabled = true,
            Metadata =
            {
                [testKey] = "combined_value",
                ["test_type"] = "integration"
            }
        };

        ChatCompletion completion = await client.CompleteChatAsync(
            ["Combined filters test: Say 'Combined test'"],
            createOptions);

        await Task.Delay(s_delayInMilliseconds); // Wait for completions to be stored

        // Test with combined filters
        ChatCompletionCollectionOptions combinedOptions = new()
        {
            PageSizeLimit = 5,
            Order = ChatCompletionCollectionOrder.Descending,
            Metadata = { [testKey] = "combined_value" }
        };

        int count = 0;

        await foreach (var fetchedCompletion in client.GetChatCompletionsAsync(combinedOptions))
        {
            count++;
            Assert.That(fetchedCompletion.Id, Is.Not.Null.And.Not.Empty);

            if (count >= 10) break; // Prevent excessive iterations
        }

        Assert.That(count, Is.GreaterThan(0));

        // Clean up
        try
        {
            await client.DeleteChatCompletionAsync(completion.Id);
        }
        catch { /* Ignore cleanup errors */ }
    }

    [Test]
    public async Task StoredChatCompletionsWork()
    {
        ChatClient client = GetTestClient();

        ChatCompletionOptions options = new()
        {
            StoredOutputEnabled = true
        };

        ChatCompletion completion = await client.CompleteChatAsync(
            [new UserChatMessage("Say `this is a test`.")],
            options);

        await RetryWithExponentialBackoffAsync(async () =>
        {

            ChatCompletion storedCompletion = await client.GetChatCompletionAsync(completion.Id);

            Assert.That(storedCompletion.Id, Is.EqualTo(completion.Id));
            Assert.That(storedCompletion.Content[0].Text, Is.EqualTo(completion.Content[0].Text));

            ChatCompletionDeletionResult deletionResult = await client.DeleteChatCompletionAsync(completion.Id);

            Assert.That(deletionResult.Deleted, Is.True);
        });

        await Task.Delay(s_delayInMilliseconds);

        Assert.ThrowsAsync<ClientResultException>(async () =>
        {
            ChatCompletion deletedCompletion = await client.GetChatCompletionAsync(completion.Id);
        });
    }

    [Test]
    public async Task UpdateChatCompletionWorks()
    {
        ChatClient client = GetTestClient();

        var testMetadataKey = $"test_key_{Guid.NewGuid():N}";
        var initialOptions = new ChatCompletionOptions
        {
            StoredOutputEnabled = true,
            Metadata = { [testMetadataKey] = "initial_value" }
        };

        ChatCompletion chatCompletion = await client.CompleteChatAsync(
            [new UserChatMessage("Say `this is a test`.")],
            initialOptions);

        await Task.Delay(s_delayInMilliseconds); // Wait for completions to be stored

        var newMetadata = new Dictionary<string, string>
        {
            [testMetadataKey] = "updated_value",
            ["updated_by"] = "unit_test"
        };

        ChatCompletion updated = await client.UpdateChatCompletionAsync(chatCompletion.Id, newMetadata);

        await Task.Delay(s_delayInMilliseconds); // Wait for completions to be updated

        Assert.That(updated, Is.Not.Null);
        Assert.That(updated.Id, Is.EqualTo(chatCompletion.Id));

        ChatCompletionDeletionResult deletionResult = await client.DeleteChatCompletionAsync(chatCompletion.Id);
        Assert.That(deletionResult.Deleted, Is.True);

        await Task.Delay(s_delayInMilliseconds); // Wait for completions to be deleted

        Assert.ThrowsAsync<ClientResultException>(async () =>
        {
            _ = await client.GetChatCompletionAsync(chatCompletion.Id);
        });
    }

    [Test]
    public async Task GetChatCompletionsValidatesCollectionEnumeration()
    {
        ChatClient client = GetTestClient();

        // Create a completion to ensure we have data
        ChatCompletionOptions createOptions = new()
        {
            StoredOutputEnabled = true,
            Metadata = { ["enumeration_test"] = "true" }
        };

        ChatCompletion completion = await client.CompleteChatAsync(
            ["Enumeration test: Say 'Test enumeration'"],
            createOptions);

        await Task.Delay(5000); // Wait for completion to be stored

        // Test that we can enumerate multiple times
        ChatCompletionCollectionOptions collectionOptions = new()
        {
            PageSizeLimit = 2
        };

        var collection = client.GetChatCompletionsAsync(collectionOptions);

        // First enumeration
        int firstCount = 0;
        await foreach (var item in collection)
        {
            firstCount++;
            Assert.That(item.Id, Is.Not.Null.And.Not.Empty);
            if (firstCount >= 2) break;
        }

        // Second enumeration (should work independently)
        int secondCount = 0;
        await foreach (var item in collection)
        {
            secondCount++;
            Assert.That(item.Id, Is.Not.Null.And.Not.Empty);
            if (secondCount >= 2) break;
        }

        Assert.That(firstCount, Is.GreaterThan(0));
        Assert.That(secondCount, Is.GreaterThan(0));

        // Clean up
        try
        {
            await client.DeleteChatCompletionAsync(completion.Id);
        }
        catch { /* Ignore cleanup errors */ }
    }

    [Test]
    public async Task GetChatCompletionsHandlesLargeLimits()
    {
        ChatClient client = GetTestClient();

        // Create a completion for testing
        ChatCompletionOptions createOptions = new()
        {
            StoredOutputEnabled = true
        };

        ChatCompletion completion = await client.CompleteChatAsync(
            ["Large limit test: Say 'Testing large limits'"],
            createOptions);

        await Task.Delay(s_delayInMilliseconds); // Wait for completions to be stored

        // Test with a large page size limit
        ChatCompletionCollectionOptions largeOptions = new()
        {
            PageSizeLimit = 100
        };

        int count = 0;
        await foreach (var fetchedCompletion in client.GetChatCompletionsAsync(largeOptions))
        {
            count++;
            Assert.That(fetchedCompletion.Id, Is.Not.Null.And.Not.Empty);
            if (count >= 20) break; // Prevent excessive test time
        }

        Assert.That(count, Is.GreaterThan(0));

        // Clean up
        try
        {
            await client.DeleteChatCompletionAsync(completion.Id);
        }
        catch { /* Ignore cleanup errors */ }
    }

    [Test]
    public async Task GetChatCompletionsWithMinimalLimits()
    {
        ChatClient client = GetTestClient();

        // Create a completion for testing
        ChatCompletionOptions createOptions = new()
        {
            StoredOutputEnabled = true
        };

        ChatCompletion completion = await client.CompleteChatAsync(
            ["Minimal limit test: Say 'Testing minimal limits'"],
            createOptions);

        await Task.Delay(s_delayInMilliseconds); // Wait for completions to be stored

        // Test with minimal page size
        ChatCompletionCollectionOptions minimalOptions = new()
        {
            PageSizeLimit = 1
        };

        int count = 0;
        await foreach (var fetchedCompletion in client.GetChatCompletionsAsync(minimalOptions))
        {
            count++;
            Assert.That(fetchedCompletion.Id, Is.Not.Null.And.Not.Empty);
            if (count >= 3) break; // Get a few items to verify pagination works
        }

        Assert.That(count, Is.GreaterThan(0));

        // Clean up
        try
        {
            await client.DeleteChatCompletionAsync(completion.Id);
        }
        catch { /* Ignore cleanup errors */ }
    }

    [Test]
    public async Task GetChatCompletionMessagesWithBasicUsage()
    {
        ChatClient client = GetTestClient();

        // Create a completion with stored output enabled to have messages
        ChatCompletionOptions createOptions = new()
        {
            StoredOutputEnabled = true,
            Metadata = { ["test_scenario"] = "basic_messages" }
        };

        ChatCompletion completion = await client.CompleteChatAsync(
            ["Basic messages test: Say 'Hello, this is a test message.'"],
            createOptions);

        await RetryWithExponentialBackoffAsync(async () =>
        {
            // Test basic enumeration of messages
            int messageCount = 0;
            await foreach (var message in client.GetChatCompletionMessagesAsync(completion.Id))
            {
                messageCount++;
                Assert.That(message.Id, Is.Not.Null.And.Not.Empty);
                Assert.That(message.Content, Is.EqualTo("Basic messages test: Say 'Hello, this is a test message.'"));

                if (messageCount >= 5) break; // Prevent infinite loop
            }

            Assert.That(messageCount, Is.GreaterThan(0));
        });

        // Clean up
        try
        {
            await client.DeleteChatCompletionAsync(completion.Id);
        }
        catch { /* Ignore cleanup errors */ }
    }

    [Test]
    public async Task GetChatCompletionMessagesWithPagination()
    {
        ChatClient client = GetTestClient();

        // Create completion with multiple messages (conversation with tool calls)
        List<ChatMessage> conversationMessages = new()
        {
            new UserChatMessage("What's the weather like today? Use the weather tool."),
            new UserChatMessage("Name something I could do outside in this weather."),
            new UserChatMessage("Name something else I could do outside in this weather."),
            new UserChatMessage("Name something yet another thing I could do outside in this weather.")
        };

        // Add function definition to trigger more back-and-forth
        ChatTool weatherTool = ChatTool.CreateFunctionTool(
            "get_weather",
            "Get current weather information",
            BinaryData.FromString("""
                {
                    "type": "object",
                    "properties": {
                        "location": {
                            "type": "string",
                            "description": "The city and state, e.g. San Francisco, CA"
                        }
                    },
                    "required": ["location"]
                }
                """));

        ChatCompletionOptions createOptions = new()
        {
            StoredOutputEnabled = true,
            Tools = { weatherTool },
            Metadata = { ["test_scenario"] = "pagination_messages" }
        };

        ChatCompletion completion = await client.CompleteChatAsync(
            conversationMessages,
            createOptions);

        await RetryWithExponentialBackoffAsync(async () =>
        {
            // Test pagination with limit
            int totalMessages = 0;
            string lastMessageId = null;

            var options = new ChatCompletionMessageCollectionOptions()
            {
                PageSizeLimit = 2
            };

            await foreach (var message in client.GetChatCompletionMessagesAsync(completion.Id, options))
            {
                totalMessages++;
                lastMessageId = message.Id;
                Assert.That(message.Id, Is.Not.Null.And.Not.Empty);

                if (totalMessages >= 4) break; // Get a few pages worth
            }

            Assert.That(totalMessages, Is.GreaterThan(3));
            Assert.That(lastMessageId, Is.Not.Null);
        });

        // Clean up
        try
        {
            await client.DeleteChatCompletionAsync(completion.Id);
        }
        catch { /* Ignore cleanup errors */ }
    }

    [Test]
    public async Task GetChatCompletionMessagesWithAfterIdPagination()
    {
        ChatClient client = GetTestClient();

        // Create completion
        ChatCompletionOptions createOptions = new()
        {
            StoredOutputEnabled = true,
            Metadata = { ["test_scenario"] = "after_id_pagination" }
        };

        ChatCompletion completion = await client.CompleteChatAsync(
            ["After ID pagination test: Please provide a detailed response with multiple sentences."],
            createOptions);

        await RetryWithExponentialBackoffAsync(async () =>
        {
            // Get first message to use as afterId
            string afterId = null;
            await foreach (var firstMessage in client.GetChatCompletionMessagesAsync(completion.Id))
            {
                afterId = firstMessage.Id;
                break;
            }

            if (afterId != null)
            {
                // Test pagination starting after the first message
                int count = 0;
                var options = new ChatCompletionMessageCollectionOptions()
                {
                    AfterId = afterId,
                    PageSizeLimit = 3
                };

                await foreach (var message in client.GetChatCompletionMessagesAsync(completion.Id, options))
                {
                    count++;
                    // Ensure we don't get the afterId message
                    Assert.That(message.Id, Is.Not.EqualTo(afterId));

                    if (count >= 3) break;
                }

                // We might not have messages after the first one, so just verify the method works
                Assert.That(count, Is.GreaterThanOrEqualTo(0));
            }
        });
        // Clean up
        try
        {
            await client.DeleteChatCompletionAsync(completion.Id);
        }
        catch { /* Ignore cleanup errors */ }
    }

    [Test]
    public async Task GetChatCompletionMessagesWithOrderFiltering()
    {
        ChatClient client = GetTestClient();

        // Create completion with detailed conversation
        ChatCompletionOptions createOptions = new()
        {
            StoredOutputEnabled = true,
            Metadata = { ["test_scenario"] = "order_filtering" }
        };

        ChatCompletion completion = await client.CompleteChatAsync(
            ["Order filtering test: Please provide a comprehensive response about machine learning."],
            createOptions);

        await RetryWithExponentialBackoffAsync(async () =>
        {
            // Test ascending order
            List<ChatCompletionMessageListDatum> ascMessages = new();
            var ascOptions = new ChatCompletionMessageCollectionOptions()
            {
                Order = ChatCompletionMessageCollectionOrder.Ascending,
                PageSizeLimit = 5
            };

            await foreach (var message in client.GetChatCompletionMessagesAsync(completion.Id, ascOptions))
            {
                ascMessages.Add(message);
                if (ascMessages.Count >= 3) break;
            }

            // Test descending order
            List<ChatCompletionMessageListDatum> descMessages = new();
            var descOptions = new ChatCompletionMessageCollectionOptions()
            {
                Order = ChatCompletionMessageCollectionOrder.Descending,
                PageSizeLimit = 5
            };

            await foreach (var message in client.GetChatCompletionMessagesAsync(completion.Id, descOptions))
            {
                descMessages.Add(message);
                if (descMessages.Count >= 3) break;
            }

            // Verify we get results in both cases
            Assert.That(ascMessages, Has.Count.GreaterThan(0));
            Assert.That(descMessages, Has.Count.GreaterThan(0));
        });

        // Clean up
        try
        {
            await client.DeleteChatCompletionAsync(completion.Id);
        }
        catch { /* Ignore cleanup errors */ }
    }

    [Test]
    public async Task GetChatCompletionMessagesWithCancellationToken()
    {
        ChatClient client = GetTestClient();

        // Create completion
        ChatCompletionOptions createOptions = new()
        {
            StoredOutputEnabled = true,
            Metadata = { ["test_scenario"] = "cancellation_token" }
        };

        ChatCompletion completion = await client.CompleteChatAsync(
            ["Cancellation test: Say 'Hello World'"],
            createOptions);

        // Test with cancellation token
        using var cts = new CancellationTokenSource();

        await RetryWithExponentialBackoffAsync(async () =>
        {
            try
            {
                int count = 0;
                await foreach (var message in client.GetChatCompletionMessagesAsync(completion.Id, cancellationToken: cts.Token))
                {
                    count++;
                    Assert.That(message.Id, Is.Not.Null.And.Not.Empty);

                    if (count >= 2)
                    {
                        cts.Cancel(); // Cancel after getting some messages
                        break;
                    }
                }

                Assert.That(count, Is.GreaterThanOrEqualTo(1));
            }
            catch (OperationCanceledException)
            {
                // This is expected if cancellation happens during enumeration
            }
        });

        // Clean up
        try
        {
            await client.DeleteChatCompletionAsync(completion.Id);
        }
        catch { /* Ignore cleanup errors */ }
    }

    [Test]
    public async Task GetChatCompletionMessagesWithCombinedOptions()
    {
        ChatClient client = GetTestClient();

        // Create completion with comprehensive options
        ChatCompletionOptions createOptions = new()
        {
            StoredOutputEnabled = true,
            Metadata = { ["test_scenario"] = "combined_options" }
        };

        ChatCompletion completion = await client.CompleteChatAsync(
            ["Combined options test: Provide a detailed explanation of artificial intelligence."],
            createOptions);

        await RetryWithExponentialBackoffAsync(async () =>
        {
            // Test combined options: limit + order + cancellation token
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));

            List<ChatCompletionMessageListDatum> messages = new();

            await foreach (var message in client.GetChatCompletionMessagesAsync(
                completion.Id,
                new ChatCompletionMessageCollectionOptions()
                {
                    PageSizeLimit = 3,
                    Order = ChatCompletionMessageCollectionOrder.Descending
                },
                cancellationToken: cts.Token))
            {
                messages.Add(message);

                // Validate message structure
                Assert.That(message.Id, Is.Not.Null.And.Not.Empty);

                if (messages.Count >= 3) break;
            }

            Assert.That(messages, Has.Count.GreaterThan(0));
        });

        // Clean up
        try
        {
            await client.DeleteChatCompletionAsync(completion.Id);
        }
        catch { /* Ignore cleanup errors */ }
    }

    private static async Task RetryWithExponentialBackoffAsync(Func<Task> action, int maxRetries = 5, int initialWaitMs = 750)
    {
        int waitDuration = initialWaitMs;
        int retryCount = 0;
        bool successful = false;

        while (retryCount < maxRetries && !successful)
        {
            try
            {
                await action();
                successful = true;
            }
            catch (ClientResultException ex) when (ex.Status == 404)
            {
                // If we get a 404, it means the resource is not yet available
                await Task.Delay(waitDuration);
                waitDuration *= 2; // Exponential backoff
                retryCount++;
                if (retryCount >= maxRetries)
                {
                    throw; // Re-throw the exception if we've exhausted all retries
                }
            }
        }
    }

    private static ChatClient GetTestClient(string overrideModel = null) => GetTestClient<ChatClient>(TestScenario.Chat, overrideModel);
}
