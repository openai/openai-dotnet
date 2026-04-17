using Microsoft.ClientModel.TestFramework.Mocks;
using NUnit.Framework;
using OpenAI.Realtime;
using System;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.Linq;
using System.Reflection;

namespace OpenAI.Tests.Realtime;

#pragma warning disable OPENAI001
#pragma warning disable OPENAI002

[Category("Smoke")]
public class RealtimeClientOptionsSmokeTests
{
    [Test]
    public void DefaultOptionsHaveNullProperties()
    {
        RealtimeClientOptions options = new();

        Assert.That(options.Endpoint, Is.Null);
        Assert.That(options.OrganizationId, Is.Null);
        Assert.That(options.ProjectId, Is.Null);
        Assert.That(options.UserAgentApplicationId, Is.Null);
    }

    [Test]
    public void OptionsPropertiesCanBeSet()
    {
        Uri endpoint = new("https://custom.endpoint.com/v1");
        RealtimeClientOptions options = new()
        {
            Endpoint = endpoint,
            OrganizationId = "org-test123",
            ProjectId = "proj-test456",
            UserAgentApplicationId = "my-app/1.0",
        };

        Assert.That(options.Endpoint, Is.EqualTo(endpoint));
        Assert.That(options.OrganizationId, Is.EqualTo("org-test123"));
        Assert.That(options.ProjectId, Is.EqualTo("proj-test456"));
        Assert.That(options.UserAgentApplicationId, Is.EqualTo("my-app/1.0"));
    }

    [Test]
    public void OptionsInheritFromClientPipelineOptions()
    {
        RealtimeClientOptions options = new();

        Assert.That(options, Is.InstanceOf<ClientPipelineOptions>());
        Assert.That(options, Is.Not.InstanceOf<OpenAIClientOptions>());
    }

    [Test]
    public void FrozenOptionsCannotBeModified()
    {
        RealtimeClientOptions options = new()
        {
            Endpoint = new Uri("https://example.com"),
            OrganizationId = "org-1",
            ProjectId = "proj-1",
            UserAgentApplicationId = "app-1",
        };

        options.Freeze();

        Assert.Throws<InvalidOperationException>(() => options.Endpoint = new Uri("https://other.com"));
        Assert.Throws<InvalidOperationException>(() => options.OrganizationId = "org-2");
        Assert.Throws<InvalidOperationException>(() => options.ProjectId = "proj-2");
        Assert.Throws<InvalidOperationException>(() => options.UserAgentApplicationId = "app-2");
    }

    [Test]
    public void ClientAcceptsOptions()
    {
        RealtimeClientOptions options = new()
        {
            Endpoint = new Uri("https://custom.openai.com/v1"),
        };

        RealtimeClient client = new(new ApiKeyCredential("test-key"), options);

        Assert.That(client, Is.Not.Null);
        Assert.That(client.Endpoint, Is.EqualTo(new Uri("https://custom.openai.com/v1")));
    }

    [Test]
    public void ClientUsesDefaultEndpointWhenOptionsOmitEndpoint()
    {
        RealtimeClient client = new(new ApiKeyCredential("test-key"), new RealtimeClientOptions());

        Assert.That(client.Endpoint, Is.EqualTo(new Uri("https://api.openai.com/v1")));
    }

    [Test]
    public void ClientAcceptsNullOptions()
    {
        RealtimeClient client = new(new ApiKeyCredential("test-key"), (RealtimeClientOptions)null);

        Assert.That(client, Is.Not.Null);
        Assert.That(client.Endpoint, Is.EqualTo(new Uri("https://api.openai.com/v1")));
    }

    [Test]
    public void ClientWithAuthenticationPolicyAcceptsOptions()
    {
        RealtimeClientOptions options = new()
        {
            Endpoint = new Uri("https://custom.openai.com/v1"),
        };

        AuthenticationPolicy policy = ApiKeyAuthenticationPolicy.CreateHeaderApiKeyPolicy(
            new ApiKeyCredential("test-key"), "Authorization", "Bearer");

        RealtimeClient client = new(policy, options);

        Assert.That(client, Is.Not.Null);
        Assert.That(client.Endpoint, Is.EqualTo(new Uri("https://custom.openai.com/v1")));
    }

    // Verifies that OpenAITestEnvironment.CreateRealtimeClientOptions propagates every
    // public settable property from ClientPipelineOptions. If a new property is added to
    // ClientPipelineOptions, SampleValueFor will throw and force an update to
    // OpenAITestEnvironment.CopyPipelineOptions.
    [Test]
    public void CreateRealtimeClientOptionsCopiesAllPipelineProperties()
    {
        PropertyInfo[] properties = typeof(ClientPipelineOptions)
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.GetSetMethod(nonPublic: false) is not null)
            .ToArray();

        OpenAIClientOptions source = new();
        foreach (PropertyInfo property in properties)
        {
            property.SetValue(source, SampleValueFor(property));
        }

        RealtimeClientOptions result = OpenAITestEnvironment.CreateRealtimeClientOptions(source);

        foreach (PropertyInfo property in properties)
        {
            Assert.That(
                property.GetValue(result),
                Is.EqualTo(property.GetValue(source)),
                $"ClientPipelineOptions.{property.Name} was not copied. Update OpenAITestEnvironment.CopyPipelineOptions.");
        }

        static object SampleValueFor(PropertyInfo property) => property.Name switch
        {
            nameof(ClientPipelineOptions.RetryPolicy) => ClientRetryPolicy.Default,
            nameof(ClientPipelineOptions.MessageLoggingPolicy) => MessageLoggingPolicy.Default,
            nameof(ClientPipelineOptions.Transport) => new MockPipelineTransport(_ => new MockPipelineResponse(200)),
            nameof(ClientPipelineOptions.NetworkTimeout) => TimeSpan.FromSeconds(42),
            nameof(ClientPipelineOptions.ClientLoggingOptions) => new ClientLoggingOptions(),
            nameof(ClientPipelineOptions.EnableDistributedTracing) => true,
            _ => throw new InvalidOperationException(
                $"No sample value defined for ClientPipelineOptions.{property.Name}. " +
                "Add one here and update OpenAITestEnvironment.CopyPipelineOptions."),
        };
    }
}
