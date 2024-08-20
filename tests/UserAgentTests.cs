using NUnit.Framework;
using OpenAI.Chat;
using System;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.IO;

namespace OpenAI.Tests.Miscellaneous;

public partial class UserAgentTests
{
    [Test]
    public void DefaultUserAgentStringWorks() => UserAgentStringWorks(useApplicationId: false);

    [Test]
    public void UserAgentWithApplicationIdWorks() => UserAgentStringWorks(useApplicationId: true);

    private void UserAgentStringWorks(bool useApplicationId)
    {
        ApiKeyCredential mockKeyCredential = new("no-real-key-needed");
        string userAgent = null;
        TestPipelinePolicy policy = new((m) =>
        {
            _ = m?.Request?.Headers?.TryGetValue("User-Agent", out userAgent);
        });

        OpenAIClientOptions options = useApplicationId ? new()
        {
            ApplicationId = "test-application-id",
        } : new();
        options.AddPolicy(policy, PipelinePosition.BeforeTransport);

        ChatClient client = new("no-real-model-needed", Environment.GetEnvironmentVariable("OPENAI_API_KEY"), options);
        RequestOptions noThrowOptions = new() { ErrorOptions = ClientErrorBehaviors.NoThrow, };
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
