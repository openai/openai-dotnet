using Microsoft.Extensions.Hosting;
using System;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.Diagnostics.CodeAnalysis;

namespace OpenAI.Chat;

/// <summary>
/// Extension methods for adding OpenAI clients with configuration and DI support following System.ClientModel pattern.
/// See: https://github.com/Azure/azure-sdk-for-net/blob/main/sdk/core/System.ClientModel/src/docs/ConfigurationAndDependencyInjection.md
/// </summary>
[Experimental("SCME0002")]
public static class OpenAIChatHostBuilderExtensions
{
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
}
