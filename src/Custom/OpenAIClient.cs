using OpenAI.Assistants;
using OpenAI.Audio;
using OpenAI.Batch;
using OpenAI.Chat;
using OpenAI.Embeddings;
using OpenAI.Files;
using OpenAI.FineTuning;
using OpenAI.Images;
using OpenAI.LegacyCompletions;
using OpenAI.Models;
using OpenAI.Moderations;
using OpenAI.VectorStores;
using System;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.Versioning;

namespace OpenAI;

/// <summary>
/// A top-level client factory that enables convenient creation of scenario-specific sub-clients while reusing shared
/// configuration details like endpoint, authentication, and pipeline customization.
/// </summary>
[CodeGenModel("OpenAIClient")]
[CodeGenSuppress("OpenAIClient", typeof(ApiKeyCredential))]
[CodeGenSuppress("OpenAIClient", typeof(Uri), typeof(ApiKeyCredential), typeof(OpenAIClientOptions))]
[CodeGenSuppress("GetAssistantClientClient")]
[CodeGenSuppress("GetAudioClientClient")]
[CodeGenSuppress("GetBatchClientClient")]
[CodeGenSuppress("GetChatClientClient")]
[CodeGenSuppress("GetEmbeddingClientClient")]
[CodeGenSuppress("GetFileClientClient")]
[CodeGenSuppress("GetFineTuningClientClient")]
[CodeGenSuppress("GetImageClientClient")]
[CodeGenSuppress("GetInternalAssistantMessageClientClient")]
[CodeGenSuppress("GetInternalAssistantRunClientClient")]
[CodeGenSuppress("GetInternalAssistantThreadClientClient")]
[CodeGenSuppress("GetLegacyCompletionClientClient")]
[CodeGenSuppress("GetModelClientClient")]
[CodeGenSuppress("GetModerationClientClient")]
[CodeGenSuppress("GetVectorStoreClientClient")]
public partial class OpenAIClient
{
    private readonly OpenAIClientOptions _options;

    /// <summary>
    /// The configured connection endpoint.
    /// </summary>
    protected Uri Endpoint => _endpoint;

    /// <summary>
    /// Creates a new instance of <see cref="OpenAIClient"/>. This type is used to share common
    /// <see cref="ClientPipeline"/> and client configuration details across scenario client instances created via
    /// methods like <see cref="GetChatClient(string)"/>.
    /// </summary>
    /// <param name="credential"> The API key to use when authenticating the client. </param>
    /// <param name="options"> A common client options definition that all clients created by this <see cref="OpenAIClient"/> should use. </param>
    /// <exception cref="ArgumentNullException"> The provided <paramref name="credential"/> is <c>null</c>. </exception>
    public OpenAIClient(ApiKeyCredential credential, OpenAIClientOptions options = null)
        : this(CreatePipeline(GetApiKey(credential, requireExplicitCredential: true), options), GetEndpoint(options), options)
    {
        _keyCredential = credential;
    }

    /// <summary>
    /// Creates a new instance of <see cref="OpenAIClient"/>. This type is used to share common
    /// <see cref="ClientPipeline"/> and client configuration details across scenario client instances created via
    /// methods like <see cref="GetChatClient(string)"/>.
    /// <para>
    /// This constructor overload will use the value of the <c>OPENAI_API_KEY</c> environment variable as its
    /// authentication mechanism. To provide an explicit credential, use an alternate constructor like
    /// <see cref="OpenAIClient(ApiKeyCredential,OpenAIClientOptions)"/>.
    /// </para>
    /// </summary>
    /// <param name="options"> A common client options definition that all clients created by this <see cref="OpenAIClient"/> should use. </param>
    public OpenAIClient(OpenAIClientOptions options = default)
        : this(CreatePipeline(GetApiKey(), options), GetEndpoint(options), options)
    {}

    /// <summary>
    /// Creates a new instance of <see cref="OpenAIClient"/>.
    /// </summary>
    /// <param name="pipeline"> The common client pipeline that should be used for all created scenario clients. </param>
    /// <param name="endpoint"> The HTTP endpoint to use. </param>
    /// <param name="options"> The common client options that should be used for all created scenario clients. </param>
    protected OpenAIClient(ClientPipeline pipeline, Uri endpoint, OpenAIClientOptions options)
    {
        _pipeline = pipeline;
        _endpoint = endpoint;
        _options = options;
    }

