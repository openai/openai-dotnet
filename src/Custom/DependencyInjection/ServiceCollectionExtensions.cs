using Microsoft.Extensions.Hosting;
using System;
using System.ClientModel;
using System.ClientModel.Primitives;

namespace OpenAI.Custom.DependencyInjection;

/// <summary>
/// Extension methods for adding OpenAI clients with configuration and DI support following System.ClientModel pattern.
/// See: https://github.com/Azure/azure-sdk-for-net/blob/main/sdk/core/System.ClientModel/src/docs/ConfigurationAndDependencyInjection.md
/// </summary>
#pragma warning disable SCME0002
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds OpenAIClient with configuration and DI support using System.ClientModel.
    /// </summary>
    public static IClientBuilder AddOpenAIClient(
        this IHostApplicationBuilder builder,
        string sectionName = "OpenAI")
    {
        if (builder is null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        return builder.AddClient<OpenAIClient, OpenAIServiceOptions>(sectionName);
    }

    /// <summary>
    /// Adds a keyed OpenAIClient (for multiple instances with different configs).
    /// </summary>
    public static IClientBuilder AddKeyedOpenAIClient(
        this IHostApplicationBuilder builder,
        string serviceKey,
        string sectionName = "OpenAI")
    {
        if (builder is null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        return builder.AddKeyedClient<OpenAIClient, OpenAIServiceOptions>(serviceKey, sectionName);
    }
}
#pragma warning restore SCME0002