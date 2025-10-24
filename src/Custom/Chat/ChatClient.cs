using OpenAI.Telemetry;
using System;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace OpenAI.Chat;

// CUSTOM:
// - Renamed.
// - Suppressed constructor that takes endpoint parameter; endpoint is now a property in the options class.
// - Suppressed methods that only take the options parameter.
/// <summary> The service client for OpenAI chat operations. </summary>
[CodeGenType("Chat")]
[CodeGenSuppress("ChatClient", typeof(ClientPipeline), typeof(Uri))]
[CodeGenSuppress("CompleteChat", typeof(ChatCompletionOptions), typeof(CancellationToken))]
[CodeGenSuppress("CompleteChatAsync", typeof(ChatCompletionOptions), typeof(CancellationToken))]
[CodeGenSuppress("GetChatCompletionMessagesAsync", typeof(string), typeof(string), typeof(int?), typeof(string), typeof(RequestOptions))]
[CodeGenSuppress("GetChatCompletionMessages", typeof(string), typeof(string), typeof(int?), typeof(string), typeof(RequestOptions))]
[CodeGenSuppress("GetChatCompletions", typeof(string), typeof(string), typeof(int?), typeof(string), typeof(IDictionary<string, string>), typeof(string), typeof(RequestOptions))]
[CodeGenSuppress("GetChatCompletionMessages", typeof(string), typeof(string), typeof(int?), typeof(string), typeof(IDictionary<string, string>), typeof(string), typeof(RequestOptions))]
public partial class ChatClient
{
    private readonly string _model;
    private readonly OpenTelemetrySource _telemetry;
    private static readonly InternalChatCompletionStreamOptions s_includeUsageStreamOptions = new(includeUsage: true, patch: default);

    // CUSTOM: Added as a convenience.
    /// <summary> Initializes a new instance of <see cref="ChatClient"/>. </summary>
    /// <param name="model"> The name of the model to use in requests sent to the service. To learn more about the available models, see <see href="https://platform.openai.com/docs/models"/>. </param>
    /// <param name="apiKey"> The API key to authenticate with the service. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="model"/> or <paramref name="apiKey"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="model"/> is an empty string, and was expected to be non-empty. </exception>
    public ChatClient(string model, string apiKey) : this(model, new ApiKeyCredential(apiKey), new OpenAIClientOptions())
    {
    }

    // CUSTOM:
    // - Added `model` parameter.
    // - Used a custom pipeline.
    // - Demoted the endpoint parameter to be a property in the options class.
    /// <summary> Initializes a new instance of <see cref="ChatClient"/>. </summary>
    /// <param name="model"> The name of the model to use in requests sent to the service. To learn more about the available models, see <see href="https://platform.openai.com/docs/models"/>. </param>
    /// <param name="credential"> The <see cref="ApiKeyCredential"/> to authenticate with the service. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="model"/> or <paramref name="credential"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="model"/> is an empty string, and was expected to be non-empty. </exception>
    public ChatClient(string model, ApiKeyCredential credential) : this(model, credential, new OpenAIClientOptions())
    {
    }

    // CUSTOM:
    // - Added `model` parameter.
    // - Used a custom pipeline.
    // - Demoted the endpoint parameter to be a property in the options class.
    // - Added telemetry support.
    /// <summary> Initializes a new instance of <see cref="ChatClient"/>. </summary>
    /// <param name="model"> The name of the model to use in requests sent to the service. To learn more about the available models, see <see href="https://platform.openai.com/docs/models"/>. </param>
    /// <param name="credential"> The <see cref="ApiKeyCredential"/> to authenticate with the service. </param>
    /// <param name="options"> The options to configure the client. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="model"/> or <paramref name="credential"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="model"/> is an empty string, and was expected to be non-empty. </exception>
    public ChatClient(string model, ApiKeyCredential credential, OpenAIClientOptions options) : this(model, OpenAIClient.CreateApiKeyAuthenticationPolicy(credential), options)
    {
    }

