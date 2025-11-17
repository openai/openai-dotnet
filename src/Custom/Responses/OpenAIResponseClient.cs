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
[CodeGenSuppress("DeleteResponse", typeof(string), typeof(string), typeof(CancellationToken))]
[CodeGenSuppress("DeleteResponseAsync", typeof(string), typeof(string), typeof(CancellationToken))]
[CodeGenSuppress("CancelResponse", typeof(string), typeof(IEnumerable<IncludedResponseProperty>), typeof(bool?), typeof(int?), typeof(CancellationToken))]
[CodeGenSuppress("CancelResponseAsync", typeof(string), typeof(IEnumerable<IncludedResponseProperty>), typeof(bool?), typeof(int?), typeof(CancellationToken))]
[CodeGenSuppress("GetResponse", typeof(string), typeof(IEnumerable<IncludedResponseProperty>), typeof(bool?), typeof(int?), typeof(bool?), typeof(CancellationToken))]
[CodeGenSuppress("GetResponseAsync", typeof(string), typeof(IEnumerable<IncludedResponseProperty>), typeof(bool?), typeof(int?), typeof(bool?), typeof(CancellationToken))]
public partial class OpenAIResponseClient
{
    private readonly string _model;

    // CUSTOM: Added as a convenience.
    /// <summary> Initializes a new instance of <see cref="OpenAIResponseClient"/>. </summary>
    /// <param name="model"> The name of the model to use in requests sent to the service. To learn more about the available models, see <see href="https://platform.openai.com/docs/models"/>. </param>
    /// <param name="apiKey"> The API key to authenticate with the service. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="model"/> or <paramref name="apiKey"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="model"/> is an empty string, and was expected to be non-empty. </exception>
    public OpenAIResponseClient(string model, string apiKey) : this(model, new ApiKeyCredential(apiKey), new OpenAIClientOptions())
    {
    }

    // CUSTOM:
    // - Added `model` parameter.
    // - Used a custom pipeline.
    // - Demoted the endpoint parameter to be a property in the options class.
    /// <summary> Initializes a new instance of <see cref="OpenAIResponseClient"/>. </summary>
    /// <param name="model"> The name of the model to use in requests sent to the service. To learn more about the available models, see <see href="https://platform.openai.com/docs/models"/>. </param>
    /// <param name="credential"> The <see cref="ApiKeyCredential"/> to authenticate with the service. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="model"/> or <paramref name="credential"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="model"/> is an empty string, and was expected to be non-empty. </exception>
    public OpenAIResponseClient(string model, ApiKeyCredential credential) : this(model, credential, new OpenAIClientOptions())
    {
    }

    // CUSTOM:
    // - Added `model` parameter.
    // - Used a custom pipeline.
    // - Demoted the endpoint parameter to be a property in the options class.
    /// <summary> Initializes a new instance of <see cref="OpenAIResponseClient"/>. </summary>
    /// <param name="model"> The name of the model to use in requests sent to the service. To learn more about the available models, see <see href="https://platform.openai.com/docs/models"/>. </param>
    /// <param name="credential"> The <see cref="ApiKeyCredential"/> to authenticate with the service. </param>
    /// <param name="options"> The options to configure the client. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="model"/> or <paramref name="credential"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="model"/> is an empty string, and was expected to be non-empty. </exception>
    public OpenAIResponseClient(string model, ApiKeyCredential credential, OpenAIClientOptions options) : this(model, OpenAIClient.CreateApiKeyAuthenticationPolicy(credential), options)
    {
    }

    // CUSTOM: Added as a convenience.
    /// <summary> Initializes a new instance of <see cref="OpenAIResponseClient"/>. </summary>
    /// <param name="model"> The name of the model to use in requests sent to the service. To learn more about the available models, see <see href="https://platform.openai.com/docs/models"/>. </param>
    /// <param name="authenticationPolicy"> The authentication policy used to authenticate with the service. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="model"/> or <paramref name="authenticationPolicy"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="model"/> is an empty string, and was expected to be non-empty. </exception>
    public OpenAIResponseClient(string model, AuthenticationPolicy authenticationPolicy) : this(model, authenticationPolicy, new OpenAIClientOptions())
    {
    }