    /// <summary>
    /// Gets a new instance of <see cref="AssistantClient"/> that reuses the client configuration details provided to
    /// the <see cref="OpenAIClient"/> instance.
    /// </summary>
    /// <remarks>
    /// This method is functionally equivalent to using the <see cref="AssistantClient"/> constructor directly with
    /// the same configuration details.
    /// </remarks>
    /// <returns> A new <see cref="AssistantClient"/>. </returns>
    [Experimental("OPENAI001")]
    public virtual AssistantClient GetAssistantClient() => new(_pipeline, _endpoint, _options);

    /// <summary>
    /// Gets a new instance of <see cref="AudioClient"/> that reuses the client configuration details provided to
    /// the <see cref="OpenAIClient"/> instance.
    /// </summary>
    /// <remarks>
    /// This method is functionally equivalent to using the <see cref="AudioClient"/> constructor directly with
    /// the same configuration details.
    /// </remarks>
    /// <returns> A new <see cref="AudioClient"/>. </returns>
    public virtual AudioClient GetAudioClient(string model) => new(_pipeline, model, _endpoint, _options);

    /// <summary>
    /// Gets a new instance of <see cref="BatchClient"/> that reuses the client configuration details provided to
    /// the <see cref="OpenAIClient"/> instance.
    /// </summary>
    /// <remarks>
    /// This method is functionally equivalent to using the <see cref="BatchClient"/> constructor directly with
    /// the same configuration details.
    /// </remarks>
    /// <returns> A new <see cref="BatchClient"/>. </returns>
    public virtual BatchClient GetBatchClient() => new(_pipeline, _endpoint, _options);

    /// <summary>
    /// Gets a new instance of <see cref="ChatClient"/> that reuses the client configuration details provided to
    /// the <see cref="OpenAIClient"/> instance.
    /// </summary>
    /// <remarks>
    /// This method is functionally equivalent to using the <see cref="ChatClient"/> constructor directly with
    /// the same configuration details.
    /// </remarks>
    /// <returns> A new <see cref="ChatClient"/>. </returns>
    public virtual ChatClient GetChatClient(string model) => new(_pipeline, model, _endpoint, _options);

    /// <summary>
    /// Gets a new instance of <see cref="EmbeddingClient"/> that reuses the client configuration details provided to
    /// the <see cref="OpenAIClient"/> instance.
    /// </summary>
    /// <remarks>
    /// This method is functionally equivalent to using the <see cref="EmbeddingClient"/> constructor directly with
    /// the same configuration details.
    /// </remarks>
    /// <returns> A new <see cref="EmbeddingClient"/>. </returns>
    public virtual EmbeddingClient GetEmbeddingClient(string model) => new(_pipeline, model, _endpoint, _options);

    /// <summary>
    /// Gets a new instance of <see cref="FileClient"/> that reuses the client configuration details provided to
    /// the <see cref="OpenAIClient"/> instance.
    /// </summary>
    /// <remarks>
    /// This method is functionally equivalent to using the <see cref="FileClient"/> constructor directly with
    /// the same configuration details.
    /// </remarks>
    /// <returns> A new <see cref="FileClient"/>. </returns>
    public virtual FileClient GetFileClient() => new(_pipeline, _endpoint, _options);

    /// <summary>
    /// Gets a new instance of <see cref="FineTuningClient"/> that reuses the client configuration details provided to
    /// the <see cref="OpenAIClient"/> instance.
    /// </summary>
    /// <remarks>
    /// This method is functionally equivalent to using the <see cref="FineTuningClient"/> constructor directly with
    /// the same configuration details.
    /// </remarks>
    /// <returns> A new <see cref="FineTuningClient"/>. </returns>
    public virtual FineTuningClient GetFineTuningClient() => new(_pipeline, _endpoint, _options);

    /// <summary>
    /// Gets a new instance of <see cref="ImageClient"/> that reuses the client configuration details provided to
    /// the <see cref="OpenAIClient"/> instance.
    /// </summary>
    /// <remarks>
    /// This method is functionally equivalent to using the <see cref="ImageClient"/> constructor directly with
    /// the same configuration details.
    /// </remarks>
    /// <returns> A new <see cref="ImageClient"/>. </returns>
    public virtual ImageClient GetImageClient(string model) => new(_pipeline, model, _endpoint, _options);

    /// <summary>
    /// Gets a new instance of <see cref="ModelClient"/> that reuses the client configuration details provided to
    /// the <see cref="OpenAIClient"/> instance.
    /// </summary>
    /// <remarks>
    /// This method is functionally equivalent to using the <see cref="ModelClient"/> constructor directly with
    /// the same configuration details.
    /// </remarks>
    /// <returns> A new <see cref="ModelClient"/>. </returns>
    public virtual ModelClient GetModelClient() => new(_pipeline, _endpoint, _options);

