using Microsoft.Extensions.Hosting;
using OpenAI.Audio;
using OpenAI.Chat;
using OpenAI.Embeddings;
using OpenAI.Images;
using OpenAI.Moderations;
using System;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.Diagnostics.CodeAnalysis;

namespace OpenAI.DependencyInjection;

/// <summary>
/// Extension methods for adding OpenAI clients with configuration and DI support following System.ClientModel pattern.
/// See: https://github.com/Azure/azure-sdk-for-net/blob/main/sdk/core/System.ClientModel/src/docs/ConfigurationAndDependencyInjection.md
/// </summary>
[Experimental("SCME0002")]
public static class OpenAIHostBuilderExtensions
{
    public static IClientBuilder AddChatClient(
        this IHostApplicationBuilder builder,
        string sectionName = "Chat")
    {
        if (builder is null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        return builder.AddClient<ChatClient, ChatClientSettings>(sectionName);
    }

    public static IClientBuilder AddKeyedChatClient(
        this IHostApplicationBuilder builder,
        string serviceKey,
        string sectionName = "Chat")
    {
        if (builder is null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        return builder.AddKeyedClient<ChatClient, ChatClientSettings>(serviceKey, sectionName);
    }

    public static IClientBuilder AddEmbeddingClient(
        this IHostApplicationBuilder builder,
        string sectionName = "Embedding")
    {
        if (builder is null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        return builder.AddClient<EmbeddingClient, EmbeddingClientSettings>(sectionName);
    }

    public static IClientBuilder AddKeyedEmbeddingClient(
        this IHostApplicationBuilder builder,
        string serviceKey,
        string sectionName = "Embedding")
    {
        if (builder is null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        return builder.AddKeyedClient<EmbeddingClient, EmbeddingClientSettings>(serviceKey, sectionName);
    }

    public static IClientBuilder AddAudioClient(
        this IHostApplicationBuilder builder,
        string sectionName = "Audio")
    {
        if (builder is null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        return builder.AddClient<AudioClient, AudioClientSettings>(sectionName);
    }

    public static IClientBuilder AddImageClient(
        this IHostApplicationBuilder builder,
        string sectionName = "Image")
    {
        if (builder is null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        return builder.AddClient<ImageClient, ImageClientSettings>(sectionName);
    }

    public static IClientBuilder AddModerationClient(
        this IHostApplicationBuilder builder,
        string sectionName = "Moderation")
    {
        if (builder is null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        return builder.AddClient<ModerationClient, ModerationClientSettings>(sectionName);
    }
}