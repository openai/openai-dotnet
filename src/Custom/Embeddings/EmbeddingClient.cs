using System;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace OpenAI.Embeddings;

// CUSTOM:
// - Renamed.
// - Suppressed constructor that takes endpoint parameter; endpoint is now a property in the options class.
// - Suppressed methods that only take the options parameter.
/// <summary> The service client for OpenAI embedding operations. </summary>
[CodeGenClient("Embeddings")]
[CodeGenSuppress("EmbeddingClient", typeof(ClientPipeline), typeof(ApiKeyCredential), typeof(Uri))]
[CodeGenSuppress("CreateEmbeddingAsync", typeof(EmbeddingGenerationOptions))]
[CodeGenSuppress("CreateEmbedding", typeof(EmbeddingGenerationOptions))]
public partial class EmbeddingClient
{
    private readonly string _model;

    // CUSTOM:
    // - Added `model` parameter.
    // - Used a custom pipeline.
    // - Demoted the endpoint parameter to be a property in the options class.
    /// <summary> Initializes a new instance of <see cref="EmbeddingClient">. </summary>
    /// <param name="model"> The name of the model to use in requests sent to the service. To learn more about the available models, see <see href="https://platform.openai.com/docs/models"/>. </param>
    /// <param name="credential"> The API key to authenticate with the service. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="model"/> or <paramref name="credential"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="model"/> is an empty string, and was expected to be non-empty. </exception>
    public EmbeddingClient(string model, ApiKeyCredential credential) : this(model, credential, new OpenAIClientOptions())
    {
    }

    // CUSTOM:
    // - Added `model` parameter.
    // - Used a custom pipeline.
    // - Demoted the endpoint parameter to be a property in the options class.
    /// <summary> Initializes a new instance of <see cref="EmbeddingClient">. </summary>
    /// <param name="model"> The name of the model to use in requests sent to the service. To learn more about the available models, see <see href="https://platform.openai.com/docs/models"/>. </param>
    /// <param name="credential"> The API key to authenticate with the service. </param>
    /// <param name="options"> The options to configure the client. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="model"/> or <paramref name="credential"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="model"/> is an empty string, and was expected to be non-empty. </exception>
    public EmbeddingClient(string model, ApiKeyCredential credential, OpenAIClientOptions options)
    {
        Argument.AssertNotNullOrEmpty(model, nameof(model));
        Argument.AssertNotNull(credential, nameof(credential));
        options ??= new OpenAIClientOptions();

        _model = model;
        _pipeline = OpenAIClient.CreatePipeline(credential, options);
        _endpoint = OpenAIClient.GetEndpoint(options);
    }

    // CUSTOM:
    // - Added `model` parameter.
    // - Used a custom pipeline.
    // - Demoted the endpoint parameter to be a property in the options class.
    // - Made protected.
    /// <summary> Initializes a new instance of <see cref="EmbeddingClient">. </summary>
    /// <param name="pipeline"> The HTTP pipeline to send and receive REST requests and responses. </param>
    /// <param name="model"> The name of the model to use in requests sent to the service. To learn more about the available models, see <see href="https://platform.openai.com/docs/models"/>. </param>
    /// <param name="options"> The options to configure the client. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="pipeline"/> or <paramref name="model"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="model"/> is an empty string, and was expected to be non-empty. </exception>
    protected internal EmbeddingClient(ClientPipeline pipeline, string model, OpenAIClientOptions options)
    {
        Argument.AssertNotNull(pipeline, nameof(pipeline));
        Argument.AssertNotNullOrEmpty(model, nameof(model));
        options ??= new OpenAIClientOptions();

        _model = model;
        _pipeline = pipeline;
        _endpoint = OpenAIClient.GetEndpoint(options);
    }

    // CUSTOM: Added to simplify generating a single embedding from a string input.
    /// <summary> Generates an embedding representing the text input. </summary>
    /// <param name="input"> The text input to generate an embedding for. </param>
    /// <param name="options"> The options to configure the embedding generation. </param>
    /// <param name="cancellationToken"> A token that can be used to cancel this method call. </param>
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
    /// <summary> Generates an embedding representing the text input. </summary>
    /// <param name="input"> The text input to generate an embedding for. </param>
    /// <param name="options"> The options to configure the embedding generation. </param>
    /// <param name="cancellationToken"> A token that can be used to cancel this method call. </param>
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
    /// <summary> Generates embeddings representing the text inputs. </summary>
    /// <param name="inputs"> The text inputs to generate embeddings for. </param>
    /// <param name="options"> The options to configure the embedding generation. </param>
    /// <param name="cancellationToken"> A token that can be used to cancel this method call. </param>
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
    /// <summary> Generates embeddings representing the text inputs. </summary>
    /// <param name="inputs"> The text inputs to generate embeddings for. </param>
    /// <param name="options"> The options to configure the embedding generation. </param>
    /// <param name="cancellationToken"> A token that can be used to cancel this method call. </param>
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
    /// <summary> Generates embeddings representing the text inputs. </summary>
    /// <param name="inputs"> The text inputs to generate embeddings for. </param>
    /// <param name="options"> The options to configure the embedding generation. </param>
    /// <param name="cancellationToken"> A token that can be used to cancel this method call. </param>
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
    /// <summary> Generates embeddings representing the text inputs. </summary>
    /// <param name="inputs"> The text inputs to generate embeddings for. </param>
    /// <param name="options"> The options to configure the embedding generation. </param>
    /// <param name="cancellationToken"> A token that can be used to cancel this method call. </param>
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
