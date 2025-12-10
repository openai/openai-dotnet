using Microsoft.ClientModel.TestFramework;
using NUnit.Framework;
using OpenAI.Responses;
using OpenAI.Tests.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using static OpenAI.Tests.TestHelpers;

namespace OpenAI.Tests.Responses;

#pragma warning disable OPENAICUA001

[Category("Responses")]
[Category("ResponsesStore")]
public partial class ResponseStoreTests : OpenAIRecordedTestBase
{
    public ResponseStoreTests(bool isAsync) : base(isAsync)
    {
    }

    [RecordedTest]
    public async Task GetInputItemsCollectionPage()
    {
        ResponsesClient client = GetTestClient();

        // Create a response with multiple input items
        List<ResponseItem> inputItems = new()
        {
            ResponseItem.CreateUserMessageItem("Item 1"),
            ResponseItem.CreateUserMessageItem("Item 2"),
            ResponseItem.CreateUserMessageItem("Item 3"),
            ResponseItem.CreateUserMessageItem("Item 4")
        };

        ResponseResult response = await client.CreateResponseAsync(new("gpt-4o-mini", inputItems));

        // Paginate through input items with a small page size
        var options = new ResponseItemCollectionOptions(response.Id)
        {
            PageSizeLimit = 2
        };

        ResponseItemCollectionPage page1 = await client.GetResponseInputItemCollectionPageAsync(options);

        Assert.That(page1.Data, Is.Not.Null);
        Assert.That(page1.Data, Has.Count.EqualTo(2));
        Assert.That(page1.FirstId, Is.Not.Null.And.Not.Empty);
        Assert.That(page1.LastId, Is.Not.Null.And.Not.Empty);
        Assert.That(page1.HasMore, Is.True);

        options.AfterId = page1.LastId;

        ResponseItemCollectionPage page2 = await client.GetResponseInputItemCollectionPageAsync(options);

        Assert.That(page2.Data, Is.Not.Null);
        Assert.That(page2.Data, Has.Count.EqualTo(2));
        Assert.That(page2.FirstId, Is.Not.Null.And.Not.Empty);
        Assert.That(page2.LastId, Is.Not.Null.And.Not.Empty);
        Assert.That(page2.HasMore, Is.False);
    }

    [RecordedTest]
    public async Task GetInputItemsWithPagination()
    {
        ResponsesClient client = GetTestClient();

        // Create a response with multiple input items
        List<ResponseItem> inputItems = new()
        {
            ResponseItem.CreateUserMessageItem("Item 1"),
            ResponseItem.CreateUserMessageItem("Item 2"),
            ResponseItem.CreateUserMessageItem("Item 3"),
            ResponseItem.CreateUserMessageItem("Item 4")
        };

        ResponseResult response = await client.CreateResponseAsync(new("gpt-4o-mini", inputItems));

        // Paginate through input items with a small page size
        var options = new ResponseItemCollectionOptions(response.Id)
        {
            PageSizeLimit = 2
        };

        int totalCount = 0;
        string lastId = null;

        await foreach (ResponseItem item in client.GetResponseInputItemsAsync(options))
        {
            totalCount++;
            lastId = item.Id;
            Assert.That(item.Id, Is.Not.Null.And.Not.Empty);
            if (totalCount >= 3) break; // Read more than a page to validate pagination
        }

        Assert.That(totalCount, Is.GreaterThanOrEqualTo(2));
        Assert.That(lastId, Is.Not.Null);
    }

    [RecordedTest]
    public async Task GetInputItemsWithPaginationNoOptions()
    {
        ResponsesClient client = GetTestClient();

        // Create a response with multiple input items
        List<ResponseItem> inputItems = new()
        {
            ResponseItem.CreateUserMessageItem("Item 1"),
            ResponseItem.CreateUserMessageItem("Item 2"),
            ResponseItem.CreateUserMessageItem("Item 3"),
            ResponseItem.CreateUserMessageItem("Item 4")
        };

        ResponseResult response = await client.CreateResponseAsync(new("gpt-4o-mini", inputItems));

        int totalCount = 0;
        string lastId = null;

        await foreach (ResponseItem item in client.GetResponseInputItemsAsync(response.Id))
        {
            Assert.That(item.Id, Is.Not.Null.And.Not.Empty);

            totalCount++;
            lastId = item.Id;
        }

        Assert.That(totalCount, Is.EqualTo(4));
        Assert.That(lastId, Is.Not.Null);
    }

