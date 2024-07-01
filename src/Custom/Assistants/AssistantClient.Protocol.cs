using OpenAI.Utility;
using System;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OpenAI.Assistants;

public partial class AssistantClient
{
    /// <summary>
    /// [Protocol Method] Create an assistant with a model and instructions.
    /// </summary>
    /// <param name="content"> The content to send as the body of the request. </param>
    /// <param name="options"> The request options, which can override default behaviors of the client pipeline on a per-call basis. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="content"/> is null. </exception>
    /// <exception cref="ClientResultException"> Service returned a non-success status code. </exception>
    /// <returns> The response returned from the service. </returns>
    public virtual async Task<ClientResult> CreateAssistantAsync(BinaryContent content, RequestOptions options = null)
    {
        Argument.AssertNotNull(content, nameof(content));

        using PipelineMessage message = CreateCreateAssistantRequest(content, options);
        return ClientResult.FromResponse(await _pipeline.ProcessMessageAsync(message, options).ConfigureAwait(false));
    }

    /// <summary>
    /// [Protocol Method] Create an assistant with a model and instructions.
    /// </summary>
    /// <param name="content"> The content to send as the body of the request. </param>
    /// <param name="options"> The request options, which can override default behaviors of the client pipeline on a per-call basis. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="content"/> is null. </exception>
    /// <exception cref="ClientResultException"> Service returned a non-success status code. </exception>
    /// <returns> The response returned from the service. </returns>
    public virtual ClientResult CreateAssistant(BinaryContent content, RequestOptions options = null)
    {
        Argument.AssertNotNull(content, nameof(content));

        using PipelineMessage message = CreateCreateAssistantRequest(content, options);
        return ClientResult.FromResponse(_pipeline.ProcessMessage(message, options));
    }

    /// <summary>
    /// [Protocol Method] Returns a list of assistants.
    /// </summary>
    /// <param name="limit">
    /// A limit on the number of objects to be returned. Limit can range between 1 and 100, and the
    /// default is 20.
    /// </param>
    /// <param name="order">
    /// Sort order by the `created_at` timestamp of the objects. `asc` for ascending order and`desc`
    /// for descending order. Allowed values: "asc" | "desc"
    /// </param>
    /// <param name="after">
    /// A cursor for use in pagination. `after` is an object ID that defines your place in the list.
    /// For instance, if you make a list request and receive 100 objects, ending with obj_foo, your
    /// subsequent call can include after=obj_foo in order to fetch the next page of the list.
    /// </param>
    /// <param name="before">
    /// A cursor for use in pagination. `before` is an object ID that defines your place in the list.
    /// For instance, if you make a list request and receive 100 objects, ending with obj_foo, your
    /// subsequent call can include before=obj_foo in order to fetch the previous page of the list.
    /// </param>
    /// <param name="options"> The request options, which can override default behaviors of the client pipeline on a per-call basis. </param>
    /// <exception cref="ClientResultException"> Service returned a non-success status code. </exception>
    /// <returns> The response returned from the service. </returns>
    public virtual IAsyncEnumerable<ClientResult> GetAssistantsAsync(int? limit, string order, string after, string before, RequestOptions options)
    {
        AssistantCollectionPageToken firstPageToken = AssistantCollectionPageToken.FromOptions(limit, order, after, before);
        return OpenAIPageCollectionHelpers.CreateProtocolAsync(firstPageToken, GetAssistantsPageAsync, AssistantCollectionPageToken.FromToken, options);
    }

    internal virtual async Task<ClientResult> GetAssistantsPageAsync(ContinuationToken pageToken, RequestOptions options)
    {
        AssistantCollectionPageToken token = AssistantCollectionPageToken.FromToken(pageToken);
        return await GetAssistantsPageAsync(token.Limit, token.Order, token.After, token.Before, options).ConfigureAwait(false);
    }

    internal virtual async Task<ClientResult> GetAssistantsPageAsync(int? limit, string order, string after, string before, RequestOptions options)
    {
        using PipelineMessage message = CreateGetAssistantsRequest(limit, order, after, before, options);
        return ClientResult.FromResponse(await _pipeline.ProcessMessageAsync(message, options).ConfigureAwait(false));
    }

