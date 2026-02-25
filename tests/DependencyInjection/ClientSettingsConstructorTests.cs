using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Moq;
using NUnit.Framework;
using OpenAI.Assistants;
using OpenAI.Audio;
using OpenAI.Batch;
using OpenAI.Chat;
using OpenAI.Containers;
using OpenAI.Conversations;
using OpenAI.Embeddings;
using OpenAI.Evals;
using OpenAI.Files;
using OpenAI.FineTuning;
using OpenAI.Graders;
using OpenAI.Images;
using OpenAI.Models;
using OpenAI.Moderations;
using OpenAI.Realtime;
using OpenAI.Responses;
using OpenAI.VectorStores;
using OpenAI.Videos;
using System;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace OpenAI.Tests.DependencyInjection;

[Experimental("SCME0002")]
[TestFixture]
[NonParallelizable]
public class ClientSettingsConstructorTests
{
    #region GetClientSettings Binding Tests

    [Test]
    public void GetClientSettings_ChatClientSettings_BindsAllProperties()
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>
            {
                ["Chat:Credential:CredentialSource"] = "ApiKey",
                ["Chat:Credential:Key"] = "sk-test-key",
                ["Chat:Model"] = "gpt-4o-mini",
                ["Chat:Options:Endpoint"] = "https://custom.openai.com",
                ["Chat:Options:OrganizationId"] = "org-123",
                ["Chat:Options:ProjectId"] = "proj-456",
                ["Chat:Options:UserAgentApplicationId"] = "my-app"
            })
            .Build();

        var settings = config.GetClientSettings<ChatClientSettings>("Chat");

        Assert.That(settings, Is.Not.Null);
        Assert.That(settings.Model, Is.EqualTo("gpt-4o-mini"));
        Assert.That(settings.Credential, Is.Not.Null);
        Assert.That(settings.Options, Is.Not.Null);
        Assert.That(settings.Options.Endpoint, Is.EqualTo(new Uri("https://custom.openai.com")));
        Assert.That(settings.Options.OrganizationId, Is.EqualTo("org-123"));
        Assert.That(settings.Options.ProjectId, Is.EqualTo("proj-456"));
        Assert.That(settings.Options.UserAgentApplicationId, Is.EqualTo("my-app"));
    }

    [Test]
    public void GetClientSettings_ChatClientSettings_WithoutOptions_OptionsIsNull()
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>
            {
                ["Chat:Credential:CredentialSource"] = "ApiKey",
                ["Chat:Credential:Key"] = "sk-test-key",
                ["Chat:Model"] = "gpt-4"
            })
            .Build();

        var settings = config.GetClientSettings<ChatClientSettings>("Chat");

        Assert.That(settings.Model, Is.EqualTo("gpt-4"));
        Assert.That(settings.Options, Is.Null);
    }

    [Test]
    public void GetClientSettings_ChatClientSettings_EmptyConfig_PropertiesAreDefault()
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>())
            .Build();

        var settings = config.GetClientSettings<ChatClientSettings>("Chat");

        Assert.That(settings, Is.Not.Null);
        Assert.That(settings.Model, Is.Null);
        Assert.That(settings.Options, Is.Null);
        Assert.That(settings.Credential.CredentialSource, Is.Null);
    }

    [Test]
    public void GetClientSettings_AudioClientSettings_BindsModelAndOptions()
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>
            {
                ["Audio:Credential:CredentialSource"] = "ApiKey",
                ["Audio:Credential:Key"] = "sk-test-key",
                ["Audio:Model"] = "whisper-1",
                ["Audio:Options:Endpoint"] = "https://audio.openai.com"
            })
            .Build();

        var settings = config.GetClientSettings<AudioClientSettings>("Audio");

        Assert.That(settings.Model, Is.EqualTo("whisper-1"));
        Assert.That(settings.Options, Is.Not.Null);
        Assert.That(settings.Options.Endpoint, Is.EqualTo(new Uri("https://audio.openai.com")));
    }

    [Test]
    public void GetClientSettings_EmbeddingClientSettings_BindsModelAndOptions()
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>
            {
                ["Embedding:Credential:CredentialSource"] = "ApiKey",
                ["Embedding:Credential:Key"] = "sk-test-key",
                ["Embedding:Model"] = "text-embedding-3-small",
                ["Embedding:Options:OrganizationId"] = "org-embed"
            })
            .Build();

        var settings = config.GetClientSettings<EmbeddingClientSettings>("Embedding");

        Assert.That(settings.Model, Is.EqualTo("text-embedding-3-small"));
        Assert.That(settings.Options.OrganizationId, Is.EqualTo("org-embed"));
    }

    [Test]
    public void GetClientSettings_ImageClientSettings_BindsModelAndOptions()
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>
            {
                ["Image:Credential:CredentialSource"] = "ApiKey",
                ["Image:Credential:Key"] = "sk-test-key",
                ["Image:Model"] = "dall-e-3",
                ["Image:Options:ProjectId"] = "proj-img"
            })
            .Build();

        var settings = config.GetClientSettings<ImageClientSettings>("Image");

        Assert.That(settings.Model, Is.EqualTo("dall-e-3"));
        Assert.That(settings.Options.ProjectId, Is.EqualTo("proj-img"));
    }

    [Test]
    public void GetClientSettings_ModerationClientSettings_BindsModel()
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>
            {
                ["Moderation:Credential:CredentialSource"] = "ApiKey",
                ["Moderation:Credential:Key"] = "sk-test-key",
                ["Moderation:Model"] = "text-moderation-latest"
            })
            .Build();

        var settings = config.GetClientSettings<ModerationClientSettings>("Moderation");

        Assert.That(settings.Model, Is.EqualTo("text-moderation-latest"));
    }

    [Test]
    public void GetClientSettings_ResponsesClientSettings_BindsModel()
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>
            {
                ["Responses:Credential:CredentialSource"] = "ApiKey",
                ["Responses:Credential:Key"] = "sk-test-key",
                ["Responses:Model"] = "gpt-4o"
            })
            .Build();

        var settings = config.GetClientSettings<ResponsesClientSettings>("Responses");

        Assert.That(settings.Model, Is.EqualTo("gpt-4o"));
    }

    [Test]
    public void GetClientSettings_AssistantClientSettings_BindsOptionsOnly()
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>
            {
                ["Assistant:Credential:CredentialSource"] = "ApiKey",
                ["Assistant:Credential:Key"] = "sk-test-key",
                ["Assistant:Options:Endpoint"] = "https://assistant.openai.com",
                ["Assistant:Options:OrganizationId"] = "org-assist"
            })
            .Build();

        var settings = config.GetClientSettings<AssistantClientSettings>("Assistant");

        Assert.That(settings, Is.Not.Null);
        Assert.That(settings.Credential, Is.Not.Null);
        Assert.That(settings.Options, Is.Not.Null);
        Assert.That(settings.Options.Endpoint, Is.EqualTo(new Uri("https://assistant.openai.com")));
        Assert.That(settings.Options.OrganizationId, Is.EqualTo("org-assist"));
    }

    [Test]
    public void GetClientSettings_BatchClientSettings_BindsOptions()
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>
            {
                ["Batch:Credential:CredentialSource"] = "ApiKey",
                ["Batch:Credential:Key"] = "sk-test-key",
                ["Batch:Options:Endpoint"] = "https://batch.openai.com"
            })
            .Build();

        var settings = config.GetClientSettings<BatchClientSettings>("Batch");

        Assert.That(settings.Options.Endpoint, Is.EqualTo(new Uri("https://batch.openai.com")));
    }

    [Test]
    public void GetClientSettings_RealtimeClientSettings_BindsOptions()
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>
            {
                ["Realtime:Credential:CredentialSource"] = "ApiKey",
                ["Realtime:Credential:Key"] = "sk-test-key",
                ["Realtime:Options:Endpoint"] = "https://realtime.openai.com"
            })
            .Build();

        var settings = config.GetClientSettings<RealtimeClientSettings>("Realtime");

        Assert.That(settings.Options.Endpoint, Is.EqualTo(new Uri("https://realtime.openai.com")));
    }

    [Test]
    public void GetClientSettings_WithOnlyCredential_CredentialIsSet()
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>
            {
                ["Section:Credential:CredentialSource"] = "ApiKey",
                ["Section:Credential:Key"] = "sk-only-key"
            })
            .Build();

        var settings = config.GetClientSettings<BatchClientSettings>("Section");

        Assert.That(settings.Credential.CredentialSource, Is.EqualTo("ApiKey"));
        Assert.That(settings.Options, Is.Null);
    }

    [Test]
    public void GetClientSettings_MissingSectionName_ReturnsDefaultSettings()
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>
            {
                ["Other:Credential:CredentialSource"] = "ApiKey",
                ["Other:Credential:Key"] = "sk-test-key"
            })
            .Build();

        var settings = config.GetClientSettings<ChatClientSettings>("NonExistentSection");

        Assert.That(settings, Is.Not.Null);
        Assert.That(settings.Model, Is.Null);
        Assert.That(settings.Options, Is.Null);
        Assert.That(settings.Credential.CredentialSource, Is.Null);
    }

    #endregion

    #region Constructor Null Settings Tests

    [Test]
    public void ChatClient_NullSettings_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new ChatClient((ChatClientSettings)null));
    }

    [Test]
    public void AudioClient_NullSettings_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new AudioClient((AudioClientSettings)null));
    }

    [Test]
    public void EmbeddingClient_NullSettings_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new EmbeddingClient((EmbeddingClientSettings)null));
    }

    [Test]
    public void ImageClient_NullSettings_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new ImageClient((ImageClientSettings)null));
    }

    [Test]
    public void ModerationClient_NullSettings_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new ModerationClient((ModerationClientSettings)null));
    }

    [Test]
    public void ResponsesClient_NullSettings_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new ResponsesClient((ResponsesClientSettings)null));
    }

    [Test]
    public void AssistantClient_NullSettings_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new AssistantClient((AssistantClientSettings)null));
    }

    [Test]
    public void BatchClient_NullSettings_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new BatchClient((BatchClientSettings)null));
    }

    [Test]
    public void ContainerClient_NullSettings_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new ContainerClient((ContainerClientSettings)null));
    }

    [Test]
    public void ConversationClient_NullSettings_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new ConversationClient((ConversationClientSettings)null));
    }

    [Test]
    public void EvaluationClient_NullSettings_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new EvaluationClient((EvaluationClientSettings)null));
    }

    [Test]
    public void OpenAIFileClient_NullSettings_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new OpenAIFileClient((OpenAIFileClientSettings)null));
    }

    [Test]
    public void FineTuningClient_NullSettings_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new FineTuningClient((FineTuningClientSettings)null));
    }

    [Test]
    public void GraderClient_NullSettings_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new GraderClient((GraderClientSettings)null));
    }

    [Test]
    public void OpenAIModelClient_NullSettings_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new OpenAIModelClient((OpenAIModelClientSettings)null));
    }

    [Test]
    public void VectorStoreClient_NullSettings_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new VectorStoreClient((VectorStoreClientSettings)null));
    }

    [Test]
    public void VideoClient_NullSettings_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new VideoClient((VideoClientSettings)null));
    }

    [Test]
    public void RealtimeClient_NullSettings_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new RealtimeClient((RealtimeClientSettings)null));
    }

    #endregion

    #region Constructor Valid Settings Tests - Model-Based Clients

    [Test]
    public void ChatClient_ValidSettings_ConstructsSuccessfully()
    {
        var settings = CreateModelSettings<ChatClientSettings>("gpt-4o-mini");

        var client = new ChatClient(settings);

        Assert.That(client, Is.Not.Null);
        Assert.That(client.Endpoint, Is.Not.Null);
    }

    [Test]
    public void ChatClient_ValidSettings_CustomEndpoint_UsesCustomEndpoint()
    {
        var settings = CreateModelSettings<ChatClientSettings>("gpt-4", "https://custom.api.com");

        var client = new ChatClient(settings);

        Assert.That(client.Endpoint, Is.EqualTo(new Uri("https://custom.api.com")));
    }

    [Test]
    public void AudioClient_ValidSettings_ConstructsSuccessfully()
    {
        var settings = CreateModelSettings<AudioClientSettings>("whisper-1");

        var client = new AudioClient(settings);

        Assert.That(client, Is.Not.Null);
        Assert.That(client.Endpoint, Is.Not.Null);
    }

    [Test]
    public void EmbeddingClient_ValidSettings_ConstructsSuccessfully()
    {
        var settings = CreateModelSettings<EmbeddingClientSettings>("text-embedding-3-small");

        var client = new EmbeddingClient(settings);

        Assert.That(client, Is.Not.Null);
        Assert.That(client.Endpoint, Is.Not.Null);
    }

    [Test]
    public void ImageClient_ValidSettings_ConstructsSuccessfully()
    {
        var settings = CreateModelSettings<ImageClientSettings>("dall-e-3");

        var client = new ImageClient(settings);

        Assert.That(client, Is.Not.Null);
        Assert.That(client.Endpoint, Is.Not.Null);
    }

    [Test]
    public void ModerationClient_ValidSettings_ConstructsSuccessfully()
    {
        var settings = CreateModelSettings<ModerationClientSettings>("text-moderation-latest");

        var client = new ModerationClient(settings);

        Assert.That(client, Is.Not.Null);
        Assert.That(client.Endpoint, Is.Not.Null);
    }

    [Test]
    public void ResponsesClient_ValidSettings_ConstructsSuccessfully()
    {
        var settings = CreateModelSettings<ResponsesClientSettings>("gpt-4o");

        var client = new ResponsesClient(settings);

        Assert.That(client, Is.Not.Null);
        Assert.That(client.Endpoint, Is.Not.Null);
    }

    #endregion

    #region Constructor Valid Settings Tests - Non-Model Clients

    [Test]
    public void AssistantClient_ValidSettings_ConstructsSuccessfully()
    {
        var settings = CreateNonModelSettings<AssistantClientSettings>();

        var client = new AssistantClient(settings);

        Assert.That(client, Is.Not.Null);
        Assert.That(client.Endpoint, Is.Not.Null);
    }

    [Test]
    public void BatchClient_ValidSettings_ConstructsSuccessfully()
    {
        var settings = CreateNonModelSettings<BatchClientSettings>();

        var client = new BatchClient(settings);

        Assert.That(client, Is.Not.Null);
        Assert.That(client.Endpoint, Is.Not.Null);
    }

    [Test]
    public void ContainerClient_ValidSettings_ConstructsSuccessfully()
    {
        var settings = CreateNonModelSettings<ContainerClientSettings>();

        var client = new ContainerClient(settings);

        Assert.That(client, Is.Not.Null);
        Assert.That(client.Endpoint, Is.Not.Null);
    }

    [Test]
    public void ConversationClient_ValidSettings_ConstructsSuccessfully()
    {
        var settings = CreateNonModelSettings<ConversationClientSettings>();

        var client = new ConversationClient(settings);

        Assert.That(client, Is.Not.Null);
        Assert.That(client.Endpoint, Is.Not.Null);
    }

    [Test]
    public void EvaluationClient_ValidSettings_ConstructsSuccessfully()
    {
        var settings = CreateNonModelSettings<EvaluationClientSettings>();

        var client = new EvaluationClient(settings);

        Assert.That(client, Is.Not.Null);
        Assert.That(client.Endpoint, Is.Not.Null);
    }

    [Test]
    public void OpenAIFileClient_ValidSettings_ConstructsSuccessfully()
    {
        var settings = CreateNonModelSettings<OpenAIFileClientSettings>();

        var client = new OpenAIFileClient(settings);

        Assert.That(client, Is.Not.Null);
        Assert.That(client.Endpoint, Is.Not.Null);
    }

    [Test]
    public void FineTuningClient_ValidSettings_ConstructsSuccessfully()
    {
        var settings = CreateNonModelSettings<FineTuningClientSettings>();

        var client = new FineTuningClient(settings);

        Assert.That(client, Is.Not.Null);
        Assert.That(client.Endpoint, Is.Not.Null);
    }

    [Test]
    public void GraderClient_ValidSettings_ConstructsSuccessfully()
    {
        var settings = CreateNonModelSettings<GraderClientSettings>();

        var client = new GraderClient(settings);

        Assert.That(client, Is.Not.Null);
        Assert.That(client.Endpoint, Is.Not.Null);
    }

    [Test]
    public void OpenAIModelClient_ValidSettings_ConstructsSuccessfully()
    {
        var settings = CreateNonModelSettings<OpenAIModelClientSettings>();

        var client = new OpenAIModelClient(settings);

        Assert.That(client, Is.Not.Null);
        Assert.That(client.Endpoint, Is.Not.Null);
    }

    [Test]
    public void VectorStoreClient_ValidSettings_ConstructsSuccessfully()
    {
        var settings = CreateNonModelSettings<VectorStoreClientSettings>();

        var client = new VectorStoreClient(settings);

        Assert.That(client, Is.Not.Null);
        Assert.That(client.Endpoint, Is.Not.Null);
    }

    [Test]
    public void VideoClient_ValidSettings_ConstructsSuccessfully()
    {
        var settings = CreateNonModelSettings<VideoClientSettings>();

        var client = new VideoClient(settings);

        Assert.That(client, Is.Not.Null);
        Assert.That(client.Endpoint, Is.Not.Null);
    }

    [Test]
    public void RealtimeClient_ValidSettings_ConstructsSuccessfully()
    {
        var settings = CreateNonModelSettings<RealtimeClientSettings>();

        var client = new RealtimeClient(settings);

        Assert.That(client, Is.Not.Null);
        Assert.That(client.Endpoint, Is.Not.Null);
    }

    #endregion

    #region Constructor Missing Required Fields Tests

    [Test]
    public void ChatClient_SettingsWithoutModel_Throws()
    {
        var settings = CreateNonModelSettings<ChatClientSettings>();

        Assert.Throws<ArgumentNullException>(() => new ChatClient(settings));
    }

    [Test]
    public void AudioClient_SettingsWithoutModel_Throws()
    {
        var settings = CreateNonModelSettings<AudioClientSettings>();

        Assert.Throws<ArgumentNullException>(() => new AudioClient(settings));
    }

    [Test]
    public void EmbeddingClient_SettingsWithoutModel_Throws()
    {
        var settings = CreateNonModelSettings<EmbeddingClientSettings>();

        Assert.Throws<ArgumentNullException>(() => new EmbeddingClient(settings));
    }

    [Test]
    public void ImageClient_SettingsWithoutModel_Throws()
    {
        var settings = CreateNonModelSettings<ImageClientSettings>();

        Assert.Throws<ArgumentNullException>(() => new ImageClient(settings));
    }

    [Test]
    public void ModerationClient_SettingsWithoutModel_Throws()
    {
        var settings = CreateNonModelSettings<ModerationClientSettings>();

        Assert.Throws<ArgumentNullException>(() => new ModerationClient(settings));
    }

    [Test]
    public void ResponsesClient_SettingsWithoutModel_Throws()
    {
        var settings = CreateNonModelSettings<ResponsesClientSettings>();

        Assert.Throws<ArgumentNullException>(() => new ResponsesClient(settings));
    }

    [Test]
    public void ChatClient_SettingsWithEmptyModel_Throws()
    {
        var settings = CreateModelSettings<ChatClientSettings>("");

        Assert.That(() => new ChatClient(settings), Throws.InstanceOf<ArgumentException>());
    }

    [Test]
    public void ChatClient_SettingsWithoutCredential_Throws()
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>
            {
                ["Chat:Model"] = "gpt-4"
            })
            .Build();

        var settings = config.GetClientSettings<ChatClientSettings>("Chat");

        Assert.Throws<ArgumentNullException>(() => new ChatClient(settings));
    }

    [Test]
    public void AssistantClient_SettingsWithoutCredential_Throws()
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>())
            .Build();

        var settings = config.GetClientSettings<AssistantClientSettings>("Assistant");

        Assert.Throws<ArgumentNullException>(() => new AssistantClient(settings));
    }

    #endregion

    #region Constructor Settings with Options Tests

    [Test]
    public void ChatClient_SettingsWithNullOptions_UsesDefaultEndpoint()
    {
        var settings = CreateModelSettings<ChatClientSettings>("gpt-4");

        var client = new ChatClient(settings);

        Assert.That(client.Endpoint, Is.EqualTo(new Uri("https://api.openai.com/v1")));
    }

    [Test]
    public void AssistantClient_SettingsWithNullOptions_UsesDefaultEndpoint()
    {
        var settings = CreateNonModelSettings<AssistantClientSettings>();

        var client = new AssistantClient(settings);

        Assert.That(client.Endpoint, Is.EqualTo(new Uri("https://api.openai.com/v1")));
    }

    [Test]
    public void ChatClient_SettingsWithCustomEndpoint_UsesCustomEndpoint()
    {
        var settings = CreateModelSettings<ChatClientSettings>("gpt-4", "https://my-proxy.example.com");

        var client = new ChatClient(settings);

        Assert.That(client.Endpoint, Is.EqualTo(new Uri("https://my-proxy.example.com")));
    }

    [Test]
    public void AssistantClient_SettingsWithCustomEndpoint_UsesCustomEndpoint()
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>
            {
                ["Assistant:Credential:CredentialSource"] = "ApiKey",
                ["Assistant:Credential:Key"] = "sk-test-key",
                ["Assistant:Options:Endpoint"] = "https://my-proxy.example.com"
            })
            .Build();

        var settings = config.GetClientSettings<AssistantClientSettings>("Assistant");
        var client = new AssistantClient(settings);

        Assert.That(client.Endpoint, Is.EqualTo(new Uri("https://my-proxy.example.com")));
    }

    [Test]
    public void RealtimeClient_SettingsWithCustomEndpoint_UsesCustomEndpoint()
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>
            {
                ["Realtime:Credential:CredentialSource"] = "ApiKey",
                ["Realtime:Credential:Key"] = "sk-test-key",
                ["Realtime:Options:Endpoint"] = "https://realtime-proxy.example.com"
            })
            .Build();

        var settings = config.GetClientSettings<RealtimeClientSettings>("Realtime");
        var client = new RealtimeClient(settings);

        Assert.That(client.Endpoint, Is.Not.Null);
    }

    #endregion

    #region Helper Methods

    private static T CreateModelSettings<T>(string model, string endpoint = null) where T : ClientSettings, new()
    {
        var configValues = new Dictionary<string, string>
        {
            ["Section:Credential:CredentialSource"] = "ApiKey",
            ["Section:Credential:Key"] = "sk-test-key",
            ["Section:Model"] = model
        };

        if (endpoint != null)
        {
            configValues["Section:Options:Endpoint"] = endpoint;
        }

        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(configValues)
            .Build();

        return config.GetClientSettings<T>("Section");
    }

    private static T CreateNonModelSettings<T>(string endpoint = null) where T : ClientSettings, new()
    {
        var configValues = new Dictionary<string, string>
        {
            ["Section:Credential:CredentialSource"] = "ApiKey",
            ["Section:Credential:Key"] = "sk-test-key"
        };

        if (endpoint != null)
        {
            configValues["Section:Options:Endpoint"] = endpoint;
        }

        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(configValues)
            .Build();

        return config.GetClientSettings<T>("Section");
    }

    #endregion
}