    [RecordedTest]
    public async Task GetInputItemsWithMultiPartPagination()
    {
        ResponsesClient client = GetTestClient();

        string filePath = Path.Join("Assets", "files_travis_favorite_food.pdf");

        // Create a response with multiple input items
        List<ResponseItem> inputItems = new()
        {
            ResponseItem.CreateUserMessageItem([
                ResponseContentPart.CreateInputTextPart("What is input text?"),
                ResponseContentPart.CreateInputFilePart(BinaryData.FromBytes(File.ReadAllBytes(filePath)), "application/pdf", "test_favorite_foods.pdf"),
                ResponseContentPart.CreateInputImagePart(new Uri("https://upload.wikimedia.org/wikipedia/commons/c/c3/Openai.png")),
            ]),
            ResponseItem.CreateUserMessageItem("Item 2"),
            ResponseItem.CreateUserMessageItem("Item 3"),
            ResponseItem.CreateUserMessageItem("Item 4")
        };

        ResponseResult response = await client.CreateResponseAsync(new("gpt-4o-mini", inputItems));

        // Paginate through input items with a small page size
        var options = new ResponseItemCollectionOptions(response.Id)
        {
            PageSizeLimit = 2,
            Order = ResponseItemCollectionOrder.Ascending
        };

        int totalCount = 0;
        string lastId = null;
        bool hasMultipleContentParts = false;

        await foreach (ResponseItem item in client.GetResponseInputItemsAsync(options))
        {
            totalCount++;
            lastId = item.Id;
            Assert.That(item.Id, Is.Not.Null.And.Not.Empty);

            if (item is MessageResponseItem ri)
            {
                hasMultipleContentParts |= ri.Content.Count > 1;
            }

            if (totalCount >= 3) break; // Read more than a page to validate pagination
        }

        Assert.That(totalCount, Is.GreaterThanOrEqualTo(2));
        Assert.That(lastId, Is.Not.Null);
        Assert.That(hasMultipleContentParts, "Expected at least one message with multiple content parts.");
    }

    [RecordedTest]
    public async Task GetInputItemsWithAfterIdPagination()
    {
        ResponsesClient client = GetTestClient();

        // Ensure multiple input items exist to paginate
        List<ResponseItem> inputItems = new()
        {
            ResponseItem.CreateUserMessageItem("A"),
            ResponseItem.CreateUserMessageItem("B"),
            ResponseItem.CreateUserMessageItem("C")
        };

        ResponseResult response = await client.CreateResponseAsync(new("gpt-4o-mini", inputItems));

        string afterId = null;
        await foreach (ResponseItem first in client.GetResponseInputItemsAsync(new ResponseItemCollectionOptions(response.Id)))
        {
            afterId = first.Id;
            break;
        }

        Assert.That(afterId, Is.Not.Null);

        int count = 0;
        var options = new ResponseItemCollectionOptions(response.Id)
        {
            AfterId = afterId,
            PageSizeLimit = 2
        };

        await foreach (ResponseItem item in client.GetResponseInputItemsAsync(options))
        {
            count++;
            Assert.That(item.Id, Is.Not.EqualTo(afterId));
            if (count >= 2) break;
        }

        Assert.That(count, Is.GreaterThanOrEqualTo(0));
    }

    [RecordedTest]
    public async Task GetInputItemsWithOrderFiltering()
    {
        ResponsesClient client = GetTestClient();

        // Create inputs in a defined sequence
        List<ResponseItem> inputItems = new()
        {
            ResponseItem.CreateUserMessageItem("First"),
            ResponseItem.CreateUserMessageItem("Second")
        };

        ResponseResult response = await client.CreateResponseAsync(new("gpt-4o-mini", inputItems));

        // Ascending
        var ascOptions = new ResponseItemCollectionOptions(response.Id)
        {
            Order = ResponseItemCollectionOrder.Ascending,
            PageSizeLimit = 5
        };

        var asc = new List<ResponseItem>();
        await foreach (ResponseItem item in client.GetResponseInputItemsAsync(ascOptions))
        {
            asc.Add(item);
            if (asc.Count >= 2) break;
        }

        // Descending
        var descOptions = new ResponseItemCollectionOptions(response.Id)
        {
            Order = ResponseItemCollectionOrder.Descending,
            PageSizeLimit = 5
        };

        var desc = new List<ResponseItem>();
        await foreach (ResponseItem item in client.GetResponseInputItemsAsync(descOptions))
        {
            desc.Add(item);
            if (desc.Count >= 2) break;
        }

        Assert.That(asc, Has.Count.GreaterThan(0));
        Assert.That(desc, Has.Count.GreaterThan(0));
        Assert.That(asc[0].Id, Is.Not.Null.And.Not.Empty);
        Assert.That(desc[0].Id, Is.Not.Null.And.Not.Empty);
        Assert.That(asc[0].Id, Is.Not.EqualTo(desc[0].Id));
    }

