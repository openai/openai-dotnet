using Microsoft.Extensions.Hosting;
using System;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.Diagnostics.CodeAnalysis;

namespace OpenAI.Assistants;

/// <summary>
/// Extension methods for adding OpenAI clients with configuration and DI support following System.ClientModel pattern.
/// See: https://github.com/Azure/azure-sdk-for-net/blob/main/sdk/core/System.ClientModel/src/docs/ConfigurationAndDependencyInjection.md
/// </summary>
[Experimental("SCME0002")]
public static class OpenAIAssistantsHostBuilderExtensions
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
}