    /// <summary>
    /// [Protocol Method] Returns a list of assistants.
    /// </summary>
    /// <param name="limit">
    /// A limit on the number of objects to be returned. Limit can range between 1 and 100, and the
    /// default is 20.
    /// </param>
    /// <param name="order">
    /// Sort order by the `created_at` timestamp of the objects. `asc` for ascending order and`desc`
    /// for descending order. Allowed values: "asc" | "desc"
    /// </param>
    /// <param name="after">
    /// A cursor for use in pagination. `after` is an object ID that defines your place in the list.
    /// For instance, if you make a list request and receive 100 objects, ending with obj_foo, your
    /// subsequent call can include after=obj_foo in order to fetch the next page of the list.
    /// </param>
    /// <param name="before">
    /// A cursor for use in pagination. `before` is an object ID that defines your place in the list.
    /// For instance, if you make a list request and receive 100 objects, ending with obj_foo, your
    /// subsequent call can include before=obj_foo in order to fetch the previous page of the list.
    /// </param>
    /// <param name="options"> The request options, which can override default behaviors of the client pipeline on a per-call basis. </param>
    /// <exception cref="ClientResultException"> Service returned a non-success status code. </exception>
    /// <returns> The response returned from the service. </returns>
    public virtual IEnumerable<ClientResult> GetAssistants(int? limit, string order, string after, string before, RequestOptions options)
    {
        AssistantCollectionPageToken firstPageToken = AssistantCollectionPageToken.FromOptions(limit, order, after, before);
        return OpenAIPageCollectionHelpers.CreateProtocol(firstPageToken, GetAssistantsPage, AssistantCollectionPageToken.FromToken, options);
    }

    internal virtual ClientResult GetAssistantsPage(ContinuationToken pageToken, RequestOptions options)
    {
        AssistantCollectionPageToken token = AssistantCollectionPageToken.FromToken(pageToken);
        return GetAssistantsPage(token.Limit, token.Order, token.After, token.Before, options);
    }

    internal virtual ClientResult GetAssistantsPage(int? limit, string order, string after, string before, RequestOptions options)
    {
        using PipelineMessage message = CreateGetAssistantsRequest(limit, order, after, before, options);
        return ClientResult.FromResponse(_pipeline.ProcessMessage(message, options));
    }

    /// <summary>
    /// [Protocol Method] Retrieves an assistant.
    /// </summary>
    /// <param name="assistantId"> The ID of the assistant to retrieve. </param>
    /// <param name="options"> The request options, which can override default behaviors of the client pipeline on a per-call basis. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="assistantId"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="assistantId"/> is an empty string, and was expected to be non-empty. </exception>
    /// <exception cref="ClientResultException"> Service returned a non-success status code. </exception>
    /// <returns> The response returned from the service. </returns>
    public virtual async Task<ClientResult> GetAssistantAsync(string assistantId, RequestOptions options)
    {
        Argument.AssertNotNullOrEmpty(assistantId, nameof(assistantId));

        using PipelineMessage message = CreateGetAssistantRequest(assistantId, options);
        return ClientResult.FromResponse(await _pipeline.ProcessMessageAsync(message, options).ConfigureAwait(false));
    }

    /// <summary>
    /// [Protocol Method] Retrieves an assistant.
    /// </summary>
    /// <param name="assistantId"> The ID of the assistant to retrieve. </param>
    /// <param name="options"> The request options, which can override default behaviors of the client pipeline on a per-call basis. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="assistantId"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="assistantId"/> is an empty string, and was expected to be non-empty. </exception>
    /// <exception cref="ClientResultException"> Service returned a non-success status code. </exception>
    /// <returns> The response returned from the service. </returns>
    public virtual ClientResult GetAssistant(string assistantId, RequestOptions options)
    {
        Argument.AssertNotNullOrEmpty(assistantId, nameof(assistantId));

        using PipelineMessage message = CreateGetAssistantRequest(assistantId, options);
        return ClientResult.FromResponse(_pipeline.ProcessMessage(message, options));
    }

    /// <summary>
    /// [Protocol Method] Modifies an assistant.
    /// </summary>
    /// <param name="assistantId"> The ID of the assistant to modify. </param>
    /// <param name="content"> The content to send as the body of the request. </param>
    /// <param name="options"> The request options, which can override default behaviors of the client pipeline on a per-call basis. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="assistantId"/> or <paramref name="content"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="assistantId"/> is an empty string, and was expected to be non-empty. </exception>
    /// <exception cref="ClientResultException"> Service returned a non-success status code. </exception>
    /// <returns> The response returned from the service. </returns>
    public virtual async Task<ClientResult> ModifyAssistantAsync(string assistantId, BinaryContent content, RequestOptions options = null)
    {
        Argument.AssertNotNullOrEmpty(assistantId, nameof(assistantId));
        Argument.AssertNotNull(content, nameof(content));

        using PipelineMessage message = CreateModifyAssistantRequest(assistantId, content, options);
        return ClientResult.FromResponse(await _pipeline.ProcessMessageAsync(message, options).ConfigureAwait(false));
    }

