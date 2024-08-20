using OpenAI.Telemetry;
using System;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace OpenAI.Chat;

// CUSTOM:
// - Renamed.
// - Suppressed constructor that takes endpoint parameter; endpoint is now a property in the options class.
// - Suppressed methods that only take the options parameter.
/// <summary> The service client for OpenAI chat operations. </summary>
[CodeGenClient("Chat")]
[CodeGenSuppress("ChatClient", typeof(ClientPipeline), typeof(ApiKeyCredential), typeof(Uri))]
[CodeGenSuppress("CreateChatCompletionAsync", typeof(ChatCompletionOptions))]
[CodeGenSuppress("CreateChatCompletion", typeof(ChatCompletionOptions))]
public partial class ChatClient
{
    private readonly string _model;
    private readonly OpenTelemetrySource _telemetry;

    // CUSTOM:
    // - Added `model` parameter.
    // - Used a custom pipeline.
    // - Demoted the endpoint parameter to be a property in the options class.
    /// <summary> Initializes a new instance of <see cref="ChatClient">. </summary>
    /// <param name="model"> The name of the model to use in requests sent to the service. To learn more about the available models, see <see href="https://platform.openai.com/docs/models"/>. </param>
    /// <param name="credential"> The API key to authenticate with the service. </param>
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
    /// <summary> Initializes a new instance of <see cref="ChatClient">. </summary>
    /// <param name="model"> The name of the model to use in requests sent to the service. To learn more about the available models, see <see href="https://platform.openai.com/docs/models"/>. </param>
    /// <param name="credential"> The API key to authenticate with the service. </param>
    /// <param name="options"> The options to configure the client. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="model"/> or <paramref name="credential"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="model"/> is an empty string, and was expected to be non-empty. </exception>
    public ChatClient(string model, ApiKeyCredential credential, OpenAIClientOptions options)
    {
        Argument.AssertNotNullOrEmpty(model, nameof(model));
        Argument.AssertNotNull(credential, nameof(credential));
        options ??= new OpenAIClientOptions();

        _model = model;
        _pipeline = OpenAIClient.CreatePipeline(credential, options);
        _endpoint = OpenAIClient.GetEndpoint(options);
        _telemetry = new OpenTelemetrySource(model, _endpoint);
    }

    // CUSTOM:
    // - Added `model` parameter.
    // - Used a custom pipeline.
    // - Demoted the endpoint parameter to be a property in the options class.
    // - Added telemetry support.
    // - Made protected.
    /// <summary> Initializes a new instance of <see cref="ChatClient">. </summary>
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
        _pipeline = pipeline;
        _endpoint = OpenAIClient.GetEndpoint(options);
        _telemetry = new OpenTelemetrySource(model, _endpoint);
    }

    /// <summary> Generates a completion for the given chat. </summary>
    /// <param name="messages"> The messages comprising the chat so far. </param>
    /// <param name="options"> The options to configure the chat completion. </param>
    /// <param name="cancellationToken"> A token that can be used to cancel this method call. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="messages"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="messages"/> is an empty collection, and was expected to be non-empty. </exception>
    public virtual async Task<ClientResult<ChatCompletion>> CompleteChatAsync(IEnumerable<ChatMessage> messages, ChatCompletionOptions options = null, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNullOrEmpty(messages, nameof(messages));

        options ??= new();
        CreateChatCompletionOptions(messages, ref options);
        using OpenTelemetryScope scope = _telemetry.StartChatScope(options);

        try
        {
            using BinaryContent content = options.ToBinaryContent();

            ClientResult result = await CompleteChatAsync(content, cancellationToken.ToRequestOptions()).ConfigureAwait(false);
            ChatCompletion chatCompletion = ChatCompletion.FromResponse(result.GetRawResponse());
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
            ChatCompletion chatCompletion = ChatCompletion.FromResponse(result.GetRawResponse());

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
        Argument.AssertNotNull(messages, nameof(messages));

        options ??= new();
        CreateChatCompletionOptions(messages, ref options, stream: true);

        using BinaryContent content = options.ToBinaryContent();

        async Task<ClientResult> getResultAsync() =>
            await CompleteChatAsync(content, cancellationToken.ToRequestOptions(streaming: true)).ConfigureAwait(false);
        return new AsyncStreamingChatCompletionUpdateCollection(getResultAsync);
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
    public virtual CollectionResult<StreamingChatCompletionUpdate> CompleteChatStreaming(IEnumerable<ChatMessage> messages, ChatCompletionOptions options = null, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNull(messages, nameof(messages));

        options ??= new();
        CreateChatCompletionOptions(messages, ref options, stream: true);

        using BinaryContent content = options.ToBinaryContent();
        ClientResult getResult() => CompleteChat(content, cancellationToken.ToRequestOptions(streaming: true));
        return new StreamingChatCompletionUpdateCollection(getResult);
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
    ///     <see cref="AsyncCollectionResult{T}"/> implements the <see cref="IAsyncEnumerable{T}"/> interface and can be
    ///     enumerated over using the <c>await foreach</c> pattern.
    /// </remarks>
    /// <param name="messages"> The messages comprising the chat so far. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="messages"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="messages"/> is an empty collection, and was expected to be non-empty. </exception>
    public virtual CollectionResult<StreamingChatCompletionUpdate> CompleteChatStreaming(params ChatMessage[] messages)
        => CompleteChatStreaming(messages, default(ChatCompletionOptions));

    private void CreateChatCompletionOptions(IEnumerable<ChatMessage> messages, ref ChatCompletionOptions options, bool stream = false)
    {
        options.Messages = messages.ToList();
        options.Model = _model;
        options.Stream = stream
            ? true
            : null;
        options.StreamOptions = stream ? options.StreamOptions : null;
    }
}