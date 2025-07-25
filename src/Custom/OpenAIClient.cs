using OpenAI.Assistants;
using OpenAI.Audio;
using OpenAI.Batch;
using OpenAI.Chat;
using OpenAI.Containers;
using OpenAI.Embeddings;
using OpenAI.Evals;
using OpenAI.Files;
using OpenAI.FineTuning;
using OpenAI.Graders;
using OpenAI.Images;
using OpenAI.Models;
using OpenAI.Moderations;
using OpenAI.Realtime;
using OpenAI.Responses;
using OpenAI.VectorStores;
using System;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.Diagnostics.CodeAnalysis;

namespace OpenAI;

// CUSTOM:
// - Suppressed constructor that takes endpoint parameter; endpoint is now a property in the options class.
// - Suppressed cached clients. Clients are not singletons, and users can create multiple clients of the same type
//   if needed (e.g., to target different OpenAI models). The Get*Client methods return new client instances.
/// <summary>
/// A top-level client factory that enables convenient creation of scenario-specific sub-clients while reusing shared
/// configuration details like endpoint, authentication, and pipeline customization.
/// </summary>
[CodeGenType("OpenAIClient")]
[CodeGenSuppress("OpenAIClient", typeof(ApiKeyCredential))]
[CodeGenSuppress("OpenAIClient", typeof(Uri), typeof(ApiKeyCredential), typeof(OpenAIClientOptions))]
[CodeGenSuppress("_cachedAssistantClient")]
[CodeGenSuppress("_cachedAudioClient")]
[CodeGenSuppress("_cachedBatchClient")]
[CodeGenSuppress("_cachedChatClient")]
[CodeGenSuppress("_cachedLegacyCompletionClient")]
[CodeGenSuppress("_cachedContainerClient")]
[CodeGenSuppress("_cachedEmbeddingClient")]
[CodeGenSuppress("_cachedEvaluationClient")]
[CodeGenSuppress("_cachedOpenAIFileClient")]
[CodeGenSuppress("_cachedFineTuningClient")]
[CodeGenSuppress("_cachedGraderClient")]
[CodeGenSuppress("_cachedImageClient")]
[CodeGenSuppress("_cachedOpenAIModelClient")]
[CodeGenSuppress("_cachedModerationClient")]
[CodeGenSuppress("_cachedRealtimeClient")]
[CodeGenSuppress("_cachedOpenAIResponseClient")]
[CodeGenSuppress("_cachedVectorStoreClient")]
[CodeGenSuppress("_cachedInternalAssistantMessageClient")]
[CodeGenSuppress("_cachedInternalAssistantRunClient")]
[CodeGenSuppress("_cachedInternalAssistantThreadClient")]
[CodeGenSuppress("_cachedInternalUploadsClient")]
[CodeGenSuppress("GetAssistantClient")]
[CodeGenSuppress("GetAudioClient")]
[CodeGenSuppress("GetBatchClient")]
[CodeGenSuppress("GetChatClient")]
[CodeGenSuppress("GetLegacyCompletionClient")]
[CodeGenSuppress("GetContainerClient")]
[CodeGenSuppress("GetEmbeddingClient")]
[CodeGenSuppress("GetEvaluationClient")]
[CodeGenSuppress("GetOpenAIFileClient")]
[CodeGenSuppress("GetFineTuningClient")]
[CodeGenSuppress("GetGraderClient")]
[CodeGenSuppress("GetImageClient")]
[CodeGenSuppress("GetOpenAIModelClient")]
[CodeGenSuppress("GetModerationClient")]
[CodeGenSuppress("GetRealtimeClient")]
[CodeGenSuppress("GetOpenAIResponseClient")]
[CodeGenSuppress("GetVectorStoreClient")]
[CodeGenSuppress("GetInternalAssistantMessageClient")]
[CodeGenSuppress("GetInternalAssistantRunClient")]
[CodeGenSuppress("GetInternalAssistantThreadClient")]
[CodeGenSuppress("GetInternalUploadsClient")]
public partial class OpenAIClient
{
    private const string OpenAIV1Endpoint = "https://api.openai.com/v1";

    private static class KnownHeaderNames
    {
        public const string OpenAIOrganization = "OpenAI-Organization";
        public const string OpenAIProject = "OpenAI-Project";
        public const string UserAgent = "User-Agent";
    }

    private readonly OpenAIClientOptions _options;

    // CUSTOM: Added as a convenience.
    /// <summary> Initializes a new instance of <see cref="OpenAIClient"/>. </summary>
    /// <param name="apiKey"> The API key to authenticate with the service. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="apiKey"/> is null. </exception>
    public OpenAIClient(string apiKey) : this(new ApiKeyCredential(apiKey), new OpenAIClientOptions())
    {
    }

    // CUSTOM:
    // - Used a custom pipeline.
    // - Demoted the endpoint parameter to be a property in the options class.
    /// <summary> Initializes a new instance of <see cref="OpenAIClient"/>. </summary>
    /// <param name="credential"> The API key to authenticate with the service. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="credential"/> is null. </exception>
    public OpenAIClient(ApiKeyCredential credential) : this(credential, new OpenAIClientOptions())
    {
    }

