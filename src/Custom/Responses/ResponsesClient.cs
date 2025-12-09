using System;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace OpenAI.Responses;

// CUSTOM:
// - Renamed.
// - Suppressed GetResponse methods in favor of methods that take a property bag.
[CodeGenType("Responses")]
[CodeGenSuppress("GetResponse", typeof(string), typeof(IEnumerable<IncludedResponseProperty>), typeof(bool?), typeof(int?), typeof(bool?), typeof(CancellationToken))]
[CodeGenSuppress("GetResponseAsync", typeof(string), typeof(IEnumerable<IncludedResponseProperty>), typeof(bool?), typeof(int?), typeof(bool?), typeof(CancellationToken))]
[CodeGenSuppress("GetResponseInputItems", typeof(string), typeof(ResponseItemCollectionOptions), typeof(CancellationToken))]
[CodeGenSuppress("GetResponseInputItemsAsync", typeof(string), typeof(ResponseItemCollectionOptions), typeof(CancellationToken))]
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

    #region CreateResponse

    // CUSTOM: Added protocol model method.
    public virtual ClientResult<ResponseResult> CreateResponse(CreateResponseOptions options, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNull(options, nameof(options));

        if (options.StreamingEnabled == true)
        {
            throw new InvalidOperationException(
                $"{nameof(CreateResponseOptions.StreamingEnabled)} must not be set to true when calling {nameof(CreateResponse)}. "
                + $"For streaming scenarios, call {nameof(CreateResponseStreaming)} instead.");
        }

        ClientResult result = CreateResponse(options, cancellationToken.ToRequestOptions());
        return ClientResult.FromValue((ResponseResult)result, result.GetRawResponse());
    }

    // CUSTOM: Added protocol model method.
    public virtual Task<ClientResult<ResponseResult>> CreateResponseAsync(CreateResponseOptions options, CancellationToken cancellationToken = default)
    {
        return CreateResponseAsync(options, cancellationToken.ToRequestOptions() ?? new RequestOptions());
    }

    internal virtual async Task<ClientResult<ResponseResult>> CreateResponseAsync(CreateResponseOptions options, RequestOptions requestOptions)
    {
        Argument.AssertNotNull(options, nameof(options));
        Argument.AssertNotNull(requestOptions, nameof(requestOptions));

        if (options.StreamingEnabled == true)
        {
            throw new InvalidOperationException(
                $"{nameof(CreateResponseOptions.StreamingEnabled)} must not be set to true when calling {nameof(CreateResponseAsync)} "
                + $"For streaming scenarios, call {nameof(CreateResponseStreamingAsync)} instead.");
        }

        if (requestOptions.BufferResponse is false)
        {
            throw new InvalidOperationException($"{nameof(RequestOptions.BufferResponse)} must be set to true when calling {nameof(CreateResponseAsync)}.");
        }

        ClientResult result = await CreateResponseAsync((BinaryContent)options, requestOptions).ConfigureAwait(false);
        return ClientResult.FromValue((ResponseResult)result, result.GetRawResponse());
    }

    // CUSTOM: Added convenience method with no options.
    public virtual ClientResult<ResponseResult> CreateResponse(string model, IEnumerable<ResponseItem> inputItems, string previousResponseId = null, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNullOrEmpty(model, nameof(model));
        Argument.AssertNotNull(inputItems, nameof(inputItems));

        var options = new CreateResponseOptions
        {
            Model = model,
            PreviousResponseId = previousResponseId,
            StreamingEnabled = false,
        };

        foreach (var item in inputItems)
        {
            options.InputItems.Add(item);
        }

        return CreateResponse(options, cancellationToken);
    }

    // CUSTOM: Added convenience method with no options.
    public virtual Task<ClientResult<ResponseResult>> CreateResponseAsync(string model, IEnumerable<ResponseItem> inputItems, string previousResponseId = null, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNullOrEmpty(model, nameof(model));
        Argument.AssertNotNull(inputItems, nameof(inputItems));

        var options = new CreateResponseOptions
        {
            Model = model,
            PreviousResponseId = previousResponseId,
            StreamingEnabled = false,
        };

        foreach (var item in inputItems)
        {
            options.InputItems.Add(item);
        }

        return CreateResponseAsync(options, cancellationToken);
    }

    // CUSTOM: Added protocol model method.
    public virtual CollectionResult<StreamingResponseUpdate> CreateResponseStreaming(CreateResponseOptions options, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNull(options, nameof(options));

        if (options.StreamingEnabled != true)
        {
            throw new InvalidOperationException(
                $"{nameof(CreateResponseOptions.StreamingEnabled)} must be set to true when calling {nameof(CreateResponseStreaming)}. "
                + $"For non-streaming scenarios, call {nameof(CreateResponse)} instead.");
        }

        return new SseUpdateCollection<StreamingResponseUpdate>(
            () => CreateResponse(options, cancellationToken.ToRequestOptions(streaming: true)),
            StreamingResponseUpdate.DeserializeStreamingResponseUpdate,
            cancellationToken);
    }

    // CUSTOM: Added protocol model method.
    public virtual AsyncCollectionResult<StreamingResponseUpdate> CreateResponseStreamingAsync(CreateResponseOptions options, CancellationToken cancellationToken = default)
    {
        return CreateResponseStreamingAsync(options, cancellationToken.ToRequestOptions(streaming: true));
    }

    internal AsyncCollectionResult<StreamingResponseUpdate> CreateResponseStreamingAsync(CreateResponseOptions options, RequestOptions requestOptions)
    {
        Argument.AssertNotNull(options, nameof(options));
        Argument.AssertNotNull(requestOptions, nameof(requestOptions));

        if (options.StreamingEnabled != true)
        {
            throw new InvalidOperationException(
                $"{nameof(CreateResponseOptions.StreamingEnabled)} must be set to true when calling {nameof(CreateResponseStreamingAsync)}. "
                + $"For non-streaming scenarios, call {nameof(CreateResponseAsync)} instead.");
        }

        if (requestOptions.BufferResponse is true)
        {
            throw new InvalidOperationException($"{nameof(RequestOptions.BufferResponse)} must be set to false when calling {nameof(CreateResponseStreamingAsync)}.");
        }

        return new AsyncSseUpdateCollection<StreamingResponseUpdate>(
            async () => await CreateResponseAsync((BinaryContent)options, requestOptions).ConfigureAwait(false),
            StreamingResponseUpdate.DeserializeStreamingResponseUpdate,
            requestOptions.CancellationToken);
    }

    // CUSTOM: Added convenience method with no options.
    public virtual CollectionResult<StreamingResponseUpdate> CreateResponseStreaming(string model, IEnumerable<ResponseItem> inputItems, string previousResponseId = null, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNullOrEmpty(model, nameof(model));
        Argument.AssertNotNull(inputItems, nameof(inputItems));

        var options = new CreateResponseOptions
        {
            Model = model,
            PreviousResponseId = previousResponseId,
            StreamingEnabled = true,
        };

        foreach (var item in inputItems)
        {
            options.InputItems.Add(item);
        }

        return CreateResponseStreaming(options, cancellationToken);
    }

    // CUSTOM: Added convenience method with no options.
    public virtual AsyncCollectionResult<StreamingResponseUpdate> CreateResponseStreamingAsync(string model, IEnumerable<ResponseItem> inputItems, string previousResponseId = null, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNullOrEmpty(model, nameof(model));
        Argument.AssertNotNull(inputItems, nameof(inputItems));

        var options = new CreateResponseOptions
        {
            Model = model,
            PreviousResponseId = previousResponseId,
            StreamingEnabled = true,
        };

        foreach (var item in inputItems)
        {
            options.InputItems.Add(item);
        }

        return CreateResponseStreamingAsync(options, cancellationToken);
    }

    #endregion

    #region GetResponse

    // CUSTOM: Added protocol model method.
    public virtual ClientResult<ResponseResult> GetResponse(GetResponseOptions options, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNull(options, nameof(options));
        Argument.AssertNotNullOrEmpty(options.ResponseId, nameof(options.ResponseId));

        if (options.StreamingEnabled == true)
        {
            throw new InvalidOperationException(
                $"{nameof(GetResponseOptions.StreamingEnabled)} must not be set to true when calling {nameof(GetResponse)}. "
                + $"For streaming scenarios, call {nameof(GetResponseStreaming)} instead.");
        }

        ClientResult result = GetResponse(
            responseId: options.ResponseId,
            include: options.IncludedProperties,
            stream: options.StreamingEnabled,
            startingAfter: options.StartingAfter,
            includeObfuscation: options.IncludeObfuscation,
            cancellationToken.ToRequestOptions());
        return ClientResult.FromValue((ResponseResult)result, result.GetRawResponse());
    }

    // CUSTOM: Added protocol model method.
    public virtual Task<ClientResult<ResponseResult>> GetResponseAsync(GetResponseOptions options, CancellationToken cancellationToken = default)
    {
        return GetResponseAsync(options, cancellationToken.ToRequestOptions() ?? new RequestOptions());
    }

    internal async Task<ClientResult<ResponseResult>> GetResponseAsync(GetResponseOptions options, RequestOptions requestOptions)
    {
        Argument.AssertNotNull(options, nameof(options));
        Argument.AssertNotNullOrEmpty(options.ResponseId, nameof(options.ResponseId));
        Argument.AssertNotNull(requestOptions, nameof(requestOptions));

        if (options.StreamingEnabled == true)
        {
            throw new InvalidOperationException(
                $"{nameof(GetResponseOptions.StreamingEnabled)} must not be set to true when calling {nameof(GetResponseAsync)}. "
                + $"For streaming scenarios, call {nameof(GetResponseStreamingAsync)} instead.");
        }

        if (requestOptions.BufferResponse is false)
        {
            throw new InvalidOperationException($"{nameof(RequestOptions.BufferResponse)} must be set to true when calling {nameof(GetResponseAsync)}.");
        }

        ClientResult result = await GetResponseAsync(
            responseId: options.ResponseId,
            include: options.IncludedProperties,
            stream: options.StreamingEnabled,
            startingAfter: options.StartingAfter,
            includeObfuscation: options.IncludeObfuscation,
            requestOptions).ConfigureAwait(false);
        return ClientResult.FromValue((ResponseResult)result, result.GetRawResponse());
    }

    // CUSTOM: Added convenience method with no options.
    public virtual ClientResult<ResponseResult> GetResponse(string responseId, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNullOrEmpty(responseId, nameof(responseId));

        ClientResult result = GetResponse(responseId: responseId, include: null, stream: null, startingAfter: null, includeObfuscation: null, cancellationToken.ToRequestOptions());
        return ClientResult.FromValue((ResponseResult)result, result.GetRawResponse());
    }

    // CUSTOM: Added convenience method with no options.
    public virtual async Task<ClientResult<ResponseResult>> GetResponseAsync(string responseId, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNullOrEmpty(responseId, nameof(responseId));

        ClientResult result = await GetResponseAsync(responseId: responseId, include: null, stream: null, startingAfter: null, includeObfuscation: null, cancellationToken.ToRequestOptions()).ConfigureAwait(false);
        return ClientResult.FromValue((ResponseResult)result, result.GetRawResponse());
    }

    // CUSTOM: Added protocol model method.
    public virtual CollectionResult<StreamingResponseUpdate> GetResponseStreaming(GetResponseOptions options, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNull(options, nameof(options));
        Argument.AssertNotNullOrEmpty(options.ResponseId, nameof(options.ResponseId));

        if (options.StreamingEnabled != true)
        {
            throw new InvalidOperationException(
                $"{nameof(GetResponseOptions.StreamingEnabled)} must be set to true when calling {nameof(GetResponseStreaming)}. "
                + $"For non-streaming scenarios, call {nameof(GetResponse)} instead.");
        }

        return new SseUpdateCollection<StreamingResponseUpdate>(
            () => GetResponse(
                responseId: options.ResponseId,
                include: options.IncludedProperties,
                stream: options.StreamingEnabled,
                startingAfter: options.StartingAfter,
                includeObfuscation: options.IncludeObfuscation,
                cancellationToken.ToRequestOptions(streaming: true)),
            StreamingResponseUpdate.DeserializeStreamingResponseUpdate,
            cancellationToken);
    }

    // CUSTOM: Added protocol model method.
    public virtual AsyncCollectionResult<StreamingResponseUpdate> GetResponseStreamingAsync(GetResponseOptions options, CancellationToken cancellationToken = default)
    {
        return GetResponseStreamingAsync(options, cancellationToken.ToRequestOptions(streaming: true));
    }

    internal AsyncCollectionResult<StreamingResponseUpdate> GetResponseStreamingAsync(GetResponseOptions options, RequestOptions requestOptions)
    {
        Argument.AssertNotNull(options, nameof(options));
        Argument.AssertNotNullOrEmpty(options.ResponseId, nameof(options.ResponseId));
        Argument.AssertNotNull(requestOptions, nameof(requestOptions));

        if (options.StreamingEnabled != true)
        {
            throw new InvalidOperationException(
                $"{nameof(GetResponseOptions.StreamingEnabled)} must be set to true when calling {nameof(GetResponseStreamingAsync)}. "
                + $"For non-streaming scenarios, call {nameof(GetResponseAsync)} instead.");
        }

        if (requestOptions.BufferResponse is true)
        {
            throw new InvalidOperationException($"{nameof(RequestOptions.BufferResponse)} must be set to false when calling {nameof(GetResponseStreamingAsync)}.");
        }

        return new AsyncSseUpdateCollection<StreamingResponseUpdate>(
            async () => await GetResponseAsync(
                responseId: options.ResponseId,
                include: options.IncludedProperties,
                stream: options.StreamingEnabled,
                startingAfter: options.StartingAfter,
                includeObfuscation: options.IncludeObfuscation,
                requestOptions).ConfigureAwait(false),
            StreamingResponseUpdate.DeserializeStreamingResponseUpdate,
            requestOptions.CancellationToken);
    }

    // CUSTOM: Added convenience method with no options.
    public virtual CollectionResult<StreamingResponseUpdate> GetResponseStreaming(string responseId, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNullOrEmpty(responseId, nameof(responseId));

        return new SseUpdateCollection<StreamingResponseUpdate>(
            () => GetResponse(responseId: responseId, include: null, stream: true, startingAfter: null, includeObfuscation: null, cancellationToken.ToRequestOptions(streaming: true)),
            StreamingResponseUpdate.DeserializeStreamingResponseUpdate,
            cancellationToken);
    }

    // CUSTOM: Added convenience method with no options.
    public virtual AsyncCollectionResult<StreamingResponseUpdate> GetResponseStreamingAsync(string responseId, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNullOrEmpty(responseId, nameof(responseId));

        return new AsyncSseUpdateCollection<StreamingResponseUpdate>(
            async () => await GetResponseAsync(responseId: responseId, include: null, stream: true, startingAfter: null, includeObfuscation: null, cancellationToken.ToRequestOptions()).ConfigureAwait(false),
            StreamingResponseUpdate.DeserializeStreamingResponseUpdate,
            cancellationToken);
    }

    #endregion

    #region GetResponseInputItems

    public virtual ClientResult GetResponseInputItems(string responseId, int? limit, string order, string after, string before, RequestOptions options)
    {
        Argument.AssertNotNullOrEmpty(responseId, nameof(responseId));

        using PipelineMessage message = CreateGetResponseInputItemsRequest(responseId, limit, after, order, before, options);
        return ClientResult.FromResponse(Pipeline.ProcessMessage(message, options));
    }

    public virtual async Task<ClientResult> GetResponseInputItemsAsync(string responseId, int? limit, string order, string after, string before, RequestOptions options)
    {
        Argument.AssertNotNullOrEmpty(responseId, nameof(responseId));

        using PipelineMessage message = CreateGetResponseInputItemsRequest(responseId, limit, after, order, before, options);
        return ClientResult.FromResponse(await Pipeline.ProcessMessageAsync(message, options).ConfigureAwait(false));
    }

    public virtual ClientResult<ResponseItemCollectionPage> GetResponseInputItemCollectionPage(ResponseItemCollectionOptions options, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNull(options, nameof(options));
        Argument.AssertNotNullOrEmpty(options.ResponseId, nameof(options.ResponseId));

        PipelineMessage message = CreateGetResponseInputItemsRequest(options.ResponseId, options.PageSizeLimit, options.AfterId, options.Order.ToString(), options.BeforeId, cancellationToken.ToRequestOptions());
        ClientResult result = ClientResult.FromResponse(Pipeline.ProcessMessage(message, cancellationToken.ToRequestOptions()));
        return ClientResult.FromValue((ResponseItemCollectionPage)result, result.GetRawResponse());
    }

    public virtual async Task<ClientResult<ResponseItemCollectionPage>> GetResponseInputItemCollectionPageAsync(ResponseItemCollectionOptions options, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNull(options, nameof(options));
        Argument.AssertNotNullOrEmpty(options.ResponseId, nameof(options.ResponseId));

        PipelineMessage message = CreateGetResponseInputItemsRequest(options.ResponseId, options.PageSizeLimit, options.AfterId, options.Order.ToString(), options.BeforeId, cancellationToken.ToRequestOptions());
        ClientResult result = ClientResult.FromResponse(await Pipeline.ProcessMessageAsync(message, cancellationToken.ToRequestOptions()).ConfigureAwait(false));
        return ClientResult.FromValue((ResponseItemCollectionPage)result, result.GetRawResponse());
    }

    public virtual CollectionResult<ResponseItem> GetResponseInputItems(ResponseItemCollectionOptions options, CancellationToken cancellationToken = default)
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

    #endregion
}