    // CUSTOM: Added as a convenience.
    /// <summary> Initializes a new instance of <see cref="ChatClient"/>. </summary>
    /// <param name="model"> The name of the model to use in requests sent to the service. To learn more about the available models, see <see href="https://platform.openai.com/docs/models"/>. </param>
    /// <param name="authenticationPolicy"> The authentication policy used to authenticate with the service. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="model"/> or <paramref name="authenticationPolicy"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="model"/> is an empty string, and was expected to be non-empty. </exception>
    [Experimental("OPENAI001")]
    public ChatClient(string model, AuthenticationPolicy authenticationPolicy) : this(model, authenticationPolicy, new OpenAIClientOptions())
    {
    }

    // CUSTOM: Added as a convenience.
    /// <summary> Initializes a new instance of <see cref="ChatClient"/>. </summary>
    /// <param name="model"> The name of the model to use in requests sent to the service. To learn more about the available models, see <see href="https://platform.openai.com/docs/models"/>. </param>
    /// <param name="authenticationPolicy"> The authentication policy used to authenticate with the service. </param>
    /// <param name="options"> The options to configure the client. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="model"/> or <paramref name="authenticationPolicy"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="model"/> is an empty string, and was expected to be non-empty. </exception>
    [Experimental("OPENAI001")]
    public ChatClient(string model, AuthenticationPolicy authenticationPolicy, OpenAIClientOptions options)
    {
        Argument.AssertNotNullOrEmpty(model, nameof(model));
        Argument.AssertNotNull(authenticationPolicy, nameof(authenticationPolicy));
        options ??= new OpenAIClientOptions();

        _model = model;
        Pipeline = OpenAIClient.CreatePipeline(authenticationPolicy, options);
        _endpoint = OpenAIClient.GetEndpoint(options);
        _telemetry = new OpenTelemetrySource(model, _endpoint);
    }

    // CUSTOM:
    // - Added `model` parameter.
    // - Used a custom pipeline.
    // - Demoted the endpoint parameter to be a property in the options class.
    // - Added telemetry support.
    // - Made protected.
    /// <summary> Initializes a new instance of <see cref="ChatClient"/>. </summary>
    /// <param name="pipeline"> The HTTP pipeline to send and receive REST requests and responses. </param>
    /// <param name="model"> The name of the model to use in requests sent to the service. To learn more about the available models, see <see href="https://platform.openai.com/docs/models"/>. </param>
    /// <param name="options"> The options to configure the client. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="pipeline"/> or <paramref name="model"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="model"/> is an empty string, and was expected to be non-empty. </exception>
    protected internal ChatClient(ClientPipeline pipeline, string model, OpenAIClientOptions options)
    {
        Argument.AssertNotNull(pipeline, nameof(pipeline));
        Argument.AssertNotNullOrEmpty(model, nameof(model));
        options ??= new OpenAIClientOptions();

        _model = model;
        Pipeline = pipeline;
        _endpoint = OpenAIClient.GetEndpoint(options);
        _telemetry = new OpenTelemetrySource(model, _endpoint);
    }

    /// <summary>
    /// Gets the name of the model used in requests sent to the service.
    /// </summary>
    [Experimental("OPENAI001")]
    public virtual string Model => _model;

    /// <summary>
    /// Gets the endpoint URI for the service.
    /// </summary>
    [Experimental("OPENAI001")]
    public Uri Endpoint => _endpoint;

    /// <summary> Generates a completion for the given chat. </summary>
    /// <param name="messages"> The messages comprising the chat so far. </param>
    /// <param name="options"> The options to configure the chat completion. </param>
    /// <param name="cancellationToken"> A token that can be used to cancel this method call. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="messages"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="messages"/> is an empty collection, and was expected to be non-empty. </exception>
    public virtual Task<ClientResult<ChatCompletion>> CompleteChatAsync(IEnumerable<ChatMessage> messages, ChatCompletionOptions options = null, CancellationToken cancellationToken = default)
    {
        return CompleteChatAsync(messages, options, cancellationToken.ToRequestOptions() ?? new RequestOptions());
    }

