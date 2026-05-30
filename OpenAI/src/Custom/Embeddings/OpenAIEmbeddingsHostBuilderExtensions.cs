using Microsoft.Extensions.Hosting;
using System;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.Diagnostics.CodeAnalysis;

namespace OpenAI.Embeddings;

/// <summary>
/// Extension methods for adding OpenAI clients with configuration and DI support following System.ClientModel pattern.
/// See: https://github.com/Azure/azure-sdk-for-net/blob/main/sdk/core/System.ClientModel/src/docs/ConfigurationAndDependencyInjection.md
/// </summary>
[Experimental("SCME0002")]
public static class OpenAIEmbeddingsHostBuilderExtensions
{
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
}
