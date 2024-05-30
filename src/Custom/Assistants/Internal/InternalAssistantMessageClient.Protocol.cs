using System;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.Threading.Tasks;

namespace OpenAI.Assistants;

internal partial class InternalAssistantMessageClient
{
    /// <summary>
    /// [Protocol Method] Create a message.
    /// </summary>
    /// <param name="threadId"> The ID of the [thread](/docs/api-reference/threads) to create a message for. </param>
    /// <param name="content"> The content to send as the body of the request. </param>
    /// <param name="options"> The request options, which can override default behaviors of the client pipeline on a per-call basis. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="threadId"/> or <paramref name="content"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="threadId"/> is an empty string, and was expected to be non-empty. </exception>
    /// <exception cref="ClientResultException"> Service returned a non-success status code. </exception>
    /// <returns> The response returned from the service. </returns>
    public virtual async Task<ClientResult> CreateMessageAsync(string threadId, BinaryContent content, RequestOptions options = null)
    {
        Argument.AssertNotNullOrEmpty(threadId, nameof(threadId));
        Argument.AssertNotNull(content, nameof(content));

        using PipelineMessage message = CreateCreateMessageRequest(threadId, content, options);
        return ClientResult.FromResponse(await _pipeline.ProcessMessageAsync(message, options).ConfigureAwait(false));
    }

    /// <summary>
    /// [Protocol Method] Create a message.
    /// </summary>
    /// <param name="threadId"> The ID of the [thread](/docs/api-reference/threads) to create a message for. </param>
    /// <param name="content"> The content to send as the body of the request. </param>
    /// <param name="options"> The request options, which can override default behaviors of the client pipeline on a per-call basis. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="threadId"/> or <paramref name="content"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="threadId"/> is an empty string, and was expected to be non-empty. </exception>
    /// <exception cref="ClientResultException"> Service returned a non-success status code. </exception>
    /// <returns> The response returned from the service. </returns>
    public virtual ClientResult CreateMessage(string threadId, BinaryContent content, RequestOptions options = null)
    {
        Argument.AssertNotNullOrEmpty(threadId, nameof(threadId));
        Argument.AssertNotNull(content, nameof(content));

        using PipelineMessage message = CreateCreateMessageRequest(threadId, content, options);
        return ClientResult.FromResponse(_pipeline.ProcessMessage(message, options));
    }

    /// <summary>
    /// [Protocol Method] Returns a list of messages for a given thread.
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
    /// <returns> The response returned from the service. </returns>
    public virtual async Task<ClientResult> GetMessagesAsync(string threadId, int? limit, string order, string after, string before, RequestOptions options)
    {
        Argument.AssertNotNullOrEmpty(threadId, nameof(threadId));

        using PipelineMessage message = CreateGetMessagesRequest(threadId, limit, order, after, before, options);
        return ClientResult.FromResponse(await _pipeline.ProcessMessageAsync(message, options).ConfigureAwait(false));
    }

    /// <summary>
    /// [Protocol Method] Returns a list of messages for a given thread.
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
    /// <returns> The response returned from the service. </returns>
    public virtual ClientResult GetMessages(string threadId, int? limit, string order, string after, string before, RequestOptions options)
    {
        Argument.AssertNotNullOrEmpty(threadId, nameof(threadId));

        using PipelineMessage message = CreateGetMessagesRequest(threadId, limit, order, after, before, options);
        return ClientResult.FromResponse(_pipeline.ProcessMessage(message, options));
    }

    /// <summary>
    /// [Protocol Method] Retrieve a message.
    /// </summary>
    /// <param name="threadId"> The ID of the [thread](/docs/api-reference/threads) to which this message belongs. </param>
    /// <param name="messageId"> The ID of the message to retrieve. </param>
    /// <param name="options"> The request options, which can override default behaviors of the client pipeline on a per-call basis. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="threadId"/> or <paramref name="messageId"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="threadId"/> or <paramref name="messageId"/> is an empty string, and was expected to be non-empty. </exception>
    /// <exception cref="ClientResultException"> Service returned a non-success status code. </exception>
    /// <returns> The response returned from the service. </returns>
    public virtual async Task<ClientResult> GetMessageAsync(string threadId, string messageId, RequestOptions options)
    {
        Argument.AssertNotNullOrEmpty(threadId, nameof(threadId));
        Argument.AssertNotNullOrEmpty(messageId, nameof(messageId));

        using PipelineMessage message = CreateGetMessageRequest(threadId, messageId, options);
        return ClientResult.FromResponse(await _pipeline.ProcessMessageAsync(message, options).ConfigureAwait(false));
    }

    /// <summary>
    /// [Protocol Method] Retrieve a message.
    /// </summary>
    /// <param name="threadId"> The ID of the [thread](/docs/api-reference/threads) to which this message belongs. </param>
    /// <param name="messageId"> The ID of the message to retrieve. </param>
    /// <param name="options"> The request options, which can override default behaviors of the client pipeline on a per-call basis. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="threadId"/> or <paramref name="messageId"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="threadId"/> or <paramref name="messageId"/> is an empty string, and was expected to be non-empty. </exception>
    /// <exception cref="ClientResultException"> Service returned a non-success status code. </exception>
    /// <returns> The response returned from the service. </returns>
    public virtual ClientResult GetMessage(string threadId, string messageId, RequestOptions options)
    {
        Argument.AssertNotNullOrEmpty(threadId, nameof(threadId));
        Argument.AssertNotNullOrEmpty(messageId, nameof(messageId));

        using PipelineMessage message = CreateGetMessageRequest(threadId, messageId, options);
        return ClientResult.FromResponse(_pipeline.ProcessMessage(message, options));
    }

