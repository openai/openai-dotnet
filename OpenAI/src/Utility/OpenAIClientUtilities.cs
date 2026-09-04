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
            perCallPolicies:
            [
                CreateAddCustomHeadersPolicy(userAgentApplicationId, organizationId, projectId),
                new GenericActionPipelinePolicy(message =>
                    message.ResponseClassifier = new RetryAfterMessageClassifier(message.ResponseClassifier)),
            ],
            perTryPolicies: [authenticationPolicy],
            beforeTransportPolicies: []);
    }

    public static Uri GetEndpoint(Uri endpoint)
    {
        return endpoint ?? new Uri(OpenAIV1Endpoint);
    }

    private static PipelinePolicy CreateAddCustomHeadersPolicy(string userAgentApplicationId, string organizationId, string projectId)
    {
        // Evaluated once when the pipeline is built, so the request path is a single field test.
        bool isTelemetryDisabled = PlatformTelemetry.IsTelemetryDisabled();

        // Opting out means no telemetry work is performed at all. The application id feeds only the user
        // agent, so when it will not be sent the value is unused and neither composed nor validated: the
        // length bound exists to keep the header from being inflated, and there is no header to inflate.
        TelemetryDetails telemetryDetails = isTelemetryDisabled
            ? null
            : new TelemetryDetails(typeof(OpenAIClientUtilities).Assembly, userAgentApplicationId);

        return new GenericActionPipelinePolicy((message) =>
        {
            if (message?.Request?.Headers is null)
            {
                return;
            }

            // Opting out of telemetry suppresses both the platform metadata headers and the user agent that
            // this library would otherwise add. A caller-supplied user agent is still honored.
            if (!isTelemetryDisabled)
            {
                if (!message.Request.Headers.TryGetValue(UserAgentHeaderName, out string _))
                {
                    message.Request.Headers.Set(UserAgentHeaderName, telemetryDetails.ToString());
                }

                PlatformTelemetry.ApplyTo(message.Request);
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
