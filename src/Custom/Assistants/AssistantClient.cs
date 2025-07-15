using System;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace OpenAI.Assistants;

/// <summary> The service client for OpenAI assistants operations. </summary>
[CodeGenType("Assistants")]
[CodeGenSuppress("AssistantClient", typeof(ClientPipeline), typeof(Uri))]
[CodeGenSuppress("CreateAssistantAsync", typeof(AssistantCreationOptions), typeof(CancellationToken))]
[CodeGenSuppress("CreateAssistant", typeof(AssistantCreationOptions), typeof(CancellationToken))]
[CodeGenSuppress("GetAssistantAsync", typeof(string))]
[CodeGenSuppress("GetAssistant", typeof(string))]
[CodeGenSuppress("ModifyAssistantAsync", typeof(string), typeof(AssistantModificationOptions))]
[CodeGenSuppress("ModifyAssistant", typeof(string), typeof(AssistantModificationOptions))]
[CodeGenSuppress("DeleteAssistantAsync", typeof(string))]
[CodeGenSuppress("DeleteAssistant", typeof(string))]
[CodeGenSuppress("GetAssistantsAsync", typeof(int?), typeof(OpenAI.VectorStores.VectorStoreCollectionOrder?), typeof(string), typeof(string), typeof(CancellationToken))]
[CodeGenSuppress("GetAssistants", typeof(int?), typeof(OpenAI.VectorStores.VectorStoreCollectionOrder?), typeof(string), typeof(string), typeof(CancellationToken))]
public partial class AssistantClient
{
    private readonly InternalAssistantMessageClient _messageSubClient;
    private readonly InternalAssistantRunClient _runSubClient;
    private readonly InternalAssistantThreadClient _threadSubClient;

    // CUSTOM: Added as a convenience.
    /// <summary> Initializes a new instance of <see cref="AssistantClient"/>. </summary>
    /// <param name="apiKey"> The API key to authenticate with the service. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="apiKey"/> is null. </exception>
    public AssistantClient(string apiKey) : this(new ApiKeyCredential(apiKey), new OpenAIClientOptions())
    {
    }

    // CUSTOM:
    // - Used a custom pipeline.
    // - Demoted the endpoint parameter to be a property in the options class.
    /// <summary> Initializes a new instance of <see cref="AssistantClient"/>. </summary>
    /// <param name="credential"> The API key to authenticate with the service. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="credential"/> is null. </exception>
    public AssistantClient(ApiKeyCredential credential) : this(credential, new OpenAIClientOptions())
    {
    }

    // CUSTOM:
    // - Used a custom pipeline.
    // - Demoted the endpoint parameter to be a property in the options class.
    /// <summary> Initializes a new instance of <see cref="AssistantClient"/>. </summary>
    /// <param name="credential"> The API key to authenticate with the service. </param>
    /// <param name="options"> The options to configure the client. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="credential"/> is null. </exception>
    public AssistantClient(ApiKeyCredential credential, OpenAIClientOptions options)
    {
        Argument.AssertNotNull(credential, nameof(credential));
        options ??= new OpenAIClientOptions();

        Pipeline = OpenAIClient.CreatePipeline(credential, options);
        _endpoint = OpenAIClient.GetEndpoint(options);
        _messageSubClient = new(Pipeline, options);
        _runSubClient = new(Pipeline, options);
        _threadSubClient = new(Pipeline, options);
    }

    // CUSTOM:
    // - Used a custom pipeline.
    // - Demoted the endpoint parameter to be a property in the options class.
    // - Made protected.
    /// <summary> Initializes a new instance of <see cref="AssistantClient"/>. </summary>
    /// <param name="pipeline"> The HTTP pipeline to send and receive REST requests and responses. </param>
    /// <param name="options"> The options to configure the client. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="pipeline"/> is null. </exception>
    protected internal AssistantClient(ClientPipeline pipeline, OpenAIClientOptions options)
    {
        Argument.AssertNotNull(pipeline, nameof(pipeline));
        options ??= new OpenAIClientOptions();

        Pipeline = pipeline;
        _endpoint = OpenAIClient.GetEndpoint(options);
        _messageSubClient = new(Pipeline, options);
        _runSubClient = new(Pipeline, options);
        _threadSubClient = new(Pipeline, options);
    }

    /// <summary> Creates a new assistant. </summary>
    /// <param name="model"> The default model that the assistant should use. </param>
    /// <param name="options"> The additional <see cref="AssistantCreationOptions"/> to use. </param>
    /// <param name="cancellationToken">A token that can be used to cancel this method call.</param>
    /// <exception cref="ArgumentException"> <paramref name="model"/> is null or empty. </exception>
    public virtual async Task<ClientResult<Assistant>> CreateAssistantAsync(string model, AssistantCreationOptions options = null, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNullOrEmpty(model, nameof(model));
        options ??= new();
        options.Model = model;

         ClientResult protocolResult = await CreateAssistantAsync(options?.ToBinaryContent(), cancellationToken.ToRequestOptions()).ConfigureAwait(false);
        return ClientResult.FromValue(Assistant.FromClientResult(protocolResult), protocolResult.GetRawResponse());
    }

    /// <summary> Creates a new assistant. </summary>
    /// <param name="model"> The default model that the assistant should use. </param>
    /// <param name="options"> The additional <see cref="AssistantCreationOptions"/> to use. </param>
    /// <param name="cancellationToken">A token that can be used to cancel this method call.</param>
    /// <exception cref="ArgumentException"> <paramref name="model"/> is null or empty. </exception>
    public virtual ClientResult<Assistant> CreateAssistant(string model, AssistantCreationOptions options = null, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNullOrEmpty(model, nameof(model));
        options ??= new();
        options.Model = model;

        ClientResult protocolResult = CreateAssistant(options?.ToBinaryContent(), cancellationToken.ToRequestOptions());
         return ClientResult.FromValue(Assistant.FromClientResult(protocolResult), protocolResult.GetRawResponse());
    }

    /// <summary>
    /// Gets a page collection holding <see cref="Assistant"/> instances.
    /// </summary>
    /// <param name="options"> Options describing the collection to return. </param>
    /// <param name="cancellationToken">A token that can be used to cancel this method call.</param>
    /// <returns> A collection of <see cref="Assistant"/>. </returns>
    public virtual AsyncCollectionResult<Assistant> GetAssistantsAsync(
        AssistantCollectionOptions options = default,
        CancellationToken cancellationToken = default)
    {
        AsyncCollectionResult result = GetAssistantsAsync(options?.PageSizeLimit, options?.Order?.ToString(), options?.AfterId, options?.BeforeId, cancellationToken.ToRequestOptions());

        if (result is not AsyncCollectionResult<Assistant> assistantCollection)
        {
            throw new InvalidOperationException("Failed to cast protocol return type to expected collection type 'AsyncCollectionResult<Assistant>'.");
        }

        return assistantCollection;
    }

    /// <summary>
    /// Rehydrates a page collection holding <see cref="Assistant"/> instances from a page token.
    /// </summary>
    /// <param name="firstPageToken"> Page token corresponding to the first page of the collection to rehydrate. </param>
    /// <param name="cancellationToken">A token that can be used to cancel this method call.</param>
    /// <returns> A collection of <see cref="Assistant"/>. </returns>
    public virtual AsyncCollectionResult<Assistant> GetAssistantsAsync(
        ContinuationToken firstPageToken,
        CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNull(firstPageToken, nameof(firstPageToken));

        AssistantCollectionPageToken pageToken = AssistantCollectionPageToken.FromToken(firstPageToken);
        AsyncCollectionResult result = GetAssistantsAsync(pageToken?.Limit, pageToken?.Order, pageToken?.After, pageToken.Before, cancellationToken.ToRequestOptions());

        if (result is not AsyncCollectionResult<Assistant> assistantCollection)
        {
            throw new InvalidOperationException("Failed to cast protocol return type to expected collection type 'AsyncCollectionResult<Assistant>'.");
        }

        return assistantCollection;
    }

