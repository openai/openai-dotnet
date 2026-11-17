using System;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using Microsoft.ClientModel.TestFramework.Mocks;
using NUnit.Framework;
using OpenAI.Chat;

namespace OpenAI.Tests.Miscellaneous;

/// <summary>
/// Verifies the telemetry opt-out, which suppresses both the SDK platform metadata headers and the
/// <c>User-Agent</c> the library would otherwise add.
/// </summary>
/// <remarks>
/// The configuration is process-global and is read when a pipeline is built, so these tests cannot run
/// concurrently with anything that constructs a client.
/// </remarks>
[NonParallelizable]
[Category("Smoke")]
public class PlatformTelemetryOptOutTests
{
    private const string EnvironmentVariableName = "OPENAI_DISABLE_TELEMETRY";

    private static readonly string[] s_headerNames =
    [
        PlatformTelemetry.LangHeaderName,
        PlatformTelemetry.PackageVersionHeaderName,
        PlatformTelemetry.RuntimeHeaderName,
        PlatformTelemetry.RuntimeVersionHeaderName,
        PlatformTelemetry.OSHeaderName,
        PlatformTelemetry.ArchHeaderName,
    ];

    private static readonly ApiKeyCredential s_credential = new("fake-key");

    private string _originalEnvironmentValue;

    [SetUp]
    public void SetUpTelemetryConfiguration()
    {
        _originalEnvironmentValue = Environment.GetEnvironmentVariable(EnvironmentVariableName);
        Environment.SetEnvironmentVariable(EnvironmentVariableName, null);
    }

    [TearDown]
    public void ResetTelemetryConfiguration()
    {
        Environment.SetEnvironmentVariable(EnvironmentVariableName, _originalEnvironmentValue);
    }

    [Test]
    public void TelemetryIsEnabledByDefault()
    {
        Assert.That(PlatformTelemetry.IsTelemetryDisabled(), Is.False);
    }

    [TestCase("true")]
    [TestCase("TRUE")]
    [TestCase("1")]
    public void TheEnvironmentVariableDisablesTelemetry(string value)
    {
        Environment.SetEnvironmentVariable(EnvironmentVariableName, value);

        Assert.That(PlatformTelemetry.IsTelemetryDisabled(), Is.True);
    }

    [TestCase("false")]
    [TestCase("0")]
    [TestCase("")]
    public void TheEnvironmentVariableLeavesTelemetryEnabledWhenNotSet(string value)
    {
        Environment.SetEnvironmentVariable(EnvironmentVariableName, value);

        Assert.That(PlatformTelemetry.IsTelemetryDisabled(), Is.False);
    }

    [Test]
    public void OptingOutSuppressesThePlatformHeaders()
    {
        Environment.SetEnvironmentVariable(EnvironmentVariableName, "true");

        PipelineRequest request = SendRequest();

        foreach (string name in s_headerNames)
        {
            Assert.That(request.Headers.TryGetValue(name, out string _), Is.False, $"'{name}' was applied while opted out.");
        }
    }

    [Test]
    public void OptingOutSuppressesTheUserAgent()
    {
        // This matches the behavior of Azure.Core when telemetry is disabled. Neither HttpClient nor the
        // transport injects a default, so an opted-out request carries no user agent at all.
        Environment.SetEnvironmentVariable(EnvironmentVariableName, "true");

        PipelineRequest request = SendRequest();

        Assert.That(request.Headers.TryGetValue("User-Agent", out string _), Is.False);
    }

    [Test]
    public void OptingOutPreservesACallerSuppliedUserAgent()
    {
        // The opt-out only stops the library from adding its own value.
        Environment.SetEnvironmentVariable(EnvironmentVariableName, "true");

        TestPipelinePolicy overridePolicy = new(message =>
        {
            message?.Request?.Headers?.Set("User-Agent", "caller-supplied");
        });

        PipelineRequest request = SendRequest(options => options.AddPolicy(overridePolicy, PipelinePosition.PerCall));

        Assert.That(request.Headers.TryGetValue("User-Agent", out string userAgent), Is.True);
        Assert.That(userAgent, Is.EqualTo("caller-supplied"));
    }

    [Test]
    public void OptingOutPreservesAuthenticationAndScopingHeaders()
    {
        // The opt-out governs telemetry only; headers the service needs to route and authorize the request are
        // unaffected.
        Environment.SetEnvironmentVariable(EnvironmentVariableName, "true");

        PipelineRequest request = SendRequest(options =>
        {
            options.OrganizationId = "org-id";
            options.ProjectId = "project-id";
        });

        Assert.That(PlatformTelemetryHeaderTests.GetHeader(request, "Authorization"), Is.EqualTo("Bearer fake-key"));
        Assert.That(PlatformTelemetryHeaderTests.GetHeader(request, "OpenAI-Organization"), Is.EqualTo("org-id"));
        Assert.That(PlatformTelemetryHeaderTests.GetHeader(request, "OpenAI-Project"), Is.EqualTo("project-id"));
    }

    [Test]
    public void OptingOutIgnoresTheUserAgentApplicationId()
    {
        // The application id feeds only the user agent, so when it will not be sent the value is unused. It is
        // neither composed into a string nor validated: the length bound exists to keep the header from being
        // inflated, and an opted-out request carries no such header.
        Environment.SetEnvironmentVariable(EnvironmentVariableName, "true");

        Assert.That(
            () => SendRequest(options => options.UserAgentApplicationId = new string('a', 513)),
            Throws.Nothing);
    }

    [Test]
    public void TheAppContextSwitchDisablesTelemetry()
    {
        Assert.That(
            AppContextSwitchHelper.GetConfigValue(
                isSwitchSet: true,
                switchValue: true,
                environmentValue: null),
            Is.True);
    }

    [Test]
    public void TheAppContextSwitchTakesPrecedenceOverTheEnvironmentVariable()
    {
        Assert.That(
            AppContextSwitchHelper.GetConfigValue(
                isSwitchSet: true,
                switchValue: false,
                environmentValue: "true"),
            Is.False);
    }

    private static PipelineRequest SendRequest(Action<OpenAIClientOptions> configure = null)
    {
        List<PipelineRequest> captured = [];

        OpenAIClientOptions options = new()
        {
            Transport = new MockPipelineTransport(_ =>
                new MockPipelineResponse(200).WithContent(BinaryContent.Create(BinaryData.FromString("{}"))))
        };

        configure?.Invoke(options);
        options.AddPolicy(new TestPipelinePolicy(message => captured.Add(message?.Request)), PipelinePosition.BeforeTransport);

        ChatClient client = new("model", s_credential, options);
        client.CompleteChat(
            BinaryContent.Create(BinaryData.FromString("{}")),
            new RequestOptions { ErrorOptions = ClientErrorBehaviors.NoThrow });

        Assert.That(captured, Is.Not.Empty, "No request reached the transport.");
        return captured[captured.Count - 1];
    }
}