    internal async Task<ClientResult<ChatCompletion>> CompleteChatAsync(IEnumerable<ChatMessage> messages, ChatCompletionOptions options, RequestOptions requestOptions)
    {
        Argument.AssertNotNullOrEmpty(messages, nameof(messages));
        Argument.AssertNotNull(requestOptions, nameof(requestOptions));
        if (requestOptions.BufferResponse is false)
        {
            throw new InvalidOperationException("'requestOptions.BufferResponse' must be 'true' when calling 'CompleteChatAsync'.");
        }

        options ??= new();
        CreateChatCompletionOptions(messages, ref options);
        using OpenTelemetryScope scope = _telemetry.StartChatScope(options);

        try
        {
            using BinaryContent content = options.ToBinaryContent();

            ClientResult result = await CompleteChatAsync(content, requestOptions).ConfigureAwait(false);
            ChatCompletion chatCompletion = (ChatCompletion)result;
            scope?.RecordChatCompletion(chatCompletion);
            return ClientResult.FromValue(chatCompletion, result.GetRawResponse());
        }
        catch (Exception ex)
        {
            scope?.RecordException(ex);
            throw;
        }
    }

    /// <summary> Generates a completion for the given chat. </summary>
    /// <param name="messages"> The messages comprising the chat so far. </param>
    /// <param name="options"> The options to configure the chat completion. </param>
    /// <param name="cancellationToken"> A token that can be used to cancel this method call. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="messages"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="messages"/> is an empty collection, and was expected to be non-empty. </exception>
    public virtual ClientResult<ChatCompletion> CompleteChat(IEnumerable<ChatMessage> messages, ChatCompletionOptions options = null, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNullOrEmpty(messages, nameof(messages));

        options ??= new();
        CreateChatCompletionOptions(messages, ref options);
        using OpenTelemetryScope scope = _telemetry.StartChatScope(options);

        try
        {
            using BinaryContent content = options.ToBinaryContent();
            ClientResult result = CompleteChat(content, cancellationToken.ToRequestOptions());
            ChatCompletion chatCompletion = (ChatCompletion)result;

            scope?.RecordChatCompletion(chatCompletion);
            return ClientResult.FromValue(chatCompletion, result.GetRawResponse());
        }
        catch (Exception ex)
        {
            scope?.RecordException(ex);
            throw;
        }
    }

    /// <summary> Generates a completion for the given chat. </summary>
    /// <param name="messages"> The messages comprising the chat so far. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="messages"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="messages"/> is an empty collection, and was expected to be non-empty. </exception>
    public virtual async Task<ClientResult<ChatCompletion>> CompleteChatAsync(params ChatMessage[] messages)
        => await CompleteChatAsync(messages, default(ChatCompletionOptions)).ConfigureAwait(false);

    /// <summary> Generates a completion for the given chat. </summary>
    /// <param name="messages"> The messages comprising the chat so far. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="messages"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="messages"/> is an empty collection, and was expected to be non-empty. </exception>
    public virtual ClientResult<ChatCompletion> CompleteChat(params ChatMessage[] messages)
        => CompleteChat(messages, default(ChatCompletionOptions));

    [Experimental("OPENAI001")]
    [OverloadResolutionPriority(2)]
    public virtual ClientResult<ChatCompletionResult> CompleteChat(CreateChatCompletionOptions options, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNull(options, nameof(options));

        ClientResult result = this.CompleteChat(options.Body, cancellationToken.CanBeCanceled ? new RequestOptions { CancellationToken = cancellationToken } : null);
        return ClientResult.FromValue((ChatCompletionResult)result.GetRawResponse().Content, result.GetRawResponse());
    }

    [Experimental("OPENAI001")]
    [OverloadResolutionPriority(2)]
    public virtual async Task<ClientResult<ChatCompletionResult>> CompleteChatAsync(CreateChatCompletionOptions options, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNull(options, nameof(options));

        ClientResult result = await this.CompleteChatAsync(options, cancellationToken.CanBeCanceled ? new RequestOptions { CancellationToken = cancellationToken } : null).ConfigureAwait(false);
        // this doesn't work for streaming responses because it will not serialize correctly. We need a CompleteChatStreaming.
        return ClientResult.FromValue((ChatCompletionResult)result.GetRawResponse().Content, result.GetRawResponse());
    }

