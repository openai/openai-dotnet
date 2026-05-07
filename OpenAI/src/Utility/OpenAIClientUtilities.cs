using System;
using System.ClientModel;
using System.ClientModel.Primitives;

namespace OpenAI;

internal static class OpenAIClientUtilities
{
    public const string OpenAIV1Endpoint = "https://api.openai.com/v1";

    private const string AuthorizationHeader = "Authorization";
    private const string AuthorizationApiKeyPrefix = "Bearer";

    private const string OpenAIOrganizationHeaderName = "OpenAI-Organization";
    private const string OpenAIProjectHeaderName = "OpenAI-Project";
    private const string UserAgentHeaderName = "User-Agent";

    public static AuthenticationPolicy CreateApiKeyAuthenticationPolicy(ApiKeyCredential credential)
    {
        Argument.AssertNotNull(credential, nameof(credential));
        return ApiKeyAuthenticationPolicy.CreateHeaderApiKeyPolicy(credential, AuthorizationHeader, AuthorizationApiKeyPrefix);
    }

    public static ClientPipeline CreatePipeline(
        AuthenticationPolicy authenticationPolicy,
        ClientPipelineOptions options,
        string userAgentApplicationId,
        string organizationId,
        string projectId)
    {
        return ClientPipeline.Create(
            options: options,
            perCallPolicies: [CreateAddCustomHeadersPolicy(userAgentApplicationId, organizationId, projectId)],
            perTryPolicies: [authenticationPolicy],
            beforeTransportPolicies: []);
    }

    public static Uri GetEndpoint(Uri endpoint)
    {
        return endpoint ?? new Uri(OpenAIV1Endpoint);
    }

    private static PipelinePolicy CreateAddCustomHeadersPolicy(string userAgentApplicationId, string organizationId, string projectId)
    {
        TelemetryDetails telemetryDetails = new(typeof(OpenAIClientUtilities).Assembly, userAgentApplicationId);
        return new GenericActionPipelinePolicy((message) =>
        {
            if (message?.Request?.Headers?.TryGetValue(UserAgentHeaderName, out string _) == false)
            {
                message.Request.Headers.Set(UserAgentHeaderName, telemetryDetails.ToString());
            }

            if (!string.IsNullOrEmpty(organizationId))
            {
                message.Request.Headers.Set(OpenAIOrganizationHeaderName, organizationId);
            }

            if (!string.IsNullOrEmpty(projectId))
            {
                message.Request.Headers.Set(OpenAIProjectHeaderName, projectId);
            }
        });
    }
}
