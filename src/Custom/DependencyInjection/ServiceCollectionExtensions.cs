using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using OpenAI.Audio;
using OpenAI.Chat;
using OpenAI.Embeddings;
using OpenAI.Images;
using OpenAI.Moderations;
using System;
using System.ClientModel;

namespace OpenAI.Extensions.DependencyInjection;

/// <summary>
/// Extension methods for configuring OpenAI services in dependency injection.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds OpenAI services to the service collection with configuration from IConfiguration.
    /// </summary>
    /// <param name="services">The service collection to add services to.</param>
    /// <param name="configuration">The configuration section containing OpenAI settings.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddOpenAI(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        return services.AddOpenAI(options =>
        {
            var endpoint = configuration["Endpoint"];
            if (!string.IsNullOrEmpty(endpoint) && Uri.TryCreate(endpoint, UriKind.Absolute, out var uri))
            {
                options.Endpoint = uri;
            }

            var organizationId = configuration["OrganizationId"];
            if (!string.IsNullOrEmpty(organizationId))
            {
                options.OrganizationId = organizationId;
            }

            var projectId = configuration["ProjectId"];
            if (!string.IsNullOrEmpty(projectId))
            {
                options.ProjectId = projectId;
            }

            var userAgentApplicationId = configuration["UserAgentApplicationId"];
            if (!string.IsNullOrEmpty(userAgentApplicationId))
            {
                options.UserAgentApplicationId = userAgentApplicationId;
            }
        });
    }

    /// <summary>
    /// Adds OpenAI services to the service collection with configuration action.
    /// </summary>
    /// <param name="services">The service collection to add services to.</param>
    /// <param name="configureOptions">Action to configure OpenAI client options.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddOpenAI(
        this IServiceCollection services,
        Action<OpenAIClientOptions> configureOptions)
    {
        if (services == null) throw new ArgumentNullException(nameof(services));
        if (configureOptions == null) throw new ArgumentNullException(nameof(configureOptions));

        services.Configure(configureOptions);
        services.AddSingleton(serviceProvider =>
        {
            var options = serviceProvider.GetRequiredService<IOptions<OpenAIClientOptions>>().Value;
            var apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");

            if (string.IsNullOrEmpty(apiKey))
            {
                throw new InvalidOperationException(
                    "OpenAI API key not found. Set the OPENAI_API_KEY environment variable or configure the ApiKey in OpenAIClientOptions.");
            }

            return new OpenAIClient(new ApiKeyCredential(apiKey), options);
        });

        return services;
    }

    /// <summary>
    /// Adds OpenAI services to the service collection with an API key.
    /// </summary>
    /// <param name="services">The service collection to add services to.</param>
    /// <param name="apiKey">The OpenAI API key.</param>
    /// <param name="configureOptions">Optional action to configure additional client options.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddOpenAI(
        this IServiceCollection services,
        string apiKey,
        Action<OpenAIClientOptions> configureOptions = null)
    {
        if (services == null) throw new ArgumentNullException(nameof(services));
        if (string.IsNullOrEmpty(apiKey)) throw new ArgumentNullException(nameof(apiKey));

        if (configureOptions != null)
        {
            services.Configure(configureOptions);
        }

        services.AddSingleton<OpenAIClient>(serviceProvider =>
        {
            var options = configureOptions != null
                ? serviceProvider.GetRequiredService<IOptions<OpenAIClientOptions>>().Value
                : new OpenAIClientOptions();

            return new OpenAIClient(new ApiKeyCredential(apiKey), options);
        });

        return services;
    }

    /// <summary>
    /// Adds OpenAI services to the service collection with a credential.
    /// </summary>
    /// <param name="services">The service collection to add services to.</param>
    /// <param name="credential">The API key credential.</param>
    /// <param name="configureOptions">Optional action to configure additional client options.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddOpenAI(
        this IServiceCollection services,
        ApiKeyCredential credential,
        Action<OpenAIClientOptions> configureOptions = null)
    {
        if (services == null) throw new ArgumentNullException(nameof(services));
        if (credential == null) throw new ArgumentNullException(nameof(credential));

        if (configureOptions != null)
        {
            services.Configure(configureOptions);
        }

        services.AddSingleton<OpenAIClient>(serviceProvider =>
        {
            var options = configureOptions != null
                ? serviceProvider.GetRequiredService<IOptions<OpenAIClientOptions>>().Value
                : new OpenAIClientOptions();

            return new OpenAIClient(credential, options);
        });

        return services;
    }

    /// <summary>
    /// Adds a ChatClient to the service collection for a specific model.
    /// </summary>
    /// <param name="services">The service collection to add services to.</param>
    /// <param name="model">The chat model to use (e.g., "gpt-4o", "gpt-3.5-turbo").</param>
    /// <param name="apiKey">The OpenAI API key. If null, uses environment variable OPENAI_API_KEY.</param>
    /// <param name="configureOptions">Optional action to configure client options.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddOpenAIChat(
        this IServiceCollection services,
        string model,
        string apiKey = null,
        Action<OpenAIClientOptions> configureOptions = null)
    {
        if (services == null) throw new ArgumentNullException(nameof(services));
        if (string.IsNullOrEmpty(model)) throw new ArgumentNullException(nameof(model));

        if (configureOptions != null)
        {
            services.Configure(configureOptions);
        }

        services.AddSingleton<ChatClient>(serviceProvider =>
        {
            var resolvedApiKey = apiKey ?? Environment.GetEnvironmentVariable("OPENAI_API_KEY");

            if (string.IsNullOrEmpty(resolvedApiKey))
            {
                throw new InvalidOperationException(
                    "OpenAI API key not found. Provide an API key parameter or set the OPENAI_API_KEY environment variable.");
            }

            var options = configureOptions != null
                ? serviceProvider.GetRequiredService<IOptions<OpenAIClientOptions>>().Value
                : new OpenAIClientOptions();

            return new ChatClient(model, new ApiKeyCredential(resolvedApiKey), options);
        });

        return services;
    }

    /// <summary>
    /// Adds a ChatClient to the service collection using an existing OpenAIClient.
    /// </summary>
    /// <param name="services">The service collection to add services to.</param>
    /// <param name="model">The chat model to use (e.g., "gpt-4o", "gpt-3.5-turbo").</param>
    /// <returns>The service collection for chaining.</returns>
    /// <remarks>This method requires that an OpenAIClient has already been registered.</remarks>
    public static IServiceCollection AddOpenAIChat(
        this IServiceCollection services,
        string model)
    {
        if (services == null) throw new ArgumentNullException(nameof(services));
        if (string.IsNullOrEmpty(model)) throw new ArgumentNullException(nameof(model));

        services.AddSingleton<ChatClient>(serviceProvider =>
        {
            var openAIClient = serviceProvider.GetRequiredService<OpenAIClient>();
            return openAIClient.GetChatClient(model);
        });

        return services;
    }

    /// <summary>
    /// Adds an EmbeddingClient to the service collection for a specific model.
    /// </summary>
    /// <param name="services">The service collection to add services to.</param>
    /// <param name="model">The embedding model to use (e.g., "text-embedding-3-small", "text-embedding-3-large").</param>
    /// <param name="apiKey">The OpenAI API key. If null, uses environment variable OPENAI_API_KEY.</param>
    /// <param name="configureOptions">Optional action to configure client options.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddOpenAIEmbeddings(
        this IServiceCollection services,
        string model,
        string apiKey = null,
        Action<OpenAIClientOptions> configureOptions = null)
    {
        if (services == null) throw new ArgumentNullException(nameof(services));
        if (string.IsNullOrEmpty(model)) throw new ArgumentNullException(nameof(model));

        if (configureOptions != null)
        {
            services.Configure(configureOptions);
        }

        services.AddSingleton<EmbeddingClient>(serviceProvider =>
        {
            var resolvedApiKey = apiKey ?? Environment.GetEnvironmentVariable("OPENAI_API_KEY");

            if (string.IsNullOrEmpty(resolvedApiKey))
            {
                throw new InvalidOperationException(
                    "OpenAI API key not found. Provide an API key parameter or set the OPENAI_API_KEY environment variable.");
            }

            var options = configureOptions != null
                ? serviceProvider.GetRequiredService<IOptions<OpenAIClientOptions>>().Value
                : new OpenAIClientOptions();

            return new EmbeddingClient(model, new ApiKeyCredential(resolvedApiKey), options);
        });

        return services;
    }

    /// <summary>
    /// Adds an EmbeddingClient to the service collection using an existing OpenAIClient.
    /// </summary>
    /// <param name="services">The service collection to add services to.</param>
    /// <param name="model">The embedding model to use (e.g., "text-embedding-3-small", "text-embedding-3-large").</param>
    /// <returns>The service collection for chaining.</returns>
    /// <remarks>This method requires that an OpenAIClient has already been registered.</remarks>
    public static IServiceCollection AddOpenAIEmbeddings(
        this IServiceCollection services,
        string model)
    {
        if (services == null) throw new ArgumentNullException(nameof(services));
        if (string.IsNullOrEmpty(model)) throw new ArgumentNullException(nameof(model));

        services.AddSingleton<EmbeddingClient>(serviceProvider =>
        {
            var openAIClient = serviceProvider.GetRequiredService<OpenAIClient>();
            return openAIClient.GetEmbeddingClient(model);
        });

        return services;
    }

    /// <summary>
    /// Adds an AudioClient to the service collection for a specific model.
    /// </summary>
    /// <param name="services">The service collection to add services to.</param>
    /// <param name="model">The audio model to use (e.g., "whisper-1", "tts-1").</param>
    /// <param name="apiKey">The OpenAI API key. If null, uses environment variable OPENAI_API_KEY.</param>
    /// <param name="configureOptions">Optional action to configure client options.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddOpenAIAudio(
        this IServiceCollection services,
        string model,
        string apiKey = null,
        Action<OpenAIClientOptions> configureOptions = null)
    {
        if (services == null) throw new ArgumentNullException(nameof(services));
        if (string.IsNullOrEmpty(model)) throw new ArgumentNullException(nameof(model));

        if (configureOptions != null)
        {
            services.Configure(configureOptions);
        }

        services.AddSingleton<AudioClient>(serviceProvider =>
        {
            var resolvedApiKey = apiKey ?? Environment.GetEnvironmentVariable("OPENAI_API_KEY");

            if (string.IsNullOrEmpty(resolvedApiKey))
            {
                throw new InvalidOperationException(
                    "OpenAI API key not found. Provide an API key parameter or set the OPENAI_API_KEY environment variable.");
            }

            var options = configureOptions != null
                ? serviceProvider.GetRequiredService<IOptions<OpenAIClientOptions>>().Value
                : new OpenAIClientOptions();

            return new AudioClient(model, new ApiKeyCredential(resolvedApiKey), options);
        });

        return services;
    }

    /// <summary>
    /// Adds an AudioClient to the service collection using an existing OpenAIClient.
    /// </summary>
    /// <param name="services">The service collection to add services to.</param>
    /// <param name="model">The audio model to use (e.g., "whisper-1", "tts-1").</param>
    /// <returns>The service collection for chaining.</returns>
    /// <remarks>This method requires that an OpenAIClient has already been registered.</remarks>
    public static IServiceCollection AddOpenAIAudio(
        this IServiceCollection services,
        string model)
    {
        if (services == null) throw new ArgumentNullException(nameof(services));
        if (string.IsNullOrEmpty(model)) throw new ArgumentNullException(nameof(model));

        services.AddSingleton<AudioClient>(serviceProvider =>
        {
            var openAIClient = serviceProvider.GetRequiredService<OpenAIClient>();
            return openAIClient.GetAudioClient(model);
        });

        return services;
    }

    /// <summary>
    /// Adds an ImageClient to the service collection for a specific model.
    /// </summary>
    /// <param name="services">The service collection to add services to.</param>
    /// <param name="model">The image model to use (e.g., "dall-e-3", "dall-e-2").</param>
    /// <param name="apiKey">The OpenAI API key. If null, uses environment variable OPENAI_API_KEY.</param>
    /// <param name="configureOptions">Optional action to configure client options.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddOpenAIImages(
        this IServiceCollection services,
        string model,
        string apiKey = null,
        Action<OpenAIClientOptions> configureOptions = null)
    {
        if (services == null) throw new ArgumentNullException(nameof(services));
        if (string.IsNullOrEmpty(model)) throw new ArgumentNullException(nameof(model));

        if (configureOptions != null)
        {
            services.Configure(configureOptions);
        }

        services.AddSingleton<ImageClient>(serviceProvider =>
        {
            var resolvedApiKey = apiKey ?? Environment.GetEnvironmentVariable("OPENAI_API_KEY");

            if (string.IsNullOrEmpty(resolvedApiKey))
            {
                throw new InvalidOperationException(
                    "OpenAI API key not found. Provide an API key parameter or set the OPENAI_API_KEY environment variable.");
            }

            var options = configureOptions != null
                ? serviceProvider.GetRequiredService<IOptions<OpenAIClientOptions>>().Value
                : new OpenAIClientOptions();

            return new ImageClient(model, new ApiKeyCredential(resolvedApiKey), options);
        });

        return services;
    }

    /// <summary>
    /// Adds an ImageClient to the service collection using an existing OpenAIClient.
    /// </summary>
    /// <param name="services">The service collection to add services to.</param>
    /// <param name="model">The image model to use (e.g., "dall-e-3", "dall-e-2").</param>
    /// <returns>The service collection for chaining.</returns>
    /// <remarks>This method requires that an OpenAIClient has already been registered.</remarks>
    public static IServiceCollection AddOpenAIImages(
        this IServiceCollection services,
        string model)
    {
        if (services == null) throw new ArgumentNullException(nameof(services));
        if (string.IsNullOrEmpty(model)) throw new ArgumentNullException(nameof(model));

        services.AddSingleton<ImageClient>(serviceProvider =>
        {
            var openAIClient = serviceProvider.GetRequiredService<OpenAIClient>();
            return openAIClient.GetImageClient(model);
        });

        return services;
    }

    /// <summary>
    /// Adds a ModerationClient to the service collection for a specific model.
    /// </summary>
    /// <param name="services">The service collection to add services to.</param>
    /// <param name="model">The moderation model to use (e.g., "text-moderation-latest", "text-moderation-stable").</param>
    /// <param name="apiKey">The OpenAI API key. If null, uses environment variable OPENAI_API_KEY.</param>
    /// <param name="configureOptions">Optional action to configure client options.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddOpenAIModeration(
        this IServiceCollection services,
        string model,
        string apiKey = null,
        Action<OpenAIClientOptions> configureOptions = null)
    {
        if (services == null) throw new ArgumentNullException(nameof(services));
        if (string.IsNullOrEmpty(model)) throw new ArgumentNullException(nameof(model));

        if (configureOptions != null)
        {
            services.Configure(configureOptions);
        }

        services.AddSingleton<ModerationClient>(serviceProvider =>
        {
            var resolvedApiKey = apiKey ?? Environment.GetEnvironmentVariable("OPENAI_API_KEY");

            if (string.IsNullOrEmpty(resolvedApiKey))
            {
                throw new InvalidOperationException(
                    "OpenAI API key not found. Provide an API key parameter or set the OPENAI_API_KEY environment variable.");
            }

            var options = configureOptions != null
                ? serviceProvider.GetRequiredService<IOptions<OpenAIClientOptions>>().Value
                : new OpenAIClientOptions();

            return new ModerationClient(model, new ApiKeyCredential(resolvedApiKey), options);
        });

        return services;
    }

    /// <summary>
    /// Adds a ModerationClient to the service collection using an existing OpenAIClient.
    /// </summary>
    /// <param name="services">The service collection to add services to.</param>
    /// <param name="model">The moderation model to use (e.g., "text-moderation-latest", "text-moderation-stable").</param>
    /// <returns>The service collection for chaining.</returns>
    /// <remarks>This method requires that an OpenAIClient has already been registered.</remarks>
    public static IServiceCollection AddOpenAIModeration(
        this IServiceCollection services,
        string model)
    {
        if (services == null) throw new ArgumentNullException(nameof(services));
        if (string.IsNullOrEmpty(model)) throw new ArgumentNullException(nameof(model));

        services.AddSingleton<ModerationClient>(serviceProvider =>
        {
            var openAIClient = serviceProvider.GetRequiredService<OpenAIClient>();
            return openAIClient.GetModerationClient(model);
        });

        return services;
    }
}