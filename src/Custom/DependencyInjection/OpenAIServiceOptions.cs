using Microsoft.Extensions.Configuration;
using System;
using System.ClientModel.Primitives;

namespace OpenAI.Custom.DependencyInjection;

/// <summary>
/// Settings used to configure an <see cref="OpenAIClient"/> that can be loaded from an <see cref="IConfigurationSection"/>.
/// </summary>
/// <remarks>
/// This class follows a configuration pattern inspired by System.ClientModel.
/// Configuration can be loaded from appsettings.json with the following structure:
/// </remarks>
#pragma warning disable SCME0002
public sealed class OpenAIServiceOptions : ClientSettings
#pragma warning restore SCME0002
{
    /// <summary>
    /// Gets or sets the endpoint URI for the OpenAI service.
    /// </summary>
    public Uri Endpoint { get; set; }

    protected override void BindCore(IConfigurationSection section)
    {
        if (section is null)
        {
            throw new ArgumentNullException(nameof(section));
        }

        // Bind Endpoint
        var endpointValue = section["Endpoint"];
        if (!string.IsNullOrEmpty(endpointValue) && Uri.TryCreate(endpointValue, UriKind.Absolute, out var endpoint))
        {
            Endpoint = endpoint;
        }

        // Bind Credential section
        var credSection = section.GetSection("Credential");
        if (credSection.Exists())
        {
            AuthCredential ??= new CredentialSettings();
            AuthCredential.Bind(credSection);
        }

        // Bind nested Options
        var optionsSection = section.GetSection("Options");
        if (optionsSection.Exists())
        {
            Options ??= new OpenAIClientOptions();

            var organizationId = optionsSection["OrganizationId"];
            if (!string.IsNullOrEmpty(organizationId))
            {
                Options.OrganizationId = organizationId;
            }

            var projectId = optionsSection["ProjectId"];
            if (!string.IsNullOrEmpty(projectId))
            {
                Options.ProjectId = projectId;
            }

            var userAgentApplicationId = optionsSection["UserAgentApplicationId"];
            if (!string.IsNullOrEmpty(userAgentApplicationId))
            {
                Options.UserAgentApplicationId = userAgentApplicationId;
            }

            var optionsEndpoint = optionsSection["Endpoint"];
            if (!string.IsNullOrEmpty(optionsEndpoint) && Uri.TryCreate(optionsEndpoint, UriKind.Absolute, out var optionsEndpointUri))
            {
                Options.Endpoint = optionsEndpointUri;
            }
        }

        // Bind default models
        DefaultChatModel = section["DefaultChatModel"];
        DefaultEmbeddingModel = section["DefaultEmbeddingModel"];
        DefaultAudioModel = section["DefaultAudioModel"];
        DefaultImageModel = section["DefaultImageModel"];
        DefaultModerationModel = section["DefaultModerationModel"];
    }

    /// <summary>
    /// Gets or sets the credential settings for authentication.
    /// </summary>
    public CredentialSettings AuthCredential { get; set; }

    /// <summary>
    /// Gets or sets the client options for the OpenAI client.
    /// </summary>
    public OpenAIClientOptions Options { get; set; }

    /// <summary>
    /// The default chat model to use when registering ChatClient without specifying a model.
    /// </summary>
    public string DefaultChatModel { get; set; } = "gpt-4o";

    /// <summary>
    /// The default embedding model to use when registering EmbeddingClient without specifying a model.
    /// </summary>
    public string DefaultEmbeddingModel { get; set; } = "text-embedding-3-small";

    /// <summary>
    /// The default audio model to use when registering AudioClient without specifying a model.
    /// </summary>
    public string DefaultAudioModel { get; set; } = "whisper-1";

    /// <summary>
    /// The default image model to use when registering ImageClient without specifying a model.
    /// </summary>
    public string DefaultImageModel { get; set; } = "dall-e-3";

    /// <summary>
    /// The default moderation model to use when registering ModerationClient without specifying a model.
    /// </summary>
    public string DefaultModerationModel { get; set; } = "text-moderation-latest";
}

/// <summary>
/// Settings for configuring authentication credentials.
/// </summary>
public sealed class CredentialSettings
{
    /// <summary>
    /// Gets or sets the API key.
    /// </summary>
    public string CredentialSource { get; set; }

    /// <summary>
    /// API key when CredentialSource = "ApiKey".
    /// Prefer environment variable for security.
    /// </summary>
    public string Key { get; set; }

    public void Bind(IConfigurationSection section)
    {
        if (section == null) return;

        CredentialSource = section["CredentialSource"];
        Key = section["Key"];
    }
}