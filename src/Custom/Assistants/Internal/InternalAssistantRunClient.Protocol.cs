using System;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.Threading.Tasks;

namespace OpenAI.Assistants;

internal partial class InternalAssistantRunClient
{
    /// <summary>
    /// [Protocol Method] Create a thread and run it in one request.
    /// </summary>
    /// <param name="content"> The content to send as the body of the request. </param>
    /// <param name="options"> The request options, which can override default behaviors of the client pipeline on a per-call basis. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="content"/> is null. </exception>
    /// <exception cref="ClientResultException"> Service returned a non-success status code. </exception>
    /// <returns> The response returned from the service. </returns>
    public virtual async Task<ClientResult> CreateThreadAndRunAsync(BinaryContent content, RequestOptions options = null)
    {
        Argument.AssertNotNull(content, nameof(content));

        PipelineMessage message = null;
        try
        {
            message = CreateCreateThreadAndRunRequest(content, options);
            return ClientResult.FromResponse(await _pipeline.ProcessMessageAsync(message, options).ConfigureAwait(false));
        }
        finally
        {
            if (options?.BufferResponse != false)
            {
                message.Dispose();
            }
        }
    }

    /// <summary>
    /// [Protocol Method] Create a thread and run it in one request.
    /// </summary>
    /// <param name="content"> The content to send as the body of the request. </param>
    /// <param name="options"> The request options, which can override default behaviors of the client pipeline on a per-call basis. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="content"/> is null. </exception>
    /// <exception cref="ClientResultException"> Service returned a non-success status code. </exception>
    /// <returns> The response returned from the service. </returns>
    public virtual ClientResult CreateThreadAndRun(BinaryContent content, RequestOptions options = null)
    {
        Argument.AssertNotNull(content, nameof(content));

        PipelineMessage message = null;
        try
        {
            message = CreateCreateThreadAndRunRequest(content, options);
            return ClientResult.FromResponse(_pipeline.ProcessMessage(message, options));
        }
        finally
        {
            if (options?.BufferResponse != false)
            {
                message.Dispose();
            }
        }
    }

    /// <summary>
    /// [Protocol Method] Create a run.
    /// </summary>
    /// <param name="threadId"> The ID of the thread to run. </param>
    /// <param name="content"> The content to send as the body of the request. </param>
    /// <param name="options"> The request options, which can override default behaviors of the client pipeline on a per-call basis. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="threadId"/> or <paramref name="content"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="threadId"/> is an empty string, and was expected to be non-empty. </exception>
    /// <exception cref="ClientResultException"> Service returned a non-success status code. </exception>
    /// <returns> The response returned from the service. </returns>
    public virtual async Task<ClientResult> CreateRunAsync(string threadId, BinaryContent content, RequestOptions options = null)
    {
        Argument.AssertNotNullOrEmpty(threadId, nameof(threadId));
        Argument.AssertNotNull(content, nameof(content));

        PipelineMessage message = null;
        try
        {
            message = CreateCreateRunRequest(threadId, content, options);
            return ClientResult.FromResponse(await _pipeline.ProcessMessageAsync(message, options).ConfigureAwait(false));
        }
        finally
        {
            if (options?.BufferResponse != false)
            {
                message.Dispose();
            }
        }
    }

    /// <summary>
    /// [Protocol Method] Create a run.
    /// </summary>
    /// <param name="threadId"> The ID of the thread to run. </param>
    /// <param name="content"> The content to send as the body of the request. </param>
    /// <param name="options"> The request options, which can override default behaviors of the client pipeline on a per-call basis. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="threadId"/> or <paramref name="content"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="threadId"/> is an empty string, and was expected to be non-empty. </exception>
    /// <exception cref="ClientResultException"> Service returned a non-success status code. </exception>
    /// <returns> The response returned from the service. </returns>
    public virtual ClientResult CreateRun(string threadId, BinaryContent content, RequestOptions options = null)
    {
        Argument.AssertNotNullOrEmpty(threadId, nameof(threadId));
        Argument.AssertNotNull(content, nameof(content));

        PipelineMessage message = null;
        try
        {
            message = CreateCreateRunRequest(threadId, content, options);
            return ClientResult.FromResponse(_pipeline.ProcessMessage(message, options));
        }
        finally
        {
            if (options?.BufferResponse != false)
            {
                message.Dispose();
            }
        }
    }

    /// <summary>
    /// [Protocol Method] Returns a list of runs belonging to a thread.
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
    /// <returns> The response returned from the service. </returns>
    public virtual async Task<ClientResult> GetRunsAsync(string threadId, int? limit, string order, string after, string before, RequestOptions options)
    {
        Argument.AssertNotNullOrEmpty(threadId, nameof(threadId));

        using PipelineMessage message = CreateGetRunsRequest(threadId, limit, order, after, before, options);
        return ClientResult.FromResponse(await _pipeline.ProcessMessageAsync(message, options).ConfigureAwait(false));
    }

    /// <summary>
    /// [Protocol Method] Returns a list of runs belonging to a thread.
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
    /// <returns> The response returned from the service. </returns>
    public virtual ClientResult GetRuns(string threadId, int? limit, string order, string after, string before, RequestOptions options)
    {
        Argument.AssertNotNullOrEmpty(threadId, nameof(threadId));

        using PipelineMessage message = CreateGetRunsRequest(threadId, limit, order, after, before, options);
        return ClientResult.FromResponse(_pipeline.ProcessMessage(message, options));
    }

}
