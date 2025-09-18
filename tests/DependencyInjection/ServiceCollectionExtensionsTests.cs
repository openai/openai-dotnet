using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using OpenAI.Audio;
using OpenAI.Chat;
using OpenAI.Embeddings;
using OpenAI.Extensions.DependencyInjection;
using OpenAI.Images;
using OpenAI.Moderations;
using System;
using System.ClientModel;

namespace OpenAI.Tests.DependencyInjection;

[TestFixture]
public class ServiceCollectionExtensionsTests
{
    private IServiceCollection _services;
    private const string TestApiKey = "test-api-key";
    private const string TestModel = "test-model";

    [SetUp]
    public void Setup()
    {
        _services = new ServiceCollection();
    }

    [Test]
    public void AddOpenAI_WithApiKey_RegistersOpenAIClient()
    {
        // Act
        _services.AddOpenAI(TestApiKey);
        var serviceProvider = _services.BuildServiceProvider();

        // Assert
        var client = serviceProvider.GetService<OpenAIClient>();
        Assert.That(client, Is.Not.Null);
    }

    [Test]
    public void AddOpenAI_WithApiKeyAndOptions_RegistersOpenAIClientWithOptions()
    {
        // Arrange
        var testEndpoint = new Uri("https://test.openai.com");

        // Act
        _services.AddOpenAI(TestApiKey, options =>
        {
            options.Endpoint = testEndpoint;
        });
        var serviceProvider = _services.BuildServiceProvider();

        // Assert
        var client = serviceProvider.GetService<OpenAIClient>();
        Assert.That(client, Is.Not.Null);
        Assert.That(client.Endpoint, Is.EqualTo(testEndpoint));
    }

    [Test]
    public void AddOpenAI_WithCredential_RegistersOpenAIClient()
    {
        // Arrange
        var credential = new ApiKeyCredential(TestApiKey);

        // Act
        _services.AddOpenAI(credential);
        var serviceProvider = _services.BuildServiceProvider();

        // Assert
        var client = serviceProvider.GetService<OpenAIClient>();
        Assert.That(client, Is.Not.Null);
    }

    [Test]
    public void AddOpenAI_WithNullServices_ThrowsArgumentNullException()
    {
        // Assert
        Assert.Throws<ArgumentNullException>(() =>
            ServiceCollectionExtensions.AddOpenAI(null, TestApiKey));
    }

    [Test]
    public void AddOpenAI_WithNullApiKey_ThrowsArgumentNullException()
    {
        // Assert
        Assert.Throws<ArgumentNullException>(() =>
            _services.AddOpenAI((string)null));
    }

    [Test]
    public void AddOpenAIChat_WithModel_RegistersChatClient()
    {
        // Act
        _services.AddOpenAIChat(TestModel, TestApiKey);
        var serviceProvider = _services.BuildServiceProvider();

        // Assert
        var client = serviceProvider.GetService<ChatClient>();
        Assert.That(client, Is.Not.Null);
    }

    [Test]
    public void AddOpenAIChat_WithExistingOpenAIClient_RegistersChatClient()
    {
        // Arrange
        _services.AddOpenAI(TestApiKey);

        // Act
        _services.AddOpenAIChat(TestModel);
        var serviceProvider = _services.BuildServiceProvider();

        // Assert
        var chatClient = serviceProvider.GetService<ChatClient>();
        var openAIClient = serviceProvider.GetService<OpenAIClient>();

        Assert.That(chatClient, Is.Not.Null);
        Assert.That(openAIClient, Is.Not.Null);
    }

    [Test]
    public void AddOpenAIChat_WithNullModel_ThrowsArgumentNullException()
    {
        // Assert
        Assert.Throws<ArgumentNullException>(() =>
            _services.AddOpenAIChat(null, TestApiKey));
    }

    [Test]
    public void AddOpenAIEmbeddings_WithModel_RegistersEmbeddingClient()
    {
        // Act
        _services.AddOpenAIEmbeddings(TestModel, TestApiKey);
        var serviceProvider = _services.BuildServiceProvider();

        // Assert
        var client = serviceProvider.GetService<EmbeddingClient>();
        Assert.That(client, Is.Not.Null);
    }

    [Test]
    public void AddOpenAIEmbeddings_WithExistingOpenAIClient_RegistersEmbeddingClient()
    {
        // Arrange
        _services.AddOpenAI(TestApiKey);

        // Act
        _services.AddOpenAIEmbeddings(TestModel);
        var serviceProvider = _services.BuildServiceProvider();

        // Assert
        var embeddingClient = serviceProvider.GetService<EmbeddingClient>();
        var openAIClient = serviceProvider.GetService<OpenAIClient>();

        Assert.That(embeddingClient, Is.Not.Null);
        Assert.That(openAIClient, Is.Not.Null);
    }

    [Test]
    public void AddOpenAIAudio_WithModel_RegistersAudioClient()
    {
        // Act
        _services.AddOpenAIAudio(TestModel, TestApiKey);
        var serviceProvider = _services.BuildServiceProvider();

        // Assert
        var client = serviceProvider.GetService<AudioClient>();
        Assert.That(client, Is.Not.Null);
    }