    /// <summary>
    /// [Protocol Method] Modifies an assistant.
    /// </summary>
    /// <param name="assistantId"> The ID of the assistant to modify. </param>
    /// <param name="content"> The content to send as the body of the request. </param>
    /// <param name="options"> The request options, which can override default behaviors of the client pipeline on a per-call basis. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="assistantId"/> or <paramref name="content"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="assistantId"/> is an empty string, and was expected to be non-empty. </exception>
    /// <exception cref="ClientResultException"> Service returned a non-success status code. </exception>
    /// <returns> The response returned from the service. </returns>
    public virtual ClientResult ModifyAssistant(string assistantId, BinaryContent content, RequestOptions options = null)
    {
        Argument.AssertNotNullOrEmpty(assistantId, nameof(assistantId));
        Argument.AssertNotNull(content, nameof(content));

        using PipelineMessage message = CreateModifyAssistantRequest(assistantId, content, options);
        return ClientResult.FromResponse(_pipeline.ProcessMessage(message, options));
    }

    /// <summary>
    /// [Protocol Method] Delete an assistant.
    /// </summary>
    /// <param name="assistantId"> The ID of the assistant to delete. </param>
    /// <param name="options"> The request options, which can override default behaviors of the client pipeline on a per-call basis. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="assistantId"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="assistantId"/> is an empty string, and was expected to be non-empty. </exception>
    /// <exception cref="ClientResultException"> Service returned a non-success status code. </exception>
    /// <returns> The response returned from the service. </returns>
    public virtual async Task<ClientResult> DeleteAssistantAsync(string assistantId, RequestOptions options)
    {
        Argument.AssertNotNullOrEmpty(assistantId, nameof(assistantId));

        using PipelineMessage message = CreateDeleteAssistantRequest(assistantId, options);
        return ClientResult.FromResponse(await _pipeline.ProcessMessageAsync(message, options).ConfigureAwait(false));
    }

    /// <summary>
    /// [Protocol Method] Delete an assistant.
    /// </summary>
    /// <param name="assistantId"> The ID of the assistant to delete. </param>
    /// <param name="options"> The request options, which can override default behaviors of the client pipeline on a per-call basis. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="assistantId"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="assistantId"/> is an empty string, and was expected to be non-empty. </exception>
    /// <exception cref="ClientResultException"> Service returned a non-success status code. </exception>
    /// <returns> The response returned from the service. </returns>
    public virtual ClientResult DeleteAssistant(string assistantId, RequestOptions options)
    {
        Argument.AssertNotNullOrEmpty(assistantId, nameof(assistantId));

        using PipelineMessage message = CreateDeleteAssistantRequest(assistantId, options);
        return ClientResult.FromResponse(_pipeline.ProcessMessage(message, options));
    }

    /// <inheritdoc cref="InternalAssistantMessageClient.CreateMessageAsync"/>
    public virtual Task<ClientResult> CreateMessageAsync(string threadId, BinaryContent content, RequestOptions options = null)
        => _messageSubClient.CreateMessageAsync(threadId, content, options);

    /// <inheritdoc cref="InternalAssistantMessageClient.CreateMessage"/>
    public virtual ClientResult CreateMessage(string threadId, BinaryContent content, RequestOptions options = null)
        => _messageSubClient.CreateMessage(threadId, content, options);

    public virtual IAsyncEnumerable<ClientResult> GetMessagesAsync(string threadId, int? limit, string order, string after, string before, RequestOptions options)
    {
        MessageCollectionPageToken firstPageToken = MessageCollectionPageToken.FromOptions(threadId, limit, order, after, before);
        return OpenAIPageCollectionHelpers.CreateProtocolAsync(firstPageToken, GetMessagesPageAsync, MessageCollectionPageToken.FromToken, options);
    }

