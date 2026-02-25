using Microsoft.TypeSpec.Generator.Customizations;
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
// - Suppressed GetResponse convenience methods in favor of methods that take a property bag.
// - Suppressed GetResponseInputItems convenience methods in favor of methods that take a property bag.
// - Suppressed GetResponseInputItems protocol methods that return CollectionResult in favor of returning ClientResult.
[CodeGenType("Responses")]
[CodeGenSuppress("GetResponse", typeof(string), typeof(IEnumerable<IncludedResponseProperty>), typeof(bool?), typeof(int?), typeof(bool?), typeof(CancellationToken))]
[CodeGenSuppress("GetResponseAsync", typeof(string), typeof(IEnumerable<IncludedResponseProperty>), typeof(bool?), typeof(int?), typeof(bool?), typeof(CancellationToken))]
[CodeGenSuppress("GetResponseInputItems", typeof(string), typeof(ResponseItemCollectionOptions), typeof(CancellationToken))]
[CodeGenSuppress("GetResponseInputItemsAsync", typeof(string), typeof(ResponseItemCollectionOptions), typeof(CancellationToken))]
[CodeGenSuppress("GetResponseInputItems", typeof(string), typeof(int?), typeof(string), typeof(string), typeof(string), typeof(RequestOptions))]
[CodeGenSuppress("GetResponseInputItemsAsync", typeof(string), typeof(int?), typeof(string), typeof(string), typeof(string), typeof(RequestOptions))]

public partial class ResponsesClient
{
    private readonly string _model;

    // CUSTOM: Added as a convenience.
    /// <summary> Initializes a new instance of <see cref="ResponsesClient"/>. </summary>
    /// <param name="model"> The name of the model to use in requests sent to the service. To learn more about the available models, see <see href="https://platform.openai.com/docs/models"/>. </param>
    /// <param name="apiKey"> The API key to authenticate with the service. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="model"/> or <paramref name="apiKey"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="model"/> is an empty string, and was expected to be non-empty. </exception>
    public ResponsesClient(string model, string apiKey) : this(model, new ApiKeyCredential(apiKey), new OpenAIClientOptions())
    {
    }

    // CUSTOM:
    // - Added `model` parameter.
    // - Used a custom pipeline.
    // - Demoted the endpoint parameter to be a property in the options class.
    /// <summary> Initializes a new instance of <see cref="ResponsesClient"/>. </summary>
    /// <param name="model"> The name of the model to use in requests sent to the service. To learn more about the available models, see <see href="https://platform.openai.com/docs/models"/>. </param>
    /// <param name="credential"> The <see cref="ApiKeyCredential"/> to authenticate with the service. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="model"/> or <paramref name="credential"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="model"/> is an empty string, and was expected to be non-empty. </exception>
    public ResponsesClient(string model, ApiKeyCredential credential) : this(model, credential, new OpenAIClientOptions())
    {
    }

    // CUSTOM:
    // - Added `model` parameter.
    // - Used a custom pipeline.
    // - Demoted the endpoint parameter to be a property in the options class.
    /// <summary> Initializes a new instance of <see cref="ResponsesClient"/>. </summary>
    /// <param name="model"> The name of the model to use in requests sent to the service. To learn more about the available models, see <see href="https://platform.openai.com/docs/models"/>. </param>
    /// <param name="credential"> The <see cref="ApiKeyCredential"/> to authenticate with the service. </param>
    /// <param name="options"> The options to configure the client. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="model"/> or <paramref name="credential"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="model"/> is an empty string, and was expected to be non-empty. </exception>
    public ResponsesClient(string model, ApiKeyCredential credential, OpenAIClientOptions options) : this(model, OpenAIClient.CreateApiKeyAuthenticationPolicy(credential), options)
    {
    }

    // CUSTOM: Added as a convenience.
    /// <summary> Initializes a new instance of <see cref="ResponsesClient"/>. </summary>
    /// <param name="model"> The name of the model to use in requests sent to the service. To learn more about the available models, see <see href="https://platform.openai.com/docs/models"/>. </param>
    /// <param name="authenticationPolicy"> The authentication policy used to authenticate with the service. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="model"/> or <paramref name="authenticationPolicy"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="model"/> is an empty string, and was expected to be non-empty. </exception>
    public ResponsesClient(string model, AuthenticationPolicy authenticationPolicy) : this(model, authenticationPolicy, new OpenAIClientOptions())
    {
    }

