using Microsoft.Extensions.Hosting;
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
using System.Diagnostics.CodeAnalysis;

namespace OpenAI.DependencyInjection;

/// <summary>
/// Extension methods for adding OpenAI clients with configuration and DI support following System.ClientModel pattern.
/// See: https://github.com/Azure/azure-sdk-for-net/blob/main/sdk/core/System.ClientModel/src/docs/ConfigurationAndDependencyInjection.md
/// </summary>
[Experimental("SCME0002")]
public static class OpenAIHostBuilderExtensions
{
    public static IClientBuilder AddAssistantClient(
        this IHostApplicationBuilder builder,
        string sectionName)
    {
        if (builder is null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        return builder.AddClient<AssistantClient, AssistantClientSettings>(sectionName);
    }

    public static IClientBuilder AddKeyedAssistantClient(
        this IHostApplicationBuilder builder,
        string serviceKey,
        string sectionName)
    {
        if (builder is null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        return builder.AddKeyedClient<AssistantClient, AssistantClientSettings>(serviceKey, sectionName);
    }

    public static IClientBuilder AddAudioClient(
        this IHostApplicationBuilder builder,
        string sectionName)
    {
        if (builder is null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        return builder.AddClient<AudioClient, AudioClientSettings>(sectionName);
    }

    public static IClientBuilder AddKeyedAudioClient(
        this IHostApplicationBuilder builder,
        string serviceKey,
        string sectionName)
    {
        if (builder is null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        return builder.AddKeyedClient<AudioClient, AudioClientSettings>(serviceKey, sectionName);
    }

    public static IClientBuilder AddBatchClient(
        this IHostApplicationBuilder builder,
        string sectionName)
    {
        if (builder is null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        return builder.AddClient<BatchClient, BatchClientSettings>(sectionName);
    }

    public static IClientBuilder AddKeyedBatchClient(
        this IHostApplicationBuilder builder,
        string serviceKey,
        string sectionName)
    {
        if (builder is null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        return builder.AddKeyedClient<BatchClient, BatchClientSettings>(serviceKey, sectionName);
    }

    public static IClientBuilder AddChatClient(
        this IHostApplicationBuilder builder,
        string sectionName)
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
        string sectionName)
    {
        if (builder is null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        return builder.AddKeyedClient<ChatClient, ChatClientSettings>(serviceKey, sectionName);
    }

    public static IClientBuilder AddContainerClient(
        this IHostApplicationBuilder builder,
        string sectionName)
    {
        if (builder is null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        return builder.AddClient<ContainerClient, ContainerClientSettings>(sectionName);
    }

    public static IClientBuilder AddKeyedContainerClient(
        this IHostApplicationBuilder builder,
        string serviceKey,
        string sectionName)
    {
        if (builder is null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        return builder.AddKeyedClient<ContainerClient, ContainerClientSettings>(serviceKey, sectionName);
    }

    public static IClientBuilder AddConversationClient(
        this IHostApplicationBuilder builder,
        string sectionName)
    {
        if (builder is null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        return builder.AddClient<ConversationClient, ConversationClientSettings>(sectionName);
    }

    public static IClientBuilder AddKeyedConversationClient(
        this IHostApplicationBuilder builder,
        string serviceKey,
        string sectionName)
    {
        if (builder is null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        return builder.AddKeyedClient<ConversationClient, ConversationClientSettings>(serviceKey, sectionName);
    }

    public static IClientBuilder AddEmbeddingClient(
        this IHostApplicationBuilder builder,
        string sectionName)
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
        string sectionName)
    {
        if (builder is null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        return builder.AddKeyedClient<EmbeddingClient, EmbeddingClientSettings>(serviceKey, sectionName);
    }

    public static IClientBuilder AddEvaluationClient(
        this IHostApplicationBuilder builder,
        string sectionName)
    {
        if (builder is null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        return builder.AddClient<EvaluationClient, EvaluationClientSettings>(sectionName);
    }

    public static IClientBuilder AddKeyedEvaluationClient(
        this IHostApplicationBuilder builder,
        string serviceKey,
        string sectionName)
    {
        if (builder is null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        return builder.AddKeyedClient<EvaluationClient, EvaluationClientSettings>(serviceKey, sectionName);
    }

    public static IClientBuilder AddOpenAIFileClient(
        this IHostApplicationBuilder builder,
        string sectionName)
    {
        if (builder is null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        return builder.AddClient<OpenAIFileClient, OpenAIFileClientSettings>(sectionName);
    }

    public static IClientBuilder AddKeyedOpenAIFileClient(
        this IHostApplicationBuilder builder,
        string serviceKey,
        string sectionName)
    {
        if (builder is null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        return builder.AddKeyedClient<OpenAIFileClient, OpenAIFileClientSettings>(serviceKey, sectionName);
    }

    public static IClientBuilder AddFineTuningClient(
        this IHostApplicationBuilder builder,
        string sectionName)
    {
        if (builder is null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        return builder.AddClient<FineTuningClient, FineTuningClientSettings>(sectionName);
    }

    public static IClientBuilder AddKeyedFineTuningClient(
        this IHostApplicationBuilder builder,
        string serviceKey,
        string sectionName)
    {
        if (builder is null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        return builder.AddKeyedClient<FineTuningClient, FineTuningClientSettings>(serviceKey, sectionName);
    }

    public static IClientBuilder AddGraderClient(
        this IHostApplicationBuilder builder,
        string sectionName)
    {
        if (builder is null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        return builder.AddClient<GraderClient, GraderClientSettings>(sectionName);
    }

    public static IClientBuilder AddKeyedGraderClient(
        this IHostApplicationBuilder builder,
        string serviceKey,
        string sectionName)
    {
        if (builder is null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        return builder.AddKeyedClient<GraderClient, GraderClientSettings>(serviceKey, sectionName);
    }

    public static IClientBuilder AddImageClient(
        this IHostApplicationBuilder builder,
        string sectionName)
    {
        if (builder is null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        return builder.AddClient<ImageClient, ImageClientSettings>(sectionName);
    }

    public static IClientBuilder AddKeyedImageClient(
        this IHostApplicationBuilder builder,
        string serviceKey,
        string sectionName)
    {
        if (builder is null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        return builder.AddKeyedClient<ImageClient, ImageClientSettings>(serviceKey, sectionName);
    }

    public static IClientBuilder AddOpenAIModelClient(
        this IHostApplicationBuilder builder,
        string sectionName)
    {
        if (builder is null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        return builder.AddClient<OpenAIModelClient, OpenAIModelClientSettings>(sectionName);
    }

    public static IClientBuilder AddKeyedOpenAIModelClient(
        this IHostApplicationBuilder builder,
        string serviceKey,
        string sectionName)
    {
        if (builder is null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        return builder.AddKeyedClient<OpenAIModelClient, OpenAIModelClientSettings>(serviceKey, sectionName);
    }

    public static IClientBuilder AddModerationClient(
        this IHostApplicationBuilder builder,
        string sectionName)
    {
        if (builder is null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        return builder.AddClient<ModerationClient, ModerationClientSettings>(sectionName);
    }

    public static IClientBuilder AddKeyedModerationClient(
        this IHostApplicationBuilder builder,
        string serviceKey,
        string sectionName)
    {
        if (builder is null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        return builder.AddKeyedClient<ModerationClient, ModerationClientSettings>(serviceKey, sectionName);
    }

    public static IClientBuilder AddRealtimeClient(
        this IHostApplicationBuilder builder,
        string sectionName)
    {
        if (builder is null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        return builder.AddClient<RealtimeClient, RealtimeClientSettings>(sectionName);
    }

    public static IClientBuilder AddKeyedRealtimeClient(
        this IHostApplicationBuilder builder,
        string serviceKey,
        string sectionName)
    {
        if (builder is null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        return builder.AddKeyedClient<RealtimeClient, RealtimeClientSettings>(serviceKey, sectionName);
    }

    public static IClientBuilder AddResponsesClient(
        this IHostApplicationBuilder builder,
        string sectionName)
    {
        if (builder is null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        return builder.AddClient<ResponsesClient, ResponsesClientSettings>(sectionName);
    }

    public static IClientBuilder AddKeyedResponsesClient(
        this IHostApplicationBuilder builder,
        string serviceKey,
        string sectionName)
    {
        if (builder is null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        return builder.AddKeyedClient<ResponsesClient, ResponsesClientSettings>(serviceKey, sectionName);
    }

    public static IClientBuilder AddVectorStoreClient(
        this IHostApplicationBuilder builder,
        string sectionName)
    {
        if (builder is null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        return builder.AddClient<VectorStoreClient, VectorStoreClientSettings>(sectionName);
    }

    public static IClientBuilder AddKeyedVectorStoreClient(
        this IHostApplicationBuilder builder,
        string serviceKey,
        string sectionName)
    {
        if (builder is null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        return builder.AddKeyedClient<VectorStoreClient, VectorStoreClientSettings>(serviceKey, sectionName);
    }

    public static IClientBuilder AddVideoClient(
        this IHostApplicationBuilder builder,
        string sectionName)
    {
        if (builder is null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        return builder.AddClient<VideoClient, VideoClientSettings>(sectionName);
    }

    public static IClientBuilder AddKeyedVideoClient(
        this IHostApplicationBuilder builder,
        string serviceKey,
        string sectionName)
    {
        if (builder is null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        return builder.AddKeyedClient<VideoClient, VideoClientSettings>(serviceKey, sectionName);
    }
}
