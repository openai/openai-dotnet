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
        // Constructed unconditionally, and deliberately not deferred until telemetry is known to be enabled:
        // this is where the caller-supplied application id is validated, so deferring it would let the
        // telemetry opt-out determine whether an invalid value is rejected. Validation of a caller's options
        // must not vary by machine. The cost is paid once, when the pipeline is built.
        TelemetryDetails telemetryDetails = new(typeof(OpenAIClientUtilities).Assembly, userAgentApplicationId);

        // Evaluated once when the pipeline is built, so the request path is a single field test.
        bool isTelemetryDisabled = PlatformTelemetry.IsTelemetryDisabled();

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