    /// <summary>
    /// Gets a page collection holding <see cref="Assistant"/> instances.
    /// </summary>
    /// <param name="options"> Options describing the collection to return. </param>
    /// <param name="cancellationToken">A token that can be used to cancel this method call.</param>
    /// <returns> A collection of <see cref="Assistant"/>. </returns>
    public virtual CollectionResult<Assistant> GetAssistants(
        AssistantCollectionOptions options = default,
        CancellationToken cancellationToken = default)
    {
        CollectionResult result = GetAssistants(options?.PageSizeLimit, options?.Order?.ToString(), options?.AfterId, options?.BeforeId, cancellationToken.ToRequestOptions());

        if (result is not CollectionResult<Assistant> assistantCollection)
        {
            throw new InvalidOperationException("Failed to cast protocol return type to expected collection type 'CollectionResult<Assistant>'.");
        }

        return assistantCollection;
    }

    /// <summary>
    /// Rehydrates a page collection holding <see cref="Assistant"/> instances from a page token.
    /// </summary>
    /// <param name="firstPageToken"> Page token corresponding to the first page of the collection to rehydrate. </param>
    /// <param name="cancellationToken">A token that can be used to cancel this method call.</param>
    /// <returns> A collection of <see cref="Assistant"/>. </returns>
    public virtual CollectionResult<Assistant> GetAssistants(
        ContinuationToken firstPageToken,
        CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNull(firstPageToken, nameof(firstPageToken));

        AssistantCollectionPageToken pageToken = AssistantCollectionPageToken.FromToken(firstPageToken);
        CollectionResult result = GetAssistants(pageToken?.Limit, pageToken?.Order, pageToken?.After, pageToken.Before, cancellationToken.ToRequestOptions());

        if (result is not CollectionResult<Assistant> assistantCollection)
        {
            throw new InvalidOperationException("Failed to cast protocol return type to expected collection type 'CollectionResult<Assistant>'.");
        }

        return assistantCollection;
    }

    /// <summary>
    /// Gets an instance representing an existing <see cref="Assistant"/> based on its ID.
    /// </summary>
    /// <param name="assistantId"> The ID of the Assistant to retrieve. </param>
    /// <param name="cancellationToken">A token that can be used to cancel this method call.</param>
    /// <returns>An <see cref="Assistant"/> instance representing the state of the Assistant with the provided ID.</returns>
    public virtual async Task<ClientResult<Assistant>> GetAssistantAsync(string assistantId, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNullOrEmpty(assistantId, nameof(assistantId));

        ClientResult protocolResult = await GetAssistantAsync(assistantId, cancellationToken.ToRequestOptions()).ConfigureAwait(false);
        return ClientResult.FromValue(Assistant.FromClientResult(protocolResult), protocolResult.GetRawResponse());
    }

    /// <summary>
    /// Gets an instance representing an existing <see cref="Assistant"/> based on its ID.
    /// </summary>
    /// <param name="assistantId"> The ID of the Assistant to retrieve. </param>
    /// <param name="cancellationToken">A token that can be used to cancel this method call.</param>
    /// <returns>An <see cref="Assistant"/> instance representing the state of the Assistant with the provided ID.</returns>
    public virtual ClientResult<Assistant> GetAssistant(string assistantId, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNullOrEmpty(assistantId, nameof(assistantId));

        ClientResult protocolResult = GetAssistant(assistantId, cancellationToken.ToRequestOptions());
        return ClientResult.FromValue(Assistant.FromClientResult(protocolResult), protocolResult.GetRawResponse());
    }

    /// <summary>
    /// Modifies an existing <see cref="Assistant"/>.
    /// </summary>
    /// <param name="assistantId"> The ID of the Assistant to retrieve. </param>
    /// <param name="options"> The new options to apply to the existing Assistant. </param>
    /// <param name="cancellationToken"> A token that can be used to cancel this method call. </param>
    /// <returns> An updated <see cref="Assistant"/> instance representing the state of the Assistant with the provided ID. </returns>
    public virtual async Task<ClientResult<Assistant>> ModifyAssistantAsync(string assistantId, AssistantModificationOptions options, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNullOrEmpty(assistantId, nameof(assistantId));
        Argument.AssertNotNull(options, nameof(options));

        using BinaryContent content = options?.ToBinaryContent();
        ClientResult protocolResult
            = await ModifyAssistantAsync(assistantId, content, cancellationToken.ToRequestOptions()).ConfigureAwait(false);
        return ClientResult.FromValue(Assistant.FromClientResult(protocolResult), protocolResult.GetRawResponse());
    }

    /// <summary>
    /// Modifies an existing <see cref="Assistant"/>.
    /// </summary>
    /// <param name="assistantId"> The ID of the Assistant to retrieve. </param>
    /// <param name="options"> The new options to apply to the existing Assistant. </param>
    /// <param name="cancellationToken"> A token that can be used to cancel this method call. </param>
    /// <returns> An updated <see cref="Assistant"/> instance representing the state of the Assistant with the provided ID. </returns>
    public virtual ClientResult<Assistant> ModifyAssistant(string assistantId, AssistantModificationOptions options, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNullOrEmpty(assistantId, nameof(assistantId));
        Argument.AssertNotNull(options, nameof(options));

        using BinaryContent content = options?.ToBinaryContent();
        ClientResult protocolResult = ModifyAssistant(assistantId, content, null);
        return ClientResult.FromValue(Assistant.FromClientResult(protocolResult), protocolResult.GetRawResponse());
    }

    /// <summary>
    /// Deletes an existing <see cref="Assistant"/>.
    /// </summary>
    /// <param name="assistantId"> The ID of the assistant to delete. </param>
    /// <param name="cancellationToken">A token that can be used to cancel this method call.</param>
    /// <returns> A <see cref="AssistantDeletionResult"/> instance. </returns>
    public virtual async Task<ClientResult<AssistantDeletionResult>> DeleteAssistantAsync(string assistantId, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNullOrEmpty(assistantId, nameof(assistantId));

        ClientResult protocolResult = await DeleteAssistantAsync(assistantId, cancellationToken.ToRequestOptions()).ConfigureAwait(false);
        return ClientResult.FromValue(AssistantDeletionResult.FromClientResult(protocolResult), protocolResult.GetRawResponse());
    }

    /// <summary>
    /// Deletes an existing <see cref="Assistant"/>.
    /// </summary>
    /// <param name="assistantId"> The ID of the assistant to delete. </param>
    /// <param name="cancellationToken">A token that can be used to cancel this method call.</param>
    /// <returns> A <see cref="AssistantDeletionResult"/> instance. </returns>
    public virtual ClientResult<AssistantDeletionResult> DeleteAssistant(string assistantId, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNullOrEmpty(assistantId, nameof(assistantId));

        ClientResult protocolResult = DeleteAssistant(assistantId, cancellationToken.ToRequestOptions());
        return ClientResult.FromValue(AssistantDeletionResult.FromClientResult(protocolResult), protocolResult.GetRawResponse());
    }

    /// <summary>
    /// Creates a new <see cref="AssistantThread"/>.
    /// </summary>
    /// <param name="options"> Additional options to use when creating the thread. </param>
    /// <param name="cancellationToken">A token that can be used to cancel this method call.</param>
    /// <returns> A new thread. </returns>
    public virtual async Task<ClientResult<AssistantThread>> CreateThreadAsync(ThreadCreationOptions options = null, CancellationToken cancellationToken = default)
    {
        ClientResult protocolResult = await CreateThreadAsync(options?.ToBinaryContent(), cancellationToken.ToRequestOptions()).ConfigureAwait(false);
        return ClientResult.FromValue(AssistantThread.FromClientResult(protocolResult), protocolResult.GetRawResponse());;
    }

    /// <summary>
    /// Creates a new <see cref="AssistantThread"/>.
    /// </summary>
    /// <param name="options"> Additional options to use when creating the thread. </param>
    /// <param name="cancellationToken">A token that can be used to cancel this method call.</param>
    /// <returns> A new thread. </returns>
    public virtual ClientResult<AssistantThread> CreateThread(ThreadCreationOptions options = null, CancellationToken cancellationToken = default)
    {
        ClientResult protocolResult = CreateThread(options?.ToBinaryContent(), cancellationToken.ToRequestOptions());
        return ClientResult.FromValue(AssistantThread.FromClientResult(protocolResult), protocolResult.GetRawResponse());
    }