    [Test]
    public void AddOpenAIAudio_WithExistingOpenAIClient_RegistersAudioClient()
    {
        // Arrange
        _services.AddOpenAI(TestApiKey);

        // Act
        _services.AddOpenAIAudio(TestModel);
        var serviceProvider = _services.BuildServiceProvider();

        // Assert
        var audioClient = serviceProvider.GetService<AudioClient>();
        var openAIClient = serviceProvider.GetService<OpenAIClient>();

        Assert.That(audioClient, Is.Not.Null);
        Assert.That(openAIClient, Is.Not.Null);
    }

    [Test]
    public void AddOpenAIImages_WithModel_RegistersImageClient()
    {
        // Act
        _services.AddOpenAIImages(TestModel, TestApiKey);
        var serviceProvider = _services.BuildServiceProvider();

        // Assert
        var client = serviceProvider.GetService<ImageClient>();
        Assert.That(client, Is.Not.Null);
    }

    [Test]
    public void AddOpenAIImages_WithExistingOpenAIClient_RegistersImageClient()
    {
        // Arrange
        _services.AddOpenAI(TestApiKey);

        // Act
        _services.AddOpenAIImages(TestModel);
        var serviceProvider = _services.BuildServiceProvider();

        // Assert
        var imageClient = serviceProvider.GetService<ImageClient>();
        var openAIClient = serviceProvider.GetService<OpenAIClient>();

        Assert.That(imageClient, Is.Not.Null);
        Assert.That(openAIClient, Is.Not.Null);
    }

    [Test]
    public void AddOpenAIModeration_WithModel_RegistersModerationClient()
    {
        // Act
        _services.AddOpenAIModeration(TestModel, TestApiKey);
        var serviceProvider = _services.BuildServiceProvider();

        // Assert
        var client = serviceProvider.GetService<ModerationClient>();
        Assert.That(client, Is.Not.Null);
    }

    [Test]
    public void AddOpenAIModeration_WithExistingOpenAIClient_RegistersModerationClient()
    {
        // Arrange
        _services.AddOpenAI(TestApiKey);

        // Act
        _services.AddOpenAIModeration(TestModel);
        var serviceProvider = _services.BuildServiceProvider();

        // Assert
        var moderationClient = serviceProvider.GetService<ModerationClient>();
        var openAIClient = serviceProvider.GetService<OpenAIClient>();

        Assert.That(moderationClient, Is.Not.Null);
        Assert.That(openAIClient, Is.Not.Null);
    }

    [Test]
    public void AddOpenAI_WithEnvironmentVariableApiKey_RegistersClient()
    {
        // Arrange
        Environment.SetEnvironmentVariable("OPENAI_API_KEY", TestApiKey);

        try
        {
            // Act
            _services.AddOpenAI(options => { });
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
    public void AddOpenAI_WithoutApiKeyOrEnvironmentVariable_ThrowsInvalidOperationException()
    {
        // Arrange
        Environment.SetEnvironmentVariable("OPENAI_API_KEY", null);

        // Act & Assert
        _services.AddOpenAI(options => { });
        var serviceProvider = _services.BuildServiceProvider();

        Assert.Throws<InvalidOperationException>(() =>
            serviceProvider.GetService<OpenAIClient>());
    }

    [Test]
    public void AllExtensionMethods_RegisterClientsAsSingleton()
    {
        // Act
        _services.AddOpenAI(TestApiKey);
        _services.AddOpenAIChat(TestModel);
        _services.AddOpenAIEmbeddings(TestModel);
        _services.AddOpenAIAudio(TestModel);
        _services.AddOpenAIImages(TestModel);
        _services.AddOpenAIModeration(TestModel);

        var serviceProvider = _services.BuildServiceProvider();

        // Assert - Check that the same instance is returned (singleton behavior)
        var openAIClient1 = serviceProvider.GetService<OpenAIClient>();
        var openAIClient2 = serviceProvider.GetService<OpenAIClient>();
        Assert.That(openAIClient1, Is.SameAs(openAIClient2));

        var chatClient1 = serviceProvider.GetService<ChatClient>();
        var chatClient2 = serviceProvider.GetService<ChatClient>();
        Assert.That(chatClient1, Is.SameAs(chatClient2));

        var embeddingClient1 = serviceProvider.GetService<EmbeddingClient>();
        var embeddingClient2 = serviceProvider.GetService<EmbeddingClient>();
        Assert.That(embeddingClient1, Is.SameAs(embeddingClient2));

        var audioClient1 = serviceProvider.GetService<AudioClient>();
        var audioClient2 = serviceProvider.GetService<AudioClient>();
        Assert.That(audioClient1, Is.SameAs(audioClient2));

        var imageClient1 = serviceProvider.GetService<ImageClient>();
        var imageClient2 = serviceProvider.GetService<ImageClient>();
        Assert.That(imageClient1, Is.SameAs(imageClient2));

        var moderationClient1 = serviceProvider.GetService<ModerationClient>();
        var moderationClient2 = serviceProvider.GetService<ModerationClient>();
        Assert.That(moderationClient1, Is.SameAs(moderationClient2));
    }
}