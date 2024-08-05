using System;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace OpenAI.Chat;

[CodeGenClient("Chat")]
[CodeGenSuppress("CreateChatCompletionAsync", typeof(ChatCompletionOptions))]
[CodeGenSuppress("CreateChatCompletion", typeof(ChatCompletionOptions))]
public partial class ChatClient
{
    private readonly string _model;

    /// <summary>
    /// Initializes a new instance of <see cref="ChatClient"/> that will use an API key when authenticating.
    /// </summary>
    /// <param name="model"> The model name for chat completions that the client should use. </param>
    /// <param name="credential"> The API key used to authenticate with the service endpoint. </param>
    /// <param name="options"> Additional options to customize the client. </param>
    /// <exception cref="ArgumentNullException"> The provided <paramref name="credential"/> was null. </exception>
    public ChatClient(string model, ApiKeyCredential credential, OpenAIClientOptions options = null)
        : this(
              OpenAIClient.CreatePipeline(OpenAIClient.GetApiKey(credential, requireExplicitCredential: true), options),
              model,
              OpenAIClient.GetEndpoint(options),
              options)
    { }

    /// <summary>
    /// Initializes a new instance of <see cref="ChatClient"/> that will use an API key from the OPENAI_API_KEY
    /// environment variable when authenticating.
    /// </summary>
    /// <remarks>
    /// To provide an explicit credential instead of using the environment variable, use an alternate constructor like
    /// <see cref="ChatClient(string,ApiKeyCredential,OpenAIClientOptions)"/>.
    /// </remarks>
    /// <param name="model"> The model name for chat completions that the client should use. </param>
    /// <param name="options"> Additional options to customize the client. </param>
    /// <exception cref="InvalidOperationException"> The OPENAI_API_KEY environment variable was not found. </exception>
    public ChatClient(string model, OpenAIClientOptions options = null)
        : this(
              OpenAIClient.CreatePipeline(OpenAIClient.GetApiKey(), options),
              model,
              OpenAIClient.GetEndpoint(options),
              options)
    { }

    /// <summary>
    /// Initializes a new instance of <see cref="ChatClient"/>.
    /// </summary>
    /// <param name="pipeline"> The <see cref="ClientPipeline"/> instance to use. </param>
    /// <param name="model"> The model name to use. </param>
    /// <param name="endpoint"> The endpoint to use. </param>
    protected internal ChatClient(ClientPipeline pipeline, string model, Uri endpoint, OpenAIClientOptions options)
    {
        Argument.AssertNotNullOrEmpty(model, nameof(model));

        _model = model;
        _pipeline = pipeline;
        _endpoint = endpoint;
    }

    /// <summary>
    /// Generates a single chat completion result for a provided set of input chat messages.
    /// </summary>
    /// <param name="messages"> The messages to provide as input and history for chat completion. </param>
    /// <param name="options"> Additional options for the chat completion request. </param>
    /// <param name="cancellationToken">A token that can be used to cancel this method call.</param>
    /// <returns> A result for a single chat completion. </returns>
    public virtual async Task<ClientResult<ChatCompletion>> CompleteChatAsync(IEnumerable<ChatMessage> messages, ChatCompletionOptions options = null, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNullOrEmpty(messages, nameof(messages));

        options ??= new();
        CreateChatCompletionOptions(messages, ref options);

        using BinaryContent content = options.ToBinaryContent();

        ClientResult result = await CompleteChatAsync(content, cancellationToken.ToRequestOptions()).ConfigureAwait(false);
        return ClientResult.FromValue(ChatCompletion.FromResponse(result.GetRawResponse()), result.GetRawResponse());
    }

    /// <summary>
    /// Generates a single chat completion result for a provided set of input chat messages.
    /// </summary>
    /// <param name="messages"> The messages to provide as input and history for chat completion. </param>
    /// <returns> A result for a single chat completion. </returns>
    public virtual async Task<ClientResult<ChatCompletion>> CompleteChatAsync(params ChatMessage[] messages)
        => await CompleteChatAsync(messages, default(ChatCompletionOptions)).ConfigureAwait(false);

