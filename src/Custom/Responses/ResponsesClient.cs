using System;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace OpenAI.Responses;

// CUSTOM:
// - Added Experimental attribute.
// - Renamed.
[CodeGenType("Responses")]
[CodeGenSuppress("DeleteResponse", typeof(string), typeof(string), typeof(CancellationToken))]
[CodeGenSuppress("DeleteResponseAsync", typeof(string), typeof(string), typeof(CancellationToken))]
[CodeGenSuppress("CancelResponse", typeof(string), typeof(IEnumerable<IncludedResponseProperty>), typeof(bool?), typeof(int?), typeof(CancellationToken))]
[CodeGenSuppress("CancelResponseAsync", typeof(string), typeof(IEnumerable<IncludedResponseProperty>), typeof(bool?), typeof(int?), typeof(CancellationToken))]
[CodeGenSuppress("GetResponse", typeof(string), typeof(IEnumerable<IncludedResponseProperty>), typeof(bool?), typeof(int?), typeof(bool?), typeof(CancellationToken))]
[CodeGenSuppress("GetResponseAsync", typeof(string), typeof(IEnumerable<IncludedResponseProperty>), typeof(bool?), typeof(int?), typeof(bool?), typeof(CancellationToken))]
[CodeGenSuppress("GetResponseInputItems", typeof(ResponseItemCollectionOptions), typeof(CancellationToken))]
[CodeGenSuppress("GetResponseInputItems", typeof(ResponseItemCollectionOptions), typeof(CancellationToken))]
[CodeGenSuppress("GetResponseInputItems", typeof(string), typeof(int?), typeof(string), typeof(string), typeof(string), typeof(RequestOptions))]
[CodeGenSuppress("GetResponseInputItemsAsync", typeof(string), typeof(int?), typeof(string), typeof(string), typeof(string), typeof(RequestOptions))]

public partial class ResponsesClient
{
    // CUSTOM: Added as a convenience.
    /// <summary> Initializes a new instance of <see cref="ResponsesClient"/>. </summary>
    /// <param name="apiKey"> The API key to authenticate with the service. </param>
    public ResponsesClient(string apiKey) : this(new ApiKeyCredential(apiKey), new OpenAIClientOptions())
    {
    }

    // CUSTOM:
    // - Added `model` parameter.
    // - Used a custom pipeline.
    // - Demoted the endpoint parameter to be a property in the options class.
    /// <summary> Initializes a new instance of <see cref="ResponsesClient"/>. </summary>
    /// <param name="credential"> The <see cref="ApiKeyCredential"/> to authenticate with the service. </param>
    public ResponsesClient(ApiKeyCredential credential) : this(credential, new OpenAIClientOptions())
    {
    }

    // CUSTOM:
    // - Added `model` parameter.
    // - Used a custom pipeline.
    // - Demoted the endpoint parameter to be a property in the options class.
    /// <summary> Initializes a new instance of <see cref="ResponsesClient"/>. </summary>
    /// <param name="credential"> The <see cref="ApiKeyCredential"/> to authenticate with the service. </param>
    /// <param name="options"> The options to configure the client. </param>
    public ResponsesClient(ApiKeyCredential credential, OpenAIClientOptions options) : this(OpenAIClient.CreateApiKeyAuthenticationPolicy(credential), options)
    {
    }

    // CUSTOM: Added as a convenience.
    /// <summary> Initializes a new instance of <see cref="ResponsesClient"/>. </summary>
    /// <param name="authenticationPolicy"> The authentication policy used to authenticate with the service. </param>
    public ResponsesClient(AuthenticationPolicy authenticationPolicy) : this(authenticationPolicy, new OpenAIClientOptions())
    {
    }

    // CUSTOM: Added as a convenience.
    /// <summary> Initializes a new instance of <see cref="ResponsesClient"/>. </summary>
    /// <param name="authenticationPolicy"> The authentication policy used to authenticate with the service. </param>
    /// <param name="options"> The options to configure the client. </param>
    /// <exception cref="ArgumentNullException"><paramref name="authenticationPolicy"/> is null. </exception>
    public ResponsesClient(AuthenticationPolicy authenticationPolicy, OpenAIClientOptions options)
    {
        Argument.AssertNotNull(authenticationPolicy, nameof(authenticationPolicy));
        options ??= new OpenAIClientOptions();

        Pipeline = OpenAIClient.CreatePipeline(authenticationPolicy, options);
        _endpoint = OpenAIClient.GetEndpoint(options);
    }

