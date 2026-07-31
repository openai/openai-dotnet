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

    [Test]
    public void UserAgentIsGeneratedWhenThePlatformCannotBeDescribed()
    {
        // FrameworkDescription and OSDescription are telemetry-only inputs that are not guaranteed to be
        // readable on every runtime. A client must not fail to be created because one of them threw, whether
        // or not telemetry is enabled.
        string userAgent = null;

        Assert.DoesNotThrow(() => userAgent = TelemetryDetails.GenerateUserAgentString(
            typeof(OpenAIClient).Assembly,
            null,
            new ThrowingRuntimeInformation(throwForFramework: true, throwForOperatingSystem: true)));

        Assert.That(userAgent, Does.Contain("OpenAI/"));
        Assert.That(userAgent, Does.Contain("unknown"));
    }

    [Test]
    public void UserAgentRetainsTheDescriptionThatCanBeRead()
    {
        // The two descriptions are read independently, so losing one does not discard the other.
        string userAgent = TelemetryDetails.GenerateUserAgentString(
            typeof(OpenAIClient).Assembly,
            null,
            new ThrowingRuntimeInformation(throwForFramework: false, throwForOperatingSystem: true));

        Assert.That(userAgent, Does.Contain(System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription));
        Assert.That(userAgent, Does.Contain("unknown"));
    }

    private class ThrowingRuntimeInformation : TelemetryDetails.RuntimeInformationWrapper
    {
        private readonly bool _throwForFramework;
        private readonly bool _throwForOperatingSystem;

        public ThrowingRuntimeInformation(bool throwForFramework, bool throwForOperatingSystem)
        {
            _throwForFramework = throwForFramework;
            _throwForOperatingSystem = throwForOperatingSystem;
        }

        public override string FrameworkDescription
            => _throwForFramework ? throw new PlatformNotSupportedException() : base.FrameworkDescription;

        public override string OSDescription
            => _throwForOperatingSystem ? throw new PlatformNotSupportedException() : base.OSDescription;
    }
}