    /// <summary>
    /// [Protocol Method] Modifies a message.
    /// </summary>
    /// <param name="threadId"> The ID of the thread to which this message belongs. </param>
    /// <param name="messageId"> The ID of the message to modify. </param>
    /// <param name="content"> The content to send as the body of the request. </param>
    /// <param name="options"> The request options, which can override default behaviors of the client pipeline on a per-call basis. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="threadId"/>, <paramref name="messageId"/> or <paramref name="content"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="threadId"/> or <paramref name="messageId"/> is an empty string, and was expected to be non-empty. </exception>
    /// <exception cref="ClientResultException"> Service returned a non-success status code. </exception>
    /// <returns> The response returned from the service. </returns>
    public virtual async Task<ClientResult> ModifyMessageAsync(string threadId, string messageId, BinaryContent content, RequestOptions options = null)
    {
        Argument.AssertNotNullOrEmpty(threadId, nameof(threadId));
        Argument.AssertNotNullOrEmpty(messageId, nameof(messageId));
        Argument.AssertNotNull(content, nameof(content));

        using PipelineMessage message = CreateModifyMessageRequest(threadId, messageId, content, options);
        return ClientResult.FromResponse(await _pipeline.ProcessMessageAsync(message, options).ConfigureAwait(false));
    }

    /// <summary>
    /// [Protocol Method] Modifies a message.
    /// </summary>
    /// <param name="threadId"> The ID of the thread to which this message belongs. </param>
    /// <param name="messageId"> The ID of the message to modify. </param>
    /// <param name="content"> The content to send as the body of the request. </param>
    /// <param name="options"> The request options, which can override default behaviors of the client pipeline on a per-call basis. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="threadId"/>, <paramref name="messageId"/> or <paramref name="content"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="threadId"/> or <paramref name="messageId"/> is an empty string, and was expected to be non-empty. </exception>
    /// <exception cref="ClientResultException"> Service returned a non-success status code. </exception>
    /// <returns> The response returned from the service. </returns>
    public virtual ClientResult ModifyMessage(string threadId, string messageId, BinaryContent content, RequestOptions options = null)
    {
        Argument.AssertNotNullOrEmpty(threadId, nameof(threadId));
        Argument.AssertNotNullOrEmpty(messageId, nameof(messageId));
        Argument.AssertNotNull(content, nameof(content));

        using PipelineMessage message = CreateModifyMessageRequest(threadId, messageId, content, options);
        return ClientResult.FromResponse(_pipeline.ProcessMessage(message, options));
    }

    /// <summary>
    /// [Protocol Method] Deletes a message.
    /// </summary>
    /// <param name="threadId"> The ID of the thread to which this message belongs. </param>
    /// <param name="messageId"> The ID of the message to delete. </param>
    /// <param name="options"> The request options, which can override default behaviors of the client pipeline on a per-call basis. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="threadId"/> or <paramref name="messageId"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="threadId"/> or <paramref name="messageId"/> is an empty string, and was expected to be non-empty. </exception>
    /// <exception cref="ClientResultException"> Service returned a non-success status code. </exception>
    /// <returns> The response returned from the service. </returns>
    public virtual async Task<ClientResult> DeleteMessageAsync(string threadId, string messageId, RequestOptions options)
    {
        Argument.AssertNotNullOrEmpty(threadId, nameof(threadId));
        Argument.AssertNotNullOrEmpty(messageId, nameof(messageId));

        using PipelineMessage message = CreateDeleteMessageRequest(threadId, messageId, options);
        return ClientResult.FromResponse(await _pipeline.ProcessMessageAsync(message, options).ConfigureAwait(false));
    }

    /// <summary>
    /// [Protocol Method] Deletes a message.
    /// </summary>
    /// <param name="threadId"> The ID of the thread to which this message belongs. </param>
    /// <param name="messageId"> The ID of the message to delete. </param>
    /// <param name="options"> The request options, which can override default behaviors of the client pipeline on a per-call basis. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="threadId"/> or <paramref name="messageId"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="threadId"/> or <paramref name="messageId"/> is an empty string, and was expected to be non-empty. </exception>
    /// <exception cref="ClientResultException"> Service returned a non-success status code. </exception>
    /// <returns> The response returned from the service. </returns>
    public virtual ClientResult DeleteMessage(string threadId, string messageId, RequestOptions options)
    {
        Argument.AssertNotNullOrEmpty(threadId, nameof(threadId));
        Argument.AssertNotNullOrEmpty(messageId, nameof(messageId));

        using PipelineMessage message = CreateDeleteMessageRequest(threadId, messageId, options);
        return ClientResult.FromResponse(_pipeline.ProcessMessage(message, options));
    }
}