    [OverloadResolutionPriority(1)]
    public virtual ClientResult CompleteChat(CreateChatCompletionOptions options, RequestOptions requestOptions = default)
    {
        Argument.AssertNotNull(options, nameof(options));

        return CompleteChat((BinaryContent)options, requestOptions);
    }

    [OverloadResolutionPriority(1)]
    public virtual async Task<ClientResult> CompleteChatAsync(CreateChatCompletionOptions options, RequestOptions requestOptions = default)
    {
        Argument.AssertNotNull(options, nameof(options));

        return await CompleteChatAsync((BinaryContent)options, requestOptions).ConfigureAwait(false);
    }

    /// <summary>
    ///     Generates a completion for the given chat. The completion is streamed back token by token as it is being
    ///     generated by the model instead of waiting for it to be finished first.
    /// </summary>
    /// <remarks>
    ///     <see cref="CollectionResult{T}"/> implements the <see cref="IEnumerable{T}"/> interface and can be
    ///     enumerated over using the <c>await foreach</c> pattern.
    /// </remarks>
    /// <param name="options"> The request containing the messages comprising the chat so far in addition to other options. </param>
    /// <param name="cancellationToken"> A token that can be used to cancel this method call. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="options"/> is null. </exception>
    public virtual CollectionResult<StreamingChatCompletionUpdate> CompleteChatStreaming(CreateChatCompletionOptions options, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNull(options, nameof(options));

        options.Stream = true;
        options.StreamOptions = new(true, default);

        return new SseUpdateCollection<StreamingChatCompletionUpdate>(
            () => CompleteChat(options, cancellationToken.ToRequestOptions(streaming: true)),
            StreamingChatCompletionUpdate.DeserializeStreamingChatCompletionUpdate,
            cancellationToken);
    }

    public virtual AsyncCollectionResult<StreamingChatCompletionUpdate> CompleteChatStreamingAsync(CreateChatCompletionOptions options, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNull(options, nameof(options));

        options.Stream = true;
        options.StreamOptions = new(true, default);

        return new AsyncSseUpdateCollection<StreamingChatCompletionUpdate>(
            async () => await CompleteChatAsync(options, cancellationToken.ToRequestOptions(streaming: true)).ConfigureAwait(false),
            StreamingChatCompletionUpdate.DeserializeStreamingChatCompletionUpdate,
            cancellationToken);
    }

    /// <summary>
    ///     Generates a completion for the given chat. The completion is streamed back token by token as it is being
    ///     generated by the model instead of waiting for it to be finished first.
    /// </summary>
    /// <remarks>
    ///     <see cref="AsyncCollectionResult{T}"/> implements the <see cref="IAsyncEnumerable{T}"/> interface and can be
    ///     enumerated over using the <c>await foreach</c> pattern.
    /// </remarks>
    /// <param name="messages"> The messages comprising the chat so far. </param>
    /// <param name="options"> The options to configure the chat completion. </param>
    /// <param name="cancellationToken"> A token that can be used to cancel this method call. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="messages"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="messages"/> is an empty collection, and was expected to be non-empty. </exception>
    public virtual AsyncCollectionResult<StreamingChatCompletionUpdate> CompleteChatStreamingAsync(IEnumerable<ChatMessage> messages, ChatCompletionOptions options = null, CancellationToken cancellationToken = default)
    {
        return CompleteChatStreamingAsync(messages, options, cancellationToken.ToRequestOptions(streaming: true));
    }

    internal AsyncCollectionResult<StreamingChatCompletionUpdate> CompleteChatStreamingAsync(IEnumerable<ChatMessage> messages, ChatCompletionOptions options, RequestOptions requestOptions)
    {
        Argument.AssertNotNull(messages, nameof(messages));
        Argument.AssertNotNull(requestOptions, nameof(requestOptions));
        if (requestOptions.BufferResponse is true)
        {
            throw new InvalidOperationException("'requestOptions.BufferResponse' must be 'false' when calling 'CompleteChatStreamingAsync'.");
        }

        options ??= new();
        CreateChatCompletionOptions(messages, ref options, stream: true);

        using BinaryContent content = options.ToBinaryContent();
        return new AsyncSseUpdateCollection<StreamingChatCompletionUpdate>(
            async () => await CompleteChatAsync(content, requestOptions).ConfigureAwait(false),
            StreamingChatCompletionUpdate.DeserializeStreamingChatCompletionUpdate,
            requestOptions.CancellationToken);
    }