    /// <summary>
    /// Generates a single chat completion result for a provided set of input chat messages.
    /// </summary>
    /// <param name="messages"> The messages to provide as input and history for chat completion. </param>
    /// <param name="options"> Additional options for the chat completion request. </param>
    /// <param name="cancellationToken">A token that can be used to cancel this method call.</param>
    /// <returns> A result for a single chat completion. </returns>
    public virtual ClientResult<ChatCompletion> CompleteChat(IEnumerable<ChatMessage> messages, ChatCompletionOptions options = null, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNullOrEmpty(messages, nameof(messages));

        options ??= new();
        CreateChatCompletionOptions(messages, ref options);

        using BinaryContent content = options.ToBinaryContent();
        ClientResult result = CompleteChat(content, cancellationToken.ToRequestOptions());
        return ClientResult.FromValue(ChatCompletion.FromResponse(result.GetRawResponse()), result.GetRawResponse());

    }

    /// <summary>
    /// Generates a single chat completion result for a provided set of input chat messages.
    /// </summary>
    /// <param name="messages"> The messages to provide as input and history for chat completion. </param>
    /// <returns> A result for a single chat completion. </returns>
    public virtual ClientResult<ChatCompletion> CompleteChat(params ChatMessage[] messages)
        => CompleteChat(messages, default(ChatCompletionOptions));

    /// <summary>
    /// Begins a streaming response for a chat completion request using the provided chat messages as input and
    /// history.
    /// </summary>
    /// <remarks>
    /// <see cref="AsyncCollectionResult{T}"/> can be enumerated over using the <c>await foreach</c> pattern using the
    /// <see cref="IAsyncEnumerable{T}"/> interface.
    /// </remarks>
    /// <param name="messages"> The messages to provide as input for chat completion. </param>
    /// <param name="options"> Additional options for the chat completion request. </param>
    /// <param name="cancellationToken">A token that can be used to cancel this method call.</param>
    /// <returns> A streaming result with incremental chat completion updates. </returns>
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
    /// Begins a streaming response for a chat completion request using the provided chat messages as input and
    /// history.
    /// </summary>
    /// <remarks>
    /// <see cref="AsyncCollectionResult{T}"/> can be enumerated over using the <c>await foreach</c> pattern using the
    /// <see cref="IAsyncEnumerable{T}"/> interface.
    /// </remarks>
    /// <param name="messages"> The messages to provide as input for chat completion. </param>
    /// <returns> A streaming result with incremental chat completion updates. </returns>
    public virtual AsyncCollectionResult<StreamingChatCompletionUpdate> CompleteChatStreamingAsync(params ChatMessage[] messages)
        => CompleteChatStreamingAsync(messages, default(ChatCompletionOptions));

    /// <summary>
    /// Begins a streaming response for a chat completion request using the provided chat messages as input and
    /// history.
    /// </summary>
    /// <remarks>
    /// <see cref="CollectionResult{T}"/> can be enumerated over using the <c>foreach</c> pattern using the
    /// <see cref="IEnumerable{T}"/> interface.
    /// </remarks>
    /// <param name="messages"> The messages to provide as input for chat completion. </param>
    /// <param name="options"> Additional options for the chat completion request. </param>
    /// <param name="cancellationToken">A token that can be used to cancel this method call.</param>
    /// <returns> A streaming result with incremental chat completion updates. </returns>
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
    /// Begins a streaming response for a chat completion request using the provided chat messages as input and
    /// history.
    /// </summary>
    /// <remarks>
    /// <see cref="CollectionResult{T}"/> can be enumerated over using the <c>foreach</c> pattern using the
    /// <see cref="IEnumerable{T}"/> interface.
    /// </remarks>
    /// <param name="messages"> The messages to provide as input for chat completion. </param>
    /// <returns> A streaming result with incremental chat completion updates. </returns>
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