    // CUSTOM: Added as a convenience.
    /// <summary> Initializes a new instance of <see cref="OpenAIResponseClient"/>. </summary>
    /// <param name="model"> The name of the model to use in requests sent to the service. To learn more about the available models, see <see href="https://platform.openai.com/docs/models"/>. </param>
    /// <param name="authenticationPolicy"> The authentication policy used to authenticate with the service. </param>
    /// <param name="options"> The options to configure the client. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="model"/> or <paramref name="authenticationPolicy"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="model"/> is an empty string, and was expected to be non-empty. </exception>
    public OpenAIResponseClient(string model, AuthenticationPolicy authenticationPolicy, OpenAIClientOptions options)
    {
        Argument.AssertNotNullOrEmpty(model, nameof(model));
        Argument.AssertNotNull(authenticationPolicy, nameof(authenticationPolicy));
        options ??= new OpenAIClientOptions();

        _model = model;
        Pipeline = OpenAIClient.CreatePipeline(authenticationPolicy, options);
        _endpoint = OpenAIClient.GetEndpoint(options);
    }

    // CUSTOM:
    // - Added `model` parameter.
    // - Used a custom pipeline.
    // - Demoted the endpoint parameter to be a property in the options class.
    // - Made protected.
    /// <summary> Initializes a new instance of <see cref="OpenAIResponseClient"/>. </summary>
    /// <param name="pipeline"> The HTTP pipeline to send and receive REST requests and responses. </param>
    /// <param name="model"> The name of the model to use in requests sent to the service. To learn more about the available models, see <see href="https://platform.openai.com/docs/models"/>. </param>
    /// <param name="options"> The options to configure the client. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="pipeline"/> or <paramref name="model"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="model"/> is an empty string, and was expected to be non-empty. </exception>
    protected internal OpenAIResponseClient(ClientPipeline pipeline, string model, OpenAIClientOptions options)
    {
        Argument.AssertNotNull(pipeline, nameof(pipeline));
        options ??= new OpenAIClientOptions();

        _model = model;
        Pipeline = pipeline;
        _endpoint = OpenAIClient.GetEndpoint(options);
    }

    /// <summary>
    /// Gets the endpoint URI for the service.
    /// </summary>
    [Experimental("OPENAI001")]
    public Uri Endpoint => _endpoint;

    /// <summary>
    /// Gets the name of the model used in requests sent to the service.
    /// </summary>
    [Experimental("OPENAI001")]
    public string Model => _model;

    internal async Task<ClientResult<OpenAIResponse>> CreateResponseAsync(IEnumerable<ResponseItem> inputItems, ResponseCreationOptions options, RequestOptions requestOptions)
    {
        Argument.AssertNotNullOrEmpty(inputItems, nameof(inputItems));
        Argument.AssertNotNull(requestOptions, nameof(requestOptions));
        if (requestOptions.BufferResponse is false)
        {
            throw new InvalidOperationException("'requestOptions.BufferResponse' must be 'true' when calling 'CreateResponseAsync'.");
        }

        using BinaryContent content = CreatePerCallOptions(options, inputItems, stream: false).ToBinaryContent();
        ClientResult protocolResult = await CreateResponseAsync(content, requestOptions).ConfigureAwait(false);
        OpenAIResponse convenienceValue = (OpenAIResponse)protocolResult;
        return ClientResult.FromValue(convenienceValue, protocolResult.GetRawResponse());
    }

    public virtual Task<ClientResult<OpenAIResponse>> CreateResponseAsync(IEnumerable<ResponseItem> inputItems, ResponseCreationOptions options = null, CancellationToken cancellationToken = default)
    {
        return CreateResponseAsync(inputItems, options, cancellationToken.ToRequestOptions() ?? new RequestOptions());
    }

    public virtual ClientResult<OpenAIResponse> CreateResponse(IEnumerable<ResponseItem> inputItems, ResponseCreationOptions options = null, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNullOrEmpty(inputItems, nameof(inputItems));

        using BinaryContent content = CreatePerCallOptions(options, inputItems, stream: false).ToBinaryContent();
        ClientResult protocolResult = CreateResponse(content, cancellationToken.ToRequestOptions());
        OpenAIResponse convenienceValue = (OpenAIResponse)protocolResult;
        return ClientResult.FromValue(convenienceValue, protocolResult.GetRawResponse());
    }

