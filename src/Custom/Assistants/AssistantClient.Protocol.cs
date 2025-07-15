using System;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.ComponentModel;
using System.Threading.Tasks;

namespace OpenAI.Assistants;

[CodeGenSuppress("GetAssistantsAsync", typeof(int?), typeof(string), typeof(string), typeof(string), typeof(RequestOptions))]
[CodeGenSuppress("GetAssistants", typeof(int?), typeof(string), typeof(string), typeof(string), typeof(RequestOptions))]
public partial class AssistantClient
{
    /// <summary>
    /// [Protocol Method] Returns a paginated collection of assistants.
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
    /// <returns> A collection of service responses, each holding a page of values. </returns>
    public virtual AsyncCollectionResult GetAssistantsAsync(int? limit, string order, string after, string before, RequestOptions options)
    {
        return new AsyncAssistantCollectionResult(this, Pipeline, options, limit, order, after, before);
    }

    /// <summary>
    /// [Protocol Method] Returns a paginated collection of assistants.
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
    /// <returns> A collection of service responses, each holding a page of values. </returns>
    public virtual CollectionResult GetAssistants(int? limit, string order, string after, string before, RequestOptions options)
    {
        return new AssistantCollectionResult(this, Pipeline, options, limit, order, after, before);
    }

    /// <inheritdoc cref="InternalAssistantMessageClient.CreateMessageAsync"/>
    public virtual Task<ClientResult> CreateMessageAsync(string threadId, BinaryContent content, RequestOptions options = null)
        => _messageSubClient.CreateMessageAsync(threadId, content, options);

    /// <inheritdoc cref="InternalAssistantMessageClient.CreateMessage"/>
    public virtual ClientResult CreateMessage(string threadId, BinaryContent content, RequestOptions options = null)
        => _messageSubClient.CreateMessage(threadId, content, options);

    /// <summary>
    /// [Protocol Method] Returns a paginated collection of messages for a given thread.
    /// </summary>
    /// <param name="threadId"> The ID of the [thread](/docs/api-reference/threads) the messages belong to. </param>
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
    /// <exception cref="ArgumentNullException"> <paramref name="threadId"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="threadId"/> is an empty string, and was expected to be non-empty. </exception>
    /// <exception cref="ClientResultException"> Service returned a non-success status code. </exception>
    /// <returns> A collection of service responses, each holding a page of values. </returns>
    public virtual AsyncCollectionResult GetMessagesAsync(string threadId, int? limit, string order, string after, string before, RequestOptions options)
    {
        Argument.AssertNotNullOrEmpty(threadId, nameof(threadId));

        return new AsyncMessageCollectionResult(_messageSubClient, options, threadId, limit, order, after, before);
    }

    /// <summary>
    /// [Protocol Method] Returns a paginated collection of messages for a given thread.
    /// </summary>
    /// <param name="threadId"> The ID of the [thread](/docs/api-reference/threads) the messages belong to. </param>
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
    /// <exception cref="ArgumentNullException"> <paramref name="threadId"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="threadId"/> is an empty string, and was expected to be non-empty. </exception>
    /// <exception cref="ClientResultException"> Service returned a non-success status code. </exception>
    /// <returns> A collection of service responses, each holding a page of values. </returns>
    public virtual CollectionResult GetMessages(string threadId, int? limit, string order, string after, string before, RequestOptions options)
    {
        Argument.AssertNotNullOrEmpty(threadId, nameof(threadId));

        return new MessageCollectionResult(_messageSubClient, options, threadId, limit, order, after, before);
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

    /// <summary>
    /// [Protocol Method] Returns a paginated collection of runs belonging to a thread.
    /// </summary>
    /// <param name="threadId"> The ID of the thread the run belongs to. </param>
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
    /// <exception cref="ArgumentNullException"> <paramref name="threadId"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="threadId"/> is an empty string, and was expected to be non-empty. </exception>
    /// <exception cref="ClientResultException"> Service returned a non-success status code. </exception>
    /// <returns> A collection of service responses, each holding a page of values. </returns>
    public virtual AsyncCollectionResult GetRunsAsync(string threadId, int? limit, string order, string after, string before, RequestOptions options)
    {
        Argument.AssertNotNullOrEmpty(threadId, nameof(threadId));

        return new AsyncRunCollectionResult(_runSubClient, options, threadId, limit, order, after, before);
    }

    /// <summary>
    /// [Protocol Method] Returns a paginated collection of runs belonging to a thread.
    /// </summary>
    /// <param name="threadId"> The ID of the thread the run belongs to. </param>
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
    /// <exception cref="ArgumentNullException"> <paramref name="threadId"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="threadId"/> is an empty string, and was expected to be non-empty. </exception>
    /// <exception cref="ClientResultException"> Service returned a non-success status code. </exception>
    /// <returns> A collection of service responses, each holding a page of values. </returns>
    public virtual CollectionResult GetRuns(string threadId, int? limit, string order, string after, string before, RequestOptions options)
    {
        Argument.AssertNotNullOrEmpty(threadId, nameof(threadId));

        return new RunCollectionResult(_runSubClient, options, threadId, limit, order, after, before);
    }

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

    /// <summary>
    /// [Protocol Method] Returns a paginated collection of run steps belonging to a run.
    /// </summary>
    /// <param name="threadId"> The ID of the thread the run and run steps belong to. </param>
    /// <param name="runId"> The ID of the run the run steps belong to. </param>
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
    /// <exception cref="ArgumentNullException"> <paramref name="threadId"/> or <paramref name="runId"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="threadId"/> or <paramref name="runId"/> is an empty string, and was expected to be non-empty. </exception>
    /// <exception cref="ClientResultException"> Service returned a non-success status code. </exception>
    /// <returns> A collection of service responses, each holding a page of values. </returns>
    public virtual AsyncCollectionResult GetRunStepsAsync(string threadId, string runId, int? limit, string order, string after, string before, RequestOptions options)
    {
        Argument.AssertNotNullOrEmpty(threadId, nameof(threadId));
        Argument.AssertNotNullOrEmpty(runId, nameof(runId));

        return new AsyncRunStepCollectionResult(_runSubClient, options, threadId, runId, limit, order, after, before);
    }

    /// <summary>
    /// [Protocol Method] Returns a paginated collection of run steps belonging to a run.
    /// </summary>
    /// <param name="threadId"> The ID of the thread the run and run steps belong to. </param>
    /// <param name="runId"> The ID of the run the run steps belong to. </param>
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
    /// <exception cref="ArgumentNullException"> <paramref name="threadId"/> or <paramref name="runId"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="threadId"/> or <paramref name="runId"/> is an empty string, and was expected to be non-empty. </exception>
    /// <exception cref="ClientResultException"> Service returned a non-success status code. </exception>
    /// <returns> A collection of service responses, each holding a page of values. </returns>
    public virtual CollectionResult GetRunSteps(string threadId, string runId, int? limit, string order, string after, string before, RequestOptions options)
    {
        Argument.AssertNotNullOrEmpty(threadId, nameof(threadId));
        Argument.AssertNotNullOrEmpty(runId, nameof(runId));

        return new RunStepCollectionResult(_runSubClient, options, threadId, runId, limit, order, after, before);
    }

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