    /// <summary>
    /// Gets an existing <see cref="AssistantThread"/>, retrieved via a known ID.
    /// </summary>
    /// <param name="threadId"> The ID of the thread to retrieve. </param>
    /// <param name="cancellationToken">A token that can be used to cancel this method call.</param>
    /// <returns> The existing thread instance. </returns>
    public virtual async Task<ClientResult<AssistantThread>> GetThreadAsync(string threadId, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNullOrEmpty(threadId, nameof(threadId));

        ClientResult protocolResult = await GetThreadAsync(threadId, cancellationToken.ToRequestOptions()).ConfigureAwait(false);
        return ClientResult.FromValue(AssistantThread.FromClientResult(protocolResult), protocolResult.GetRawResponse());
    }

    /// <summary>
    /// Gets an existing <see cref="AssistantThread"/>, retrieved via a known ID.
    /// </summary>
    /// <param name="threadId"> The ID of the thread to retrieve. </param>
    /// <param name="cancellationToken">A token that can be used to cancel this method call.</param>
    /// <returns> The existing thread instance. </returns>
    public virtual ClientResult<AssistantThread> GetThread(string threadId, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNullOrEmpty(threadId, nameof(threadId));

        ClientResult protocolResult = GetThread(threadId, cancellationToken.ToRequestOptions());
        return ClientResult.FromValue(AssistantThread.FromClientResult(protocolResult), protocolResult.GetRawResponse());
    }

    /// <summary>
    /// Modifies an existing <see cref="AssistantThread"/>.
    /// </summary>
    /// <param name="threadId"> The ID of the thread to modify. </param>
    /// <param name="options"> The modifications to apply to the thread. </param>
    /// <param name="cancellationToken">A token that can be used to cancel this method call.</param>
    /// <returns> The updated <see cref="AssistantThread"/> instance. </returns>
    public virtual async Task<ClientResult<AssistantThread>> ModifyThreadAsync(string threadId, ThreadModificationOptions options, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNullOrEmpty(threadId, nameof(threadId));
        Argument.AssertNotNull(options, nameof(options));

        ClientResult protocolResult = await ModifyThreadAsync(threadId, options?.ToBinaryContent(), cancellationToken.ToRequestOptions()).ConfigureAwait(false);
        return ClientResult.FromValue(AssistantThread.FromClientResult(protocolResult), protocolResult.GetRawResponse());
    }

    /// <summary>
    /// Modifies an existing <see cref="AssistantThread"/>.
    /// </summary>
    /// <param name="threadId"> The ID of the thread to modify. </param>
    /// <param name="options"> The modifications to apply to the thread. </param>
    /// <param name="cancellationToken">A token that can be used to cancel this method call.</param>
    /// <returns> The updated <see cref="AssistantThread"/> instance. </returns>
    public virtual ClientResult<AssistantThread> ModifyThread(string threadId, ThreadModificationOptions options, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNullOrEmpty(threadId, nameof(threadId));
        Argument.AssertNotNull(options, nameof(options));

        ClientResult protocolResult = ModifyThread(threadId, options?.ToBinaryContent(), cancellationToken.ToRequestOptions());
        return ClientResult.FromValue(AssistantThread.FromClientResult(protocolResult), protocolResult.GetRawResponse());
    }

    /// <summary>
    /// Deletes an existing <see cref="AssistantThread"/>.
    /// </summary>
    /// <param name="threadId"> The ID of the thread to delete. </param>
    /// <param name="cancellationToken">A token that can be used to cancel this method call.</param>
    /// <returns> A <see cref="ThreadDeletionResult"/> instance. </returns>
    public virtual async Task<ClientResult<ThreadDeletionResult>> DeleteThreadAsync(string threadId, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNullOrEmpty(threadId, nameof(threadId));

        ClientResult protocolResult = await DeleteThreadAsync(threadId, cancellationToken.ToRequestOptions()).ConfigureAwait(false);
        return ClientResult.FromValue(ThreadDeletionResult.FromClientResult(protocolResult), protocolResult.GetRawResponse());
    }

    /// <summary>
    /// Deletes an existing <see cref="AssistantThread"/>.
    /// </summary>
    /// <param name="threadId"> The ID of the thread to delete. </param>
    /// <param name="cancellationToken">A token that can be used to cancel this method call.</param>
    /// <returns> A <see cref="ThreadDeletionResult"/> instance. </returns>
    public virtual ClientResult<ThreadDeletionResult> DeleteThread(string threadId, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNullOrEmpty(threadId, nameof(threadId));

        ClientResult protocolResult = DeleteThread(threadId, cancellationToken.ToRequestOptions());
        return ClientResult.FromValue(ThreadDeletionResult.FromClientResult(protocolResult), protocolResult.GetRawResponse());
    }

    /// <summary>
    /// Creates a new <see cref="ThreadMessage"/> on an existing <see cref="AssistantThread"/>.
    /// </summary>
    /// <param name="threadId"> The ID of the thread to associate the new message with. </param>
    /// <param name="role"> The role to associate with the new message. </param>
    /// <param name="content"> The collection of <see cref="MessageContent"/> items for the message. </param>
    /// <param name="options"> Additional options to apply to the new message. </param>
    /// <param name="cancellationToken">A token that can be used to cancel this method call.</param>
    /// <returns> A new <see cref="ThreadMessage"/>. </returns>
    public virtual async Task<ClientResult<ThreadMessage>> CreateMessageAsync(
        string threadId,
        MessageRole role,
        IEnumerable<MessageContent> content,
        MessageCreationOptions options = null,
        CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNullOrEmpty(threadId, nameof(threadId));
        options ??= new();
        options.Role = role;
        options.Content.Clear();
        foreach (MessageContent contentItem in content)
        {
            options.Content.Add(contentItem);
        }

        ClientResult protocolResult = await CreateMessageAsync(threadId, options?.ToBinaryContent(), cancellationToken.ToRequestOptions())
            .ConfigureAwait(false);
        return ClientResult.FromValue(ThreadMessage.FromClientResult(protocolResult), protocolResult.GetRawResponse());
    }

    /// <summary>
    /// Creates a new <see cref="ThreadMessage"/> on an existing <see cref="AssistantThread"/>.
    /// </summary>
    /// <param name="threadId"> The ID of the thread to associate the new message with. </param>
    /// <param name="role"> The role to associate with the new message. </param>
    /// <param name="content"> The collection of <see cref="MessageContent"/> items for the message. </param>
    /// <param name="options"> Additional options to apply to the new message. </param>
    /// <param name="cancellationToken">A token that can be used to cancel this method call.</param>
    /// <returns> A new <see cref="ThreadMessage"/>. </returns>
    public virtual ClientResult<ThreadMessage> CreateMessage(
        string threadId,
        MessageRole role,
        IEnumerable<MessageContent> content,
        MessageCreationOptions options = null,
        CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNullOrEmpty(threadId, nameof(threadId));
        options ??= new();
        options.Role = role;
        options.Content.Clear();
        foreach (MessageContent contentItem in content)
        {
            options.Content.Add(contentItem);
        }

        ClientResult protocolResult = CreateMessage(threadId, options?.ToBinaryContent(), cancellationToken.ToRequestOptions());
        return ClientResult.FromValue(ThreadMessage.FromClientResult(protocolResult), protocolResult.GetRawResponse());
    }

    /// <summary>
    /// Gets a page collection of <see cref="ThreadMessage"/> instances from an existing <see cref="AssistantThread"/>.
    /// </summary>
    /// <param name="threadId"> The ID of the thread to list messages from. </param>
    /// <param name="options"> Options describing the collection to return. </param>
    /// <param name="cancellationToken">A token that can be used to cancel this method call.</param>
    /// <returns> A collection of <see cref="ThreadMessage"/>. </returns>
    public virtual AsyncCollectionResult<ThreadMessage> GetMessagesAsync(
        string threadId,
        MessageCollectionOptions options = default,
        CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNullOrEmpty(threadId, nameof(threadId));

        AsyncCollectionResult result = GetMessagesAsync(threadId, options?.PageSizeLimit, options?.Order?.ToString(), options?.AfterId, options?.BeforeId, cancellationToken.ToRequestOptions());

        if (result is not AsyncCollectionResult<ThreadMessage> collection)
        {
            throw new InvalidOperationException("Failed to cast protocol return type to expected collection type 'AsyncCollectionResult<ThreadMessage>'.");
        }

        return collection;
    }