    public virtual async Task<ClientResult<OpenAIResponse>> CreateResponseAsync(string userInputText, ResponseCreationOptions options = null, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNullOrEmpty(userInputText, nameof(userInputText));

        return await CreateResponseAsync(
            [ResponseItem.CreateUserMessageItem(userInputText)],
            options,
            cancellationToken)
                .ConfigureAwait(false);
    }

    public virtual ClientResult<OpenAIResponse> CreateResponse(string userInputText, ResponseCreationOptions options = null, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNullOrEmpty(userInputText, nameof(userInputText));

        return CreateResponse(
            [ResponseItem.CreateUserMessageItem(userInputText)],
            options,
            cancellationToken);
    }

    internal AsyncCollectionResult<StreamingResponseUpdate> CreateResponseStreamingAsync(IEnumerable<ResponseItem> inputItems, ResponseCreationOptions options, RequestOptions requestOptions)
    {
        Argument.AssertNotNullOrEmpty(inputItems, nameof(inputItems));
        Argument.AssertNotNull(requestOptions, nameof(requestOptions));
        if (requestOptions.BufferResponse is true)
        {
            throw new InvalidOperationException("'requestOptions.BufferResponse' must be 'false' when calling 'CreateResponseStreamingAsync'.");
        }

        using BinaryContent content = CreatePerCallOptions(options, inputItems, stream: true).ToBinaryContent();
        return new AsyncSseUpdateCollection<StreamingResponseUpdate>(
            async () => await CreateResponseAsync(content, requestOptions).ConfigureAwait(false),
            StreamingResponseUpdate.DeserializeStreamingResponseUpdate,
            requestOptions.CancellationToken);
    }

    public virtual AsyncCollectionResult<StreamingResponseUpdate> CreateResponseStreamingAsync(IEnumerable<ResponseItem> inputItems, ResponseCreationOptions options = null, CancellationToken cancellationToken = default)
    {
        return CreateResponseStreamingAsync(inputItems, options, cancellationToken.ToRequestOptions(streaming: true));
    }

    public virtual CollectionResult<StreamingResponseUpdate> CreateResponseStreaming(IEnumerable<ResponseItem> inputItems, ResponseCreationOptions options = null, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNullOrEmpty(inputItems, nameof(inputItems));

        using BinaryContent content = CreatePerCallOptions(options, inputItems, stream: true).ToBinaryContent();
        return new SseUpdateCollection<StreamingResponseUpdate>(
            () => CreateResponse(content, cancellationToken.ToRequestOptions(streaming: true)),
            StreamingResponseUpdate.DeserializeStreamingResponseUpdate,
            cancellationToken);
    }

    public virtual AsyncCollectionResult<StreamingResponseUpdate> CreateResponseStreamingAsync(string userInputText, ResponseCreationOptions options = null, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNullOrEmpty(userInputText, nameof(userInputText));

        return CreateResponseStreamingAsync(
            [ResponseItem.CreateUserMessageItem(userInputText)],
            options,
            cancellationToken);
    }

    public virtual CollectionResult<StreamingResponseUpdate> CreateResponseStreaming(string userInputText, ResponseCreationOptions options = null, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNullOrEmpty(userInputText, nameof(userInputText));

        return CreateResponseStreaming(
            [ResponseItem.CreateUserMessageItem(userInputText)],
            options,
            cancellationToken);
    }

    internal async Task<ClientResult<OpenAIResponse>> GetResponseAsync(string responseId, IEnumerable<IncludedResponseProperty> include, RequestOptions requestOptions)
    {
        Argument.AssertNotNullOrEmpty(responseId, nameof(responseId));
        Argument.AssertNotNull(requestOptions, nameof(requestOptions));
        if (requestOptions.BufferResponse is false)
        {
            throw new InvalidOperationException("'requestOptions.BufferResponse' must be 'true' when calling 'GetResponseAsync'.");
        }

        ClientResult protocolResult = await GetResponseAsync(responseId, include, stream: null, startingAfter: null, includeObfuscation: null, requestOptions).ConfigureAwait(false);
        OpenAIResponse convenienceResult = (OpenAIResponse)protocolResult;
        return ClientResult.FromValue(convenienceResult, protocolResult.GetRawResponse());
    }

