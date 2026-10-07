using System;
using System.ClientModel;
using System.Linq;
using Microsoft.ClientModel.TestFramework.Mocks;
using NUnit.Framework;
using OpenAI.Assistants;

namespace OpenAI.Tests.Assistants;

#pragma warning disable OPENAI001

public class AssistantPaginationQueryOrderMockTests
{
    [Test]
    public void GetAssistantsPreservesRecordedQueryOrder()
    {
        Uri requestUri = null;
        MockPipelineResponse response = new MockPipelineResponse(200).WithContent(
            """{"object":"list","data":[],"first_id":null,"last_id":null,"has_more":false}""");
        OpenAIClientOptions clientOptions = new()
        {
            Endpoint = new Uri("https://example.invalid/v1"),
            Transport = new MockPipelineTransport(message =>
            {
                requestUri = message.Request.Uri;
                return response;
            }),
        };
        AssistantClient client = new(new ApiKeyCredential("key"), clientOptions);
        AssistantCollectionOptions options = new()
        {
            PageSizeLimit = 2,
            Order = AssistantCollectionOrder.Descending,
            AfterId = "asst_after",
            BeforeId = "asst_before",
        };

        _ = client.GetAssistants(options).GetRawPages().First();

        Assert.That(requestUri.AbsoluteUri, Is.EqualTo(
            "https://example.invalid/v1/assistants?limit=2&order=desc&after=asst_after&before=asst_before"));
    }
}
