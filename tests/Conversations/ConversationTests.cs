using Microsoft.ClientModel.TestFramework;
using NUnit.Framework;
using OpenAI.Conversations;
using OpenAI.Responses;
using OpenAI.Tests.Utility;
using System.ClientModel;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace OpenAI.Tests.Conversations;

[Category("Conversations")]
public class ConversationTests : OpenAIRecordedTestBase
{
    public ConversationTests(bool isAsync) : base(isAsync)
    {
        TestTimeoutInSeconds = 30;
    }

    [RecordedTest]
    public async Task CanCreateGetUpdateAndDeleteConversationAsyncWithCancellationToken()
    {
        ConversationClient client = GetProxiedOpenAIClient<ConversationClient>();
        string conversationId = null;

        try
        {
            ConversationCreationOptions createOptions = new()
            {
                Metadata = { ["topic"] = "test" },
                Items = { ResponseItem.CreateUserMessageItem("tell me a joke") },
            };

            ClientResult<ConversationResource> createResult = await client.CreateConversationAsync(createOptions);
            ConversationResource conversation = createResult.Value;
            conversationId = conversation.Id;

            Validate(conversation);
            Assert.That(conversation.Metadata["topic"], Is.EqualTo("test"));

            ClientResult<ConversationResource> getResult = await client.GetConversationAsync(conversationId);
            ConversationResource retrievedConversation = getResult.Value;

            Validate(retrievedConversation);
            Assert.That(retrievedConversation.Id, Is.EqualTo(conversationId));
            Assert.That(retrievedConversation.Metadata["topic"], Is.EqualTo("test"));

            ConversationUpdateOptions updateOptions = new(new Dictionary<string, string>()
            {
                ["topic"] = "updated-test",
                ["purpose"] = "conversation-test",
            });

            ClientResult<ConversationResource> updateResult = await client.UpdateConversationAsync(conversationId, updateOptions);
            ConversationResource updatedConversation = updateResult.Value;

            Validate(updatedConversation);
            Assert.That(updatedConversation.Id, Is.EqualTo(conversationId));
            Assert.That(updatedConversation.Metadata["topic"], Is.EqualTo("updated-test"));
            Assert.That(updatedConversation.Metadata["purpose"], Is.EqualTo("conversation-test"));

            ClientResult<ConversationDeletionResult> deleteResult = await client.DeleteConversationAsync(conversationId);
            ConversationDeletionResult deletedConversation = deleteResult.Value;
            conversationId = null;

            Assert.That(deletedConversation.Deleted, Is.True);
            Assert.That(deletedConversation.ConversationId, Is.EqualTo(conversation.Id));
        }
        finally
        {
            if (conversationId is not null)
            {
                await client.DeleteConversationAsync(conversationId);
            }
        }
    }

    private static void Validate(ConversationResource conversation)
    {
        Assert.That(conversation, Is.Not.Null);
        Assert.That(conversation.Id, Is.Not.Null.And.Not.Empty);
        Assert.That(conversation.Object, Is.EqualTo("conversation"));
        Assert.That(conversation.CreatedAt, Is.GreaterThan(default(System.DateTimeOffset)));
    }
}