    internal virtual async Task<ClientResult> GetMessagesPageAsync(ContinuationToken pageToken, RequestOptions options)
    {
        MessageCollectionPageToken token = MessageCollectionPageToken.FromToken(pageToken);
        return await GetMessagesPageAsync(token.ThreadId, token.Limit, token.Order, token.After, token.Before, options).ConfigureAwait(false);
    }

    /// <inheritdoc cref="InternalAssistantMessageClient.GetMessagesAsync"/>
    internal virtual async Task<ClientResult> GetMessagesPageAsync(string threadId, int? limit, string order, string after, string before, RequestOptions options)
        => await _messageSubClient.GetMessagesAsync(threadId, limit, order, after, before, options).ConfigureAwait(false);

    public virtual IEnumerable<ClientResult> GetMessages(string threadId, int? limit, string order, string after, string before, RequestOptions options)
    {
        IEnumerator<ClientResult> subclient = new MessageCollectionClient(_messageSubClient, threadId, limit, order, after, before, options);
        return PageCollectionHelpers.CreateProtocol(subclient);
    }

    /// <inheritdoc cref="InternalAssistantMessageClient.GetMessageAsync"/>
    public virtual Task<ClientResult> GetMessageAsync(string threadId, string messageId, RequestOptions options)
        => _messageSubClient.GetMessageAsync(threadId, messageId, options);

    /// <inheritdoc cref="InternalAssistantMessageClient.GetMessage"/>
    public virtual ClientResult GetMessage(string threadId, string messageId, RequestOptions options)
        => _messageSubClient.GetMessage(threadId, messageId, options);
    /// <inheritdoc cref="InternalAssistantMessageClient.ModifyMessageAsync"/>
    public virtual Task<ClientResult> ModifyMessageAsync(string threadId, string messageId, BinaryContent content, RequestOptions options = null)
        => _messageSubClient.ModifyMessageAsync(threadId, messageId, content, options);

    /// <inheritdoc cref="InternalAssistantMessageClient.ModifyMessage"/>
    public virtual ClientResult ModifyMessage(string threadId, string messageId, BinaryContent content, RequestOptions options = null)
        => _messageSubClient.ModifyMessage(threadId, messageId, content, options);
    /// <inheritdoc cref="InternalAssistantMessageClient.DeleteMessageAsync"/>
    public virtual Task<ClientResult> DeleteMessageAsync(string threadId, string messageId, RequestOptions options)
        => _messageSubClient.DeleteMessageAsync(threadId, messageId, options);

    /// <inheritdoc cref="InternalAssistantMessageClient.DeleteMessage"/>
    public virtual ClientResult DeleteMessage(string threadId, string messageId, RequestOptions options)
        => _messageSubClient.DeleteMessage(threadId, messageId, options);

    /// <inheritdoc cref="InternalAssistantRunClient.CreateThreadAndRunAsync"/>
    public virtual Task<ClientResult> CreateThreadAndRunAsync(BinaryContent content, RequestOptions options = null)
        => _runSubClient.CreateThreadAndRunAsync(content, options);

    /// <inheritdoc cref="InternalAssistantRunClient.CreateThreadAndRun"/>
    public virtual ClientResult CreateThreadAndRun(BinaryContent content, RequestOptions options = null)
        => _runSubClient.CreateThreadAndRun(content, options = null);

    /// <inheritdoc cref="InternalAssistantRunClient.CreateRunAsync"/>
    public virtual Task<ClientResult> CreateRunAsync(string threadId, BinaryContent content, RequestOptions options = null)
        => _runSubClient.CreateRunAsync(threadId, content, options);

    /// <inheritdoc cref="InternalAssistantRunClient.CreateRun"/>
    public virtual ClientResult CreateRun(string threadId, BinaryContent content, RequestOptions options = null)
        => _runSubClient.CreateRun(threadId, content, options);

    public virtual IAsyncEnumerable<ClientResult> GetRunsAsync(string threadId, int? limit, string order, string after, string before, RequestOptions options)
    {
        RunCollectionPageToken firstPageToken = RunCollectionPageToken.FromOptions(threadId, limit, order, after, before);
        return OpenAIPageCollectionHelpers.CreateProtocolAsync(firstPageToken, GetRunsPageAsync, RunCollectionPageToken.FromToken, options);
    }