    // CUSTOM:
    // - Added `model` parameter.
    // - Used a custom pipeline.
    // - Demoted the endpoint parameter to be a property in the options class.
    // - Made protected.
    /// <summary> Initializes a new instance of <see cref="ResponsesClient"/>. </summary>
    /// <param name="pipeline"> The HTTP pipeline to send and receive REST requests and responses. </param>
    /// <param name="options"> The options to configure the client. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="pipeline"/> is null. </exception>
    protected internal ResponsesClient(ClientPipeline pipeline, OpenAIClientOptions options)
    {
        Argument.AssertNotNull(pipeline, nameof(pipeline));
        options ??= new OpenAIClientOptions();

        Pipeline = pipeline;
        _endpoint = OpenAIClient.GetEndpoint(options);
    }

    /// <summary>
    /// Gets the endpoint URI for the service.
    /// </summary>
    [Experimental("OPENAI001")]
    public virtual Uri Endpoint => _endpoint;

    public virtual ClientResult<ResponseResult> CreateResponse(CreateResponseOptions options, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNull(options, nameof(options));
        if (options.StreamingEnabled is true)
        {
            throw new InvalidOperationException("'options.StreamingEnabled' must be 'false' when calling 'CreateResponse'.");
        }

        ClientResult result = this.CreateResponse(CreatePerCallOptions(options), cancellationToken.CanBeCanceled ? new RequestOptions { CancellationToken = cancellationToken } : null);
        return ClientResult.FromValue((ResponseResult)result.GetRawResponse().Content, result.GetRawResponse());
    }

    public virtual ClientResult<ResponseResult> CreateResponse(string model, IEnumerable<ResponseItem> inputItems, string previousResponseId = null, string conversationId = null, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNullOrEmpty(model, nameof(model));
        Argument.AssertNotNull(inputItems, nameof(inputItems));

        var options = new CreateResponseOptions
        {
            Model = model,
            PreviousResponseId = previousResponseId,
            ConversationId = conversationId,
            StreamingEnabled = false,
        };

        foreach (var item in inputItems)
        {
            options.InputItems.Add(item);
        }

        ClientResult result = this.CreateResponse(options, cancellationToken.CanBeCanceled ? new RequestOptions { CancellationToken = cancellationToken } : null);
        return ClientResult.FromValue((ResponseResult)result.GetRawResponse().Content, result.GetRawResponse());
    }

    public virtual Task<ClientResult<ResponseResult>> CreateResponseAsync(string model, IEnumerable<ResponseItem> inputItems, string previousResponseId = null, string conversationId = null, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNullOrEmpty(model, nameof(model));
        Argument.AssertNotNull(inputItems, nameof(inputItems));

        var options = new CreateResponseOptions
        {
            Model = model,
            PreviousResponseId = previousResponseId,
            ConversationId = conversationId,
            StreamingEnabled = false,
        };

        foreach (var item in inputItems)
        {
            options.InputItems.Add(item);
        }
        ClientResult result = this.CreateResponse(options, cancellationToken.CanBeCanceled ? new RequestOptions { CancellationToken = cancellationToken } : null);
        return Task.FromResult(ClientResult.FromValue((ResponseResult)result.GetRawResponse().Content, result.GetRawResponse()));
    }

    public virtual async Task<ClientResult<ResponseResult>> CreateResponseAsync(CreateResponseOptions options, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNull(options, nameof(options));

        ClientResult result = await this.CreateResponseAsync(CreatePerCallOptions(options), cancellationToken.CanBeCanceled ? new RequestOptions { CancellationToken = cancellationToken } : null).ConfigureAwait(false);
        return ClientResult.FromValue((ResponseResult)result.GetRawResponse().Content, result.GetRawResponse());
    }

    public virtual AsyncCollectionResult<StreamingResponseUpdate> CreateResponseStreamingAsync(CreateResponseOptions options, CancellationToken cancellationToken = default)
    {
        return CreateResponseStreamingAsync(CreatePerCallOptions(options, true), cancellationToken.ToRequestOptions(streaming: true));
    }

    public virtual CollectionResult<StreamingResponseUpdate> CreateResponseStreaming(string model, IEnumerable<ResponseItem> inputItems, string previousResponseId = null, string conversationId = null, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNullOrEmpty(model, nameof(model));
        Argument.AssertNotNull(inputItems, nameof(inputItems));

        var options = new CreateResponseOptions
        {
            Model = model,
            PreviousResponseId = previousResponseId,
            ConversationId = conversationId,
            StreamingEnabled = true,
        };

        foreach (var item in inputItems)
        {
            options.InputItems.Add(item);
        }

        return CreateResponseStreaming(options, cancellationToken);
    }

