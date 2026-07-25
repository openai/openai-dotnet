using Microsoft.ClientModel.TestFramework.Mocks;
using NUnit.Framework;
using OpenAI.Conversations;
using System;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.Linq;

namespace OpenAI.Tests.Conversations;

#pragma warning disable OPENAI001

[Category("Conversations")]
[Category("Smoke")]
public class ConversationMockTests
{
    private static readonly ApiKeyCredential s_fakeCredential = new("sk-not-a-real-key");
    private static readonly IncludedConversationItemProperty[] s_includedProperties =
    [
        IncludedConversationItemProperty.MessageInputImageUri,
        IncludedConversationItemProperty.WebSearchCallActionSources,
    ];

    [Test]
    public void GetConversationItemsExplodesIncludeQuery()
    {
        (ConversationClient client, Func<Uri> getRequestUri) = CreateClient();

        _ = client.GetConversationItems("conversation_123", include: s_includedProperties)
            .GetRawPages()
            .First();

        AssertIncludeQuery(getRequestUri(), "/conversations/conversation_123/items");
    }

    [Test]
    public void CreateConversationItemsExplodesIncludeQuery()
    {
        (ConversationClient client, Func<Uri> getRequestUri) = CreateClient();

        _ = client.CreateConversationItems(
            "conversation_123",
            BinaryContent.Create(BinaryData.FromString("""{"items":[]}""")),
            include: s_includedProperties);

        AssertIncludeQuery(getRequestUri(), "/conversations/conversation_123/items");
    }

    [Test]
    public void GetConversationItemExplodesIncludeQuery()
    {
        (ConversationClient client, Func<Uri> getRequestUri) = CreateClient();

        _ = client.GetConversationItem(
            "conversation_123",
            "item_123",
            include: s_includedProperties);

        AssertIncludeQuery(getRequestUri(), "/conversations/conversation_123/items/item_123");
    }

    private static (ConversationClient Client, Func<Uri> GetRequestUri) CreateClient()
    {
        Uri requestUri = null;
        MockPipelineResponse response = new(200);
        OpenAIClientOptions options = new()
        {
            Endpoint = new Uri("https://example.invalid/v1"),
            Transport = new MockPipelineTransport(message =>
            {
                requestUri = message.Request.Uri;
                return response;
            }),
        };

        return (new ConversationClient(s_fakeCredential, options), () => requestUri);
    }

    private static void AssertIncludeQuery(Uri requestUri, string path)
    {
        Assert.That(requestUri, Is.Not.Null);
        Assert.That(
            requestUri.AbsoluteUri,
            Is.EqualTo(
                $"https://example.invalid/v1{path}"
                + "?include[]=message.input_image.image_url"
                + "&include[]=web_search_call.action.sources"));
    }
}
