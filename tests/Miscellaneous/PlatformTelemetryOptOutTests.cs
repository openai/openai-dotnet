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
/// <para>
/// The configuration is process-global and is read when a pipeline is built, so these tests cannot run
/// concurrently with anything that constructs a client.
/// </para>
/// <para>
/// <see cref="AppContext.SetSwitch"/> has no way to return a switch to an undefined state, and a switch that
/// is defined as <c>false</c> still takes precedence over the environment variable. The switch-based tests are
/// therefore ordered last, so that they cannot shadow the environment-variable tests.
/// </para>
/// </remarks>
[NonParallelizable]
[Category("Smoke")]
public class PlatformTelemetryOptOutTests
{
    private const string SwitchName = "OpenAI.TelemetryDisabled";
    private const string EnvironmentVariableName = "OPENAI_TELEMETRY_DISABLED";

    private static readonly string[] s_headerNames =
    [
        "X-Stainless-Lang",
        "X-Stainless-Package-Version",
        "X-Stainless-Runtime",
        "X-Stainless-Runtime-Version",
        "X-Stainless-OS",
        "X-Stainless-Arch",
    ];

    private static readonly ApiKeyCredential s_credential = new("fake-key");

    [TearDown]
    public void ResetTelemetryConfiguration()
    {
        Environment.SetEnvironmentVariable(EnvironmentVariableName, null);
    }

    [Test]
    [Order(1)]
    public void TelemetryIsEnabledByDefault()
    {
        Assert.That(PlatformTelemetry.IsTelemetryDisabled(), Is.False);
    }

    [TestCase("true")]
    [TestCase("TRUE")]
    [TestCase("1")]
    [Order(2)]
    public void TheEnvironmentVariableDisablesTelemetry(string value)
    {
        Environment.SetEnvironmentVariable(EnvironmentVariableName, value);

        Assert.That(PlatformTelemetry.IsTelemetryDisabled(), Is.True);
    }

    [TestCase("false")]
    [TestCase("0")]
    [TestCase("")]
    [Order(3)]
    public void TheEnvironmentVariableLeavesTelemetryEnabledWhenNotSet(string value)
    {
        Environment.SetEnvironmentVariable(EnvironmentVariableName, value);

        Assert.That(PlatformTelemetry.IsTelemetryDisabled(), Is.False);
    }

    [Test]
    [Order(4)]
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
    [Order(5)]
    public void OptingOutSuppressesTheUserAgent()
    {
        // This matches the behavior of Azure.Core when telemetry is disabled. Neither HttpClient nor the
        // transport injects a default, so an opted-out request carries no user agent at all.
        Environment.SetEnvironmentVariable(EnvironmentVariableName, "true");

        PipelineRequest request = SendRequest();

        Assert.That(request.Headers.TryGetValue("User-Agent", out string _), Is.False);
    }

    [Test]
    [Order(6)]
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
    [Order(7)]
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
    [Order(100)]
    public void TheAppContextSwitchDisablesTelemetry()
    {
        AppContext.SetSwitch(SwitchName, true);

        try
        {
            Assert.That(PlatformTelemetry.IsTelemetryDisabled(), Is.True);
            Assert.That(SendRequest().Headers.TryGetValue("X-Stainless-Lang", out string _), Is.False);
        }
        finally
        {
            AppContext.SetSwitch(SwitchName, false);
        }
    }

    [Test]
    [Order(101)]
    public void TheAppContextSwitchTakesPrecedenceOverTheEnvironmentVariable()
    {
        AppContext.SetSwitch(SwitchName, false);
        Environment.SetEnvironmentVariable(EnvironmentVariableName, "true");

        Assert.That(PlatformTelemetry.IsTelemetryDisabled(), Is.False);
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