    // CUSTOM: Added as a convenience.
    /// <summary> Initializes a new instance of <see cref="ResponsesClient"/>. </summary>
    /// <param name="model"> The name of the model to use in requests sent to the service. To learn more about the available models, see <see href="https://platform.openai.com/docs/models"/>. </param>
    /// <param name="authenticationPolicy"> The authentication policy used to authenticate with the service. </param>
    /// <param name="options"> The options to configure the client. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="model"/> or <paramref name="authenticationPolicy"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="model"/> is an empty string, and was expected to be non-empty. </exception>
    public ResponsesClient(string model, AuthenticationPolicy authenticationPolicy, OpenAIClientOptions options)
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
    /// <summary> Initializes a new instance of <see cref="ResponsesClient"/>. </summary>
    /// <param name="pipeline"> The HTTP pipeline to send and receive REST requests and responses. </param>
    /// <param name="model"> The name of the model to use in requests sent to the service. To learn more about the available models, see <see href="https://platform.openai.com/docs/models"/>. </param>
    /// <param name="options"> The options to configure the client. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="pipeline"/> or <paramref name="model"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="model"/> is an empty string, and was expected to be non-empty. </exception>
    protected internal ResponsesClient(ClientPipeline pipeline, string model, OpenAIClientOptions options)
    {
        Argument.AssertNotNull(pipeline, nameof(pipeline));
        Argument.AssertNotNullOrEmpty(model, nameof(model));
        options ??= new OpenAIClientOptions();

        _model = model;
        Pipeline = pipeline;
        _endpoint = OpenAIClient.GetEndpoint(options);
    }

    [Experimental("SCME0002")]
    public ResponsesClient(ResponsesClientSettings settings)
        : this(settings?.Model, AuthenticationPolicy.Create(settings), settings?.Options)
    {
    }

    /// <summary>
    /// Gets the name of the model used in requests sent to the service.
    /// </summary>
    [Experimental("OPENAI001")]
    public string Model => _model;

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

        ClientResult result = CreateResponse(CreatePerCallOptions(options), cancellationToken.ToRequestOptions());
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