    internal async virtual Task<ClientResult> GetRunsPageAsync(ContinuationToken pageToken, RequestOptions options)
    {
        RunCollectionPageToken token = RunCollectionPageToken.FromToken(pageToken);
        return await GetRunsPageAsync(token.ThreadId, token.Limit, token.Order, token.After, token.Before, options).ConfigureAwait(false);
    }

    /// <inheritdoc cref="InternalAssistantRunClient.GetRunsAsync"/>
    internal async virtual Task<ClientResult> GetRunsPageAsync(string threadId, int? limit, string order, string after, string before, RequestOptions options)
        => await _runSubClient.GetRunsAsync(threadId, limit, order, after, before, options).ConfigureAwait(false);

    public virtual IEnumerable<ClientResult> GetRuns(string threadId, int? limit, string order, string after, string before, RequestOptions options)
    {
        RunCollectionPageToken firstPageToken = RunCollectionPageToken.FromOptions(threadId, limit, order, after, before);
        return OpenAIPageCollectionHelpers.CreateProtocol(firstPageToken, GetRunsPage, RunCollectionPageToken.FromToken, options);
    }

    internal virtual ClientResult GetRunsPage(ContinuationToken pageToken, RequestOptions options)
    {
        RunCollectionPageToken token = RunCollectionPageToken.FromToken(pageToken);
        return GetRunsPage(token.ThreadId, token.Limit, token.Order, token.After, token.Before, options);
    }

    /// <inheritdoc cref="InternalAssistantRunClient.GetRuns"/>
    internal virtual ClientResult GetRunsPage(string threadId, int? limit, string order, string after, string before, RequestOptions options)
        => _runSubClient.GetRuns(threadId, limit, order, after, before, options);

    /// <inheritdoc cref="InternalAssistantRunClient.GetRunAsync"/>
    public virtual Task<ClientResult> GetRunAsync(string threadId, string runId, RequestOptions options)
        => _runSubClient.GetRunAsync(threadId, runId, options);

    /// <inheritdoc cref="InternalAssistantRunClient.GetRun"/>
    public virtual ClientResult GetRun(string threadId, string runId, RequestOptions options)
        => _runSubClient.GetRun(threadId, runId, options);

    /// <inheritdoc cref="InternalAssistantRunClient.ModifyRunAsync"/>
    public virtual Task<ClientResult> ModifyRunAsync(string threadId, string runId, BinaryContent content, RequestOptions options = null)
        => _runSubClient.ModifyRunAsync(threadId, runId, content, options);

    /// <inheritdoc cref="InternalAssistantRunClient.ModifyRun"/>
    public virtual ClientResult ModifyRun(string threadId, string runId, BinaryContent content, RequestOptions options = null)
        => _runSubClient.ModifyRun(threadId, runId, content, options);

    /// <inheritdoc cref="InternalAssistantRunClient.CancelRunAsync"/>
    public virtual Task<ClientResult> CancelRunAsync(string threadId, string runId, RequestOptions options)
        => _runSubClient.CancelRunAsync(threadId, runId, options);

    /// <inheritdoc cref="InternalAssistantRunClient.CancelRun"/>
    public virtual ClientResult CancelRun(string threadId, string runId, RequestOptions options)
        => _runSubClient.CancelRun(threadId, runId, options);

    /// <inheritdoc cref="InternalAssistantRunClient.SubmitToolOutputsToRunAsync"/>
    public virtual Task<ClientResult> SubmitToolOutputsToRunAsync(string threadId, string runId, BinaryContent content, RequestOptions options = null)
        => _runSubClient.SubmitToolOutputsToRunAsync(threadId, runId, content, options);

    /// <inheritdoc cref="InternalAssistantRunClient.SubmitToolOutputsToRun"/>
    public virtual ClientResult SubmitToolOutputsToRun(string threadId, string runId, BinaryContent content, RequestOptions options = null)
        => _runSubClient.SubmitToolOutputsToRun(threadId, runId, content, options);

    public virtual IAsyncEnumerable<ClientResult> GetRunStepsAsync(string threadId, string runId, int? limit, string order, string after, string before, RequestOptions options)
    {
        RunStepCollectionPageToken firstPageToken = RunStepCollectionPageToken.FromOptions(threadId, runId, limit, order, after, before);
        return OpenAIPageCollectionHelpers.CreateProtocolAsync(firstPageToken, GetRunStepsPageAsync, RunStepCollectionPageToken.FromToken, options);
    }