    /// <summary>
    /// Rehydrates a page collection of <see cref="ThreadMessage"/> instances from a page token.
    /// </summary>
    /// <param name="firstPageToken"> Page token corresponding to the first page of the collection to rehydrate. </param>
    /// <param name="cancellationToken">A token that can be used to cancel this method call.</param>
    /// <returns> A collection of <see cref="ThreadMessage"/>. </returns>
    public virtual AsyncCollectionResult<ThreadMessage> GetMessagesAsync(
        ContinuationToken firstPageToken,
        CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNull(firstPageToken, nameof(firstPageToken));

        MessageCollectionPageToken pageToken = MessageCollectionPageToken.FromToken(firstPageToken);
        AsyncCollectionResult result = GetMessagesAsync(pageToken?.ThreadId, pageToken?.Limit, pageToken?.Order, pageToken?.After, pageToken?.Before, cancellationToken.ToRequestOptions());

        if (result is not AsyncCollectionResult<ThreadMessage> collection)
        {
            throw new InvalidOperationException("Failed to cast protocol return type to expected collection type 'AsyncCollectionResult<ThreadMessage>'.");
        }

        return collection;
    }

    /// <summary>
    /// Gets a page collection holding <see cref="ThreadMessage"/> instances from an existing <see cref="AssistantThread"/>.
    /// </summary>
    /// <param name="threadId"> The ID of the thread to list messages from. </param>
    /// <param name="options"> Options describing the collection to return. </param>
    /// <param name="cancellationToken">A token that can be used to cancel this method call.</param>
    /// <returns> A collection of <see cref="ThreadMessage"/>. </returns>
    public virtual CollectionResult<ThreadMessage> GetMessages(
        string threadId,
        MessageCollectionOptions options = default,
        CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNullOrEmpty(threadId, nameof(threadId));

        CollectionResult result = GetMessages(threadId, options?.PageSizeLimit, options?.Order?.ToString(), options?.AfterId, options?.BeforeId, cancellationToken.ToRequestOptions());

        if (result is not CollectionResult<ThreadMessage> collection)
        {
            throw new InvalidOperationException("Failed to cast protocol return type to expected collection type 'CollectionResult<ThreadMessage>'.");
        }

        return collection;
    }

    /// <summary>
    /// Rehydrates a page collection holding <see cref="ThreadMessage"/> instances from a page token.
    /// </summary>
    /// <param name="firstPageToken"> Page token corresponding to the first page of the collection to rehydrate. </param>
    /// <param name="cancellationToken">A token that can be used to cancel this method call.</param>
    /// <returns> A collection of <see cref="ThreadMessage"/>. </returns>
    public virtual CollectionResult<ThreadMessage> GetMessages(
        ContinuationToken firstPageToken,
        CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNull(firstPageToken, nameof(firstPageToken));

        MessageCollectionPageToken pageToken = MessageCollectionPageToken.FromToken(firstPageToken);
        CollectionResult result = GetMessages(pageToken?.ThreadId, pageToken?.Limit, pageToken?.Order, pageToken?.After, pageToken?.Before, cancellationToken.ToRequestOptions());

        if (result is not CollectionResult<ThreadMessage> collection)
        {
            throw new InvalidOperationException("Failed to cast protocol return type to expected collection type 'CollectionResult<ThreadMessage>'.");
        }

        return collection;

    }

    /// <summary>
    /// Gets an existing <see cref="ThreadMessage"/> from a known <see cref="AssistantThread"/>.
    /// </summary>
    /// <param name="threadId"> The ID of the thread to retrieve the message from. </param>
    /// <param name="messageId"> The ID of the message to retrieve. </param>
    /// <param name="cancellationToken">A token that can be used to cancel this method call.</param>
    /// <returns> The existing <see cref="ThreadMessage"/> instance. </returns>
    public virtual async Task<ClientResult<ThreadMessage>> GetMessageAsync(string threadId, string messageId, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNullOrEmpty(threadId, nameof(threadId));
        Argument.AssertNotNullOrEmpty(messageId, nameof(messageId));

        ClientResult protocolResult = await GetMessageAsync(threadId, messageId, cancellationToken.ToRequestOptions()).ConfigureAwait(false);
        return ClientResult.FromValue(ThreadMessage.FromClientResult(protocolResult), protocolResult.GetRawResponse());
    }

    /// <summary>
    /// Gets an existing <see cref="ThreadMessage"/> from a known <see cref="AssistantThread"/>.
    /// </summary>
    /// <param name="threadId"> The ID of the thread to retrieve the message from. </param>
    /// <param name="messageId"> The ID of the message to retrieve. </param>
    /// <param name="cancellationToken">A token that can be used to cancel this method call.</param>
    /// <returns> The existing <see cref="ThreadMessage"/> instance. </returns>
    public virtual ClientResult<ThreadMessage> GetMessage(string threadId, string messageId, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNullOrEmpty(threadId, nameof(threadId));
        Argument.AssertNotNullOrEmpty(messageId, nameof(messageId));

        ClientResult protocolResult = GetMessage(threadId, messageId, cancellationToken.ToRequestOptions());
        return ClientResult.FromValue(ThreadMessage.FromClientResult(protocolResult), protocolResult.GetRawResponse());
    }

    /// <summary>
    /// Modifies an existing <see cref="ThreadMessage"/>.
    /// </summary>
    /// <param name="threadId"> The ID of the thread associated with the message to modify. </param>
    /// <param name="messageId"> The ID of the message to modify. </param>
    /// <param name="options"> The changes to apply to the message. </param>
    /// <param name="cancellationToken">A token that can be used to cancel this method call.</param>
    /// <returns> The updated <see cref="ThreadMessage"/>. </returns>
    public virtual async Task<ClientResult<ThreadMessage>> ModifyMessageAsync(string threadId, string messageId, MessageModificationOptions options, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNullOrEmpty(threadId, nameof(threadId));
        Argument.AssertNotNullOrEmpty(messageId, nameof(messageId));
        Argument.AssertNotNull(options, nameof(options));

        ClientResult protocolResult = await ModifyMessageAsync(threadId, messageId, options?.ToBinaryContent(), cancellationToken.ToRequestOptions())
            .ConfigureAwait(false);
        return ClientResult.FromValue(ThreadMessage.FromClientResult(protocolResult), protocolResult.GetRawResponse());
    }

    /// <summary>
    /// Modifies an existing <see cref="ThreadMessage"/>.
    /// </summary>
    /// <param name="threadId"> The ID of the thread associated with the message to modify. </param>
    /// <param name="messageId"> The ID of the message to modify. </param>
    /// <param name="options"> The changes to apply to the message. </param>
    /// <param name="cancellationToken">A token that can be used to cancel this method call.</param>
    /// <returns> The updated <see cref="ThreadMessage"/>. </returns>
    public virtual ClientResult<ThreadMessage> ModifyMessage(string threadId, string messageId, MessageModificationOptions options, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNullOrEmpty(threadId, nameof(threadId));
        Argument.AssertNotNullOrEmpty(messageId, nameof(messageId));
        Argument.AssertNotNull(options, nameof(options));

        ClientResult protocolResult = ModifyMessage(threadId, messageId, options?.ToBinaryContent(), cancellationToken.ToRequestOptions());
        return ClientResult.FromValue(ThreadMessage.FromClientResult(protocolResult), protocolResult.GetRawResponse());
    }