    /// <summary>
    ///     Generates a completion for the given chat. The completion is streamed back token by token as it is being
    ///     generated by the model instead of waiting for it to be finished first.
    /// </summary>
    /// <remarks>
    ///     <see cref="CollectionResult{T}"/> implements the <see cref="IEnumerable{T}"/> interface and can be
    ///     enumerated over using the <c>await foreach</c> pattern.
    /// </remarks>
    /// <param name="messages"> The messages comprising the chat so far. </param>
    /// <param name="options"> The options to configure the chat completion. </param>
    /// <param name="cancellationToken"> A token that can be used to cancel this method call. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="messages"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="messages"/> is an empty collection, and was expected to be non-empty. </exception>
    public virtual CollectionResult<StreamingChatCompletionUpdate> CompleteChatStreaming(IEnumerable<ChatMessage> messages, ChatCompletionOptions options = null, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNull(messages, nameof(messages));

        options ??= new();
        CreateChatCompletionOptions(messages, ref options, stream: true);

        using BinaryContent content = options.ToBinaryContent();
        return new SseUpdateCollection<StreamingChatCompletionUpdate>(
            () => CompleteChat(content, cancellationToken.ToRequestOptions(streaming: true)),
            StreamingChatCompletionUpdate.DeserializeStreamingChatCompletionUpdate,
            cancellationToken);
    }

    /// <summary>
    ///     Generates a completion for the given chat. The completion is streamed back token by token as it is being
    ///     generated by the model instead of waiting for it to be finished first.
    /// </summary>
    /// <remarks>
    ///     <see cref="AsyncCollectionResult{T}"/> implements the <see cref="IAsyncEnumerable{T}"/> interface and can be
    ///     enumerated over using the <c>await foreach</c> pattern.
    /// </remarks>
    /// <param name="messages"> The messages comprising the chat so far. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="messages"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="messages"/> is an empty collection, and was expected to be non-empty. </exception>
    public virtual AsyncCollectionResult<StreamingChatCompletionUpdate> CompleteChatStreamingAsync(params ChatMessage[] messages)
        => CompleteChatStreamingAsync(messages, default(ChatCompletionOptions));

    /// <summary>
    ///     Generates a completion for the given chat. The completion is streamed back token by token as it is being
    ///     generated by the model instead of waiting for it to be finished first.
    /// </summary>
    /// <remarks>
    ///     <see cref="CollectionResult{T}"/> implements the <see cref="IEnumerable{T}"/> interface and can be
    ///     enumerated over using the <c>await foreach</c> pattern.
    /// </remarks>
    /// <param name="messages"> The messages comprising the chat so far. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="messages"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="messages"/> is an empty collection, and was expected to be non-empty. </exception>
    public virtual CollectionResult<StreamingChatCompletionUpdate> CompleteChatStreaming(params ChatMessage[] messages)
        => CompleteChatStreaming(messages, default(ChatCompletionOptions));

    // CUSTOM:
    // - Added Experimental attribute.
    // - Call FromClientResult.
    [Experimental("OPENAI001")]
    public virtual async Task<ClientResult<ChatCompletion>> GetChatCompletionAsync(string completionId, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNullOrEmpty(completionId, nameof(completionId));

        ClientResult result = await GetChatCompletionAsync(completionId, cancellationToken.CanBeCanceled ? new RequestOptions { CancellationToken = cancellationToken } : null).ConfigureAwait(false);
        return ClientResult.FromValue((ChatCompletion)result, result.GetRawResponse());
    }

    // CUSTOM:
    // - Added Experimental attribute.
    // - Call FromClientResult.
    [Experimental("OPENAI001")]
    public virtual ClientResult<ChatCompletion> GetChatCompletion(string completionId, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNullOrEmpty(completionId, nameof(completionId));

        ClientResult result = GetChatCompletion(completionId, cancellationToken.CanBeCanceled ? new RequestOptions { CancellationToken = cancellationToken } : null);
        return ClientResult.FromValue((ChatCompletion)result, result.GetRawResponse());
    }

