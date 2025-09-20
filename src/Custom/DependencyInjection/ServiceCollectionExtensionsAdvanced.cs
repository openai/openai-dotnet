using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.ClientModel;

namespace OpenAI.Extensions.DependencyInjection;

/// <summary>
/// Additional extension methods for configuring OpenAI services with enhanced configuration support.
/// </summary>
public static class ServiceCollectionExtensionsAdvanced
{
    /// <summary>
    /// Adds OpenAI services to the service collection with configuration from a named section.
    /// Eliminates reflection-based binding to avoid IL2026 / IL3050 warnings when trimming.
    /// </summary>
    public static IServiceCollection AddOpenAIFromConfiguration(
        this IServiceCollection services,
        IConfiguration configuration,
        string sectionName = "OpenAI")
    {
        if (services == null) throw new ArgumentNullException(nameof(services));
        if (configuration == null) throw new ArgumentNullException(nameof(configuration));
        if (string.IsNullOrEmpty(sectionName)) throw new ArgumentNullException(nameof(sectionName));

        var optionsInstance = BuildOptions(configuration, sectionName);
        services.AddSingleton(Options.Create(optionsInstance));
        services.AddSingleton(serviceProvider =>
        {
            var options = serviceProvider.GetRequiredService<IOptions<OpenAIServiceOptions>>().Value;
            var apiKey = options.ApiKey ?? Environment.GetEnvironmentVariable("OPENAI_API_KEY");

            if (string.IsNullOrEmpty(apiKey))
            {
                throw new InvalidOperationException(
                    $"OpenAI API key not found. Set the ApiKey in the '{sectionName}' configuration section or set the OPENAI_API_KEY environment variable.");
            }

            return new OpenAIClient(new ApiKeyCredential(apiKey), options);
        });

        return services;
    }

    private static OpenAIServiceOptions BuildOptions(IConfiguration configuration, string sectionName)
    {
        var section = configuration.GetSection(sectionName);
        if (!section.Exists())
        {
            throw new InvalidOperationException($"Configuration section '{sectionName}' was not found.");
        }

        var options = new OpenAIServiceOptions
        {
            ApiKey = section["ApiKey"],
            DefaultChatModel = section["DefaultChatModel"],
            DefaultEmbeddingModel = section["DefaultEmbeddingModel"],
            DefaultAudioModel = section["DefaultAudioModel"],
            DefaultImageModel = section["DefaultImageModel"],
            DefaultModerationModel = section["DefaultModerationModel"]
        };

        return options;
    }

    /// <summary>
    /// Adds a ChatClient using configuration from the registered OpenAIServiceOptions.
    /// </summary>
    public static IServiceCollection AddChatClientFromConfiguration(
        this IServiceCollection services,
        string model = null)
    {
        if (services == null) throw new ArgumentNullException(nameof(services));

        services.AddSingleton(serviceProvider =>
        {
            var openAIClient = serviceProvider.GetRequiredService<OpenAIClient>();
            var options = serviceProvider.GetRequiredService<IOptions<OpenAIServiceOptions>>().Value;
            var chatModel = model ?? options.DefaultChatModel;

            if (string.IsNullOrEmpty(chatModel))
            {
                throw new InvalidOperationException(
                    "Chat model not specified. Provide a model parameter or set DefaultChatModel in configuration.");
            }

            return openAIClient.GetChatClient(chatModel);
        });

        return services;
    }

    /// <summary>
    /// Adds an EmbeddingClient using configuration from the registered OpenAIServiceOptions.
    /// </summary>
    public static IServiceCollection AddEmbeddingClientFromConfiguration(
        this IServiceCollection services,
        string model = null)
    {
        if (services == null) throw new ArgumentNullException(nameof(services));

        services.AddSingleton(serviceProvider =>
        {
            var openAIClient = serviceProvider.GetRequiredService<OpenAIClient>();
            var options = serviceProvider.GetRequiredService<IOptions<OpenAIServiceOptions>>().Value;
            var embeddingModel = model ?? options.DefaultEmbeddingModel;

            if (string.IsNullOrEmpty(embeddingModel))
            {
                throw new InvalidOperationException(
                    "Embedding model not specified. Provide a model parameter or set DefaultEmbeddingModel in configuration.");
            }

            return openAIClient.GetEmbeddingClient(embeddingModel);
        });

        return services;
    }

    /// <summary>
    /// Adds an AudioClient using configuration from the registered OpenAIServiceOptions.
    /// </summary>
    public static IServiceCollection AddAudioClientFromConfiguration(
        this IServiceCollection services,
        string model = null)
    {
        if (services == null) throw new ArgumentNullException(nameof(services));

        services.AddSingleton(serviceProvider =>
        {
            var openAIClient = serviceProvider.GetRequiredService<OpenAIClient>();
            var options = serviceProvider.GetRequiredService<IOptions<OpenAIServiceOptions>>().Value;
            var audioModel = model ?? options.DefaultAudioModel;

            if (string.IsNullOrEmpty(audioModel))
            {
                throw new InvalidOperationException(
                    "Audio model not specified. Provide a model parameter or set DefaultAudioModel in configuration.");
            }

            return openAIClient.GetAudioClient(audioModel);
        });

        return services;
    }

    /// <summary>
    /// Adds an ImageClient using configuration from the registered OpenAIServiceOptions.
    /// </summary>
    public static IServiceCollection AddImageClientFromConfiguration(
        this IServiceCollection services,
        string model = null)
    {
        if (services == null) throw new ArgumentNullException(nameof(services));

        services.AddSingleton(serviceProvider =>
        {
            var openAIClient = serviceProvider.GetRequiredService<OpenAIClient>();
            var options = serviceProvider.GetRequiredService<IOptions<OpenAIServiceOptions>>().Value;
            var imageModel = model ?? options.DefaultImageModel;

            if (string.IsNullOrEmpty(imageModel))
            {
                throw new InvalidOperationException(
                    "Image model not specified. Provide a model parameter or set DefaultImageModel in configuration.");
            }

            return openAIClient.GetImageClient(imageModel);
        });

        return services;
    }

    /// <summary>
    /// Adds a ModerationClient using configuration from the registered OpenAIServiceOptions.
    /// </summary>
    public static IServiceCollection AddModerationClientFromConfiguration(
        this IServiceCollection services,
        string model = null)
    {
        if (services == null) throw new ArgumentNullException(nameof(services));

        services.AddSingleton(serviceProvider =>
        {
            var openAIClient = serviceProvider.GetRequiredService<OpenAIClient>();
            var options = serviceProvider.GetRequiredService<IOptions<OpenAIServiceOptions>>().Value;
            var moderationModel = model ?? options.DefaultModerationModel;

            if (string.IsNullOrEmpty(moderationModel))
            {
                throw new InvalidOperationException(
                    "Moderation model not specified. Provide a model parameter or set DefaultModerationModel in configuration.");
            }

            return openAIClient.GetModerationClient(moderationModel);
        });

        return services;
    }

    /// <summary>
    /// Adds all common OpenAI clients using configuration from the registered OpenAIServiceOptions.
    /// </summary>
    public static IServiceCollection AddAllOpenAIClientsFromConfiguration(
        this IServiceCollection services)
    {
        if (services == null) throw new ArgumentNullException(nameof(services));

        services.AddChatClientFromConfiguration();
        services.AddEmbeddingClientFromConfiguration();
        services.AddAudioClientFromConfiguration();
        services.AddImageClientFromConfiguration();
        services.AddModerationClientFromConfiguration();

        return services;
    }
}