    /// <summary>
    /// Deletes an existing <see cref="ThreadMessage"/>.
    /// </summary>
    /// <param name="threadId"> The ID of the thread associated with the message. </param>
    /// <param name="messageId"> The ID of the message. </param>
    /// <param name="cancellationToken">A token that can be used to cancel this method call.</param>
    /// <returns> A <see cref="MessageDeletionResult"/> instance. </returns>
    public virtual async Task<ClientResult<MessageDeletionResult>> DeleteMessageAsync(string threadId, string messageId, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNullOrEmpty(threadId, nameof(threadId));
        Argument.AssertNotNullOrEmpty(messageId, nameof(messageId));

        ClientResult protocolResult = await DeleteMessageAsync(threadId, messageId, cancellationToken.ToRequestOptions()).ConfigureAwait(false);
        return ClientResult.FromValue(MessageDeletionResult.FromClientResult(protocolResult), protocolResult.GetRawResponse());
    }

    /// <summary>
    /// Deletes an existing <see cref="ThreadMessage"/>.
    /// </summary>
    /// <param name="threadId"> The ID of the thread associated with the message. </param>
    /// <param name="messageId"> The ID of the message. </param>
    /// <param name="cancellationToken">A token that can be used to cancel this method call.</param>
    /// <returns> A <see cref="MessageDeletionResult"/> instance. </returns>
    public virtual ClientResult<MessageDeletionResult> DeleteMessage(string threadId, string messageId, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNullOrEmpty(threadId, nameof(threadId));
        Argument.AssertNotNullOrEmpty(messageId, nameof(messageId));

        ClientResult protocolResult = DeleteMessage(threadId, messageId, cancellationToken.ToRequestOptions());
        return ClientResult.FromValue(MessageDeletionResult.FromClientResult(protocolResult), protocolResult.GetRawResponse());
    }

    /// <summary>
    /// Begins a new <see cref="ThreadRun"/> that evaluates a <see cref="AssistantThread"/> using a specified
    /// <see cref="Assistant"/>.
    /// </summary>
    /// <param name="threadId"> The ID of the thread that the run should evaluate. </param>
    /// <param name="assistantId"> The ID of the assistant that should be used when evaluating the thread. </param>
    /// <param name="options"> Additional options for the run. </param>
    /// <param name="cancellationToken">A token that can be used to cancel this method call.</param>
    /// <returns> A new <see cref="ThreadRun"/> instance. </returns>
    public virtual async Task<ClientResult<ThreadRun>> CreateRunAsync(string threadId, string assistantId, RunCreationOptions options = null, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNullOrEmpty(threadId, nameof(threadId));
        Argument.AssertNotNullOrEmpty(assistantId, nameof(assistantId));
        options ??= new();
        options.AssistantId = assistantId;
        options.Stream = null;

        ClientResult protocolResult = await CreateRunAsync(threadId, options?.ToBinaryContent(), cancellationToken.ToRequestOptions())
            .ConfigureAwait(false);
        return ClientResult.FromValue(ThreadRun.FromClientResult(protocolResult), protocolResult.GetRawResponse());
    }

    /// <summary>
    /// Begins a new <see cref="ThreadRun"/> that evaluates a <see cref="AssistantThread"/> using a specified
    /// <see cref="Assistant"/>.
    /// </summary>
    /// <param name="threadId"> The ID of the thread that the run should evaluate. </param>
    /// <param name="assistantId"> The ID of the assistant that should be used when evaluating the thread. </param>
    /// <param name="options"> Additional options for the run. </param>
    /// <param name="cancellationToken">A token that can be used to cancel this method call.</param>
    /// <returns> A new <see cref="ThreadRun"/> instance. </returns>
    public virtual ClientResult<ThreadRun> CreateRun(string threadId, string assistantId, RunCreationOptions options = null, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNullOrEmpty(threadId, nameof(threadId));
        Argument.AssertNotNullOrEmpty(assistantId, nameof(assistantId));
        options ??= new();
        options.AssistantId = assistantId;
        options.Stream = null;

        ClientResult protocolResult = CreateRun(threadId, options?.ToBinaryContent(), cancellationToken.ToRequestOptions());
       return ClientResult.FromValue(ThreadRun.FromClientResult(protocolResult), protocolResult.GetRawResponse());
    }

    /// <summary>
    /// Begins a new streaming <see cref="ThreadRun"/> that evaluates a <see cref="AssistantThread"/> using a specified
    /// <see cref="Assistant"/>.
    /// </summary>
    /// <param name="threadId"> The ID of the thread that the run should evaluate. </param>
    /// <param name="assistantId"> The ID of the assistant that should be used when evaluating the thread. </param>
    /// <param name="options"> Additional options for the run. </param>
    /// <param name="cancellationToken">A token that can be used to cancel this method call.</param>
    public virtual AsyncCollectionResult<StreamingUpdate> CreateRunStreamingAsync(
        string threadId,
        string assistantId,
        RunCreationOptions options = null,
        CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNullOrEmpty(threadId, nameof(threadId));
        Argument.AssertNotNullOrEmpty(assistantId, nameof(assistantId));

        options ??= new();
        options.AssistantId = assistantId;
        options.Stream = true;

        return new AsyncSseUpdateCollection<StreamingUpdate>(
            async () => await CreateRunAsync(threadId, options?.ToBinaryContent(), cancellationToken.ToRequestOptions(streaming: true)).ConfigureAwait(false),
            StreamingUpdate.FromSseItem,
            cancellationToken);
    }

    /// <summary>
    /// Begins a new streaming <see cref="ThreadRun"/> that evaluates a <see cref="AssistantThread"/> using a specified
    /// <see cref="Assistant"/>.
    /// </summary>
    /// <param name="threadId"> The ID of the thread that the run should evaluate. </param>
    /// <param name="assistantId"> The ID of the assistant that should be used when evaluating the thread. </param>
    /// <param name="options"> Additional options for the run. </param>
    /// <param name="cancellationToken">A token that can be used to cancel this method call.</param>
    public virtual CollectionResult<StreamingUpdate> CreateRunStreaming(
        string threadId,
        string assistantId,
        RunCreationOptions options = null,
        CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNullOrEmpty(threadId, nameof(threadId));
        Argument.AssertNotNullOrEmpty(assistantId, nameof(assistantId));

        options ??= new();
        options.AssistantId = assistantId;
        options.Stream = true;

        return new SseUpdateCollection<StreamingUpdate>(
            () => CreateRun(threadId, options?.ToBinaryContent(), cancellationToken.ToRequestOptions(streaming: true)),
            StreamingUpdate.FromSseItem,
            cancellationToken);
    }

    /// <summary>
    /// Creates a new thread and immediately begins a run against it using the specified <see cref="Assistant"/>.
    /// </summary>
    /// <param name="assistantId"> The ID of the assistant that the new run should use. </param>
    /// <param name="threadOptions"> Options for the new thread that will be created. </param>
    /// <param name="runOptions"> Additional options to apply to the run that will begin. </param>
    /// <param name="cancellationToken">A token that can be used to cancel this method call.</param>
    /// <returns> A new <see cref="ThreadRun"/>. </returns>
    public virtual async Task<ClientResult<ThreadRun>> CreateThreadAndRunAsync(
        string assistantId,
        ThreadCreationOptions threadOptions = null,
        RunCreationOptions runOptions = null,
        CancellationToken cancellationToken = default)
    {
        runOptions ??= new();
        runOptions.Stream = null;
        BinaryContent protocolContent = CreateThreadAndRunProtocolContent(assistantId, threadOptions, runOptions);
        ClientResult protocolResult = await CreateThreadAndRunAsync(protocolContent, cancellationToken.ToRequestOptions()).ConfigureAwait(false);
        return ClientResult.FromValue(ThreadRun.FromClientResult(protocolResult), protocolResult.GetRawResponse());
    }