    [OverloadResolutionPriority(1)]
    public virtual ClientResult<ChatCompletionResult> GetChatCompletion(GetChatCompletionOptions options, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNullOrEmpty(options.CompletionId, nameof(options.CompletionId));

        ClientResult result = GetChatCompletion(options.CompletionId, cancellationToken.CanBeCanceled ? new RequestOptions { CancellationToken = cancellationToken } : null);
        return ClientResult.FromValue((ChatCompletionResult)result, result.GetRawResponse());
    }

    [OverloadResolutionPriority(1)]
    public virtual async Task<ClientResult<ChatCompletionResult>> GetChatCompletionAsync(GetChatCompletionOptions options, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNullOrEmpty(options.CompletionId, nameof(options.CompletionId));

        ClientResult result = await GetChatCompletionAsync(options.CompletionId, cancellationToken.CanBeCanceled ? new RequestOptions { CancellationToken = cancellationToken } : null);
        return ClientResult.FromValue((ChatCompletionResult)result, result.GetRawResponse());
    }

    [Experimental("OPENAI001")]
    [OverloadResolutionPriority(2)]
    public virtual ClientResult GetChatCompletion(GetChatCompletionOptions options, RequestOptions requestOptions = null)
    {
        Argument.AssertNotNullOrEmpty(options.CompletionId, nameof(options.CompletionId));

        using PipelineMessage message = CreateGetChatCompletionRequest(options.CompletionId, requestOptions);
        return ClientResult.FromResponse(Pipeline.ProcessMessage(message, requestOptions));
    }

    [Experimental("OPENAI001")]
    [OverloadResolutionPriority(2)]
    public virtual async Task<ClientResult> GetChatCompletionAsync(GetChatCompletionOptions options, RequestOptions requestOptions = null)
    {
        Argument.AssertNotNullOrEmpty(options.CompletionId, nameof(options.CompletionId));

        using PipelineMessage message = CreateGetChatCompletionRequest(options.CompletionId, requestOptions);
        return ClientResult.FromResponse(await Pipeline.ProcessMessageAsync(message, requestOptions).ConfigureAwait(false));
    }

    [Experimental("OPENAI001")]
    public virtual ClientResult<ChatCompletionList> GetChatCompletionMessages(GetChatCompletionMessageOptions options, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNullOrEmpty(options.CompletionId, nameof(options.CompletionId));

        PipelineMessage message = CreateGetChatCompletionMessagesRequest(options.CompletionId, options.After, options.Limit, options.Order, cancellationToken.ToRequestOptions());
        ClientResult result = ClientResult.FromResponse(Pipeline.ProcessMessage(message, cancellationToken.ToRequestOptions()));
        return ClientResult.FromValue((ChatCompletionList)result, result.GetRawResponse());
    }

    [Experimental("OPENAI001")]
    public virtual async Task<ClientResult<ChatCompletionList>> GetChatCompletionMessagesAsync(GetChatCompletionMessageOptions options, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNullOrEmpty(options.CompletionId, nameof(options.CompletionId));

        PipelineMessage message = CreateGetChatCompletionMessagesRequest(options.CompletionId, options.After, options.Limit, options.Order, cancellationToken.ToRequestOptions());
        ClientResult result = ClientResult.FromResponse(await Pipeline.ProcessMessageAsync(message, cancellationToken.ToRequestOptions()).ConfigureAwait(false));
        return ClientResult.FromValue((ChatCompletionList)result, result.GetRawResponse());
    }

    [Experimental("OPENAI001")]
    public virtual CollectionResult GetChatCompletionMessages(GetChatCompletionMessageOptions options, RequestOptions requestOptions = null)
    {
        Argument.AssertNotNullOrEmpty(options.CompletionId, nameof(options.CompletionId));

        return new ChatClientGetChatCompletionMessagesCollectionResult(
            this,
            options.CompletionId,
            options.After,
            options.Limit,
            options.Order,
            requestOptions);
    }

