using System;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OpenAI.Moderations;

/// <summary>
/// The service client for OpenAI moderation operations.
/// </summary>
[CodeGenClient("Moderations")]
[CodeGenSuppress("ModerationClient", typeof(ClientPipeline), typeof(ApiKeyCredential), typeof(Uri))]
[CodeGenSuppress("CreateModerationAsync", typeof(ModerationOptions))]
[CodeGenSuppress("CreateModeration", typeof(ModerationOptions))]
public partial class ModerationClient
{
    private readonly string _model;

    /// <summary>
    /// Initializes a new instance of <see cref="ModerationClient"/> that will use an API key when authenticating.
    /// </summary>
    /// <param name="model"> The model name to use for moderation operations. </param>
    /// <param name="credential"> The API key used to authenticate with the service endpoint. </param>
    /// <param name="options"> Additional options to customize the client. </param>
    /// <exception cref="ArgumentNullException"> The provided <paramref name="credential"/> was null. </exception>
    public ModerationClient(string model, ApiKeyCredential credential, OpenAIClientOptions options = null)
        : this(
              OpenAIClient.CreatePipeline(OpenAIClient.GetApiKey(credential, requireExplicitCredential: true), options),
              model,
              OpenAIClient.GetEndpoint(options),
              options)
    {
    }

    /// <summary>
    /// Initializes a new instance of <see cref="ModerationClient"/> that will use an API key from the OPENAI_API_KEY
    /// environment variable when authenticating.
    /// </summary>
    /// <remarks>
    /// To provide an explicit credential instead of using the environment variable, use an alternate constructor like
    /// <see cref="ModerationClient(string, ApiKeyCredential,OpenAIClientOptions)"/>.
    /// </remarks>
    /// <param name="model"> The model name to use for moderation operations. </param>
    /// <param name="options"> Additional options to customize the client. </param>
    /// <exception cref="InvalidOperationException"> The OPENAI_API_KEY environment variable was not found. </exception>
    public ModerationClient(string model, OpenAIClientOptions options = null)
        : this(
              OpenAIClient.CreatePipeline(OpenAIClient.GetApiKey(), options),
              model,
              OpenAIClient.GetEndpoint(options),
              options)
    {
    }

    /// <summary> Initializes a new instance of <see cref="ModerationClient"/>. </summary>
    /// <param name="pipeline"> The HTTP pipeline for sending and receiving REST requests and responses. </param>
    /// <param name="model"> The model name to use for moderation operations. </param>
    /// <param name="endpoint"> OpenAI Endpoint. </param>
    protected internal ModerationClient(ClientPipeline pipeline, string model, Uri endpoint, OpenAIClientOptions options)
    {
        _pipeline = pipeline;
        _model = model;
        _endpoint = endpoint;
    }

    /// <summary> Classifies if text is potentially harmful. </summary>
    /// <param name="input"> The text to classify. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="input"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="input"/> is an empty string, and was expected to be non-empty. </exception>
    public virtual async Task<ClientResult<ModerationResult>> ClassifyTextInputAsync(string input)
    {
        Argument.AssertNotNullOrEmpty(input, nameof(input));

        ModerationOptions options = new();
        CreateModerationOptions(BinaryData.FromObjectAsJson(input), ref options);

        using BinaryContent content = options.ToBinaryContent();
        ClientResult result = await ClassifyTextInputsAsync(content, (RequestOptions)null).ConfigureAwait(false);
        return ClientResult.FromValue(ModerationCollection.FromResponse(result.GetRawResponse()).FirstOrDefault(), result.GetRawResponse());
    }

    /// <summary> Classifies if text is potentially harmful. </summary>
    /// <param name="input"> The text to classify. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="input"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="input"/> is an empty string, and was expected to be non-empty. </exception>
    public virtual ClientResult<ModerationResult> ClassifyTextInput(string input)
    {
        Argument.AssertNotNullOrEmpty(input, nameof(input));

        ModerationOptions options = new();
        CreateModerationOptions(BinaryData.FromObjectAsJson(input), ref options);

        using BinaryContent content = options.ToBinaryContent();
        ClientResult result = ClassifyTextInputs(content, (RequestOptions)null);
        return ClientResult.FromValue(ModerationCollection.FromResponse(result.GetRawResponse()).FirstOrDefault(), result.GetRawResponse());
    }


    /// <summary> Classifies if text is potentially harmful. </summary>
    /// <param name="inputs"> The text to classify. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="inputs"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="inputs"/> is an empty collection, and was expected to be non-empty. </exception>
    public virtual async Task<ClientResult<ModerationCollection>> ClassifyTextInputsAsync(IEnumerable<string> inputs)
    {
        Argument.AssertNotNullOrEmpty(inputs, nameof(inputs));

        ModerationOptions options = new();
        CreateModerationOptions(BinaryData.FromObjectAsJson(inputs), ref options);

        using BinaryContent content = options.ToBinaryContent();
        ClientResult result = await ClassifyTextInputsAsync(content, (RequestOptions)null).ConfigureAwait(false);
        return ClientResult.FromValue(ModerationCollection.FromResponse(result.GetRawResponse()), result.GetRawResponse());
    }

    /// <summary> Classifies if text is potentially harmful. </summary>
    /// <param name="inputs"> The text to classify. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="inputs"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="inputs"/> is an empty collection, and was expected to be non-empty. </exception>
    public virtual ClientResult<ModerationCollection> ClassifyTextInputs(IEnumerable<string> inputs)
    {
        Argument.AssertNotNullOrEmpty(inputs, nameof(inputs));

        ModerationOptions options = new();
        CreateModerationOptions(BinaryData.FromObjectAsJson(inputs), ref options);

        using BinaryContent content = options.ToBinaryContent();
        ClientResult result = ClassifyTextInputs(content, (RequestOptions)null);
        return ClientResult.FromValue(ModerationCollection.FromResponse(result.GetRawResponse()), result.GetRawResponse());
    }

    private void CreateModerationOptions(BinaryData input, ref ModerationOptions options)
    {
        options.Input = input;
        options.Model = _model;
    }
}
