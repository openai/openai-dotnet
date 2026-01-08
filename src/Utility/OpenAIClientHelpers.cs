using System;
using System.ClientModel;
using System.ClientModel.Primitives;

namespace OpenAI;

internal static class OpenAIClientHelpers
{
    private const string AuthorizationHeader = "Authorization";
    private const string AuthorizationApiKeyPrefix = "Bearer";
    private const string OpenAIV1Endpoint = "https://api.openai.com/v1";

    internal static class KnownHeaderNames
    {
        public const string UserAgent = "User-Agent";
        public const string OpenAIOrganization = "OpenAI-Organization";
        public const string OpenAIProject = "OpenAI-Project";
    }

    internal static AuthenticationPolicy CreateApiKeyAuthenticationPolicy(ApiKeyCredential credential)
    {
        Argument.AssertNotNull(credential, nameof(credential));
        return ApiKeyAuthenticationPolicy.CreateHeaderApiKeyPolicy(credential, AuthorizationHeader, AuthorizationApiKeyPrefix);
    }

    internal static ClientPipeline CreatePipeline(AuthenticationPolicy authenticationPolicy, OpenAIClientOptions options)
    {
        return ClientPipeline.Create(
            options: options,
            perCallPolicies: [CreateAddCustomHeadersPolicy(options)],
            perTryPolicies: [authenticationPolicy],
            beforeTransportPolicies: []);
    }

    internal static Uri GetEndpoint(OpenAIClientOptions options = null)
    {
        return options?.Endpoint ?? new(OpenAIV1Endpoint);
    }

    private static PipelinePolicy CreateAddCustomHeadersPolicy(OpenAIClientOptions options = null)
    {
        TelemetryDetails telemetryDetails = new(typeof(OpenAIClientOptions).Assembly, options?.UserAgentApplicationId);
        return new GenericActionPipelinePolicy((message) =>
        {
            if (message?.Request?.Headers?.TryGetValue(KnownHeaderNames.UserAgent, out string _) == false)
            {
                message.Request.Headers.Set(KnownHeaderNames.UserAgent, telemetryDetails.ToString());
            }

            if (!string.IsNullOrEmpty(options?.OrganizationId))
            {
                message.Request.Headers.Set(KnownHeaderNames.OpenAIOrganization, options.OrganizationId);
            }

            if (!string.IsNullOrEmpty(options?.ProjectId))
            {
                message.Request.Headers.Set(KnownHeaderNames.OpenAIProject, options.ProjectId);
            }
        });
    }
}
