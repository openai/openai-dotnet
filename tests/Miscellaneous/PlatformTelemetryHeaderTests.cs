using System;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.Linq;
using Microsoft.ClientModel.TestFramework.Mocks;
using NUnit.Framework;
using OpenAI.Chat;
using OpenAI.Embeddings;

namespace OpenAI.Tests.Miscellaneous;

/// <summary>
/// Verifies that the SDK platform metadata headers are applied to outgoing requests by the shared pipeline
/// policy that every client is built on.
/// </summary>
[Category("Smoke")]
public class PlatformTelemetryHeaderTests
{
    private const string UserAgentHeaderName = "User-Agent";
    private const string LangHeaderName = "X-Stainless-Lang";
    private const string PackageVersionHeaderName = "X-Stainless-Package-Version";
    private const string RuntimeHeaderName = "X-Stainless-Runtime";
    private const string RuntimeVersionHeaderName = "X-Stainless-Runtime-Version";
    private const string OSHeaderName = "X-Stainless-OS";
    private const string ArchHeaderName = "X-Stainless-Arch";

    private static readonly string[] s_headerNames =
    [
        LangHeaderName,
        PackageVersionHeaderName,
        RuntimeHeaderName,
        RuntimeVersionHeaderName,
        OSHeaderName,
        ArchHeaderName,
    ];

    private static readonly ApiKeyCredential s_credential = new("fake-key");

    [Test]
    public void AllPlatformHeadersArePresent()
    {
        PipelineRequest request = SendRequest(out _);

        foreach (string name in s_headerNames)
        {
            Assert.That(request.Headers.TryGetValue(name, out string value), Is.True, $"'{name}' was not applied.");
            Assert.That(value, Is.Not.Null.And.Not.Empty, $"'{name}' was empty.");
        }
    }

    [Test]
    public void ConstantPlatformHeadersUseTheExpectedValues()
    {
        PipelineRequest request = SendRequest(out _);

        Assert.That(GetHeader(request, LangHeaderName), Is.EqualTo("csharp"));
        Assert.That(GetHeader(request, RuntimeHeaderName), Is.EqualTo("dotnet"));
    }

    [Test]
    public void PackageVersionHeaderMatchesTheUserAgent()
    {
        // The header and the user agent must never disagree, so both read the same assembly attribute and
        // apply the same commit-suffix strip.
        PipelineRequest request = SendRequest(out _);

        string version = GetHeader(request, PackageVersionHeaderName);
        string userAgent = GetHeader(request, UserAgentHeaderName);

        Assert.That(version, Is.Not.EqualTo("unknown"));
        Assert.That(userAgent, Does.Contain($"OpenAI/{version} "));
    }

    [Test]
    public void PlatformHeadersAreAppliedToEveryClient()
    {
        // The headers come from a single shared policy, so verifying a second, unrelated client guards against
        // a client being built on a pipeline that bypasses it.
        List<PipelineRequest> captured = [];

        OpenAIClientOptions options = new()
        {
            Transport = new MockPipelineTransport(_ =>
                new MockPipelineResponse(200).WithContent(BinaryContent.Create(BinaryData.FromString("{}"))))
        };

        options.AddPolicy(new TestPipelinePolicy(message => captured.Add(message?.Request)), PipelinePosition.BeforeTransport);

        EmbeddingClient client = new("model", s_credential, options);
        client.GenerateEmbeddings(
            BinaryContent.Create(BinaryData.FromString("{}")),
            new RequestOptions { ErrorOptions = ClientErrorBehaviors.NoThrow });

        Assert.That(captured, Is.Not.Empty);
        Assert.That(GetHeader(captured[0], LangHeaderName), Is.EqualTo("csharp"));
        Assert.That(GetHeader(captured[0], OSHeaderName), Is.Not.Null.And.Not.Empty);
    }

    [Test]
    public void CallerSuppliedPlatformHeadersAreNotOverwritten()
    {
        // Header lookups are case-insensitive, so a caller's value must win regardless of the casing used.
        TestPipelinePolicy overridePolicy = new(message =>
        {
            message?.Request?.Headers?.Set("x-stainless-lang", "caller-supplied");
        });

        PipelineRequest request = SendRequest(out _, configure: options => options.AddPolicy(overridePolicy, PipelinePosition.PerCall));

        Assert.That(GetHeader(request, LangHeaderName), Is.EqualTo("caller-supplied"));
    }

    [Test]
    public void PlatformHeadersAreNotDuplicatedAcrossRetries()
    {
        // The policy runs per-call rather than per-attempt and uses Set rather than Add, so a retried request
        // must not accumulate repeated values.
        int attempts = 0;
        MockPipelineTransport transport = new(_ =>
        {
            attempts++;
            return new MockPipelineResponse(attempts < 3 ? 500 : 200)
                .WithContent(BinaryContent.Create(BinaryData.FromString("{}")));
        });

        List<PipelineRequest> requests = [];
        OpenAIClientOptions options = new() { Transport = transport };
        options.AddPolicy(new TestPipelinePolicy(message => requests.Add(message?.Request)), PipelinePosition.BeforeTransport);

        ChatClient client = new("model", s_credential, options);
        client.CompleteChat(BinaryContent.Create(BinaryData.FromString("{}")), new RequestOptions { ErrorOptions = ClientErrorBehaviors.NoThrow });

        Assert.That(attempts, Is.GreaterThan(1), "The request was expected to be retried.");

        PipelineRequest request = requests[requests.Count - 1];

        foreach (string name in s_headerNames)
        {
            int count = request.Headers.Count(header => string.Equals(header.Key, name, StringComparison.OrdinalIgnoreCase));
            Assert.That(count, Is.EqualTo(1), $"'{name}' was applied {count} times.");
        }
    }

    [Test]
    public void OrganizationAndProjectHeadersAreUnaffected()
    {
        PipelineRequest request = SendRequest(out _, configure: options =>
        {
            options.OrganizationId = "org-id";
            options.ProjectId = "project-id";
        });

        Assert.That(GetHeader(request, "OpenAI-Organization"), Is.EqualTo("org-id"));
        Assert.That(GetHeader(request, "OpenAI-Project"), Is.EqualTo("project-id"));
        Assert.That(GetHeader(request, "Authorization"), Is.EqualTo("Bearer fake-key"));
    }

    internal static string GetHeader(PipelineRequest request, string name)
    {
        request.Headers.TryGetValue(name, out string value);
        return value;
    }

    internal static PipelineRequest SendRequest(out List<PipelineRequest> requests, Action<OpenAIClientOptions> configure = null)
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

        requests = captured;
        Assert.That(captured, Is.Not.Empty, "No request reached the transport.");

        return captured[captured.Count - 1];
    }
}