    internal virtual async Task<ClientResult> GetRunStepsPageAsync(ContinuationToken pageToken, RequestOptions options)
    {
        RunStepCollectionPageToken token = RunStepCollectionPageToken.FromToken(pageToken);
        return await GetRunStepsPageAsync(token.ThreadId, token.RunId, token.Limit, token.Order, token.After, token.Before, options).ConfigureAwait(false);
    }

    /// <inheritdoc cref="InternalAssistantRunClient.GetRunStepsAsync"/>
    internal virtual async Task<ClientResult> GetRunStepsPageAsync(string threadId, string runId, int? limit, string order, string after, string before, RequestOptions options)
        => await _runSubClient.GetRunStepsAsync(threadId, runId, limit, order, after, before, options).ConfigureAwait(false);

    public virtual IEnumerable<ClientResult> GetRunSteps(string threadId, string runId, int? limit, string order, string after, string before, RequestOptions options)
    {
        RunStepCollectionPageToken firstPageToken = RunStepCollectionPageToken.FromOptions(threadId, runId, limit, order, after, before);
        return OpenAIPageCollectionHelpers.CreateProtocol(firstPageToken, GetRunStepsPage, RunStepCollectionPageToken.FromToken, options);
    }

    internal virtual ClientResult GetRunStepsPage(ContinuationToken pageToken, RequestOptions options)
    {
        RunStepCollectionPageToken token = RunStepCollectionPageToken.FromToken(pageToken);
        return GetRunStepsPage(token.ThreadId, token.RunId, token.Limit, token.Order, token.After, token.Before, options);
    }

    /// <inheritdoc cref="InternalAssistantRunClient.GetRunSteps"/>
    internal virtual ClientResult GetRunStepsPage(string threadId, string runId, int? limit, string order, string after, string before, RequestOptions options)
        => _runSubClient.GetRunSteps(threadId, runId, limit, order, after, before, options);

    /// <inheritdoc cref="InternalAssistantRunClient.GetRunStepAsync"/>
    public virtual Task<ClientResult> GetRunStepAsync(string threadId, string runId, string stepId, RequestOptions options)
        => _runSubClient.GetRunStepAsync(threadId, runId, stepId, options);

    /// <inheritdoc cref="InternalAssistantRunClient.GetRunStep"/>
    public virtual ClientResult GetRunStep(string threadId, string runId, string stepId, RequestOptions options)
        => _runSubClient.GetRunStep(threadId, runId, stepId, options);

    /// <inheritdoc cref="InternalAssistantThreadClient.CreateThreadAsync"/>
    public virtual Task<ClientResult> CreateThreadAsync(BinaryContent content, RequestOptions options = null)
        => _threadSubClient.CreateThreadAsync(content, options);

    /// <inheritdoc cref="InternalAssistantThreadClient.CreateThread"/>
    public virtual ClientResult CreateThread(BinaryContent content, RequestOptions options = null)
        => _threadSubClient.CreateThread(content, options);

    /// <inheritdoc cref="InternalAssistantThreadClient.GetThreadAsync"/>
    public virtual Task<ClientResult> GetThreadAsync(string threadId, RequestOptions options)
        => _threadSubClient.GetThreadAsync(threadId, options);

    /// <inheritdoc cref="InternalAssistantThreadClient.GetThread"/>
    public virtual ClientResult GetThread(string threadId, RequestOptions options)
        => _threadSubClient.GetThread(threadId, options);

    /// <inheritdoc cref="InternalAssistantThreadClient.ModifyThreadAsync"/>
    public virtual Task<ClientResult> ModifyThreadAsync(string threadId, BinaryContent content, RequestOptions options = null)
        => _threadSubClient.ModifyThreadAsync(threadId, content, options);

    /// <inheritdoc cref="InternalAssistantThreadClient.ModifyThread"/>
    public virtual ClientResult ModifyThread(string threadId, BinaryContent content, RequestOptions options = null)
        => _threadSubClient.ModifyThread(threadId, content, options);

    /// <inheritdoc cref="InternalAssistantThreadClient.DeleteThreadAsync"/>
    public virtual Task<ClientResult> DeleteThreadAsync(string threadId, RequestOptions options)
        => _threadSubClient.DeleteThreadAsync(threadId, options);

    /// <inheritdoc cref="InternalAssistantThreadClient.DeleteThread"/>
    public virtual ClientResult DeleteThread(string threadId, RequestOptions options)
        => _threadSubClient.DeleteThread(threadId, options);
}
