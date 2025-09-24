using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NUnit.Framework;
using OpenAI.Audio;
using OpenAI.Chat;
using OpenAI.Embeddings;
using OpenAI.Extensions.DependencyInjection;
using OpenAI.Images;
using OpenAI.Moderations;
using System;
using System.Collections.Generic;

namespace OpenAI.Tests.DependencyInjection;

[TestFixture]
public class ServiceCollectionExtensionsAdvancedTests
{
    private IServiceCollection _services;
    private IConfiguration _configuration;
    private const string TestApiKey = "test-api-key";

    [SetUp]
    public void Setup()
    {
        _services = new ServiceCollection();
        _configuration = CreateTestConfiguration();
    }

    private IConfiguration CreateTestConfiguration()
    {
        var configData = new Dictionary<string, string>
        {
            ["OpenAI:ApiKey"] = TestApiKey,
            ["OpenAI:Endpoint"] = "https://api.openai.com/v1",
            ["OpenAI:DefaultChatModel"] = "gpt-4o",
            ["OpenAI:DefaultEmbeddingModel"] = "text-embedding-3-small",
            ["OpenAI:DefaultAudioModel"] = "whisper-1",
            ["OpenAI:DefaultImageModel"] = "dall-e-3",
            ["OpenAI:DefaultModerationModel"] = "text-moderation-latest"
        };

        return new ConfigurationBuilder()
            .AddInMemoryCollection(configData)
            .Build();
    }

    [Test]
    public void AddOpenAIFromConfiguration_WithValidConfiguration_RegistersOpenAIClient()
    {
        // Act
        _services.AddOpenAIFromConfiguration(_configuration);
        var serviceProvider = _services.BuildServiceProvider();

        // Assert
        var client = serviceProvider.GetService<OpenAIClient>();
        Assert.That(client, Is.Not.Null);
    }

    [Test]
    public void AddOpenAIFromConfiguration_WithCustomSectionName_RegistersOpenAIClient()
    {
        // Arrange
        var customConfigData = new Dictionary<string, string>
        {
            ["CustomOpenAI:ApiKey"] = TestApiKey,
            ["CustomOpenAI:DefaultChatModel"] = "gpt-3.5-turbo"
        };
        var customConfig = new ConfigurationBuilder()
            .AddInMemoryCollection(customConfigData)
            .Build();

        // Act
        _services.AddOpenAIFromConfiguration(customConfig, "CustomOpenAI");
        var serviceProvider = _services.BuildServiceProvider();

        // Assert
        var client = serviceProvider.GetService<OpenAIClient>();
        Assert.That(client, Is.Not.Null);
    }

    [Test]
    public void AddOpenAIFromConfiguration_BindsOptionsCorrectly()
    {
        // Act
        _services.AddOpenAIFromConfiguration(_configuration);
        var serviceProvider = _services.BuildServiceProvider();

        // Assert
        var options = serviceProvider.GetService<IOptions<OpenAIServiceOptions>>();
        Assert.That(options, Is.Not.Null);
        Assert.That(options.Value.ApiKey, Is.EqualTo(TestApiKey));
        Assert.That(options.Value.DefaultChatModel, Is.EqualTo("gpt-4o"));
        Assert.That(options.Value.DefaultEmbeddingModel, Is.EqualTo("text-embedding-3-small"));
    }

    [Test]
    public void AddChatClientFromConfiguration_WithDefaultModel_RegistersChatClient()
    {
        // Arrange
        _services.AddOpenAIFromConfiguration(_configuration);

        // Act
        _services.AddChatClientFromConfiguration();
        var serviceProvider = _services.BuildServiceProvider();

        // Assert
        var client = serviceProvider.GetService<ChatClient>();
        Assert.That(client, Is.Not.Null);
    }

    [Test]
    public void AddChatClientFromConfiguration_WithSpecificModel_RegistersChatClient()
    {
        // Arrange
        _services.AddOpenAIFromConfiguration(_configuration);

        // Act
        _services.AddChatClientFromConfiguration("gpt-3.5-turbo");
        var serviceProvider = _services.BuildServiceProvider();

        // Assert
        var client = serviceProvider.GetService<ChatClient>();
        Assert.That(client, Is.Not.Null);
    }

    [Test]
    public void AddEmbeddingClientFromConfiguration_WithDefaultModel_RegistersEmbeddingClient()
    {
        // Arrange
        _services.AddOpenAIFromConfiguration(_configuration);

        // Act
        _services.AddEmbeddingClientFromConfiguration();
        var serviceProvider = _services.BuildServiceProvider();

        // Assert
        var client = serviceProvider.GetService<EmbeddingClient>();
        Assert.That(client, Is.Not.Null);
    }

    [Test]
    public void AddAudioClientFromConfiguration_WithDefaultModel_RegistersAudioClient()
    {
        // Arrange
        _services.AddOpenAIFromConfiguration(_configuration);

        // Act
        _services.AddAudioClientFromConfiguration();
        var serviceProvider = _services.BuildServiceProvider();

        // Assert
        var client = serviceProvider.GetService<AudioClient>();
        Assert.That(client, Is.Not.Null);
    }