    public virtual AsyncCollectionResult<StreamingResponseUpdate> CreateResponseStreamingAsync(string model, IEnumerable<ResponseItem> inputItems, string previousResponseId = null, string conversationId = null, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNullOrEmpty(model, nameof(model));
        Argument.AssertNotNull(inputItems, nameof(inputItems));

        var options = new CreateResponseOptions
        {
            Model = model,
            PreviousResponseId = previousResponseId,
            ConversationId = conversationId,
            StreamingEnabled = true,
        };

        foreach (var item in inputItems)
        {
            options.InputItems.Add(item);
        }

        return CreateResponseStreamingAsync(options, cancellationToken.ToRequestOptions(streaming: true));
    }

    internal AsyncCollectionResult<StreamingResponseUpdate> CreateResponseStreamingAsync(CreateResponseOptions options, RequestOptions requestOptions)
    {
        Argument.AssertNotNull(options, nameof(options));
        Argument.AssertNotNull(requestOptions, nameof(requestOptions));
        if (requestOptions.BufferResponse is true)
        {
            throw new InvalidOperationException("'requestOptions.BufferResponse' must be 'false' when calling 'CreateResponseStreamingAsync'.");
        }

        return new AsyncSseUpdateCollection<StreamingResponseUpdate>(
            async () => await CreateResponseAsync(options, requestOptions).ConfigureAwait(false),
            StreamingResponseUpdate.DeserializeStreamingResponseUpdate,
            requestOptions.CancellationToken);
    }

    public virtual CollectionResult<StreamingResponseUpdate> CreateResponseStreaming(CreateResponseOptions options, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNull(options, nameof(options));
        if (options.StreamingEnabled is false)
        {
            throw new InvalidOperationException("'options.StreamingEnabled' must be 'true' when calling 'CreateResponseStreaming'.");
        }

        return new SseUpdateCollection<StreamingResponseUpdate>(
            () => CreateResponse(CreatePerCallOptions(options, true), cancellationToken.ToRequestOptions(streaming: true)),
            StreamingResponseUpdate.DeserializeStreamingResponseUpdate,
            cancellationToken);
    }

    internal async Task<ClientResult<ResponseResult>> GetResponseAsync(string responseId, IEnumerable<IncludedResponseProperty> include, RequestOptions requestOptions)
    {
        Argument.AssertNotNullOrEmpty(responseId, nameof(responseId));
        Argument.AssertNotNull(requestOptions, nameof(requestOptions));
        if (requestOptions.BufferResponse is false)
        {
            throw new InvalidOperationException("'requestOptions.BufferResponse' must be 'true' when calling 'GetResponseAsync'.");
        }

        ClientResult protocolResult = await GetResponseAsync(responseId, include, stream: null, startingAfter: null, includeObfuscation: null, requestOptions).ConfigureAwait(false);
        ResponseResult convenienceResult = (ResponseResult)protocolResult;
        return ClientResult.FromValue(convenienceResult, protocolResult.GetRawResponse());
    }

    public virtual ClientResult<ResponseResult> GetResponse(string responseId, CancellationToken cancellationToken = default)
    {
        var response = GetResponse(responseId, Enumerable.Empty<IncludedResponseProperty>(), null, null, null, cancellationToken.ToRequestOptions());
        return ClientResult.FromValue((ResponseResult)response, response.GetRawResponse());
    }

    public virtual async Task<ClientResult<ResponseResult>> GetResponseAsync(string responseId, CancellationToken cancellationToken = default)
    {
        var response = await GetResponseAsync(responseId, Enumerable.Empty<IncludedResponseProperty>(), null, null, null, cancellationToken.ToRequestOptions()).ConfigureAwait(false);
        return ClientResult.FromValue((ResponseResult)response, response.GetRawResponse());
    }

    public virtual async Task<ClientResult<ResponseResult>> GetResponseAsync(GetResponseOptions options, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNull(options, nameof(options));
        Argument.AssertNotNullOrEmpty(options.ResponseId, nameof(options.ResponseId));

        ClientResult protocolResult = await GetResponseAsync(options.ResponseId, options.IncludedProperties, stream: options.StreamingEnabled, startingAfter: options.StartingAfter, includeObfuscation: options.IncludeObfuscation, cancellationToken.ToRequestOptions()).ConfigureAwait(false);
        return ClientResult.FromValue((ResponseResult)protocolResult, protocolResult.GetRawResponse());
    }

