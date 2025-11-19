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
[CodeGenSuppress("CreateResponseAsync", typeof(ResponseCreationOptions), typeof(CancellationToken))]
[CodeGenSuppress("CreateResponse", typeof(ResponseCreationOptions), typeof(CancellationToken))]
[CodeGenSuppress("GetResponse", typeof(string), typeof(string), typeof(IEnumerable<InternalIncludable>), typeof(bool?), typeof(int?), typeof(CancellationToken))]
[CodeGenSuppress("GetResponseAsync", typeof(string), typeof(string), typeof(IEnumerable<InternalIncludable>), typeof(bool?), typeof(int?), typeof(CancellationToken))]
[CodeGenSuppress("DeleteResponse", typeof(string), typeof(string), typeof(CancellationToken))]
[CodeGenSuppress("DeleteResponseAsync", typeof(string), typeof(string), typeof(CancellationToken))]
[CodeGenSuppress("CancelResponse", typeof(string), typeof(IEnumerable<InternalIncludable>), typeof(bool?), typeof(int?), typeof(CancellationToken))]
[CodeGenSuppress("CancelResponseAsync", typeof(string), typeof(IEnumerable<InternalIncludable>), typeof(bool?), typeof(int?), typeof(CancellationToken))]
[CodeGenSuppress("GetResponse", typeof(string), typeof(IEnumerable<InternalIncludable>), typeof(bool?), typeof(int?), typeof(CancellationToken))]
[CodeGenSuppress("GetResponseAsync", typeof(string), typeof(IEnumerable<InternalIncludable>), typeof(bool?), typeof(int?), typeof(CancellationToken))]
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

    internal virtual Task<ClientResult<OpenAIResponse>> CreateResponseAsync(IEnumerable<ResponseItem> inputItems, string model, ResponseCreationOptions options = null, CancellationToken cancellationToken = default)
    {
        return CreateResponseAsync(inputItems, model, options, cancellationToken.ToRequestOptions() ?? new RequestOptions());
    }

    internal async Task<ClientResult<OpenAIResponse>> CreateResponseAsync(IEnumerable<ResponseItem> inputItems, string model, ResponseCreationOptions options, RequestOptions requestOptions)
    {
        Argument.AssertNotNullOrEmpty(inputItems, nameof(inputItems));
        Argument.AssertNotNull(requestOptions, nameof(requestOptions));
        if (requestOptions.BufferResponse is false)
        {
            throw new InvalidOperationException("'requestOptions.BufferResponse' must be 'true' when calling 'CreateResponseAsync'.");
        }

        using BinaryContent content = CreatePerCallOptions(options, inputItems, model, stream: false).ToBinaryContent();
        ClientResult protocolResult = await CreateResponseAsync(content, requestOptions).ConfigureAwait(false);
        OpenAIResponse convenienceValue = (OpenAIResponse)protocolResult;
        return ClientResult.FromValue(convenienceValue, protocolResult.GetRawResponse());
    }

    internal virtual ClientResult<OpenAIResponse> CreateResponse(IEnumerable<ResponseItem> inputItems, string model, ResponseCreationOptions options = null, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNullOrEmpty(inputItems, nameof(inputItems));

        using BinaryContent content = CreatePerCallOptions(options, inputItems, model, stream: false).ToBinaryContent();
        ClientResult protocolResult = CreateResponse(content, cancellationToken.ToRequestOptions());
        OpenAIResponse convenienceValue = (OpenAIResponse)protocolResult;
        return ClientResult.FromValue(convenienceValue, protocolResult.GetRawResponse());
    }

    internal virtual async Task<ClientResult<OpenAIResponse>> CreateResponseAsync(string userInputText, string model, ResponseCreationOptions options = null, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNullOrEmpty(userInputText, nameof(userInputText));

        return await CreateResponseAsync(
            [ResponseItem.CreateUserMessageItem(userInputText)],
            model,
            options,
            cancellationToken)
                .ConfigureAwait(false);
    }

    internal virtual ClientResult<OpenAIResponse> CreateResponse(string userInputText, string model, ResponseCreationOptions options = null, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNullOrEmpty(userInputText, nameof(userInputText));

        return CreateResponse(
            [ResponseItem.CreateUserMessageItem(userInputText)],
            model,
            options,
            cancellationToken);
    }

    public virtual ClientResult<ResponseResult> CreateResponse(CreateResponseOptions options, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNull(options, nameof(options));

        ClientResult result = this.CreateResponse(CreatePerCallOptions(options), cancellationToken.CanBeCanceled ? new RequestOptions { CancellationToken = cancellationToken } : null);
        return ClientResult.FromValue((ResponseResult)result.GetRawResponse().Content, result.GetRawResponse());
    }

    public virtual async Task<ClientResult<ResponseResult>> CreateResponseAsync(CreateResponseOptions options, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNull(options, nameof(options));

        ClientResult result = await this.CreateResponseAsync(CreatePerCallOptions(options), cancellationToken.CanBeCanceled ? new RequestOptions { CancellationToken = cancellationToken } : null).ConfigureAwait(false);
        return ClientResult.FromValue((ResponseResult)result.GetRawResponse().Content, result.GetRawResponse());
    }

    internal virtual AsyncCollectionResult<StreamingResponseUpdate> CreateResponseStreamingAsync(IEnumerable<ResponseItem> inputItems, string model, ResponseCreationOptions options = null, CancellationToken cancellationToken = default)
    {
        return CreateResponseStreamingAsync(inputItems, model, options, cancellationToken.ToRequestOptions(streaming: true));
    }

    internal AsyncCollectionResult<StreamingResponseUpdate> CreateResponseStreamingAsync(IEnumerable<ResponseItem> inputItems, string model, ResponseCreationOptions options, RequestOptions requestOptions)
    {
        Argument.AssertNotNullOrEmpty(inputItems, nameof(inputItems));
        Argument.AssertNotNull(requestOptions, nameof(requestOptions));
        if (requestOptions.BufferResponse is true)
        {
            throw new InvalidOperationException("'requestOptions.BufferResponse' must be 'false' when calling 'CreateResponseStreamingAsync'.");
        }

        using BinaryContent content = CreatePerCallOptions(options, inputItems, model, stream: true).ToBinaryContent();
        return new AsyncSseUpdateCollection<StreamingResponseUpdate>(
            async () => await CreateResponseAsync(content, requestOptions).ConfigureAwait(false),
            StreamingResponseUpdate.DeserializeStreamingResponseUpdate,
            requestOptions.CancellationToken);
    }

    public virtual AsyncCollectionResult<StreamingResponseUpdate> CreateResponseStreamingAsync(CreateResponseOptions options, CancellationToken cancellationToken = default)
    {
        return CreateResponseStreamingAsync(CreatePerCallOptions(options, true), cancellationToken.ToRequestOptions(streaming: true));
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

    internal virtual CollectionResult<StreamingResponseUpdate> CreateResponseStreaming(IEnumerable<ResponseItem> inputItems, string model, ResponseCreationOptions options = null, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNullOrEmpty(inputItems, nameof(inputItems));

        using BinaryContent content = CreatePerCallOptions(options, inputItems, model, stream: true).ToBinaryContent();
        return new SseUpdateCollection<StreamingResponseUpdate>(
            () => CreateResponse(content, cancellationToken.ToRequestOptions(streaming: true)),
            StreamingResponseUpdate.DeserializeStreamingResponseUpdate,
            cancellationToken);
    }

    public virtual CollectionResult<StreamingResponseUpdate> CreateResponseStreaming(CreateResponseOptions options, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNull(options, nameof(options));

        return new SseUpdateCollection<StreamingResponseUpdate>(
            () => CreateResponse(CreatePerCallOptions(options, true), cancellationToken.ToRequestOptions(streaming: true)),
            StreamingResponseUpdate.DeserializeStreamingResponseUpdate,
            cancellationToken);
    }

    internal virtual AsyncCollectionResult<StreamingResponseUpdate> CreateResponseStreamingAsync(string userInputText, string model, ResponseCreationOptions options = null, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNullOrEmpty(userInputText, nameof(userInputText));

        return CreateResponseStreamingAsync(
            [ResponseItem.CreateUserMessageItem(userInputText)],
            model,
            options,
            cancellationToken);
    }

    internal virtual CollectionResult<StreamingResponseUpdate> CreateResponseStreaming(string userInputText, string model, ResponseCreationOptions options = null, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNullOrEmpty(userInputText, nameof(userInputText));

        return CreateResponseStreaming(
            [ResponseItem.CreateUserMessageItem(userInputText)],
            model,
            options,
            cancellationToken);
    }

    internal virtual Task<ClientResult<OpenAIResponse>> GetResponseAsync(string responseId, CancellationToken cancellationToken = default)
    {
        return GetResponseAsync(responseId, cancellationToken.ToRequestOptions() ?? new RequestOptions());
    }

    internal async Task<ClientResult<OpenAIResponse>> GetResponseAsync(string responseId, RequestOptions requestOptions)
    {
        Argument.AssertNotNullOrEmpty(responseId, nameof(responseId));
        Argument.AssertNotNull(requestOptions, nameof(requestOptions));
        if (requestOptions.BufferResponse is false)
        {
            throw new InvalidOperationException("'requestOptions.BufferResponse' must be 'true' when calling 'GetResponseAsync'.");
        }

        ClientResult protocolResult = await GetResponseAsync(responseId, stream: null, startingAfter: null, requestOptions).ConfigureAwait(false);
        OpenAIResponse convenienceResult = (OpenAIResponse)protocolResult;
        return ClientResult.FromValue(convenienceResult, protocolResult.GetRawResponse());
    }

    internal virtual ClientResult<OpenAIResponse> GetResponse(string responseId, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNullOrEmpty(responseId, nameof(responseId));

        ClientResult protocolResult = GetResponse(responseId, stream: null, startingAfter: null, cancellationToken.ToRequestOptions());
        OpenAIResponse convenienceResult = (OpenAIResponse)protocolResult;
        return ClientResult.FromValue(convenienceResult, protocolResult.GetRawResponse());
    }

    public virtual async Task<ClientResult<ResponseResult>> GetResponseAsync(GetResponseOptions options, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNull(options, nameof(options));
        Argument.AssertNotNullOrEmpty(options.ResponseId, nameof(options.ResponseId));

        ClientResult protocolResult = await GetResponseAsync(options.ResponseId, stream: options.Stream, startingAfter: options.StartingAfter, cancellationToken.ToRequestOptions()).ConfigureAwait(false);
        return ClientResult.FromValue((ResponseResult)protocolResult, protocolResult.GetRawResponse());
    }

    public virtual ClientResult<ResponseResult> GetResponse(GetResponseOptions options, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNull(options, nameof(options));
        Argument.AssertNotNullOrEmpty(options.ResponseId, nameof(options.ResponseId));

        ClientResult protocolResult = GetResponse(options.ResponseId, stream: options.Stream, startingAfter: options.StartingAfter, cancellationToken.ToRequestOptions());
        return ClientResult.FromValue((ResponseResult)protocolResult, protocolResult.GetRawResponse());
    }

    internal virtual AsyncCollectionResult<StreamingResponseUpdate> GetResponseStreamingAsync(string responseId, int? startingAfter = null, CancellationToken cancellationToken = default)
    {
        return GetResponseStreamingAsync(responseId, cancellationToken.ToRequestOptions(streaming: true), startingAfter);
    }

    internal AsyncCollectionResult<StreamingResponseUpdate> GetResponseStreamingAsync(string responseId, RequestOptions requestOptions, int? startingAfter = null)
    {
        Argument.AssertNotNull(responseId, nameof(responseId));
        Argument.AssertNotNull(requestOptions, nameof(requestOptions));
        if (requestOptions.BufferResponse is true)
        {
            throw new InvalidOperationException("'requestOptions.BufferResponse' must be 'false' when calling 'GetResponseStreamingAsync'.");
        }

        return new AsyncSseUpdateCollection<StreamingResponseUpdate>(
            async () => await GetResponseAsync(responseId, stream: true, startingAfter, requestOptions).ConfigureAwait(false),
            StreamingResponseUpdate.DeserializeStreamingResponseUpdate,
            requestOptions.CancellationToken);
    }

    public virtual CollectionResult<StreamingResponseUpdate> GetResponseStreaming(GetResponseOptions options, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNull(options, nameof(options));
        Argument.AssertNotNullOrEmpty(options.ResponseId, nameof(options.ResponseId));

        return new SseUpdateCollection<StreamingResponseUpdate>(
            () => GetResponse(options.ResponseId, stream: true, startingAfter: options.StartingAfter, cancellationToken.ToRequestOptions(streaming: true)),
            StreamingResponseUpdate.DeserializeStreamingResponseUpdate,
            cancellationToken);
    }

    public virtual AsyncCollectionResult<StreamingResponseUpdate> GetResponseStreamingAsync(GetResponseOptions options, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNull(options, nameof(options));
        Argument.AssertNotNullOrEmpty(options.ResponseId, nameof(options.ResponseId));

        if (options.Stream is true)
        {
            throw new InvalidOperationException("'options.Stream' must be 'true' when calling 'GetResponseStreamingAsync'.");
        }

        return new AsyncSseUpdateCollection<StreamingResponseUpdate>(
            async () => await GetResponseAsync(options.ResponseId, options.Stream, startingAfter: options.StartingAfter, cancellationToken.ToRequestOptions()).ConfigureAwait(false),
            StreamingResponseUpdate.DeserializeStreamingResponseUpdate,
            cancellationToken);
    }

    internal virtual CollectionResult<StreamingResponseUpdate> GetResponseStreaming(string responseId, int? startingAfter = null, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNull(responseId, nameof(responseId));

        return new SseUpdateCollection<StreamingResponseUpdate>(
            () => GetResponse(responseId, stream: true, startingAfter, cancellationToken.ToRequestOptions(streaming: true)),
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

    public virtual ClientResult<ResponseItemCollection> GetResponseInputItems(GetResponseInputItemsOptions options = default, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNull(options, nameof(options));
        Argument.AssertNotNullOrEmpty(options.ResponseId, nameof(options.ResponseId));

        PipelineMessage message = CreateGetResponseInputItemsRequest(options.ResponseId, options.Limit, options.After, options.Order, options.Before, cancellationToken.ToRequestOptions());
        ClientResult result = ClientResult.FromResponse(Pipeline.ProcessMessage(message, cancellationToken.ToRequestOptions()));
        return ClientResult.FromValue((ResponseItemCollection)result, result.GetRawResponse());
    }

    public virtual async Task<ClientResult<ResponseItemCollection>> GetResponseInputItemsAsync(GetResponseInputItemsOptions options = default, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNull(options, nameof(options));
        Argument.AssertNotNullOrEmpty(options.ResponseId, nameof(options.ResponseId));

        PipelineMessage message = CreateGetResponseInputItemsRequest(options.ResponseId, options.Limit, options.After, options.Order, options.Before, cancellationToken.ToRequestOptions());
        ClientResult result = ClientResult.FromResponse(await Pipeline.ProcessMessageAsync(message, cancellationToken.ToRequestOptions()).ConfigureAwait(false));
        return ClientResult.FromValue((ResponseItemCollection)result, result.GetRawResponse());
    }

    internal virtual ResponseCreationOptions CreatePerCallOptions(ResponseCreationOptions userOptions, IEnumerable<ResponseItem> inputItems, string model, bool stream = false)
    {
        ResponseCreationOptions copiedOptions = userOptions is null
            ? new()
            : userOptions.GetClone();

        copiedOptions.Input = inputItems.ToList();

        if (stream)
        {
            copiedOptions.Stream = true;
        }

        if (string.IsNullOrEmpty(copiedOptions.Model))
        {
            copiedOptions.Model = model;
        }

        return copiedOptions;
    }

    internal virtual CreateResponseOptions CreatePerCallOptions(CreateResponseOptions userOptions, bool stream = false)
    {
        CreateResponseOptions copiedOptions = userOptions is null
            ? new()
            : userOptions.GetClone();

        if (stream)
        {
            copiedOptions.IsStreamingEnabled = true;
        }

        return copiedOptions;
    }
}