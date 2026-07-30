using System;
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
    public void DefaultUserAgentStringWorks() => UserAgentStringWorks(applicationId: null);

    [Test]
    public void UserAgentWithApplicationIdWorks() => UserAgentStringWorks(applicationId: "test-application-id");

    [Test]
    public void UserAgentApplicationIdAllowsTheMaximumLength()
    {
        UserAgentStringWorks(applicationId: new string('a', 512));
    }

    [Test]
    public void UserAgentApplicationIdRejectsValuesOverTheMaximumLength()
    {
        // The application id is echoed in the user agent of every request, so it is bounded to keep an
        // oversized value from inflating each one.
        OpenAIClientOptions options = new() { UserAgentApplicationId = new string('a', 513), };

        Assert.Throws<ArgumentOutOfRangeException>(() => TestEnvironment.GetTestClient<ChatClient>(options: options));
    }

    private void UserAgentStringWorks(string applicationId)
    {
        string userAgent = null;
        TestPipelinePolicy policy = new((m) =>
        {
            _ = m?.Request?.Headers?.TryGetValue("User-Agent", out userAgent);
        });

        OpenAIClientOptions options = applicationId is not null
            ? new() { UserAgentApplicationId = applicationId, }
            : new();

        options.AddPolicy(policy, PipelinePosition.BeforeTransport);

        ChatClient client = TestEnvironment.GetTestClient<ChatClient>(options: options);
        RequestOptions noThrowOptions = new() { ErrorOptions = ClientErrorBehaviors.NoThrow };

        using BinaryContent emptyContent = BinaryContent.Create(new MemoryStream());
        _ = client.CompleteChat(emptyContent, noThrowOptions);

        Assert.That(userAgent, Is.Not.Null.Or.Empty);

        if (applicationId is not null)
        {
            Assert.That(userAgent, Does.Contain(applicationId));
        }

        Assert.That(userAgent, Does.Contain("OpenAI/"));
    }
}