    public virtual Task<ClientResult<OpenAIResponse>> GetResponseAsync(string responseId, IEnumerable<IncludedResponseProperty> includedProperties = default, CancellationToken cancellationToken = default)
    {
        return GetResponseAsync(responseId, includedProperties, cancellationToken.ToRequestOptions() ?? new RequestOptions());
    }

    public virtual ClientResult<OpenAIResponse> GetResponse(string responseId, IEnumerable<IncludedResponseProperty> includedProperties = default, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNullOrEmpty(responseId, nameof(responseId));

        ClientResult protocolResult = GetResponse(responseId, includedProperties, stream: null, startingAfter: null, includeObfuscation: null, cancellationToken.ToRequestOptions());
        OpenAIResponse convenienceResult = (OpenAIResponse)protocolResult;
        return ClientResult.FromValue(convenienceResult, protocolResult.GetRawResponse());
    }

    internal AsyncCollectionResult<StreamingResponseUpdate> GetResponseStreamingAsync(string responseId, IEnumerable<IncludedResponseProperty> include, int? startingAfter, bool? includeObfuscation, RequestOptions requestOptions)
    {
        Argument.AssertNotNull(responseId, nameof(responseId));
        Argument.AssertNotNull(requestOptions, nameof(requestOptions));
        if (requestOptions.BufferResponse is true)
        {
            throw new InvalidOperationException("'requestOptions.BufferResponse' must be 'false' when calling 'GetResponseStreamingAsync'.");
        }

        return new AsyncSseUpdateCollection<StreamingResponseUpdate>(
            async () => await GetResponseAsync(responseId, include, stream: true, startingAfter, includeObfuscation, requestOptions).ConfigureAwait(false),
            StreamingResponseUpdate.DeserializeStreamingResponseUpdate,
            requestOptions.CancellationToken);
    }

    public virtual AsyncCollectionResult<StreamingResponseUpdate> GetResponseStreamingAsync(string responseId, IEnumerable<IncludedResponseProperty> includedProperties = default, int? startingAfter = default, bool? includeObfuscation = default, CancellationToken cancellationToken = default)
    {
        return GetResponseStreamingAsync(responseId, includedProperties, startingAfter, includeObfuscation, cancellationToken.ToRequestOptions(streaming: true));
    }

    public virtual CollectionResult<StreamingResponseUpdate> GetResponseStreaming(string responseId, IEnumerable<IncludedResponseProperty> includedProperties = default, int? startingAfter = default, bool? includeObfuscation = default, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNull(responseId, nameof(responseId));

        return new SseUpdateCollection<StreamingResponseUpdate>(
            () => GetResponse(responseId, includedProperties, stream: true, startingAfter, includeObfuscation, cancellationToken.ToRequestOptions(streaming: true)),
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

    public virtual async Task<ClientResult<OpenAIResponse>> CancelResponseAsync(string responseId, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNullOrEmpty(responseId, nameof(responseId));

        ClientResult protocolResult = await CancelResponseAsync(responseId, cancellationToken.ToRequestOptions()).ConfigureAwait(false);
        OpenAIResponse convenienceResult = (OpenAIResponse)protocolResult;
        return ClientResult.FromValue(convenienceResult, protocolResult.GetRawResponse());
    }

    public virtual ClientResult<OpenAIResponse> CancelResponse(string responseId, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNullOrEmpty(responseId, nameof(responseId));

        ClientResult protocolResult = CancelResponse(responseId, cancellationToken.ToRequestOptions());
        OpenAIResponse convenienceResult = (OpenAIResponse)protocolResult;
        return ClientResult.FromValue(convenienceResult, protocolResult.GetRawResponse());
    }

    internal virtual ResponseCreationOptions CreatePerCallOptions(ResponseCreationOptions userOptions, IEnumerable<ResponseItem> inputItems, bool stream = false)
    {
        ResponseCreationOptions copiedOptions = userOptions is null
            ? new()
            : userOptions.GetClone();

        copiedOptions.Input = inputItems.ToList();
        copiedOptions.Model = _model;

        if (stream)
        {
            copiedOptions.Stream = true;
        }

        return copiedOptions;
    }
}