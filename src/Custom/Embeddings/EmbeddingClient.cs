using System;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace OpenAI.Embeddings;

/// <summary> The service client for the OpenAI Embeddings endpoint. </summary>
[CodeGenClient("Embeddings")]
[CodeGenSuppress("EmbeddingClient", typeof(ClientPipeline), typeof(ApiKeyCredential), typeof(Uri))]
[CodeGenSuppress("CreateEmbeddingAsync", typeof(EmbeddingGenerationOptions))]
[CodeGenSuppress("CreateEmbedding", typeof(EmbeddingGenerationOptions))]
public partial class EmbeddingClient
{
    private readonly string _model;

    // CUSTOM:
    // - Added `model` parameter.
    // - Added support for retrieving credential and endpoint from environment variables.

    /// <summary>
    /// Initializes a new instance of <see cref="EmbeddingClient"/> that will use an API key when authenticating.
    /// </summary>
    /// <param name="model"> The model name to use for audio operations. </param>
    /// <param name="credential"> The API key used to authenticate with the service endpoint. </param>
    /// <param name="options"> Additional options to customize the client. </param>
    /// <exception cref="ArgumentNullException"> The provided <paramref name="credential"/> was null. </exception>
    public EmbeddingClient(string model, ApiKeyCredential credential, OpenAIClientOptions options = default)
        : this(
              OpenAIClient.CreatePipeline(OpenAIClient.GetApiKey(credential, requireExplicitCredential: true), options),
              model,
              OpenAIClient.GetEndpoint(options),
              options)
    {}

    /// <summary>
    /// Initializes a new instance of <see cref="EmbeddingClient"/> that will use an API key from the OPENAI_API_KEY
    /// environment variable when authenticating.
    /// </summary>
    /// <remarks>
    /// To provide an explicit credential instead of using the environment variable, use an alternate constructor like
    /// <see cref="EmbeddingClient(string,ApiKeyCredential,OpenAIClientOptions)"/>.
    /// </remarks>
    /// <param name="model"> The model name to use for audio operations. </param>
    /// <param name="options"> Additional options to customize the client. </param>
    /// <exception cref="InvalidOperationException"> The OPENAI_API_KEY environment variable was not found. </exception>
    public EmbeddingClient(string model, OpenAIClientOptions options = default)
        : this(
              OpenAIClient.CreatePipeline(OpenAIClient.GetApiKey(), options),
              model,
              OpenAIClient.GetEndpoint(options),
              options)
    {}

    // CUSTOM:
    // - Added `model` parameter.

    /// <summary> Initializes a new instance of EmbeddingClient. </summary>
    /// <param name="pipeline"> The HTTP pipeline for sending and receiving REST requests and responses. </param>
    /// <param name="model"> The HTTP pipeline for sending and receiving REST requests and responses. </param>
    /// <param name="endpoint"> OpenAI Endpoint. </param>
    protected internal EmbeddingClient(ClientPipeline pipeline, string model, Uri endpoint, OpenAIClientOptions options)
    {
        Argument.AssertNotNullOrEmpty(model, nameof(model));

        _pipeline = pipeline;
        _model = model;
        _endpoint = endpoint;
    }

    // CUSTOM: Added to simplify generating a single embedding from a string input.
    /// <summary> Creates an embedding vector representing the input text. </summary>
    /// <param name="input"> The string that will be turned into an embedding. </param>
    /// <param name="options"> The <see cref="EmbeddingGenerationOptions"/> to use. </param>
    /// <param name="cancellationToken">A token that can be used to cancel this method call.</param>
    /// <exception cref="ArgumentNullException"> <paramref name="input"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="input"/> is an empty string, and was expected to be non-empty. </exception>
    public virtual async Task<ClientResult<Embedding>> GenerateEmbeddingAsync(string input, EmbeddingGenerationOptions options = null, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNullOrEmpty(input, nameof(input));

        options ??= new();
        CreateEmbeddingGenerationOptions(BinaryData.FromObjectAsJson(input), ref options);

        using BinaryContent content = options.ToBinaryContent();
        ClientResult result = await GenerateEmbeddingsAsync(content, cancellationToken.ToRequestOptions()).ConfigureAwait(false);
        return ClientResult.FromValue(EmbeddingCollection.FromResponse(result.GetRawResponse()).FirstOrDefault(), result.GetRawResponse());
    }

    // CUSTOM: Added to simplify generating a single embedding from a string input.
    /// <summary> Creates an embedding vector representing the input text. </summary>
    /// <param name="input"> The string that will be turned into an embedding. </param>
    /// <param name="options"> The <see cref="EmbeddingGenerationOptions"/> to use. </param>
    /// <param name="cancellationToken">A token that can be used to cancel this method call.</param>
    /// <exception cref="ArgumentNullException"> <paramref name="input"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="input"/> is an empty string, and was expected to be non-empty. </exception>
    public virtual ClientResult<Embedding> GenerateEmbedding(string input, EmbeddingGenerationOptions options = null, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNullOrEmpty(input, nameof(input));

        options ??= new();
        CreateEmbeddingGenerationOptions(BinaryData.FromObjectAsJson(input), ref options);

        using BinaryContent content = options.ToBinaryContent();
        ClientResult result = GenerateEmbeddings(content, cancellationToken.ToRequestOptions());
        return ClientResult.FromValue(EmbeddingCollection.FromResponse(result.GetRawResponse()).FirstOrDefault(), result.GetRawResponse());
    }