    [Experimental("OPENAI001")]
    public virtual AsyncCollectionResult GetChatCompletionMessagesAsync(GetChatCompletionMessageOptions options, RequestOptions requestOptions = null)
    {
        Argument.AssertNotNullOrEmpty(options.CompletionId, nameof(options.CompletionId));

        return new ChatClientGetChatCompletionMessagesAsyncCollectionResult(
            this,
            options.CompletionId,
            options.After,
            options.Limit,
            options.Order,
            requestOptions);
    }

    // CUSTOM:
    // - Call FromClientResult.
    [Experimental("OPENAI001")]
    public virtual ClientResult<ChatCompletion> UpdateChatCompletion(string completionId, IDictionary<string, string> metadata, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNullOrEmpty(completionId, nameof(completionId));
        Argument.AssertNotNull(metadata, nameof(metadata));

        InternalUpdateChatCompletionRequest spreadModel = new InternalUpdateChatCompletionRequest(metadata, null);
        ClientResult result = this.UpdateChatCompletion(completionId, spreadModel, cancellationToken.CanBeCanceled ? new RequestOptions { CancellationToken = cancellationToken } : null);
        return ClientResult.FromValue((ChatCompletion)result, result.GetRawResponse());
    }

    // CUSTOM:
    // - Call FromClientResult.
    [Experimental("OPENAI001")]
    public virtual async Task<ClientResult<ChatCompletion>> UpdateChatCompletionAsync(string completionId, IDictionary<string, string> metadata, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNullOrEmpty(completionId, nameof(completionId));
        Argument.AssertNotNull(metadata, nameof(metadata));

        InternalUpdateChatCompletionRequest spreadModel = new InternalUpdateChatCompletionRequest(metadata, null);
        ClientResult result = await this.UpdateChatCompletionAsync(completionId, spreadModel, cancellationToken.CanBeCanceled ? new RequestOptions { CancellationToken = cancellationToken } : null).ConfigureAwait(false);
        return ClientResult.FromValue((ChatCompletion)result, result.GetRawResponse());
    }

    // CUSTOM:
    // - Added Experimental attribute.
    // - Call FromClientResult.
    [Experimental("OPENAI001")]
    public virtual async Task<ClientResult<ChatCompletionDeletionResult>> DeleteChatCompletionAsync(string completionId, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNullOrEmpty(completionId, nameof(completionId));

        ClientResult result = await DeleteChatCompletionAsync(completionId, cancellationToken.CanBeCanceled ? new RequestOptions { CancellationToken = cancellationToken } : null).ConfigureAwait(false);
        return ClientResult.FromValue((ChatCompletionDeletionResult)result, result.GetRawResponse());
    }

    // CUSTOM:
    // - Added Experimental attribute.
    // - Call FromClientResult.
    [Experimental("OPENAI001")]
    public virtual ClientResult<ChatCompletionDeletionResult> DeleteChatCompletion(string completionId, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNullOrEmpty(completionId, nameof(completionId));

        ClientResult result = DeleteChatCompletion(completionId, cancellationToken.CanBeCanceled ? new RequestOptions { CancellationToken = cancellationToken } : null);
        return ClientResult.FromValue((ChatCompletionDeletionResult)result, result.GetRawResponse());
    }

    [Experimental("OPENAI001")]
    [OverloadResolutionPriority(2)]
    public virtual ClientResult<ChatCompletionList> GetChatCompletions(GetChatCompletionsOptions options, CancellationToken cancellationToken = default)
    {
        PipelineMessage message = CreateGetChatCompletionsRequest(options.After, options.Limit, options.Order, options.Metadata, options.Model, cancellationToken.ToRequestOptions());
        ClientResult result = ClientResult.FromResponse(Pipeline.ProcessMessage(message, cancellationToken.ToRequestOptions()));
        return ClientResult.FromValue((ChatCompletionList)result, result.GetRawResponse());
    }