    /// <summary>
    /// Gets a new instance of <see cref="ModerationClient"/> that reuses the client configuration details provided to
    /// the <see cref="OpenAIClient"/> instance.
    /// </summary>
    /// <remarks>
    /// This method is functionally equivalent to using the <see cref="ModerationClient"/> constructor directly with
    /// the same configuration details.
    /// </remarks>
    /// <returns> A new <see cref="ModerationClient"/>. </returns>
    public virtual ModerationClient GetModerationClient(string model) => new(_pipeline, model, _endpoint, _options);

    /// <summary>
    /// Gets a new instance of <see cref="VectorStoreClient"/> that reuses the client configuration details provided to
    /// the <see cref="OpenAIClient"/> instance.
    /// </summary>
    /// <remarks>
    /// This method is functionally equivalent to using the <see cref="VectorStoreClient"/> constructor directly with
    /// the same configuration details.
    /// </remarks>
    /// <returns> A new <see cref="ModelClient"/>. </returns>
    [Experimental("OPENAI001")]
    public virtual VectorStoreClient GetVectorStoreClient() => new(_pipeline, _endpoint, _options);

    internal static ClientPipeline CreatePipeline(ApiKeyCredential credential, OpenAIClientOptions options = null)
    {
        return ClientPipeline.Create(
            options ?? new(),
            perCallPolicies: [],
            perTryPolicies:
            [
                ApiKeyAuthenticationPolicy.CreateHeaderApiKeyPolicy(credential, AuthorizationHeader, AuthorizationApiKeyPrefix),
                CreateAddBetaFeatureHeaderPolicy(),
                CreateAddUserAgentHeaderPolicy(options),
            ],
            beforeTransportPolicies: []);
    }

    internal static Uri GetEndpoint(OpenAIClientOptions options)
    {
        return options?.Endpoint ?? new(Environment.GetEnvironmentVariable(OpenAIEndpointEnvironmentVariable) ?? OpenAIV1Endpoint);
    }

    internal static ApiKeyCredential GetApiKey(ApiKeyCredential explicitCredential = null, bool requireExplicitCredential = false)
    {
        if (explicitCredential is not null)
        {
            return explicitCredential;
        }
        else if (requireExplicitCredential)
        {
            throw new ArgumentNullException(nameof(explicitCredential), $"A non-null credential value is required.");
        }
        else
        {
            string environmentApiKey = Environment.GetEnvironmentVariable(OpenAIApiKeyEnvironmentVariable);
            if (string.IsNullOrEmpty(environmentApiKey))
            {
                throw new InvalidOperationException(
                    $"No environment variable value was found for {OpenAIApiKeyEnvironmentVariable}. "
                    + "Please either populate this environment variable or provide authentication information directly "
                    + "to the client constructor.");
            }
            return new(environmentApiKey);
        }
    }

    private static PipelinePolicy CreateAddBetaFeatureHeaderPolicy()
    {
        return new GenericActionPipelinePolicy((message) =>
        {
            if (message?.Request?.Headers?.TryGetValue(OpenAIBetaFeatureHeaderKey, out string _) == false)
            {
                message.Request.Headers.Set(OpenAIBetaFeatureHeaderKey, OpenAIBetaAssistantsV1HeaderValue);
            }
        });
    }

    private static PipelinePolicy CreateAddUserAgentHeaderPolicy(OpenAIClientOptions options = null)
    {
        TelemetryDetails telemetryDetails = new(typeof(OpenAIClientOptions).Assembly, options?.ApplicationId);
        return new GenericActionPipelinePolicy((message) =>
        {
            if (message?.Request?.Headers?.TryGetValue(UserAgentHeaderKey, out string _) == false)
            {
                message.Request.Headers.Set(UserAgentHeaderKey, telemetryDetails.ToString());
            }
        });
    }

    private const string OpenAIBetaFeatureHeaderKey = "OpenAI-Beta";
    private const string OpenAIBetaAssistantsV1HeaderValue = "assistants=v2";
    private const string OpenAIEndpointEnvironmentVariable = "OPENAI_ENDPOINT";
    private const string OpenAIApiKeyEnvironmentVariable = "OPENAI_API_KEY";
    private const string OpenAIV1Endpoint = "https://api.openai.com/v1";
    private const string UserAgentHeaderKey = "User-Agent";
}
