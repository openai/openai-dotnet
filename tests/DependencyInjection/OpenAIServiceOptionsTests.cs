using NUnit.Framework;
using OpenAI.Extensions.DependencyInjection;
using System;

namespace OpenAI.Tests.DependencyInjection;

[TestFixture]
public class OpenAIServiceOptionsTests
{
    [Test]
    public void OpenAIServiceOptions_InheritsFromOpenAIClientOptions()
    {
        // Arrange & Act
        var options = new OpenAIServiceOptions();

        // Assert
        Assert.That(options, Is.InstanceOf<OpenAIClientOptions>());
    }

    [Test]
    public void OpenAIServiceOptions_HasDefaultValues()
    {
        // Arrange & Act
        var options = new OpenAIServiceOptions();

        // Assert
        Assert.That(options.DefaultChatModel, Is.EqualTo("gpt-4o"));
        Assert.That(options.DefaultEmbeddingModel, Is.EqualTo("text-embedding-3-small"));
        Assert.That(options.DefaultAudioModel, Is.EqualTo("whisper-1"));
        Assert.That(options.DefaultImageModel, Is.EqualTo("dall-e-3"));
        Assert.That(options.DefaultModerationModel, Is.EqualTo("text-moderation-latest"));
    }

    [Test]
    public void OpenAIServiceOptions_CanSetAllProperties()
    {
        // Arrange
        var options = new OpenAIServiceOptions();
        const string testApiKey = "test-api-key";
        const string testChatModel = "gpt-3.5-turbo";
        const string testEmbeddingModel = "text-embedding-ada-002";
        const string testAudioModel = "tts-1";
        const string testImageModel = "dall-e-2";
        const string testModerationModel = "text-moderation-stable";
        var testEndpoint = new Uri("https://test.openai.com");

        // Act
        options.ApiKey = testApiKey;
        options.DefaultChatModel = testChatModel;
        options.DefaultEmbeddingModel = testEmbeddingModel;
        options.DefaultAudioModel = testAudioModel;
        options.DefaultImageModel = testImageModel;
        options.DefaultModerationModel = testModerationModel;
        options.Endpoint = testEndpoint;

        // Assert
        Assert.That(options.ApiKey, Is.EqualTo(testApiKey));
        Assert.That(options.DefaultChatModel, Is.EqualTo(testChatModel));
        Assert.That(options.DefaultEmbeddingModel, Is.EqualTo(testEmbeddingModel));
        Assert.That(options.DefaultAudioModel, Is.EqualTo(testAudioModel));
        Assert.That(options.DefaultImageModel, Is.EqualTo(testImageModel));
        Assert.That(options.DefaultModerationModel, Is.EqualTo(testModerationModel));
        Assert.That(options.Endpoint, Is.EqualTo(testEndpoint));
    }

    [Test]
    public void OpenAIServiceOptions_InheritsBaseProperties()
    {
        // Arrange
        var options = new OpenAIServiceOptions();
        const string testOrganizationId = "test-org";
        const string testProjectId = "test-project";
        const string testUserAgent = "test-user-agent";

        // Act
        options.OrganizationId = testOrganizationId;
        options.ProjectId = testProjectId;
        options.UserAgentApplicationId = testUserAgent;

        // Assert
        Assert.That(options.OrganizationId, Is.EqualTo(testOrganizationId));
        Assert.That(options.ProjectId, Is.EqualTo(testProjectId));
        Assert.That(options.UserAgentApplicationId, Is.EqualTo(testUserAgent));
    }

    [Test]
    public void OpenAIServiceOptions_AllowsNullValues()
    {
        // Arrange & Act
        var options = new OpenAIServiceOptions
        {
            ApiKey = null,
            DefaultChatModel = null,
            DefaultEmbeddingModel = null,
            DefaultAudioModel = null,
            DefaultImageModel = null,
            DefaultModerationModel = null
        };

        // Assert
        Assert.That(options.ApiKey, Is.Null);
        Assert.That(options.DefaultChatModel, Is.Null);
        Assert.That(options.DefaultEmbeddingModel, Is.Null);
        Assert.That(options.DefaultAudioModel, Is.Null);
        Assert.That(options.DefaultImageModel, Is.Null);
        Assert.That(options.DefaultModerationModel, Is.Null);
    }
}