    /// <summary>
    /// Creates a new thread and immediately begins a run against it using the specified <see cref="Assistant"/>.
    /// </summary>
    /// <param name="assistantId"> The ID of the assistant that the new run should use. </param>
    /// <param name="threadOptions"> Options for the new thread that will be created. </param>
    /// <param name="runOptions"> Additional options to apply to the run that will begin. </param>
    /// <param name="cancellationToken">A token that can be used to cancel this method call.</param>
    /// <returns> A new <see cref="ThreadRun"/>. </returns>
    public virtual ClientResult<ThreadRun> CreateThreadAndRun(
        string assistantId,
        ThreadCreationOptions threadOptions = null,
        RunCreationOptions runOptions = null,
        CancellationToken cancellationToken = default)
    {
        runOptions ??= new();
        runOptions.Stream = null;
        BinaryContent protocolContent = CreateThreadAndRunProtocolContent(assistantId, threadOptions, runOptions);
        ClientResult protocolResult = CreateThreadAndRun(protocolContent, cancellationToken.ToRequestOptions());
        return ClientResult.FromValue(ThreadRun.FromClientResult(protocolResult), protocolResult.GetRawResponse());
    }

    /// <summary>
    /// Creates a new thread and immediately begins a streaming run against it using the specified <see cref="Assistant"/>.
    /// </summary>
    /// <param name="assistantId"> The ID of the assistant that the new run should use. </param>
    /// <param name="threadOptions"> Options for the new thread that will be created. </param>
    /// <param name="runOptions"> Additional options to apply to the run that will begin. </param>
    /// <param name="cancellationToken">A token that can be used to cancel this method call.</param>
    public virtual AsyncCollectionResult<StreamingUpdate> CreateThreadAndRunStreamingAsync(
        string assistantId,
        ThreadCreationOptions threadOptions = null,
        RunCreationOptions runOptions = null,
        CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNullOrEmpty(assistantId, nameof(assistantId));

        runOptions ??= new();
        runOptions.Stream = true;
        BinaryContent protocolContent = CreateThreadAndRunProtocolContent(assistantId, threadOptions, runOptions);

        return new AsyncSseUpdateCollection<StreamingUpdate>(
            async () => await CreateThreadAndRunAsync(protocolContent, cancellationToken.ToRequestOptions(streaming: true)).ConfigureAwait(false),
            StreamingUpdate.FromSseItem,
            cancellationToken);
    }

    /// <summary>
    /// Creates a new thread and immediately begins a streaming run against it using the specified <see cref="Assistant"/>.
    /// </summary>
    /// <param name="assistantId"> The ID of the assistant that the new run should use. </param>
    /// <param name="threadOptions"> Options for the new thread that will be created. </param>
    /// <param name="runOptions"> Additional options to apply to the run that will begin. </param>
    /// <param name="cancellationToken">A token that can be used to cancel this method call.</param>
    public virtual CollectionResult<StreamingUpdate> CreateThreadAndRunStreaming(
        string assistantId,
        ThreadCreationOptions threadOptions = null,
        RunCreationOptions runOptions = null,
        CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNullOrEmpty(assistantId, nameof(assistantId));

        runOptions ??= new();
        runOptions.Stream = true;
        BinaryContent protocolContent = CreateThreadAndRunProtocolContent(assistantId, threadOptions, runOptions);

        return new SseUpdateCollection<StreamingUpdate>(
            () => CreateThreadAndRun(protocolContent, cancellationToken.ToRequestOptions(streaming: true)),
            StreamingUpdate.FromSseItem,
            cancellationToken);
    }

    /// <summary>
    /// Gets a page collection holding <see cref="ThreadRun"/> instances associated with an existing <see cref="AssistantThread"/>.
    /// </summary>
    /// <param name="threadId"> The ID of the thread that runs in the list should be associated with. </param>
    /// <param name="options"> Options describing the collection to return. </param>
    /// <param name="cancellationToken">A token that can be used to cancel this method call.</param>
    /// <returns> A collection of <see cref="ThreadRun"/>. </returns>
    public virtual AsyncCollectionResult<ThreadRun> GetRunsAsync(
        string threadId,
        RunCollectionOptions options = default,
        CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNullOrEmpty(threadId, nameof(threadId));

        AsyncCollectionResult result = GetRunsAsync(threadId, options?.PageSizeLimit, options?.Order?.ToString(), options?.AfterId, options?.BeforeId, cancellationToken.ToRequestOptions());

        if (result is not AsyncCollectionResult<ThreadRun> collection)
        {
            throw new InvalidOperationException("Failed to cast protocol return type to expected collection type 'AsyncCollectionResult<ThreadRun>'.");
        }

        return collection;
    }

    /// <summary>
    /// Rehydrates a page collection holding <see cref="ThreadRun"/> instances from a page token.
    /// </summary>
    /// <param name="firstPageToken"> Page token corresponding to the first page of the collection to rehydrate. </param>
    /// <param name="cancellationToken">A token that can be used to cancel this method call.</param>
    /// <returns> A collection of <see cref="ThreadRun"/>. </returns>
    public virtual AsyncCollectionResult<ThreadRun> GetRunsAsync(
        ContinuationToken firstPageToken,
        CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNull(firstPageToken, nameof(firstPageToken));

        RunCollectionPageToken pageToken = RunCollectionPageToken.FromToken(firstPageToken);
        AsyncCollectionResult result = GetRunsAsync(pageToken?.ThreadId, pageToken?.Limit, pageToken?.Order, pageToken?.After, pageToken?.Before, cancellationToken.ToRequestOptions());

        if (result is not AsyncCollectionResult<ThreadRun> collection)
        {
            throw new InvalidOperationException("Failed to cast protocol return type to expected collection type 'AsyncCollectionResult<ThreadRun>'.");
        }

        return collection;
    }

    /// <summary>
    /// Gets a page collection holding <see cref="ThreadRun"/> instances associated with an existing <see cref="AssistantThread"/>.
    /// </summary>
    /// <param name="threadId"> The ID of the thread that runs in the list should be associated with. </param>
    /// <param name="options"> Options describing the collection to return. </param>
    /// <param name="cancellationToken">A token that can be used to cancel this method call.</param>
    /// <returns> A collection of <see cref="ThreadRun"/>. </returns>
    public virtual CollectionResult<ThreadRun> GetRuns(
        string threadId,
        RunCollectionOptions options = default,
        CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNullOrEmpty(threadId, nameof(threadId));

        CollectionResult result = GetRuns(threadId, options?.PageSizeLimit, options?.Order?.ToString(), options?.AfterId, options?.BeforeId, cancellationToken.ToRequestOptions());

        if (result is not CollectionResult<ThreadRun> collection)
        {
            throw new InvalidOperationException("Failed to cast protocol return type to expected collection type 'CollectionResult<ThreadRun>'.");
        }

        return collection;
    }

    /// <summary>
    /// Rehydrates a page collection holding <see cref="ThreadRun"/> instances from a page token.
    /// </summary>
    /// <param name="firstPageToken"> Page token corresponding to the first page of the collection to rehydrate. </param>
    /// <param name="cancellationToken">A token that can be used to cancel this method call.</param>
    /// <returns> A collection of <see cref="ThreadRun"/>. </returns>
    public virtual CollectionResult<ThreadRun> GetRuns(
        ContinuationToken firstPageToken,
        CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNull(firstPageToken, nameof(firstPageToken));

        RunCollectionPageToken pageToken = RunCollectionPageToken.FromToken(firstPageToken);
        CollectionResult result = GetRuns(pageToken?.ThreadId, pageToken?.Limit, pageToken?.Order, pageToken?.After, pageToken?.Before, cancellationToken.ToRequestOptions());

        if (result is not CollectionResult<ThreadRun> collection)
        {
            throw new InvalidOperationException("Failed to cast protocol return type to expected collection type 'CollectionResult<ThreadRun>'.");
        }

        return collection;
    }

    /// <summary>
    /// Gets an existing <see cref="ThreadRun"/> from a known <see cref="AssistantThread"/>.
    /// </summary>
    /// <param name="threadId"> The ID of the thread to retrieve the run from. </param>
    /// <param name="runId"> The ID of the run to retrieve. </param>
    /// <param name="cancellationToken">A token that can be used to cancel this method call.</param>
    /// <returns> The existing <see cref="ThreadRun"/> instance. </returns>
    public virtual async Task<ClientResult<ThreadRun>> GetRunAsync(string threadId, string runId, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNullOrEmpty(threadId, nameof(threadId));
        Argument.AssertNotNullOrEmpty(runId, nameof(runId));

        ClientResult protocolResult = await GetRunAsync(threadId, runId, cancellationToken.ToRequestOptions()).ConfigureAwait(false);
        return ClientResult.FromValue(ThreadRun.FromClientResult(protocolResult), protocolResult.GetRawResponse());
    }