    public virtual ClientResult<ResponseResult> GetResponse(GetResponseOptions options, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNull(options, nameof(options));
        Argument.AssertNotNullOrEmpty(options.ResponseId, nameof(options.ResponseId));

        ClientResult protocolResult = GetResponse(options.ResponseId, options.IncludedProperties, stream: options.StreamingEnabled, startingAfter: options.StartingAfter, includeObfuscation: options.IncludeObfuscation, cancellationToken.ToRequestOptions());
        return ClientResult.FromValue((ResponseResult)protocolResult, protocolResult.GetRawResponse());
    }

    internal virtual AsyncCollectionResult<StreamingResponseUpdate> GetResponseStreamingAsync(string responseId, int? startingAfter = null, CancellationToken cancellationToken = default)
    {
        return GetResponseStreamingAsync(responseId, startingAfter, cancellationToken);
    }

    public virtual CollectionResult<StreamingResponseUpdate> GetResponseStreaming(GetResponseOptions options, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNull(options, nameof(options));
        Argument.AssertNotNullOrEmpty(options.ResponseId, nameof(options.ResponseId));

        return new SseUpdateCollection<StreamingResponseUpdate>(
            () => GetResponse(options.ResponseId, options.IncludedProperties, stream: true, startingAfter: options.StartingAfter, includeObfuscation: options.IncludeObfuscation, cancellationToken.ToRequestOptions(streaming: true)),
            StreamingResponseUpdate.DeserializeStreamingResponseUpdate,
            cancellationToken);
    }

    public virtual CollectionResult<StreamingResponseUpdate> GetResponseStreaming(string responseId, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNull(responseId, nameof(responseId));

        return new SseUpdateCollection<StreamingResponseUpdate>(
            () => GetResponse(responseId, default, stream: true, startingAfter: default, includeObfuscation: default, cancellationToken.ToRequestOptions(streaming: true)),
            StreamingResponseUpdate.DeserializeStreamingResponseUpdate,
            cancellationToken);
    }

    public virtual AsyncCollectionResult<StreamingResponseUpdate> GetResponseStreamingAsync(GetResponseOptions options, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNull(options, nameof(options));
        Argument.AssertNotNullOrEmpty(options.ResponseId, nameof(options.ResponseId));

        if (options.StreamingEnabled is true)
        {
            throw new InvalidOperationException("'options.Stream' must be 'true' when calling 'GetResponseStreamingAsync'.");
        }

        return new AsyncSseUpdateCollection<StreamingResponseUpdate>(
            async () => await GetResponseAsync(options.ResponseId, options.IncludedProperties, options.StreamingEnabled, startingAfter: options.StartingAfter, includeObfuscation: options.IncludeObfuscation, cancellationToken.ToRequestOptions()).ConfigureAwait(false),
            StreamingResponseUpdate.DeserializeStreamingResponseUpdate,
            cancellationToken);
    }

    public virtual AsyncCollectionResult<StreamingResponseUpdate> GetResponseStreamingAsync(string responseId, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNull(responseId, nameof(responseId));

        return new AsyncSseUpdateCollection<StreamingResponseUpdate>(
            async () => await GetResponseAsync(responseId, default, stream: true, startingAfter: default, includeObfuscation: default, cancellationToken.ToRequestOptions()).ConfigureAwait(false),
            StreamingResponseUpdate.DeserializeStreamingResponseUpdate,
            cancellationToken);
    }

    public virtual async Task<ClientResult<ResponseDeletionResult>> DeleteResponseAsync(string responseId, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNullOrEmpty(responseId, nameof(responseId));

        ClientResult result = await DeleteResponseAsync(responseId, cancellationToken.CanBeCanceled ? new RequestOptions { CancellationToken = cancellationToken } : null).ConfigureAwait(false);
        return ClientResult.FromValue((ResponseDeletionResult)result, result.GetRawResponse());
    }

    public virtual ClientResult<ResponseDeletionResult> DeleteResponse(string responseId, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNullOrEmpty(responseId, nameof(responseId));

        ClientResult result = DeleteResponse(responseId, cancellationToken.CanBeCanceled ? new RequestOptions { CancellationToken = cancellationToken } : null);
        return ClientResult.FromValue((ResponseDeletionResult)result, result.GetRawResponse());
    }

    public virtual async Task<ClientResult<ResponseResult>> CancelResponseAsync(string responseId, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNullOrEmpty(responseId, nameof(responseId));

        ClientResult protocolResult = await CancelResponseAsync(responseId, cancellationToken.ToRequestOptions()).ConfigureAwait(false);
        ResponseResult convenienceResult = (ResponseResult)protocolResult;
        return ClientResult.FromValue(convenienceResult, protocolResult.GetRawResponse());
    }