    // CUSTOM: Added to simplify passing the input as a collection of strings instead of BinaryData.
    /// <summary> Creates an embedding vector representing the input text. </summary>
    /// <param name="inputs"> The strings that will be turned into embeddings. </param>
    /// <param name="options"> The <see cref="EmbeddingGenerationOptions"/> to use. </param>
    /// <param name="cancellationToken">A token that can be used to cancel this method call.</param>
    /// <exception cref="ArgumentNullException"> <paramref name="inputs"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="inputs"/> is an empty collection, and was expected to be non-empty. </exception>
    public virtual async Task<ClientResult<EmbeddingCollection>> GenerateEmbeddingsAsync(IEnumerable<string> inputs, EmbeddingGenerationOptions options = null, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNullOrEmpty(inputs, nameof(inputs));

        options ??= new();
        CreateEmbeddingGenerationOptions(BinaryData.FromObjectAsJson(inputs), ref options);

        using BinaryContent content = options.ToBinaryContent();
        ClientResult result = await GenerateEmbeddingsAsync(content, cancellationToken.ToRequestOptions()).ConfigureAwait(false);
        return ClientResult.FromValue(EmbeddingCollection.FromResponse(result.GetRawResponse()), result.GetRawResponse());

    }

    // CUSTOM: Added to simplify passing the input as a collection of strings instead of BinaryData.
    /// <summary> Creates an embedding vector representing the input text. </summary>
    /// <param name="inputs"> The strings that will be turned into embeddings. </param>
    /// <param name="options"> The <see cref="EmbeddingGenerationOptions"/> to use. </param>
    /// <param name="cancellationToken">A token that can be used to cancel this method call.</param>
    /// <exception cref="ArgumentNullException"> <paramref name="inputs"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="inputs"/> is an empty collection, and was expected to be non-empty. </exception>
    public virtual ClientResult<EmbeddingCollection> GenerateEmbeddings(IEnumerable<string> inputs, EmbeddingGenerationOptions options = null, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNullOrEmpty(inputs, nameof(inputs));

        options ??= new();
        CreateEmbeddingGenerationOptions(BinaryData.FromObjectAsJson(inputs), ref options);

        using BinaryContent content = options.ToBinaryContent();
        ClientResult result = GenerateEmbeddings(content, cancellationToken.ToRequestOptions());
        return ClientResult.FromValue(EmbeddingCollection.FromResponse(result.GetRawResponse()), result.GetRawResponse());
    }

    // CUSTOM: Added to simplify passing the input as a collection of a collection of tokens instead of BinaryData.
    /// <summary> Creates an embedding vector representing the input text. </summary>
    /// <param name="inputs"> The strings that will be turned into embeddings. </param>
    /// <param name="options"> The <see cref="EmbeddingGenerationOptions"/> to use. </param>
    /// <param name="cancellationToken">A token that can be used to cancel this method call.</param>
    /// <exception cref="ArgumentNullException"> <paramref name="inputs"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="inputs"/> is an empty collection, and was expected to be non-empty. </exception>
    public virtual async Task<ClientResult<EmbeddingCollection>> GenerateEmbeddingsAsync(IEnumerable<IEnumerable<int>> inputs, EmbeddingGenerationOptions options = null, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNullOrEmpty(inputs, nameof(inputs));

        options ??= new();
        CreateEmbeddingGenerationOptions(BinaryData.FromObjectAsJson(inputs), ref options);

        using BinaryContent content = options.ToBinaryContent();
        ClientResult result = await GenerateEmbeddingsAsync(content, cancellationToken.ToRequestOptions()).ConfigureAwait(false);
        return ClientResult.FromValue(EmbeddingCollection.FromResponse(result.GetRawResponse()), result.GetRawResponse());
    }

    // CUSTOM: Added to simplify passing the input as a collection of a collection of tokens instead of BinaryData.
    /// <summary> Creates an embedding vector representing the input text. </summary>
    /// <param name="inputs"> The strings that will be turned into embeddings. </param>
    /// <param name="options"> The <see cref="EmbeddingGenerationOptions"/> to use. </param>
    /// <param name="cancellationToken">A token that can be used to cancel this method call.</param>
    /// <exception cref="ArgumentNullException"> <paramref name="inputs"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="inputs"/> is an empty collection, and was expected to be non-empty. </exception>
    public virtual ClientResult<EmbeddingCollection> GenerateEmbeddings(IEnumerable<IEnumerable<int>> inputs, EmbeddingGenerationOptions options = null, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNullOrEmpty(inputs, nameof(inputs));

        options ??= new();
        CreateEmbeddingGenerationOptions(BinaryData.FromObjectAsJson(inputs), ref options);

        using BinaryContent content = options.ToBinaryContent();
        ClientResult result = GenerateEmbeddings(content, cancellationToken.ToRequestOptions());
        return ClientResult.FromValue(EmbeddingCollection.FromResponse(result.GetRawResponse()), result.GetRawResponse());
    }

    private void CreateEmbeddingGenerationOptions(BinaryData input, ref EmbeddingGenerationOptions options)
    {
        options.Input = input;
        options.Model = _model;
        options.EncodingFormat = InternalCreateEmbeddingRequestEncodingFormat.Base64;
    }
}
