using Microsoft.Extensions.Hosting;
using System;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.Diagnostics.CodeAnalysis;

namespace OpenAI.FineTuning;

/// <summary>
/// Extension methods for adding OpenAI clients with configuration and DI support following System.ClientModel pattern.
/// See: https://github.com/Azure/azure-sdk-for-net/blob/main/sdk/core/System.ClientModel/src/docs/ConfigurationAndDependencyInjection.md
/// </summary>
[Experimental("SCME0002")]
public static class OpenAIFineTuningHostBuilderExtensions
{
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
}
