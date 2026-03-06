using System.ClientModel;
using System.ClientModel.Primitives;
using System.IO;
using NUnit.Framework;
using OpenAI.Chat;

namespace OpenAI.Tests.Miscellaneous;

public class UserAgentTests
{
    private static readonly OpenAITestEnvironment TestEnvironment = new();

    [Test]
    public void DefaultUserAgentStringWorks() => UserAgentStringWorks(useApplicationId: false);

    [Test]
    public void UserAgentWithApplicationIdWorks() => UserAgentStringWorks(useApplicationId: true);

    private void UserAgentStringWorks(bool useApplicationId)
    {
        string userAgent = null;
        TestPipelinePolicy policy = new((m) =>
        {
            _ = m?.Request?.Headers?.TryGetValue("User-Agent", out userAgent);
        });

        OpenAIClientOptions options = useApplicationId
            ? new() { UserAgentApplicationId = "test-application-id",}
            : new();

        options.AddPolicy(policy, PipelinePosition.BeforeTransport);

        ChatClient client = TestEnvironment.GetTestClient<ChatClient>(options: options);
        RequestOptions noThrowOptions = new() { ErrorOptions = ClientErrorBehaviors.NoThrow };

        using BinaryContent emptyContent = BinaryContent.Create(new MemoryStream());
        _ = client.CompleteChat(emptyContent, noThrowOptions);

        Assert.That(userAgent, Is.Not.Null.Or.Empty);

        if (useApplicationId)
        {
            Assert.That(userAgent, Does.Contain("test-application-id"));
        }

        Assert.That(userAgent, Does.Contain("OpenAI/"));
    }
}