    /// <summary>
    /// Gets an existing <see cref="ThreadRun"/> from a known <see cref="AssistantThread"/>.
    /// </summary>
    /// <param name="threadId"> The ID of the thread to retrieve the run from. </param>
    /// <param name="runId"> The ID of the run to retrieve. </param>
    /// <param name="cancellationToken">A token that can be used to cancel this method call.</param>
    /// <returns> The existing <see cref="ThreadRun"/> instance. </returns>
    public virtual ClientResult<ThreadRun> GetRun(string threadId, string runId, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNullOrEmpty(threadId, nameof(threadId));
        Argument.AssertNotNullOrEmpty(runId, nameof(runId));

        ClientResult protocolResult = GetRun(threadId, runId, cancellationToken.ToRequestOptions());
        return ClientResult.FromValue(ThreadRun.FromClientResult(protocolResult), protocolResult.GetRawResponse());
    }

    /// <summary>
    /// Submits a collection of required tool call outputs to a run and resumes the run.
    /// </summary>
    /// <param name="threadId"> The thread ID of the thread being run. </param>
    /// <param name="runId"> The ID of the run that reached a <c>requires_action</c> status. </param>
    /// <param name="toolOutputs">
    /// The tool outputs, corresponding to <see cref="InternalRequiredToolCall"/> instances from the run.
    /// </param>
    /// <param name="cancellationToken">A token that can be used to cancel this method call.</param>
    /// <returns> The <see cref="ThreadRun"/>, updated after the submission was processed. </returns>
    public virtual async Task<ClientResult<ThreadRun>> SubmitToolOutputsToRunAsync(
        string threadId,
        string runId,
        IEnumerable<ToolOutput> toolOutputs,
        CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNullOrEmpty(threadId, nameof(threadId));
        Argument.AssertNotNullOrEmpty(runId, nameof(runId));

        var submitToolOutputsRunRequest = new InternalSubmitToolOutputsRunRequest(toolOutputs);
        using BinaryContent content = BinaryContent.Create(submitToolOutputsRunRequest, ModelSerializationExtensions.WireOptions);
        ClientResult protocolResult = await SubmitToolOutputsToRunAsync(threadId, runId, content, cancellationToken.ToRequestOptions())
            .ConfigureAwait(false);
        return ClientResult.FromValue(ThreadRun.FromClientResult(protocolResult), protocolResult.GetRawResponse());
    }

    /// <summary>
    /// Submits a collection of required tool call outputs to a run and resumes the run.
    /// </summary>
    /// <param name="threadId"> The thread ID of the thread being run. </param>
    /// <param name="runId"> The ID of the run that reached a <c>requires_action</c> status. </param>
    /// <param name="toolOutputs">
    /// The tool outputs, corresponding to <see cref="InternalRequiredToolCall"/> instances from the run.
    /// </param>
    /// <param name="cancellationToken">A token that can be used to cancel this method call.</param>
    /// <returns> The <see cref="ThreadRun"/>, updated after the submission was processed. </returns>
    public virtual ClientResult<ThreadRun> SubmitToolOutputsToRun(
        string threadId,
        string runId,
        IEnumerable<ToolOutput> toolOutputs,
        CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNullOrEmpty(threadId, nameof(threadId));
        Argument.AssertNotNullOrEmpty(runId, nameof(runId));

        var submitToolOutputsRunRequest = new InternalSubmitToolOutputsRunRequest(toolOutputs);
        using BinaryContent content = BinaryContent.Create(submitToolOutputsRunRequest, ModelSerializationExtensions.WireOptions);
        ClientResult protocolResult = SubmitToolOutputsToRun(threadId, runId, content, cancellationToken.ToRequestOptions());
        return ClientResult.FromValue(ThreadRun.FromClientResult(protocolResult), protocolResult.GetRawResponse());
    }

    /// <summary>
    /// Submits a collection of required tool call outputs to a run and resumes the run with streaming enabled.
    /// </summary>
    /// <param name="threadId"> The thread ID of the thread being run. </param>
    /// <param name="runId"> The ID of the run that reached a <c>requires_action</c> status. </param>
    /// <param name="toolOutputs">
    /// The tool outputs, corresponding to <see cref="InternalRequiredToolCall"/> instances from the run.
    /// </param>
    /// <param name="cancellationToken">A token that can be used to cancel this method call.</param>
    public virtual AsyncCollectionResult<StreamingUpdate> SubmitToolOutputsToRunStreamingAsync(
        string threadId,
        string runId,
        IEnumerable<ToolOutput> toolOutputs,
        CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNullOrEmpty(threadId, nameof(threadId));
        Argument.AssertNotNullOrEmpty(runId, nameof(runId));

        var submitToolOutputsRunRequest = new InternalSubmitToolOutputsRunRequest(toolOutputs.ToList(), stream: true, null);
        using BinaryContent content = BinaryContent.Create(submitToolOutputsRunRequest, ModelSerializationExtensions.WireOptions);

        return new AsyncSseUpdateCollection<StreamingUpdate>(
            async () => await SubmitToolOutputsToRunAsync(threadId, runId, content, cancellationToken.ToRequestOptions(streaming: true)).ConfigureAwait(false),
            StreamingUpdate.FromSseItem,
            cancellationToken);
    }

    /// <summary>
    /// Submits a collection of required tool call outputs to a run and resumes the run with streaming enabled.
    /// </summary>
    /// <param name="threadId"> The thread ID of the thread being run. </param>
    /// <param name="runId"> The ID of the run that reached a <c>requires_action</c> status. </param>
    /// <param name="toolOutputs">
    /// The tool outputs, corresponding to <see cref="InternalRequiredToolCall"/> instances from the run.
    /// </param>
    /// <param name="cancellationToken">A token that can be used to cancel this method call.</param>
    public virtual CollectionResult<StreamingUpdate> SubmitToolOutputsToRunStreaming(
        string threadId,
        string runId,
        IEnumerable<ToolOutput> toolOutputs,
        CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNullOrEmpty(threadId, nameof(threadId));
        Argument.AssertNotNullOrEmpty(runId, nameof(runId));

        var submitToolOutputsRunRequest = new InternalSubmitToolOutputsRunRequest(toolOutputs.ToList(), stream: true, null);
        using BinaryContent content = BinaryContent.Create(submitToolOutputsRunRequest, ModelSerializationExtensions.WireOptions);

        return new SseUpdateCollection<StreamingUpdate>(
            () => SubmitToolOutputsToRun(threadId, runId, content, cancellationToken.ToRequestOptions(streaming: true)),
            StreamingUpdate.FromSseItem,
            cancellationToken);
    }

    /// <summary>
    /// Cancels an in-progress <see cref="ThreadRun"/>.
    /// </summary>
    /// <param name="threadId"> The ID of the thread associated with the run. </param>
    /// <param name="runId"> The ID of the run to cancel. </param>
    /// <param name="cancellationToken">A token that can be used to cancel this method call.</param>
    /// <returns> An updated <see cref="ThreadRun"/> instance, reflecting the new status of the run. </returns>
    public virtual async Task<ClientResult<ThreadRun>> CancelRunAsync(string threadId, string runId, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNullOrEmpty(threadId, nameof(threadId));
        Argument.AssertNotNullOrEmpty(runId, nameof(runId));

        ClientResult protocolResult = await CancelRunAsync(threadId, runId, cancellationToken.ToRequestOptions()).ConfigureAwait(false);
        return ClientResult.FromValue(ThreadRun.FromClientResult(protocolResult), protocolResult.GetRawResponse());
    }