    [Test]
    public void AddImageClientFromConfiguration_WithDefaultModel_RegistersImageClient()
    {
        // Arrange
        _services.AddOpenAIFromConfiguration(_configuration);

        // Act
        _services.AddImageClientFromConfiguration();
        var serviceProvider = _services.BuildServiceProvider();

        // Assert
        var client = serviceProvider.GetService<ImageClient>();
        Assert.That(client, Is.Not.Null);
    }

    [Test]
    public void AddModerationClientFromConfiguration_WithDefaultModel_RegistersModerationClient()
    {
        // Arrange
        _services.AddOpenAIFromConfiguration(_configuration);

        // Act
        _services.AddModerationClientFromConfiguration();
        var serviceProvider = _services.BuildServiceProvider();

        // Assert
        var client = serviceProvider.GetService<ModerationClient>();
        Assert.That(client, Is.Not.Null);
    }

    [Test]
    public void AddAllOpenAIClientsFromConfiguration_RegistersAllClients()
    {
        // Arrange
        _services.AddOpenAIFromConfiguration(_configuration);

        // Act
        _services.AddAllOpenAIClientsFromConfiguration();
        var serviceProvider = _services.BuildServiceProvider();

        // Assert
        Assert.That(serviceProvider.GetService<ChatClient>(), Is.Not.Null);
        Assert.That(serviceProvider.GetService<EmbeddingClient>(), Is.Not.Null);
        Assert.That(serviceProvider.GetService<AudioClient>(), Is.Not.Null);
        Assert.That(serviceProvider.GetService<ImageClient>(), Is.Not.Null);
        Assert.That(serviceProvider.GetService<ModerationClient>(), Is.Not.Null);
    }

    [Test]
    public void AddChatClientFromConfiguration_WithoutOpenAIConfiguration_ThrowsInvalidOperationException()
    {
        // Act & Assert
        _services.AddChatClientFromConfiguration();
        var serviceProvider = _services.BuildServiceProvider();

        Assert.Throws<InvalidOperationException>(() =>
            serviceProvider.GetService<ChatClient>());
    }

    [Test]
    public void AddChatClientFromConfiguration_WithEmptyDefaultModel_ThrowsInvalidOperationException()
    {
        // Arrange
        var configWithEmptyModel = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>
            {
                ["OpenAI:ApiKey"] = TestApiKey,
                ["OpenAI:DefaultChatModel"] = ""
            })
            .Build();

        _services.AddOpenAIFromConfiguration(configWithEmptyModel);

        // Act & Assert
        _services.AddChatClientFromConfiguration();
        var serviceProvider = _services.BuildServiceProvider();

        Assert.Throws<InvalidOperationException>(() =>
            serviceProvider.GetService<ChatClient>());
    }

    [Test]
    public void Configuration_SupportsEnvironmentVariableOverride()
    {
        // Arrange
        const string envApiKey = "env-api-key";
        Environment.SetEnvironmentVariable("OPENAI_API_KEY", envApiKey);

        var configWithoutApiKey = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>
            {
                ["OpenAI:DefaultChatModel"] = "gpt-4o"
            })
            .Build();

        try
        {
            // Act
            _services.AddOpenAIFromConfiguration(configWithoutApiKey);
            var serviceProvider = _services.BuildServiceProvider();

            // Assert
            var client = serviceProvider.GetService<OpenAIClient>();
            Assert.That(client, Is.Not.Null);
        }
        finally
        {
            Environment.SetEnvironmentVariable("OPENAI_API_KEY", null);
        }
    }

    [Test]
    public void AllConfigurationMethods_ThrowArgumentNullException_ForNullServices()
    {
        // Assert
        Assert.Throws<ArgumentNullException>(() =>
            ServiceCollectionExtensionsAdvanced.AddOpenAIFromConfiguration(null, _configuration));

        Assert.Throws<ArgumentNullException>(() =>
            ServiceCollectionExtensionsAdvanced.AddChatClientFromConfiguration(null));

        Assert.Throws<ArgumentNullException>(() =>
            ServiceCollectionExtensionsAdvanced.AddEmbeddingClientFromConfiguration(null));

        Assert.Throws<ArgumentNullException>(() =>
            ServiceCollectionExtensionsAdvanced.AddAudioClientFromConfiguration(null));

        Assert.Throws<ArgumentNullException>(() =>
            ServiceCollectionExtensionsAdvanced.AddImageClientFromConfiguration(null));

        Assert.Throws<ArgumentNullException>(() =>
            ServiceCollectionExtensionsAdvanced.AddModerationClientFromConfiguration(null));

        Assert.Throws<ArgumentNullException>(() =>
            ServiceCollectionExtensionsAdvanced.AddAllOpenAIClientsFromConfiguration(null));
    }

    [Test]
    public void AddOpenAIFromConfiguration_ThrowsArgumentNullException_ForNullConfiguration()
    {
        // Assert
        Assert.Throws<ArgumentNullException>(() =>
            _services.AddOpenAIFromConfiguration(null));
    }

    [Test]
    public void AddOpenAIFromConfiguration_ThrowsArgumentNullException_ForNullSectionName()
    {
        // Assert
        Assert.Throws<ArgumentNullException>(() =>
            _services.AddOpenAIFromConfiguration(_configuration, null));

        Assert.Throws<ArgumentNullException>(() =>
            _services.AddOpenAIFromConfiguration(_configuration, ""));
    }
}