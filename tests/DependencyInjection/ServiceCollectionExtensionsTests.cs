using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Moq;
using NUnit.Framework;
using OpenAI.Custom.DependencyInjection;
using System;
using System.ClientModel.Primitives;

namespace OpenAI.Tests.DependencyInjection;
#pragma warning disable SCME0002
[TestFixture]
public class ServiceCollectionExtensionsTests
{
    private Mock<IHostApplicationBuilder> _mockBuilder;
    private ServiceCollection _services;
    private ConfigurationManager _configuration;

    [SetUp]
    public void Setup()
    {
        _services = new ServiceCollection();
        _configuration = new ConfigurationManager();
        _mockBuilder = new Mock<IHostApplicationBuilder>();
        _mockBuilder.Setup(b => b.Services).Returns(_services);
        _mockBuilder.Setup(b => b.Configuration).Returns(_configuration);
    }

    [Test]
    public void AddOpenAIClient_WithNullBuilder_ThrowsArgumentNullException()
    {
        // Arrange
        IHostApplicationBuilder builder = null!;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => builder.AddOpenAIClient());
    }

    [Test]
    public void AddOpenAIClient_WithValidBuilder_ReturnsClientBuilder()
    {
        // Arrange
        var builder = _mockBuilder.Object;

        // Act
        var result = builder.AddOpenAIClient();

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.InstanceOf<IClientBuilder>());
    }

    [Test]
    public void AddOpenAIClient_WithCustomSectionName_ReturnsClientBuilder()
    {
        // Arrange
        var builder = _mockBuilder.Object;
        const string customSection = "CustomOpenAI";

        // Act
        var result = builder.AddOpenAIClient(customSection);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.InstanceOf<IClientBuilder>());
    }

    [Test]
    public void AddKeyedOpenAIClient_WithNullBuilder_ThrowsArgumentNullException()
    {
        // Arrange
        IHostApplicationBuilder builder = null!;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => builder.AddKeyedOpenAIClient("key"));
    }

    [Test]
    public void AddKeyedOpenAIClient_WithValidBuilder_ReturnsClientBuilder()
    {
        // Arrange
        var builder = _mockBuilder.Object;
        const string serviceKey = "primary";

        // Act
        var result = builder.AddKeyedOpenAIClient(serviceKey);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.InstanceOf<IClientBuilder>());
    }

    [Test]
    public void AddKeyedOpenAIClient_WithCustomSectionName_ReturnsClientBuilder()
    {
        // Arrange
        var builder = _mockBuilder.Object;
        const string serviceKey = "secondary";
        const string customSection = "CustomOpenAI";

        // Act
        var result = builder.AddKeyedOpenAIClient(serviceKey, customSection);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.InstanceOf<IClientBuilder>());
    }
}
#pragma warning restore SCME0002