    /// <summary>
    /// Cancels an in-progress <see cref="ThreadRun"/>.
    /// </summary>
    /// <param name="threadId"> The ID of the thread associated with the run. </param>
    /// <param name="runId"> The ID of the run to cancel. </param>
    /// <param name="cancellationToken">A token that can be used to cancel this method call.</param>
    /// <returns> An updated <see cref="ThreadRun"/> instance, reflecting the new status of the run. </returns>
    public virtual ClientResult<ThreadRun> CancelRun(string threadId, string runId, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNullOrEmpty(threadId, nameof(threadId));
        Argument.AssertNotNullOrEmpty(runId, nameof(runId));

        ClientResult protocolResult = CancelRun(threadId, runId, cancellationToken.ToRequestOptions());
        return ClientResult.FromValue(ThreadRun.FromClientResult(protocolResult), protocolResult.GetRawResponse());
    }

    /// <summary>
    /// Gets a page collection holding <see cref="RunStep"/> instances associated with a <see cref="ThreadRun"/>.
    /// </summary>
    /// <param name="threadId"> The ID of the thread associated with the run. </param>
    /// <param name="runId"> The ID of the run to list run steps from. </param>
    /// <param name="options"></param>
    /// <param name="cancellationToken">A token that can be used to cancel this method call.</param>
    /// <returns> A collection of <see cref="RunStep"/>. </returns>
    public virtual AsyncCollectionResult<RunStep> GetRunStepsAsync(
        string threadId,
        string runId,
        RunStepCollectionOptions options = default,
        CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNullOrEmpty(threadId, nameof(threadId));
        Argument.AssertNotNullOrEmpty(runId, nameof(runId));

        return GetRunStepsAsync(threadId, runId, options?.PageSizeLimit, options?.Order?.ToString(), options?.AfterId, options?.BeforeId, cancellationToken.ToRequestOptions())
            as AsyncCollectionResult<RunStep>;
    }

    /// <summary>
    /// Gets a page collection holding <see cref="RunStep"/> instances associated with a <see cref="ThreadRun"/>.
    /// </summary>
    /// <param name="threadId"> The ID of the thread associated with the run. </param>
    /// <param name="runId"> The ID of the run to list run steps from. </param>
    /// <param name="options"></param>
    /// <param name="cancellationToken">A token that can be used to cancel this method call.</param>
    /// <returns> A collection of <see cref="RunStep"/>. </returns>
    public virtual CollectionResult<RunStep> GetRunSteps(
        string threadId,
        string runId,
        RunStepCollectionOptions options = default,
        CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNullOrEmpty(threadId, nameof(threadId));
        Argument.AssertNotNullOrEmpty(runId, nameof(runId));

        CollectionResult result = GetRunSteps(threadId, runId, options?.PageSizeLimit, options?.Order?.ToString(), options?.AfterId, options?.BeforeId, cancellationToken.ToRequestOptions());

        if (result is not CollectionResult<RunStep> collection)
        {
            throw new InvalidOperationException("Failed to cast protocol return type to expected collection type 'CollectionResult<RunStep>'.");
        }

        return collection;
    }

    /// <summary>
    /// Rehydrates a page collection holding <see cref="RunStep"/> instances from a page token.
    /// </summary>
    /// <param name="firstPageToken"> Page token corresponding to the first page of the collection to rehydrate. </param>
    /// <param name="cancellationToken">A token that can be used to cancel this method call.</param>
    /// <returns> A collection of <see cref="RunStep"/>. </returns>
    public virtual AsyncCollectionResult<RunStep> GetRunStepsAsync(
        ContinuationToken firstPageToken,
        CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNull(firstPageToken, nameof(firstPageToken));

        RunStepCollectionPageToken pageToken = RunStepCollectionPageToken.FromToken(firstPageToken);
        AsyncCollectionResult result = GetRunStepsAsync(pageToken?.ThreadId, pageToken?.RunId, pageToken?.Limit, pageToken?.Order, pageToken?.After, pageToken?.Before, cancellationToken.ToRequestOptions());

        if (result is not AsyncCollectionResult<RunStep> collection)
        {
            throw new InvalidOperationException("Failed to cast protocol return type to expected collection type 'AsyncCollectionResult<RunStep>'.");
        }

        return collection;
    }

    /// <summary>
    /// Rehydrates a page collection holding <see cref="RunStep"/> instances from a page token.
    /// </summary>
    /// <param name="firstPageToken"> Page token corresponding to the first page of the collection to rehydrate. </param>
    /// <param name="cancellationToken">A token that can be used to cancel this method call.</param>
    /// <returns> A collection of <see cref="RunStep"/>. </returns>
    public virtual CollectionResult<RunStep> GetRunSteps(
        ContinuationToken firstPageToken,
        CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNull(firstPageToken, nameof(firstPageToken));

        RunStepCollectionPageToken pageToken = RunStepCollectionPageToken.FromToken(firstPageToken);
        CollectionResult result = GetRunSteps(pageToken?.ThreadId, pageToken?.RunId, pageToken?.Limit, pageToken?.Order, pageToken?.After, pageToken?.Before, cancellationToken.ToRequestOptions());

        if (result is not CollectionResult<RunStep> collection)
        {
            throw new InvalidOperationException("Failed to cast protocol return type to expected collection type 'CollectionResult<RunStep>'.");
        }

        return collection;
    }

    /// <summary>
    /// Gets a single run step from a run.
    /// </summary>
    /// <param name="threadId"> The ID of the thread associated with the run. </param>
    /// <param name="runId"> The ID of the run. </param>
    /// <param name="stepId"> The ID of the run step. </param>
    /// <param name="cancellationToken">A token that can be used to cancel this method call.</param>
    /// <returns> A <see cref="RunStep"/> instance corresponding to the specified step. </returns>
    public virtual async Task<ClientResult<RunStep>> GetRunStepAsync(string threadId, string runId, string stepId, CancellationToken cancellationToken = default)
    {
        ClientResult protocolResult = await GetRunStepAsync(threadId, runId, stepId, cancellationToken.ToRequestOptions()).ConfigureAwait(false);
        return ClientResult.FromValue(RunStep.FromClientResult(protocolResult), protocolResult.GetRawResponse());
    }

    /// <summary>
    /// Gets a single run step from a run.
    /// </summary>
    /// <param name="threadId"> The ID of the thread associated with the run. </param>
    /// <param name="runId"> The ID of the run. </param>
    /// <param name="stepId"> The ID of the run step. </param>
    /// <param name="cancellationToken">A token that can be used to cancel this method call.</param>
    /// <returns> A <see cref="RunStep"/> instance corresponding to the specified step. </returns>
    public virtual ClientResult<RunStep> GetRunStep(string threadId, string runId, string stepId, CancellationToken cancellationToken = default)
    {
        ClientResult protocolResult = GetRunStep(threadId, runId, stepId, cancellationToken.ToRequestOptions());
        return ClientResult.FromValue(RunStep.FromClientResult(protocolResult), protocolResult.GetRawResponse());
    }

    private static BinaryContent CreateThreadAndRunProtocolContent(
        string assistantId,
        ThreadCreationOptions threadOptions,
        RunCreationOptions runOptions)
    {
        Argument.AssertNotNullOrEmpty(assistantId, nameof(assistantId));
        InternalCreateThreadAndRunRequest internalRequest = new(
            assistantId,
            threadOptions,
            runOptions.InstructionsOverride,
            runOptions.ToolsOverride,
            runOptions.Metadata,
            runOptions.Temperature,
            // TODO: reconcile exposure of the the two different tool_resources, if needed
            runOptions.NucleusSamplingFactor,
            runOptions.Stream,
            runOptions.MaxInputTokenCount,
            runOptions.MaxOutputTokenCount,
            runOptions.TruncationStrategy,
            runOptions.AllowParallelToolCalls,
            runOptions.ModelOverride,
            threadOptions.ToolResources,
            runOptions.ResponseFormat,
            runOptions.ToolConstraint,
            additionalBinaryDataProperties: null);
        return BinaryContent.Create(internalRequest, ModelSerializationExtensions.WireOptions);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static ClientResult<T> CreateResultFromProtocol<T>(ClientResult protocolResult, Func<PipelineResponse, T> responseDeserializer)
    {
        PipelineResponse pipelineResponse = protocolResult?.GetRawResponse();
        T deserializedResultValue = responseDeserializer.Invoke(pipelineResponse);
        return ClientResult.FromValue(deserializedResultValue, pipelineResponse);
    }
}
