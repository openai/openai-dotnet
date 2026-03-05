using System;
using OpenAI.Responses;

namespace OpenAI;

/// <summary>
/// Provides conversion from <see cref="OpenAIClientOptions"/> to <see cref="ResponsesClientOptions"/>.
/// This lives in the OpenAI assembly only (not compiled into the Responses assembly).
/// </summary>
internal static class ResponsesClientOptionsFactory
{
    internal static ResponsesClientOptions FromOpenAIClientOptions(OpenAIClientOptions options)
    {
        if (options is null)
        {
            return new ResponsesClientOptions();
        }

        var result = new ResponsesClientOptions
        {
            Endpoint = options.Endpoint,
            OrganizationId = options.OrganizationId,
            ProjectId = options.ProjectId,
            UserAgentApplicationId = options.UserAgentApplicationId,
        };

        // Copy base ClientPipelineOptions properties so that any test-proxy
        // transport or custom retry/logging policy configured by the caller
        // (e.g. InstrumentClientOptions) is preserved.
        if (options.Transport is not null)
        {
            result.Transport = options.Transport;
        }

        if (options.RetryPolicy is not null)
        {
            result.RetryPolicy = options.RetryPolicy;
        }

        if (options.NetworkTimeout is not null)
        {
            result.NetworkTimeout = options.NetworkTimeout;
        }

        return result;
    }
}
