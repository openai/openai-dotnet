using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Moq;
using NUnit.Framework;
using System;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace OpenAI.Tests.DependencyInjection;

[Experimental("SCME0002")]
[TestFixture]
[NonParallelizable]
public class OpenAIHostBuilderExtensionsTests
{
    private Mock<IHostApplicationBuilder> _mockBuilder;
    private ServiceCollection _services;
    private ConfigurationManager _configurationManager;

    [SetUp]
    public void SetUp()
    {
        _services = new ServiceCollection();
        _configurationManager = new ConfigurationManager();
        _mockBuilder = new Mock<IHostApplicationBuilder>();

        _mockBuilder.Setup(b => b.Services).Returns(_services);
        _mockBuilder.Setup(b => b.Configuration).Returns(_configurationManager);
    }

    private void SetupConfiguration(Dictionary<string, string> configValues)
    {
        _configurationManager = new ConfigurationManager();
        _configurationManager.AddInMemoryCollection(configValues);
        _mockBuilder.Setup(b => b.Configuration).Returns(_configurationManager);
    }

    #region Null Argument Validation Tests

    [Test]
    public void AddAssistantClient_NullBuilder_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => ((IHostApplicationBuilder)null).AddAssistantClient("OpenAI:Assistant"));
    }

    [Test]
    public void AddKeyedAssistantClient_NullBuilder_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => ((IHostApplicationBuilder)null).AddKeyedAssistantClient("key", "OpenAI:Assistant"));
    }

    [Test]
    public void AddAudioClient_NullBuilder_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => ((IHostApplicationBuilder)null).AddAudioClient("OpenAI:Audio"));
    }

    [Test]
    public void AddKeyedAudioClient_NullBuilder_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => ((IHostApplicationBuilder)null).AddKeyedAudioClient("key", "OpenAI:Audio"));
    }

    [Test]
    public void AddBatchClient_NullBuilder_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => ((IHostApplicationBuilder)null).AddBatchClient("OpenAI:Batch"));
    }

    [Test]
    public void AddKeyedBatchClient_NullBuilder_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => ((IHostApplicationBuilder)null).AddKeyedBatchClient("key", "OpenAI:Batch"));
    }

    [Test]
    public void AddChatClient_NullBuilder_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => ((IHostApplicationBuilder)null).AddChatClient("OpenAI:Chat"));
    }

    [Test]
    public void AddKeyedChatClient_NullBuilder_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => ((IHostApplicationBuilder)null).AddKeyedChatClient("key", "OpenAI:Chat"));
    }

    [Test]
    public void AddEmbeddingClient_NullBuilder_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => ((IHostApplicationBuilder)null).AddEmbeddingClient("OpenAI:Embedding"));
    }

    [Test]
    public void AddKeyedEmbeddingClient_NullBuilder_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => ((IHostApplicationBuilder)null).AddKeyedEmbeddingClient("key", "OpenAI:Embedding"));
    }

    [Test]
    public void AddOpenAIFileClient_NullBuilder_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => ((IHostApplicationBuilder)null).AddOpenAIFileClient("OpenAI:File"));
    }

    [Test]
    public void AddKeyedOpenAIFileClient_NullBuilder_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => ((IHostApplicationBuilder)null).AddKeyedOpenAIFileClient("key", "OpenAI:File"));
    }

    [Test]
    public void AddFineTuningClient_NullBuilder_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => ((IHostApplicationBuilder)null).AddFineTuningClient("OpenAI:FineTuning"));
    }

    [Test]
    public void AddKeyedFineTuningClient_NullBuilder_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => ((IHostApplicationBuilder)null).AddKeyedFineTuningClient("key", "OpenAI:FineTuning"));
    }

    [Test]
    public void AddImageClient_NullBuilder_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => ((IHostApplicationBuilder)null).AddImageClient("OpenAI:Image"));
    }

    [Test]
    public void AddKeyedImageClient_NullBuilder_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => ((IHostApplicationBuilder)null).AddKeyedImageClient("key", "OpenAI:Image"));
    }

    [Test]
    public void AddOpenAIModelClient_NullBuilder_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => ((IHostApplicationBuilder)null).AddOpenAIModelClient("OpenAI:Model"));
    }

    [Test]
    public void AddKeyedOpenAIModelClient_NullBuilder_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => ((IHostApplicationBuilder)null).AddKeyedOpenAIModelClient("key", "OpenAI:Model"));
    }

    [Test]
    public void AddModerationClient_NullBuilder_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => ((IHostApplicationBuilder)null).AddModerationClient("OpenAI:Moderation"));
    }

    [Test]
    public void AddKeyedModerationClient_NullBuilder_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => ((IHostApplicationBuilder)null).AddKeyedModerationClient("key", "OpenAI:Moderation"));
    }

    [Test]
    public void AddRealtimeClient_NullBuilder_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => ((IHostApplicationBuilder)null).AddRealtimeClient("OpenAI:Realtime"));
    }

    [Test]
    public void AddKeyedRealtimeClient_NullBuilder_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => ((IHostApplicationBuilder)null).AddKeyedRealtimeClient("key", "OpenAI:Realtime"));
    }

    [Test]
    public void AddResponsesClient_NullBuilder_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => ((IHostApplicationBuilder)null).AddResponsesClient("OpenAI:Responses"));
    }

    [Test]
    public void AddKeyedResponsesClient_NullBuilder_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => ((IHostApplicationBuilder)null).AddKeyedResponsesClient("key", "OpenAI:Responses"));
    }

    [Test]
    public void AddVectorStoreClient_NullBuilder_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => ((IHostApplicationBuilder)null).AddVectorStoreClient("OpenAI:VectorStore"));
    }

    [Test]
    public void AddKeyedVectorStoreClient_NullBuilder_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => ((IHostApplicationBuilder)null).AddKeyedVectorStoreClient("key", "OpenAI:VectorStore"));
    }

    [Test]
    public void AddVideoClient_NullBuilder_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => ((IHostApplicationBuilder)null).AddVideoClient("OpenAI:Video"));
    }

    [Test]
    public void AddKeyedVideoClient_NullBuilder_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => ((IHostApplicationBuilder)null).AddKeyedVideoClient("key", "OpenAI:Video"));
    }

    [Test]
    public void AddContainerClient_NullBuilder_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => ((IHostApplicationBuilder)null).AddContainerClient("OpenAI:Container"));
    }

    [Test]
    public void AddKeyedContainerClient_NullBuilder_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => ((IHostApplicationBuilder)null).AddKeyedContainerClient("key", "OpenAI:Container"));
    }

    [Test]
    public void AddConversationClient_NullBuilder_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => ((IHostApplicationBuilder)null).AddConversationClient("OpenAI:Conversation"));
    }

    [Test]
    public void AddKeyedConversationClient_NullBuilder_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => ((IHostApplicationBuilder)null).AddKeyedConversationClient("key", "OpenAI:Conversation"));
    }

    [Test]
    public void AddEvaluationClient_NullBuilder_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => ((IHostApplicationBuilder)null).AddEvaluationClient("OpenAI:Evaluation"));
    }

    [Test]
    public void AddKeyedEvaluationClient_NullBuilder_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => ((IHostApplicationBuilder)null).AddKeyedEvaluationClient("key", "OpenAI:Evaluation"));
    }

    [Test]
    public void AddGraderClient_NullBuilder_ThrowsArgumentNullException()
    {
        IHostApplicationBuilder builder = null;

        Assert.Throws<ArgumentNullException>(() => builder!.AddGraderClient("OpenAI:Grader"));
        Assert.Throws<ArgumentNullException>(() => ((IHostApplicationBuilder)null).AddGraderClient("OpenAI:Grader"));
    }

    [Test]
    public void AddKeyedGraderClient_NullBuilder_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => ((IHostApplicationBuilder)null).AddKeyedGraderClient("key", "OpenAI:Grader"));
    }

    #endregion

    #region Extension Method Returns IClientBuilder Tests

    [Test]
    public void AddAssistantClient_ReturnsClientBuilder()
    {
        SetupConfiguration(new Dictionary<string, string>
        {
            ["OpenAI:Assistant:ApiKey"] = "test-key"
        });

        var result = _mockBuilder.Object.AddAssistantClient("OpenAI:Assistant");

        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.InstanceOf<IClientBuilder>());
    }

    [Test]
    public void AddKeyedAssistantClient_ReturnsClientBuilder()
    {
        SetupConfiguration(new Dictionary<string, string>
        {
            ["OpenAI:Assistant:ApiKey"] = "test-key"
        });

        var result = _mockBuilder.Object.AddKeyedAssistantClient("myKey", "OpenAI:Assistant");

        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.InstanceOf<IClientBuilder>());
    }

    [Test]
    public void AddAudioClient_ReturnsClientBuilder()
    {
        SetupConfiguration(new Dictionary<string, string>
        {
            ["OpenAI:Audio:ApiKey"] = "test-key",
            ["OpenAI:Audio:Model"] = "whisper-1"
        });

        var result = _mockBuilder.Object.AddAudioClient("OpenAI:Audio");

        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.InstanceOf<IClientBuilder>());
    }

    [Test]
    public void AddChatClient_ReturnsClientBuilder()
    {
        SetupConfiguration(new Dictionary<string, string>
        {
            ["OpenAI:Chat:ApiKey"] = "test-key",
            ["OpenAI:Chat:Model"] = "gpt-4"
        });

        var result = _mockBuilder.Object.AddChatClient("OpenAI:Chat");

        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.InstanceOf<IClientBuilder>());
    }

    [Test]
    public void AddKeyedChatClient_ReturnsClientBuilder()
    {
        SetupConfiguration(new Dictionary<string, string>
        {
            ["OpenAI:Chat:ApiKey"] = "test-key",
            ["OpenAI:Chat:Model"] = "gpt-4"
        });

        var result = _mockBuilder.Object.AddKeyedChatClient("chatKey", "OpenAI:Chat");

        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.InstanceOf<IClientBuilder>());
    }

    [Test]
    public void AddEmbeddingClient_ReturnsClientBuilder()
    {
        SetupConfiguration(new Dictionary<string, string>
        {
            ["OpenAI:Embedding:ApiKey"] = "test-key",
            ["OpenAI:Embedding:Model"] = "text-embedding-ada-002"
        });

        var result = _mockBuilder.Object.AddEmbeddingClient("OpenAI:Embedding");

        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.InstanceOf<IClientBuilder>());
    }

    [Test]
    public void AddImageClient_ReturnsClientBuilder()
    {
        SetupConfiguration(new Dictionary<string, string>
        {
            ["OpenAI:Image:ApiKey"] = "test-key",
            ["OpenAI:Image:Model"] = "dall-e-3"
        });

        var result = _mockBuilder.Object.AddImageClient("OpenAI:Image");

        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.InstanceOf<IClientBuilder>());
    }

    [Test]
    public void AddModerationClient_ReturnsClientBuilder()
    {
        SetupConfiguration(new Dictionary<string, string>
        {
            ["OpenAI:Moderation:ApiKey"] = "test-key",
            ["OpenAI:Moderation:Model"] = "text-moderation-latest"
        });

        var result = _mockBuilder.Object.AddModerationClient("OpenAI:Moderation");

        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.InstanceOf<IClientBuilder>());
    }

    [Test]
    public void AddVectorStoreClient_ReturnsClientBuilder()
    {
        SetupConfiguration(new Dictionary<string, string>
        {
            ["OpenAI:VectorStore:ApiKey"] = "test-key"
        });

        var result = _mockBuilder.Object.AddVectorStoreClient("OpenAI:VectorStore");

        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.InstanceOf<IClientBuilder>());
    }

    [Test]
    public void AddResponsesClient_ReturnsClientBuilder()
    {
        SetupConfiguration(new Dictionary<string, string>
        {
            ["OpenAI:Responses:ApiKey"] = "test-key"
        });

        var result = _mockBuilder.Object.AddResponsesClient("OpenAI:Responses");

        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.InstanceOf<IClientBuilder>());
    }

    #endregion

    #region Configuration Binding Tests

    [Test]
    public void AddChatClient_WithNestedOptions_BindsConfigurationCorrectly()
    {
        SetupConfiguration(new Dictionary<string, string>
        {
            ["OpenAI:Chat:ApiKey"] = "sk-test-api-key",
            ["OpenAI:Chat:Model"] = "gpt-4-turbo",
            ["OpenAI:Chat:Options:Endpoint"] = "https://custom.openai.com",
            ["OpenAI:Chat:Options:OrganizationId"] = "org-12345"
        });

        var result = _mockBuilder.Object.AddChatClient("OpenAI:Chat");

        Assert.That(result, Is.Not.Null);
        Assert.That(_services.Count, Is.GreaterThan(0));
    }

    [Test]
    public void AddEmbeddingClient_WithNestedOptions_BindsConfigurationCorrectly()
    {
        SetupConfiguration(new Dictionary<string, string>
        {
            ["OpenAI:Embedding:ApiKey"] = "sk-embed-key",
            ["OpenAI:Embedding:Model"] = "text-embedding-3-large",
            ["OpenAI:Embedding:Options:Endpoint"] = "https://api.openai.com/v1",
            ["OpenAI:Embedding:Options:OrganizationId"] = "org-embed"
        });

        var result = _mockBuilder.Object.AddEmbeddingClient("OpenAI:Embedding");

        Assert.That(result, Is.Not.Null);
    }

    [Test]
    public void AddAudioClient_WithModelConfiguration_BindsCorrectly()
    {
        SetupConfiguration(new Dictionary<string, string>
        {
            ["OpenAI:Audio:ApiKey"] = "sk-audio-key",
            ["OpenAI:Audio:Model"] = "whisper-1",
            ["OpenAI:Audio:Options:Endpoint"] = "https://api.openai.com"
        });

        var result = _mockBuilder.Object.AddAudioClient("OpenAI:Audio");

        Assert.That(result, Is.Not.Null);
    }

    [Test]
    public void AddImageClient_WithModelConfiguration_BindsCorrectly()
    {
        SetupConfiguration(new Dictionary<string, string>
        {
            ["OpenAI:Image:ApiKey"] = "sk-image-key",
            ["OpenAI:Image:Model"] = "dall-e-3",
            ["OpenAI:Image:Options:OrganizationId"] = "org-images"
        });

        var result = _mockBuilder.Object.AddImageClient("OpenAI:Image");

        Assert.That(result, Is.Not.Null);
    }

    [Test]
    public void AddRealtimeClient_WithConfiguration_BindsCorrectly()
    {
        SetupConfiguration(new Dictionary<string, string>
        {
            ["OpenAI:Realtime:ApiKey"] = "sk-realtime-key",
            ["OpenAI:Realtime:Model"] = "gpt-4o-realtime"
        });

        var result = _mockBuilder.Object.AddRealtimeClient("OpenAI:Realtime");

        Assert.That(result, Is.Not.Null);
    }

    #endregion

    #region Multiple Keyed Registration Tests

    [Test]
    public void AddKeyedChatClient_MultipleKeys_RegistersDistinctClients()
    {
        SetupConfiguration(new Dictionary<string, string>
        {
            ["OpenAI:Chat1:ApiKey"] = "sk-key1",
            ["OpenAI:Chat1:Model"] = "gpt-4",
            ["OpenAI:Chat2:ApiKey"] = "sk-key2",
            ["OpenAI:Chat2:Model"] = "gpt-3.5-turbo"
        });

        var result1 = _mockBuilder.Object.AddKeyedChatClient("primary", "OpenAI:Chat1");
        var result2 = _mockBuilder.Object.AddKeyedChatClient("secondary", "OpenAI:Chat2");

        Assert.That(result1, Is.Not.Null);
        Assert.That(result2, Is.Not.Null);
        Assert.That(_services.Count, Is.GreaterThanOrEqualTo(2));
    }

    [Test]
    public void AddKeyedEmbeddingClient_MultipleKeys_RegistersDistinctClients()
    {
        SetupConfiguration(new Dictionary<string, string>
        {
            ["OpenAI:Embed1:ApiKey"] = "sk-embed1",
            ["OpenAI:Embed1:Model"] = "text-embedding-ada-002",
            ["OpenAI:Embed2:ApiKey"] = "sk-embed2",
            ["OpenAI:Embed2:Model"] = "text-embedding-3-small"
        });

        var result1 = _mockBuilder.Object.AddKeyedEmbeddingClient("legacy", "OpenAI:Embed1");
        var result2 = _mockBuilder.Object.AddKeyedEmbeddingClient("modern", "OpenAI:Embed2");

        Assert.That(result1, Is.Not.Null);
        Assert.That(result2, Is.Not.Null);
    }

    [Test]
    public void AddKeyedImageClient_WithDifferentModels_RegistersCorrectly()
    {
        SetupConfiguration(new Dictionary<string, string>
        {
            ["OpenAI:Dalle2:ApiKey"] = "sk-dalle2",
            ["OpenAI:Dalle2:Model"] = "dall-e-2",
            ["OpenAI:Dalle3:ApiKey"] = "sk-dalle3",
            ["OpenAI:Dalle3:Model"] = "dall-e-3"
        });

        var dalle2 = _mockBuilder.Object.AddKeyedImageClient("dalle2", "OpenAI:Dalle2");
        var dalle3 = _mockBuilder.Object.AddKeyedImageClient("dalle3", "OpenAI:Dalle3");

        Assert.That(dalle2, Is.Not.Null);
        Assert.That(dalle3, Is.Not.Null);
    }

    #endregion

    #region Empty and Missing Configuration Tests

    [Test]
    public void AddChatClient_WithEmptySection_ReturnsClientBuilder()
    {
        SetupConfiguration(new Dictionary<string, string>());

        var result = _mockBuilder.Object.AddChatClient("OpenAI:Chat");

        Assert.That(result, Is.Not.Null);
    }

    [Test]
    public void AddChatClient_WithMissingApiKey_ReturnsClientBuilder()
    {
        SetupConfiguration(new Dictionary<string, string>
        {
            ["OpenAI:Chat:Model"] = "gpt-4"
        });

        var result = _mockBuilder.Object.AddChatClient("OpenAI:Chat");

        Assert.That(result, Is.Not.Null);
    }

    [Test]
    public void AddKeyedChatClient_WithEmptyServiceKey_ReturnsClientBuilder()
    {
        SetupConfiguration(new Dictionary<string, string>
        {
            ["OpenAI:Chat:ApiKey"] = "test-key"
        });

        var result = _mockBuilder.Object.AddKeyedChatClient(string.Empty, "OpenAI:Chat");

        Assert.That(result, Is.Not.Null);
    }

    #endregion

    #region All Client Types Registration Tests

    [Test]
    public void AddBatchClient_RegistersSuccessfully()
    {
        SetupConfiguration(new Dictionary<string, string>
        {
            ["OpenAI:Batch:ApiKey"] = "test-key"
        });

        var result = _mockBuilder.Object.AddBatchClient("OpenAI:Batch");

        Assert.That(result, Is.Not.Null);
    }

    [Test]
    public void AddKeyedBatchClient_RegistersSuccessfully()
    {
        SetupConfiguration(new Dictionary<string, string>
        {
            ["OpenAI:Batch:ApiKey"] = "test-key"
        });

        var result = _mockBuilder.Object.AddKeyedBatchClient("batchKey", "OpenAI:Batch");

        Assert.That(result, Is.Not.Null);
    }

    [Test]
    public void AddOpenAIFileClient_RegistersSuccessfully()
    {
        SetupConfiguration(new Dictionary<string, string>
        {
            ["OpenAI:File:ApiKey"] = "test-key"
        });

        var result = _mockBuilder.Object.AddOpenAIFileClient("OpenAI:File");

        Assert.That(result, Is.Not.Null);
    }

    [Test]
    public void AddKeyedOpenAIFileClient_RegistersSuccessfully()
    {
        SetupConfiguration(new Dictionary<string, string>
        {
            ["OpenAI:File:ApiKey"] = "test-key"
        });

        var result = _mockBuilder.Object.AddKeyedOpenAIFileClient("fileKey", "OpenAI:File");

        Assert.That(result, Is.Not.Null);
    }

    [Test]
    public void AddFineTuningClient_RegistersSuccessfully()
    {
        SetupConfiguration(new Dictionary<string, string>
        {
            ["OpenAI:FineTuning:ApiKey"] = "test-key"
        });

        var result = _mockBuilder.Object.AddFineTuningClient("OpenAI:FineTuning");

        Assert.That(result, Is.Not.Null);
    }

    [Test]
    public void AddKeyedFineTuningClient_RegistersSuccessfully()
    {
        SetupConfiguration(new Dictionary<string, string>
        {
            ["OpenAI:FineTuning:ApiKey"] = "test-key"
        });

        var result = _mockBuilder.Object.AddKeyedFineTuningClient("ftKey", "OpenAI:FineTuning");

        Assert.That(result, Is.Not.Null);
    }

    [Test]
    public void AddOpenAIModelClient_RegistersSuccessfully()
    {
        SetupConfiguration(new Dictionary<string, string>
        {
            ["OpenAI:Model:ApiKey"] = "test-key"
        });

        var result = _mockBuilder.Object.AddOpenAIModelClient("OpenAI:Model");

        Assert.That(result, Is.Not.Null);
    }

    [Test]
    public void AddKeyedOpenAIModelClient_RegistersSuccessfully()
    {
        SetupConfiguration(new Dictionary<string, string>
        {
            ["OpenAI:Model:ApiKey"] = "test-key"
        });

        var result = _mockBuilder.Object.AddKeyedOpenAIModelClient("modelKey", "OpenAI:Model");

        Assert.That(result, Is.Not.Null);
    }

    [Test]
    public void AddKeyedModerationClient_RegistersSuccessfully()
    {
        SetupConfiguration(new Dictionary<string, string>
        {
            ["OpenAI:Moderation:ApiKey"] = "test-key"
        });

        var result = _mockBuilder.Object.AddKeyedModerationClient("modKey", "OpenAI:Moderation");

        Assert.That(result, Is.Not.Null);
    }

    [Test]
    public void AddKeyedRealtimeClient_RegistersSuccessfully()
    {
        SetupConfiguration(new Dictionary<string, string>
        {
            ["OpenAI:Realtime:ApiKey"] = "test-key"
        });

        var result = _mockBuilder.Object.AddKeyedRealtimeClient("rtKey", "OpenAI:Realtime");

        Assert.That(result, Is.Not.Null);
    }

    [Test]
    public void AddKeyedResponsesClient_RegistersSuccessfully()
    {
        SetupConfiguration(new Dictionary<string, string>
        {
            ["OpenAI:Responses:ApiKey"] = "test-key"
        });

        var result = _mockBuilder.Object.AddKeyedResponsesClient("respKey", "OpenAI:Responses");

        Assert.That(result, Is.Not.Null);
    }

    [Test]
    public void AddKeyedVectorStoreClient_RegistersSuccessfully()
    {
        SetupConfiguration(new Dictionary<string, string>
        {
            ["OpenAI:VectorStore:ApiKey"] = "test-key"
        });

        var result = _mockBuilder.Object.AddKeyedVectorStoreClient("vsKey", "OpenAI:VectorStore");

        Assert.That(result, Is.Not.Null);
    }

    [Test]
    public void AddVideoClient_RegistersSuccessfully()
    {
        SetupConfiguration(new Dictionary<string, string>
        {
            ["OpenAI:Video:ApiKey"] = "test-key"
        });

        var result = _mockBuilder.Object.AddVideoClient("OpenAI:Video");

        Assert.That(result, Is.Not.Null);
    }

    [Test]
    public void AddKeyedVideoClient_RegistersSuccessfully()
    {
        SetupConfiguration(new Dictionary<string, string>
        {
            ["OpenAI:Video:ApiKey"] = "test-key"
        });

        var result = _mockBuilder.Object.AddKeyedVideoClient("videoKey", "OpenAI:Video");

        Assert.That(result, Is.Not.Null);
    }

    [Test]
    public void AddContainerClient_RegistersSuccessfully()
    {
        SetupConfiguration(new Dictionary<string, string>
        {
            ["OpenAI:Container:ApiKey"] = "test-key"
        });

        var result = _mockBuilder.Object.AddContainerClient("OpenAI:Container");

        Assert.That(result, Is.Not.Null);
    }

    [Test]
    public void AddKeyedContainerClient_RegistersSuccessfully()
    {
        SetupConfiguration(new Dictionary<string, string>
        {
            ["OpenAI:Container:ApiKey"] = "test-key"
        });

        var result = _mockBuilder.Object.AddKeyedContainerClient("containerKey", "OpenAI:Container");

        Assert.That(result, Is.Not.Null);
    }

    [Test]
    public void AddConversationClient_RegistersSuccessfully()
    {
        SetupConfiguration(new Dictionary<string, string>
        {
            ["OpenAI:Conversation:ApiKey"] = "test-key"
        });

        var result = _mockBuilder.Object.AddConversationClient("OpenAI:Conversation");

        Assert.That(result, Is.Not.Null);
    }

    [Test]
    public void AddKeyedConversationClient_RegistersSuccessfully()
    {
        SetupConfiguration(new Dictionary<string, string>
        {
            ["OpenAI:Conversation:ApiKey"] = "test-key"
        });

        var result = _mockBuilder.Object.AddKeyedConversationClient("convKey", "OpenAI:Conversation");

        Assert.That(result, Is.Not.Null);
    }

    [Test]
    public void AddEvaluationClient_RegistersSuccessfully()
    {
        SetupConfiguration(new Dictionary<string, string>
        {
            ["OpenAI:Evaluation:ApiKey"] = "test-key"
        });

        var result = _mockBuilder.Object.AddEvaluationClient("OpenAI:Evaluation");

        Assert.That(result, Is.Not.Null);
    }

    [Test]
    public void AddKeyedEvaluationClient_RegistersSuccessfully()
    {
        SetupConfiguration(new Dictionary<string, string>
        {
            ["OpenAI:Evaluation:ApiKey"] = "test-key"
        });

        var result = _mockBuilder.Object.AddKeyedEvaluationClient("evalKey", "OpenAI:Evaluation");

        Assert.That(result, Is.Not.Null);
    }

    [Test]
    public void AddGraderClient_RegistersSuccessfully()
    {
        SetupConfiguration(new Dictionary<string, string>
        {
            ["OpenAI:Grader:ApiKey"] = "test-key"
        });

        var result = _mockBuilder.Object.AddGraderClient("OpenAI:Grader");

        Assert.That(result, Is.Not.Null);
    }

    [Test]
    public void AddKeyedGraderClient_RegistersSuccessfully()
    {
        SetupConfiguration(new Dictionary<string, string>
        {
            ["OpenAI:Grader:ApiKey"] = "test-key"
        });

        var result = _mockBuilder.Object.AddKeyedGraderClient("graderKey", "OpenAI:Grader");

        Assert.That(result, Is.Not.Null);
    }

    [Test]
    public void AddKeyedAudioClient_RegistersSuccessfully()
    {
        SetupConfiguration(new Dictionary<string, string>
        {
            ["OpenAI:Audio:ApiKey"] = "test-key",
            ["OpenAI:Audio:Model"] = "whisper-1"
        });

        var result = _mockBuilder.Object.AddKeyedAudioClient("audioKey", "OpenAI:Audio");

        Assert.That(result, Is.Not.Null);
    }

    [Test]
    public void AddKeyedEmbeddingClient_RegistersSuccessfully()
    {
        SetupConfiguration(new Dictionary<string, string>
        {
            ["OpenAI:Embedding:ApiKey"] = "test-key",
            ["OpenAI:Embedding:Model"] = "text-embedding-ada-002"
        });

        var result = _mockBuilder.Object.AddKeyedEmbeddingClient("embedKey", "OpenAI:Embedding");

        Assert.That(result, Is.Not.Null);
    }

    [Test]
    public void AddKeyedImageClient_RegistersSuccessfully()
    {
        SetupConfiguration(new Dictionary<string, string>
        {
            ["OpenAI:Image:ApiKey"] = "test-key",
            ["OpenAI:Image:Model"] = "dall-e-3"
        });

        var result = _mockBuilder.Object.AddKeyedImageClient("imageKey", "OpenAI:Image");

        Assert.That(result, Is.Not.Null);
    }

    #endregion

    #region Service Registration Verification Tests

    [Test]
    public void AddChatClient_AddsServiceToCollection()
    {
        SetupConfiguration(new Dictionary<string, string>
        {
            ["OpenAI:Chat:ApiKey"] = "test-key",
            ["OpenAI:Chat:Model"] = "gpt-4"
        });

        _mockBuilder.Object.AddChatClient("OpenAI:Chat");

        Assert.That(_services.Count, Is.GreaterThan(0));
    }

    [Test]
    public void AddKeyedChatClient_AddsKeyedServiceToCollection()
    {
        SetupConfiguration(new Dictionary<string, string>
        {
            ["OpenAI:Chat:ApiKey"] = "test-key",
            ["OpenAI:Chat:Model"] = "gpt-4"
        });

        _mockBuilder.Object.AddKeyedChatClient("testKey", "OpenAI:Chat");

        Assert.That(_services.Count, Is.GreaterThan(0));
    }

    [Test]
    public void AddMultipleClients_RegistersAllServices()
    {
        SetupConfiguration(new Dictionary<string, string>
        {
            ["OpenAI:Chat:ApiKey"] = "test-key",
            ["OpenAI:Chat:Model"] = "gpt-4",
            ["OpenAI:Embedding:ApiKey"] = "test-key",
            ["OpenAI:Embedding:Model"] = "text-embedding-ada-002",
            ["OpenAI:Image:ApiKey"] = "test-key",
            ["OpenAI:Image:Model"] = "dall-e-3"
        });

        _mockBuilder.Object.AddChatClient("OpenAI:Chat");
        _mockBuilder.Object.AddEmbeddingClient("OpenAI:Embedding");
        _mockBuilder.Object.AddImageClient("OpenAI:Image");

        Assert.That(_services.Count, Is.GreaterThanOrEqualTo(3));
    }

    #endregion

    #region Section Name Variations Tests

    [Test]
    public void AddChatClient_WithDifferentSectionNames_ReturnsClientBuilder()
    {
        SetupConfiguration(new Dictionary<string, string>
        {
            ["MyApp:Services:OpenAI:ChatClient:ApiKey"] = "test-key",
            ["MyApp:Services:OpenAI:ChatClient:Model"] = "gpt-4"
        });

        var result = _mockBuilder.Object.AddChatClient("MyApp:Services:OpenAI:ChatClient");

        Assert.That(result, Is.Not.Null);
    }

    [Test]
    public void AddChatClient_WithSimpleSectionName_ReturnsClientBuilder()
    {
        SetupConfiguration(new Dictionary<string, string>
        {
            ["Chat:ApiKey"] = "test-key",
            ["Chat:Model"] = "gpt-4"
        });

        var result = _mockBuilder.Object.AddChatClient("Chat");

        Assert.That(result, Is.Not.Null);
    }

    [Test]
    public void AddKeyedChatClient_WithComplexSectionName_ReturnsClientBuilder()
    {
        SetupConfiguration(new Dictionary<string, string>
        {
            ["Production:AI:OpenAI:ChatGPT:ApiKey"] = "prod-key",
            ["Production:AI:OpenAI:ChatGPT:Model"] = "gpt-4-turbo"
        });

        var result = _mockBuilder.Object.AddKeyedChatClient("production-chat", "Production:AI:OpenAI:ChatGPT");

        Assert.That(result, Is.Not.Null);
    }

    #endregion
}