        ClientResult result = await CreateResponseAsync((BinaryContent)CreatePerCallOptions(options), requestOptions).ConfigureAwait(false);
        return ClientResult.FromValue((ResponseResult)result, result.GetRawResponse());
    }

    // CUSTOM: Added convenience method with no options.
    public virtual ClientResult<ResponseResult> CreateResponse(IEnumerable<ResponseItem> inputItems, string previousResponseId = null, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNull(inputItems, nameof(inputItems));

        var options = new CreateResponseOptions
        {
            PreviousResponseId = previousResponseId,
        };

        foreach (var item in inputItems)
        {
            options.InputItems.Add(item);
        }

        return CreateResponse(options, cancellationToken);
    }

    // CUSTOM: Added convenience method with no options.
    public virtual Task<ClientResult<ResponseResult>> CreateResponseAsync(IEnumerable<ResponseItem> inputItems, string previousResponseId = null, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNull(inputItems, nameof(inputItems));

        var options = new CreateResponseOptions
        {
            PreviousResponseId = previousResponseId,
        };

        foreach (var item in inputItems)
        {
            options.InputItems.Add(item);
        }

        return CreateResponseAsync(options, cancellationToken);
    }

    // CUSTOM: Added convenience method with no options.
    public virtual ClientResult<ResponseResult> CreateResponse(string userInputText, string previousResponseId = null, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNullOrEmpty(userInputText, nameof(userInputText));

        var options = new CreateResponseOptions
        {
            PreviousResponseId = previousResponseId,
        };

        options.InputItems.Add(ResponseItem.CreateUserMessageItem(userInputText));

        return CreateResponse(options, cancellationToken);
    }

    // CUSTOM: Added convenience method with no options.
    public virtual Task<ClientResult<ResponseResult>> CreateResponseAsync(string userInputText, string previousResponseId = null, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNullOrEmpty(userInputText, nameof(userInputText));

        var options = new CreateResponseOptions
        {
            PreviousResponseId = previousResponseId,
        };

        options.InputItems.Add(ResponseItem.CreateUserMessageItem(userInputText));

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
            () => CreateResponse(CreatePerCallOptions(options), cancellationToken.ToRequestOptions(streaming: true)),
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
            async () => await CreateResponseAsync((BinaryContent)CreatePerCallOptions(options), requestOptions).ConfigureAwait(false),
            StreamingResponseUpdate.DeserializeStreamingResponseUpdate,
            requestOptions.CancellationToken);
    }

    // CUSTOM: Added convenience method with no options.
    public virtual CollectionResult<StreamingResponseUpdate> CreateResponseStreaming(IEnumerable<ResponseItem> inputItems, string previousResponseId = null, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNull(inputItems, nameof(inputItems));

        var options = new CreateResponseOptions
        {
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
    public virtual AsyncCollectionResult<StreamingResponseUpdate> CreateResponseStreamingAsync(IEnumerable<ResponseItem> inputItems, string previousResponseId = null, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNull(inputItems, nameof(inputItems));

        var options = new CreateResponseOptions
        {
            PreviousResponseId = previousResponseId,
            StreamingEnabled = true,
        };

        foreach (var item in inputItems)
        {
            options.InputItems.Add(item);
        }

        return CreateResponseStreamingAsync(options, cancellationToken);
    }

    // CUSTOM: Added convenience method with no options.
    public virtual CollectionResult<StreamingResponseUpdate> CreateResponseStreaming(string userInputText, string previousResponseId = null, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNullOrEmpty(userInputText, nameof(userInputText));

        var options = new CreateResponseOptions
        {
            PreviousResponseId = previousResponseId,
            StreamingEnabled = true,
        };

        options.InputItems.Add(ResponseItem.CreateUserMessageItem(userInputText));

        return CreateResponseStreaming(options, cancellationToken);
    }

    // CUSTOM: Added convenience method with no options.
    public virtual AsyncCollectionResult<StreamingResponseUpdate> CreateResponseStreamingAsync(string userInputText, string previousResponseId = null, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNullOrEmpty(userInputText, nameof(userInputText));

        var options = new CreateResponseOptions
        {
            PreviousResponseId = previousResponseId,
            StreamingEnabled = true,
        };

        options.InputItems.Add(ResponseItem.CreateUserMessageItem(userInputText));

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

        var options = new GetResponseOptions(responseId);

        return GetResponse(options, cancellationToken);
    }

    // CUSTOM: Added convenience method with no options.
    public virtual Task<ClientResult<ResponseResult>> GetResponseAsync(string responseId, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNullOrEmpty(responseId, nameof(responseId));

        var options = new GetResponseOptions(responseId);

        return GetResponseAsync(options, cancellationToken);
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

        var options = new GetResponseOptions(responseId)
        {
            StreamingEnabled = true,
        };

        return GetResponseStreaming(options, cancellationToken);
    }

    // CUSTOM: Added convenience method with no options.
    public virtual AsyncCollectionResult<StreamingResponseUpdate> GetResponseStreamingAsync(string responseId, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNullOrEmpty(responseId, nameof(responseId));

        var options = new GetResponseOptions(responseId)
        {
            StreamingEnabled = true,
        };

        return GetResponseStreamingAsync(options, cancellationToken);
    }

    #endregion

    #region GetResponseInputItems

    // CUSTOM: Added protocol method that returns ClientResult.
    public virtual ClientResult GetResponseInputItemCollectionPage(string responseId, int? limit, string order, string after, string before, RequestOptions options)
    {
        Argument.AssertNotNullOrEmpty(responseId, nameof(responseId));

        using PipelineMessage message = CreateGetResponseInputItemsRequest(responseId, limit, order, after, before, options);
        return ClientResult.FromResponse(Pipeline.ProcessMessage(message, options));
    }

    // CUSTOM: Added protocol method that returns ClientResult.
    public virtual async Task<ClientResult> GetResponseInputItemCollectionPageAsync(string responseId, int? limit, string order, string after, string before, RequestOptions options)
    {
        Argument.AssertNotNullOrEmpty(responseId, nameof(responseId));

        using PipelineMessage message = CreateGetResponseInputItemsRequest(responseId, limit, order, after, before, options);
        return ClientResult.FromResponse(await Pipeline.ProcessMessageAsync(message, options).ConfigureAwait(false));
    }

    // CUSTOM: Added protocol model method.
    public virtual ClientResult<ResponseItemCollectionPage> GetResponseInputItemCollectionPage(ResponseItemCollectionOptions options, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNull(options, nameof(options));
        Argument.AssertNotNullOrEmpty(options.ResponseId, nameof(options.ResponseId));

        ClientResult result = GetResponseInputItemCollectionPage(
            responseId: options.ResponseId,
            limit: options.PageSizeLimit,
            order: options.Order?.ToString(),
            after: options.AfterId,
            before: options.BeforeId,
            cancellationToken.ToRequestOptions());
        return ClientResult.FromValue((ResponseItemCollectionPage)result, result.GetRawResponse());
    }

    // CUSTOM: Added protocol model method.
    public virtual async Task<ClientResult<ResponseItemCollectionPage>> GetResponseInputItemCollectionPageAsync(ResponseItemCollectionOptions options, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNull(options, nameof(options));
        Argument.AssertNotNullOrEmpty(options.ResponseId, nameof(options.ResponseId));

        ClientResult result = await GetResponseInputItemCollectionPageAsync(
            responseId: options.ResponseId,
            limit: options.PageSizeLimit,
            order: options.Order?.ToString(),
            after: options.AfterId,
            before: options.BeforeId,
            cancellationToken.ToRequestOptions()).ConfigureAwait(false);
        return ClientResult.FromValue((ResponseItemCollectionPage)result, result.GetRawResponse());
    }

    // CUSTOM: Added convenience method with pagination.
    public virtual CollectionResult<ResponseItem> GetResponseInputItems(ResponseItemCollectionOptions options, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNull(options, nameof(options));
        Argument.AssertNotNullOrEmpty(options.ResponseId, nameof(options.ResponseId));

        return new ResponsesClientGetResponseInputItemsCollectionResultOfT(
            client: this,
            responseId: options.ResponseId,
            limit: options.PageSizeLimit,
            order: options.Order?.ToString(),
            after: options.AfterId,
            before: options.BeforeId,
            cancellationToken.ToRequestOptions());
    }

    // CUSTOM: Added convenience method with pagination.
    public virtual AsyncCollectionResult<ResponseItem> GetResponseInputItemsAsync(ResponseItemCollectionOptions options, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNull(options, nameof(options));
        Argument.AssertNotNullOrEmpty(options.ResponseId, nameof(options.ResponseId));
        return new ResponsesClientGetResponseInputItemsAsyncCollectionResultOfT(
            client: this,
            responseId: options.ResponseId,
            limit: options.PageSizeLimit,
            order: options.Order?.ToString(),
            after: options.AfterId,
            before: options.BeforeId,
            cancellationToken.ToRequestOptions());
    }

    // CUSTOM: Added convenience method with pagination and no options.
    public virtual CollectionResult<ResponseItem> GetResponseInputItems(string responseId, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNullOrEmpty(responseId, nameof(responseId));

        var options = new ResponseItemCollectionOptions(responseId);

        return GetResponseInputItems(options, cancellationToken);
    }

    // CUSTOM: Added convenience method with pagination and no options.
    public virtual AsyncCollectionResult<ResponseItem> GetResponseInputItemsAsync(string responseId, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNullOrEmpty(responseId, nameof(responseId));

        var options = new ResponseItemCollectionOptions(responseId);

        return GetResponseInputItemsAsync(options, cancellationToken);
    }

    #endregion

    internal virtual CreateResponseOptions CreatePerCallOptions(CreateResponseOptions userProvidedOptions)
    {
        CreateResponseOptions clonedOptions = userProvidedOptions is null
            ? new()
            : userProvidedOptions.Clone();

        // If the model is null, use the default specified in the client.
        clonedOptions.Model ??= _model;

        return clonedOptions;
    }
}