    // CUSTOM:
    // - Used a custom pipeline.
    // - Demoted the endpoint parameter to be a property in the options class.
    /// <summary> Initializes a new instance of <see cref="OpenAIClient"/>. </summary>
    /// <param name="credential"> The API key to authenticate with the service. </param>
    /// <param name="options"> The options to configure the client. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="credential"/> is null. </exception>
    public OpenAIClient(ApiKeyCredential credential, OpenAIClientOptions options)
    {
        Argument.AssertNotNull(credential, nameof(credential));
        options ??= new OpenAIClientOptions();

        _keyCredential = credential;
        Pipeline = OpenAIClient.CreatePipeline(credential, options);
        _endpoint = OpenAIClient.GetEndpoint(options);
        _options = options;
    }

    // CUSTOM: Added protected internal constructor that takes a ClientPipeline.
    /// <summary> Initializes a new instance of <see cref="OpenAIClient"/>. </summary>
    /// <param name="pipeline"> The HTTP pipeline to send and receive REST requests and responses. </param>
    /// <param name="options"> The options to configure the client. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="pipeline"/> is null. </exception>
    protected internal OpenAIClient(ClientPipeline pipeline, OpenAIClientOptions options)
    {
        Argument.AssertNotNull(pipeline, nameof(pipeline));
        options ??= new OpenAIClientOptions();

        Pipeline = pipeline;
        _endpoint = OpenAIClient.GetEndpoint(options);
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
    public virtual AssistantClient GetAssistantClient() => new(Pipeline, _options);

    /// <summary>
    /// Gets a new instance of <see cref="AudioClient"/> that reuses the client configuration details provided to
    /// the <see cref="OpenAIClient"/> instance.
    /// </summary>
    /// <remarks>
    /// This method is functionally equivalent to using the <see cref="AudioClient"/> constructor directly with
    /// the same configuration details.
    /// </remarks>
    /// <returns> A new <see cref="AudioClient"/>. </returns>
    public virtual AudioClient GetAudioClient(string model) => new(Pipeline, model, _options);

    /// <summary>
    /// Gets a new instance of <see cref="BatchClient"/> that reuses the client configuration details provided to
    /// the <see cref="OpenAIClient"/> instance.
    /// </summary>
    /// <remarks>
    /// This method is functionally equivalent to using the <see cref="BatchClient"/> constructor directly with
    /// the same configuration details.
    /// </remarks>
    /// <returns> A new <see cref="BatchClient"/>. </returns>
    [Experimental("OPENAI001")]
    public virtual BatchClient GetBatchClient() => new(Pipeline, _options);

    /// <summary>
    /// Gets a new instance of <see cref="ChatClient"/> that reuses the client configuration details provided to
    /// the <see cref="OpenAIClient"/> instance.
    /// </summary>
    /// <remarks>
    /// This method is functionally equivalent to using the <see cref="ChatClient"/> constructor directly with
    /// the same configuration details.
    /// </remarks>
    /// <returns> A new <see cref="ChatClient"/>. </returns>
    public virtual ChatClient GetChatClient(string model) => new(Pipeline, model, _options);

    /// <summary>
    /// Gets a new instance of <see cref="EmbeddingClient"/> that reuses the client configuration details provided to
    /// the <see cref="OpenAIClient"/> instance.
    /// </summary>
    /// <remarks>
    /// This method is functionally equivalent to using the <see cref="EmbeddingClient"/> constructor directly with
    /// the same configuration details.
    /// </remarks>
    /// <returns> A new <see cref="EmbeddingClient"/>. </returns>
    public virtual EmbeddingClient GetEmbeddingClient(string model) => new(Pipeline, model, _options);

    /// <summary>
    /// Gets a new instance of <see cref="EvaluationClient"/> that reuses the client configuration details provided to
    /// the <see cref="OpenAIClient"/> instance.
    /// </summary>
    /// <returns></returns>
    [Experimental("OPENAI001")]
    public virtual EvaluationClient GetEvaluationClient() => new(Pipeline, _options);

    /// <summary>
    /// Gets a new instance of <see cref="OpenAIFileClient"/> that reuses the client configuration details provided to
    /// the <see cref="OpenAIClient"/> instance.
    /// </summary>
    /// <remarks>
    /// This method is functionally equivalent to using the <see cref="OpenAIFileClient"/> constructor directly with
    /// the same configuration details.
    /// </remarks>
    /// <returns> A new <see cref="OpenAIFileClient"/>. </returns>
    public virtual OpenAIFileClient GetOpenAIFileClient() => new(Pipeline, _options);

    /// <summary>
    /// Gets a new instance of <see cref="FineTuningClient"/> that reuses the client configuration details provided to
    /// the <see cref="OpenAIClient"/> instance.
    /// </summary>
    /// <remarks>
    /// This method is functionally equivalent to using the <see cref="FineTuningClient"/> constructor directly with
    /// the same configuration details.
    /// </remarks>
    /// <returns> A new <see cref="FineTuningClient"/>. </returns>
    [Experimental("OPENAI001")]
    public virtual FineTuningClient GetFineTuningClient() => new(Pipeline, _options);

    /// <summary>
    /// Gets a new instance of <see cref="ImageClient"/> that reuses the client configuration details provided to
    /// the <see cref="OpenAIClient"/> instance.
    /// </summary>
    /// <remarks>
    /// This method is functionally equivalent to using the <see cref="ImageClient"/> constructor directly with
    /// the same configuration details.
    /// </remarks>
    /// <returns> A new <see cref="ImageClient"/>. </returns>
    public virtual ImageClient GetImageClient(string model) => new(Pipeline, model, _options);

    /// <summary>
    /// Gets a new instance of <see cref="OpenAIModelClient"/> that reuses the client configuration details provided to
    /// the <see cref="OpenAIClient"/> instance.
    /// </summary>
    /// <remarks>
    /// This method is functionally equivalent to using the <see cref="OpenAIModelClient"/> constructor directly with
    /// the same configuration details.
    /// </remarks>
    /// <returns> A new <see cref="OpenAIModelClient"/>. </returns>
    public virtual OpenAIModelClient GetOpenAIModelClient() => new(Pipeline, _options);

    /// <summary>
    /// Gets a new instance of <see cref="ModerationClient"/> that reuses the client configuration details provided to
    /// the <see cref="OpenAIClient"/> instance.
    /// </summary>
    /// <remarks>
    /// This method is functionally equivalent to using the <see cref="ModerationClient"/> constructor directly with
    /// the same configuration details.
    /// </remarks>
    /// <returns> A new <see cref="ModerationClient"/>. </returns>
    public virtual ModerationClient GetModerationClient(string model) => new(Pipeline, model, _options);

    /// <summary>
    /// Gets a new instance of <see cref="RealtimeClient"/>, used to initiate conversation and transcription sessions. 
    /// </summary>
    /// <remarks>
    /// <para>
    /// In contrast to other clients, <see cref="RealtimeClient"/> does not manage input/output operations directly,
    /// instead delegating stateful interaction to a child session object.
    /// </para>
    /// Create <see cref="IDisposable"/> instances of <see cref="RealtimeSession"/> using methods like
    /// <see cref="RealtimeClient.StartConversationSessionAsync(string, System.Threading.CancellationToken)"/> and
    /// <see cref="RealtimeClient.StartTranscriptionSessionAsync(System.Threading.CancellationToken)"/>
    /// to access full <c>/realtime</c> functionality.
    /// </remarks>
    /// <returns></returns>
    [Experimental("OPENAI002")]
    public virtual RealtimeClient GetRealtimeClient() => new(_keyCredential, _options);

    /// <summary>
    /// Gets a new instance of <see cref="OpenAIResponseClient"/> that reuses the client configuration details provided to
    /// the <see cref="OpenAIClient"/> instance.
    /// </summary>
    /// <remarks>
    /// This method is functionally equivalent to using the <see cref="OpenAIResponseClient"/> constructor directly with
    /// the same configuration details.
    /// </remarks>
    /// <returns> A new <see cref="OpenAIResponseClient"/>. </returns>
    [Experimental("OPENAI001")]
    public virtual OpenAIResponseClient GetOpenAIResponseClient(string model) => new(Pipeline, model, _options);

    /// <summary>
    /// Gets a new instance of <see cref="VectorStoreClient"/> that reuses the client configuration details provided to
    /// the <see cref="OpenAIClient"/> instance.
    /// </summary>
    /// <remarks>
    /// This method is functionally equivalent to using the <see cref="VectorStoreClient"/> constructor directly with
    /// the same configuration details.
    /// </remarks>
    /// <returns> A new <see cref="OpenAIModelClient"/>. </returns>
    [Experimental("OPENAI001")]
    public virtual VectorStoreClient GetVectorStoreClient() => new(Pipeline, _options);

    /// <summary>
    /// Gets a new instance of <see cref="GraderClient"/> that reuses the client configuration details provided to
    /// the <see cref="OpenAIClient"/> instance.
    /// </summary>
    /// <returns></returns>
    [Experimental("OPENAI001")]
    public virtual GraderClient GetGraderClient() => new(Pipeline, _options);

    /// <summary>
    /// Gets a new instance of <see cref="ContainerClient"/> that reuses the client configuration details provided to
    /// the <see cref="OpenAIClient"/> instance.
    /// </summary>
    /// <returns></returns>
    [Experimental("OPENAI001")]
    public virtual ContainerClient GetContainerClient() => new(Pipeline, _options);

    internal static ClientPipeline CreatePipeline(ApiKeyCredential credential, OpenAIClientOptions options)
    {
        return ClientPipeline.Create(
            options,
            perCallPolicies: [
                CreateAddCustomHeadersPolicy(options),
            ],
            perTryPolicies: [
                ApiKeyAuthenticationPolicy.CreateHeaderApiKeyPolicy(credential, AuthorizationHeader, AuthorizationApiKeyPrefix)
            ],
            beforeTransportPolicies: [
            ]);
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