    public virtual ClientResult<ResponseResult> CancelResponse(string responseId, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNullOrEmpty(responseId, nameof(responseId));

        ClientResult protocolResult = CancelResponse(responseId, cancellationToken.ToRequestOptions());
        ResponseResult convenienceResult = (ResponseResult)protocolResult;
        return ClientResult.FromValue(convenienceResult, protocolResult.GetRawResponse());
    }

    public virtual ClientResult GetResponseInputItems(string responseId, int? limit, string order, string after, string before, RequestOptions options)
    {
        Argument.AssertNotNullOrEmpty(responseId, nameof(responseId));

        PipelineMessage message = CreateGetResponseInputItemsRequest(responseId, limit, after, order, before, options);
        return ClientResult.FromResponse(Pipeline.ProcessMessage(message, options));
    }

    public virtual async Task<ClientResult> GetResponseInputItemsAsync(string responseId, int? limit, string order, string after, string before, RequestOptions options)
    {
        Argument.AssertNotNullOrEmpty(responseId, nameof(responseId));

        PipelineMessage message = CreateGetResponseInputItemsRequest(responseId, limit, after, order, before, options);
        return ClientResult.FromResponse(await Pipeline.ProcessMessageAsync(message, options).ConfigureAwait(false));
    }

    public virtual ClientResult<ResponseItemCollectionPage> GetResponseInputItemCollectionPage(ResponseItemCollectionOptions options = default, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNull(options, nameof(options));
        Argument.AssertNotNullOrEmpty(options.ResponseId, nameof(options.ResponseId));

        PipelineMessage message = CreateGetResponseInputItemsRequest(options.ResponseId, options.PageSizeLimit, options.AfterId, options.Order.ToString(), options.BeforeId, cancellationToken.ToRequestOptions());
        ClientResult result = ClientResult.FromResponse(Pipeline.ProcessMessage(message, cancellationToken.ToRequestOptions()));
        return ClientResult.FromValue((ResponseItemCollectionPage)result, result.GetRawResponse());
    }

    public virtual async Task<ClientResult<ResponseItemCollectionPage>> GetResponseInputItemCollectionPageAsync(ResponseItemCollectionOptions options = default, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNull(options, nameof(options));
        Argument.AssertNotNullOrEmpty(options.ResponseId, nameof(options.ResponseId));

        PipelineMessage message = CreateGetResponseInputItemsRequest(options.ResponseId, options.PageSizeLimit, options.AfterId, options.Order.ToString(), options.BeforeId, cancellationToken.ToRequestOptions());
        ClientResult result = ClientResult.FromResponse(await Pipeline.ProcessMessageAsync(message, cancellationToken.ToRequestOptions()).ConfigureAwait(false));
        return ClientResult.FromValue((ResponseItemCollectionPage)result, result.GetRawResponse());
    }

    public virtual CollectionResult<ResponseItem> GetResponseInputItems(ResponseItemCollectionOptions options = default, CancellationToken cancellationToken = default)
        {
            Argument.AssertNotNullOrEmpty(options.ResponseId, nameof(options.ResponseId));

            return new ResponsesClientGetResponseInputItemsCollectionResultOfT(
                this,
                options.ResponseId,
                options?.PageSizeLimit,
                options?.Order?.ToString(),
                options?.AfterId,
                options?.BeforeId,
                cancellationToken.ToRequestOptions());
        }

        public virtual AsyncCollectionResult<ResponseItem> GetResponseInputItemsAsync(ResponseItemCollectionOptions options, CancellationToken cancellationToken = default)
        {
            Argument.AssertNotNullOrEmpty(options.ResponseId, nameof(options.ResponseId));

            return new ResponsesClientGetResponseInputItemsAsyncCollectionResultOfT(
                this,
                options.ResponseId,
                options?.PageSizeLimit,
                options?.Order?.ToString(),
                options?.AfterId,
                options?.BeforeId,
                cancellationToken.ToRequestOptions());
        }

    internal virtual CreateResponseOptions CreatePerCallOptions(CreateResponseOptions userOptions, bool stream = false)
    {
        CreateResponseOptions copiedOptions = userOptions is null
            ? new()
            : userOptions.GetClone();

        if (stream)
        {
            copiedOptions.StreamingEnabled = true;
        }

        return copiedOptions;
    }
}