    [RecordedTest]
    public async Task GetInputItemsHandlesLargeLimits()
    {
        ResponsesClient client = GetTestClient();

        ResponseResult response = await client.CreateResponseAsync(new(
            "gpt-4o-mini",
            [
                ResponseItem.CreateUserMessageItem("alpha"),
                ResponseItem.CreateUserMessageItem("beta"),
                ResponseItem.CreateUserMessageItem("gamma"),
            ]));

        var options = new ResponseItemCollectionOptions(response.Id) { PageSizeLimit = 100 };

        int count = 0;
        await foreach (ResponseItem item in client.GetResponseInputItemsAsync(options))
        {
            count++;
            Assert.That(item.Id, Is.Not.Null.And.Not.Empty);
            if (count >= 10) break;
        }

        Assert.That(count, Is.GreaterThan(0));
    }

    [RecordedTest]
    public async Task GetInputItemsWithMinimalLimits()
    {
        ResponsesClient client = GetTestClient();

        ResponseResult response = await client.CreateResponseAsync(new(
            "gpt-4o-mini",
            [
                ResponseItem.CreateUserMessageItem("x"),
                ResponseItem.CreateUserMessageItem("y"),
                ResponseItem.CreateUserMessageItem("z"),
            ]));

        var options = new ResponseItemCollectionOptions(response.Id) { PageSizeLimit = 1 };

        int count = 0;
        await foreach (ResponseItem item in client.GetResponseInputItemsAsync(options))
        {
            count++;
            Assert.That(item.Id, Is.Not.Null.And.Not.Empty);
            if (count >= 3) break;
        }

        Assert.That(count, Is.GreaterThan(0));
    }

    [RecordedTest]
    public async Task GetInputItemsWithCancellationToken()
    {
        ResponsesClient client = GetTestClient();

        ResponseResult response = await client.CreateResponseAsync(new(
            "gpt-4o-mini",
            [
                ResponseItem.CreateUserMessageItem("ct1"),
                ResponseItem.CreateUserMessageItem("ct2"),
                ResponseItem.CreateUserMessageItem("ct3"),
            ]));

        using var cts = new System.Threading.CancellationTokenSource();

        try
        {
            int count = 0;
            await foreach (ResponseItem item in client.GetResponseInputItemsAsync(new ResponseItemCollectionOptions(response.Id), cancellationToken: cts.Token))
            {
                count++;
                Assert.That(item.Id, Is.Not.Null.And.Not.Empty);
                if (count >= 1)
                {
                    cts.Cancel();
                    break;
                }
            }

            Assert.That(count, Is.GreaterThanOrEqualTo(1));
        }
        catch (OperationCanceledException)
        {
            // Expected if enumeration is canceled mid-stream
        }
    }

    [RecordedTest]
    public async Task GetInputItemsWithCombinedOptions()
    {
        ResponsesClient client = GetTestClient();

        ResponseResult response = await client.CreateResponseAsync(new(
            "gpt-4o-mini",
            [
                ResponseItem.CreateUserMessageItem("co1"),
                ResponseItem.CreateUserMessageItem("co2"),
                ResponseItem.CreateUserMessageItem("co3"),
            ]));

        using var cts = new System.Threading.CancellationTokenSource(TimeSpan.FromSeconds(30));

        var options = new ResponseItemCollectionOptions(response.Id)
        {
            PageSizeLimit = 2,
            Order = ResponseItemCollectionOrder.Descending
        };

        var items = new List<ResponseItem>();
        await foreach (ResponseItem item in client.GetResponseInputItemsAsync(options, cts.Token))
        {
            items.Add(item);
            Assert.That(item.Id, Is.Not.Null.And.Not.Empty);
            if (items.Count >= 3) break;
        }

        Assert.That(items, Has.Count.GreaterThan(0));
    }

    private ResponsesClient GetTestClient(string overrideModel = null) => GetProxiedOpenAIClient<ResponsesClient>(TestScenario.Responses, overrideModel);
}