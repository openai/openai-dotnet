using Microsoft.ClientModel.TestFramework.Mocks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Moq;
using NUnit.Framework;
using OpenAI.Decisions;
using System;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OpenAI.Tests.Decisions;

#pragma warning disable OPENAI001
#pragma warning disable SCME0002

[Category("Decisions")]
[Category("Smoke")]
public class DecisionsClientTests
{
    private static readonly ApiKeyCredential s_credential = new("not-a-real-key");
    private static readonly Uri s_endpoint = new("https://example.invalid/custom/v1");

    [Test]
    public void ConstructorsUseDefaultEndpoint()
    {
        AuthenticationPolicy policy = CreateAuthenticationPolicy();
        DecisionsClient[] clients =
        [
            new("not-a-real-key"),
            new(s_credential),
            new(s_credential, null),
            new(policy),
            new(policy, null),
            new DerivedDecisionsClient(new DecisionsClient(s_credential).Pipeline, null),
        ];

        foreach (DecisionsClient client in clients)
        {
            Assert.That(client.Endpoint, Is.EqualTo(new Uri("https://api.openai.com/v1")));
            Assert.That(client.Pipeline, Is.Not.Null);
        }
    }

    [Test]
    public void ConstructorsRejectNullRequiredArguments()
    {
        Assert.Throws<ArgumentNullException>(() => new DecisionsClient((string)null));
        Assert.Throws<ArgumentNullException>(() => new DecisionsClient((ApiKeyCredential)null));
        Assert.Throws<ArgumentNullException>(() => new DecisionsClient((ApiKeyCredential)null, null));
        Assert.Throws<ArgumentNullException>(() => new DecisionsClient((AuthenticationPolicy)null));
        Assert.Throws<ArgumentNullException>(() => new DecisionsClient((AuthenticationPolicy)null, null));
        Assert.Throws<ArgumentNullException>(() => new DecisionsClient((DecisionsClientSettings)null));
        Assert.Throws<ArgumentNullException>(() => new DerivedDecisionsClient(null, null));
    }

    [Test]
    public void ConstructorsHonorEndpointAndSharedPipeline()
    {
        DecisionsClientOptions options = new() { Endpoint = s_endpoint };
        DecisionsClient credentialClient = new(s_credential, options);
        DecisionsClient policyClient = new(CreateAuthenticationPolicy(), new DecisionsClientOptions { Endpoint = s_endpoint });
        DecisionsClient pipelineClient = new DerivedDecisionsClient(credentialClient.Pipeline, options);

        Assert.That(credentialClient.Endpoint, Is.EqualTo(s_endpoint));
        Assert.That(policyClient.Endpoint, Is.EqualTo(s_endpoint));
        Assert.That(pipelineClient.Endpoint, Is.EqualTo(s_endpoint));
        Assert.That(pipelineClient.Pipeline, Is.SameAs(credentialClient.Pipeline));
    }

    [Test]
    public void TestEnvironmentCreatesDecisionsClient()
    {
        OpenAIClientOptions options = new() { Endpoint = s_endpoint };
        DecisionsClient client = new OpenAITestEnvironment().GetTestClient<DecisionsClient>(options: options);

        Assert.That(client, Is.Not.Null);
        Assert.That(client.Endpoint, Is.EqualTo(s_endpoint));
        Assert.That(client.Pipeline, Is.Not.Null);
    }

    [Test]
    public void FactoryCreatesNewClientsWithSharedPipeline()
    {
        OpenAIClient topLevelClient = new(s_credential, new OpenAIClientOptions { Endpoint = s_endpoint });
        DecisionsClient first = topLevelClient.GetDecisionsClient();
        DecisionsClient second = topLevelClient.GetDecisionsClient();

        Assert.That(first, Is.Not.SameAs(second));
        Assert.That(first.Endpoint, Is.EqualTo(s_endpoint));
        Assert.That(second.Endpoint, Is.EqualTo(s_endpoint));
        Assert.That(first.Pipeline, Is.SameAs(topLevelClient.Pipeline));
        Assert.That(second.Pipeline, Is.SameAs(topLevelClient.Pipeline));
    }

    [TestCase(false, false)]
    [TestCase(false, true)]
    [TestCase(true, false)]
    [TestCase(true, true)]
    public async Task RequestsUseConfiguredPipeline(bool useFactory, bool isAsync)
    {
        Uri requestUri = null;
        Dictionary<string, string> headers = new();
        MockPipelineTransport transport = new(message =>
        {
            requestUri = message.Request.Uri;
            foreach (string name in new[] { "Authorization", "OpenAI-Organization", "OpenAI-Project", "User-Agent", "X-Custom" })
            {
                message.Request.Headers.TryGetValue(name, out string value);
                headers[name] = value;
            }
            return new MockPipelineResponse(200).WithContent("{}");
        })
        {
            ExpectSyncPipeline = !isAsync,
        };
        DecisionsClient client;
        TestPipelinePolicy policy = new(message => message.Request.Headers.Set("X-Custom", "custom-value"));

        if (useFactory)
        {
            OpenAIClientOptions options = new()
            {
                Endpoint = s_endpoint,
                OrganizationId = "org-test",
                ProjectId = "proj-test",
                UserAgentApplicationId = "decisions-test-app",
                Transport = transport,
            };
            options.AddPolicy(policy, PipelinePosition.PerCall);
            client = new OpenAIClient(s_credential, options).GetDecisionsClient();
        }
        else
        {
            DecisionsClientOptions options = new()
            {
                Endpoint = s_endpoint,
                OrganizationId = "org-test",
                ProjectId = "proj-test",
                UserAgentApplicationId = "decisions-test-app",
                Transport = transport,
            };
            options.AddPolicy(policy, PipelinePosition.PerCall);
            client = new DecisionsClient(s_credential, options);
        }

        using BinaryContent content = BinaryContent.Create(BinaryData.FromString("{}"));
        if (isAsync)
        {
            await client.CreateDecisionAsync(content);
        }
        else
        {
            client.CreateDecision(content);
        }

        Assert.That(requestUri, Is.EqualTo(new Uri($"{s_endpoint}/decisions")));
        Assert.That(headers["Authorization"], Does.StartWith("Bearer ").And.EndWith("not-a-real-key"));
        Assert.That(headers["OpenAI-Organization"], Is.EqualTo("org-test"));
        Assert.That(headers["OpenAI-Project"], Is.EqualTo("proj-test"));
        Assert.That(headers["User-Agent"], Does.Contain("decisions-test-app"));
        Assert.That(headers["X-Custom"], Is.EqualTo("custom-value"));
    }