    [Experimental("OPENAI001")]
    [OverloadResolutionPriority(2)]
    public virtual async Task<ClientResult<ChatCompletionList>> GetChatCompletionsAsync(GetChatCompletionsOptions options, CancellationToken cancellationToken = default)
    {
        PipelineMessage message = CreateGetChatCompletionsRequest(options.After, options.Limit, options.Order, options.Metadata, options.Model, cancellationToken.ToRequestOptions());
        ClientResult result = ClientResult.FromResponse(await Pipeline.ProcessMessageAsync(message, cancellationToken.ToRequestOptions()));
        return ClientResult.FromValue((ChatCompletionList)result, result.GetRawResponse());
    }

    [Experimental("OPENAI001")]
    [OverloadResolutionPriority(1)]
    public virtual CollectionResult GetChatCompletions(GetChatCompletionsOptions options, RequestOptions requestOptions)
    {
        return new ChatClientGetChatCompletionsCollectionResult(
            this,
            options.After,
            options.Limit,
            options.Order,
            options.Metadata,
            options.Model,
            requestOptions);
    }

    [Experimental("OPENAI001")]
    [OverloadResolutionPriority(1)]
    public virtual AsyncCollectionResult GetChatCompletionsAsync(GetChatCompletionsOptions options, RequestOptions requestOptions)
    {
        return new ChatClientGetChatCompletionsAsyncCollectionResult(
            this,
            options.After,
            options.Limit,
            options.Order,
            options.Metadata,
            options.Model,
            requestOptions);
    }

    [Experimental("OPENAI001")]
    [OverloadResolutionPriority(1)]
    public virtual ClientResult UpdateChatCompletion(UpdateChatCompletionOptions options, RequestOptions requestOptions = null)
    {
        Argument.AssertNotNull(options, nameof(options));

        using PipelineMessage message = CreateUpdateChatCompletionRequest(options.CompletionId, options, requestOptions);
        return ClientResult.FromResponse(Pipeline.ProcessMessage(message, requestOptions));
    }

    [Experimental("OPENAI001")]
    [OverloadResolutionPriority(1)]
    public async virtual Task<ClientResult> UpdateChatCompletionAsync(UpdateChatCompletionOptions options, RequestOptions requestOptions = null)
    {
        Argument.AssertNotNull(options, nameof(options));

        using PipelineMessage message = CreateUpdateChatCompletionRequest(options.CompletionId, options, requestOptions);
        return ClientResult.FromResponse(await Pipeline.ProcessMessageAsync(message, requestOptions).ConfigureAwait(false));
    }

    [Experimental("OPENAI001")]
    [OverloadResolutionPriority(2)]
    public virtual ClientResult<ChatCompletionResult> UpdateChatCompletion(UpdateChatCompletionOptions options, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNull(options, nameof(options));

        RequestOptions requestOptions = cancellationToken.ToRequestOptions();
        using PipelineMessage message = CreateUpdateChatCompletionRequest(options.CompletionId, options, requestOptions);
        var result = ClientResult.FromResponse(Pipeline.ProcessMessage(message, requestOptions));
        return ClientResult.FromValue((ChatCompletionResult)result.GetRawResponse().Content, result.GetRawResponse());
    }

    [Experimental("OPENAI001")]
    [OverloadResolutionPriority(2)]
    public async virtual Task<ClientResult<ChatCompletionResult>> UpdateChatCompletionAsync(UpdateChatCompletionOptions options, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNull(options, nameof(options));

        RequestOptions requestOptions = cancellationToken.ToRequestOptions();
        using PipelineMessage message = CreateUpdateChatCompletionRequest(options.CompletionId, options, requestOptions);
        var result = ClientResult.FromResponse(await Pipeline.ProcessMessageAsync(message, requestOptions).ConfigureAwait(false));
        return ClientResult.FromValue((ChatCompletionResult)result.GetRawResponse().Content, result.GetRawResponse());
    }

    internal void CreateChatCompletionOptions(IEnumerable<ChatMessage> messages, ref ChatCompletionOptions options, bool stream = false)
    {
        options.Messages = messages.ToList();
        options.Model = Model;
        if (stream)
        {
            options.Stream = true;
            options.StreamOptions = s_includeUsageStreamOptions;
        }
        else
        {
            options.Stream = null;
            options.StreamOptions = null;
        }
    }
}