    [Test]
    public void OptionsFreezeAfterClientConstruction()
    {
        DecisionsClientOptions options = new();
        _ = new DecisionsClient(s_credential, options);

        Assert.Throws<InvalidOperationException>(() => options.Endpoint = s_endpoint);
        Assert.Throws<InvalidOperationException>(() => options.OrganizationId = "org-test");
        Assert.Throws<InvalidOperationException>(() => options.ProjectId = "proj-test");
        Assert.Throws<InvalidOperationException>(() => options.UserAgentApplicationId = "test-app");
    }

    [Test]
    public void SettingsBindConfiguration()
    {
        IConfigurationRoot configuration = new ConfigurationBuilder().AddInMemoryCollection(CreateConfiguration()).Build();
        DecisionsClientSettings settings = configuration.GetClientSettings<DecisionsClientSettings>("Decisions");

        Assert.That(settings.Options.Endpoint, Is.EqualTo(s_endpoint));
        Assert.That(settings.Options.OrganizationId, Is.EqualTo("org-test"));
        Assert.That(settings.Options.ProjectId, Is.EqualTo("proj-test"));
        Assert.That(settings.Options.UserAgentApplicationId, Is.EqualTo("test-app"));
        Assert.That(new DecisionsClient(settings).Endpoint, Is.EqualTo(s_endpoint));
    }

    [Test]
    public void SettingsWithoutOptionsUseDefaults()
    {
        IConfigurationRoot configuration = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string>
        {
            ["Decisions:Credential:CredentialSource"] = "ApiKey",
            ["Decisions:Credential:Key"] = "not-a-real-key",
        }).Build();
        DecisionsClientSettings settings = configuration.GetClientSettings<DecisionsClientSettings>("Decisions");

        Assert.That(settings.Options, Is.Null);
        Assert.That(new DecisionsClient(settings).Endpoint, Is.EqualTo(new Uri("https://api.openai.com/v1")));
    }

    [TestCase(false)]
    [TestCase(true)]
    public void HostingRegistersConfiguredClient(bool keyed)
    {
        ConfigurationManager configuration = new();
        configuration.AddInMemoryCollection(CreateConfiguration());
        ServiceCollection services = new();
        Mock<IHostApplicationBuilder> builder = new();
        builder.Setup(b => b.Configuration).Returns(configuration);
        builder.Setup(b => b.Services).Returns(services);

        IClientBuilder clientBuilder = keyed
            ? builder.Object.AddKeyedDecisionsClient("decision-key", "Decisions")
            : builder.Object.AddDecisionsClient("Decisions");

        using ServiceProvider provider = services.BuildServiceProvider();
        DecisionsClient client = keyed
            ? provider.GetRequiredKeyedService<DecisionsClient>("decision-key")
            : provider.GetRequiredService<DecisionsClient>();

        Assert.That(clientBuilder, Is.Not.Null);
        Assert.That(client.Endpoint, Is.EqualTo(s_endpoint));
        Assert.That(client.Pipeline, Is.Not.Null);
    }

    [Test]
    public void HostingRejectsNullBuilder()
    {
        Assert.Throws<ArgumentNullException>(() => ((IHostApplicationBuilder)null).AddDecisionsClient("Decisions"));
        Assert.Throws<ArgumentNullException>(() => ((IHostApplicationBuilder)null).AddKeyedDecisionsClient("decision-key", "Decisions"));
    }

    private static Dictionary<string, string> CreateConfiguration() => new()
    {
        ["Decisions:Credential:CredentialSource"] = "ApiKey",
        ["Decisions:Credential:Key"] = "not-a-real-key",
        ["Decisions:Options:Endpoint"] = s_endpoint.AbsoluteUri,
        ["Decisions:Options:OrganizationId"] = "org-test",
        ["Decisions:Options:ProjectId"] = "proj-test",
        ["Decisions:Options:UserAgentApplicationId"] = "test-app",
    };

    private static AuthenticationPolicy CreateAuthenticationPolicy()
        => ApiKeyAuthenticationPolicy.CreateHeaderApiKeyPolicy(s_credential, "Authorization", "Bearer");

    private sealed class DerivedDecisionsClient : DecisionsClient
    {
        public DerivedDecisionsClient(ClientPipeline pipeline, DecisionsClientOptions options) : base(pipeline, options)
        {
        }
    }
}
