using System;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace OpenAI.Chat;

/// <summary> The service client for the OpenAI Chat Completions endpoint. </summary>
[CodeGenSuppress("CreateChatCompletionAsync", typeof(BinaryContent), typeof(RequestOptions))]
[CodeGenSuppress("CreateChatCompletion", typeof(BinaryContent), typeof(RequestOptions))]
[CodeGenSuppress("GetChatCompletionMessagesAsync", typeof(string), typeof(string), typeof(int?), typeof(string), typeof(RequestOptions))]
[CodeGenSuppress("GetChatCompletionMessages", typeof(string), typeof(string), typeof(int?), typeof(string), typeof(RequestOptions))]
[CodeGenSuppress("GetChatCompletionsAsync", typeof(string), typeof(int?), typeof(string), typeof(IDictionary<string, string>), typeof(string), typeof(RequestOptions))]
[CodeGenSuppress("GetChatCompletions", typeof(string), typeof(int?), typeof(string), typeof(IDictionary<string, string>), typeof(string), typeof(RequestOptions))]
[CodeGenSuppress("UpdateChatCompletionAsync", typeof(string), typeof(BinaryContent), typeof(RequestOptions))]
[CodeGenSuppress("UpdateChatCompletion", typeof(string), typeof(BinaryContent), typeof(RequestOptions))]
public partial class ChatClient
{
    /// <summary>
    /// [Protocol Method] Creates a model response for the given chat conversation.
    /// </summary>
    /// <param name="content"> The content to send as the body of the request. </param>
    /// <param name="options"> The request options, which can override default behaviors of the client pipeline on a per-call basis. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="content"/> is null. </exception>
    /// <exception cref="ClientResultException"> Service returned a non-success status code. </exception>
    /// <returns> The response returned from the service. </returns>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual async Task<ClientResult> CompleteChatAsync(BinaryContent content, RequestOptions options = null)
    {
        Argument.AssertNotNull(content, nameof(content));

        using PipelineMessage message = CreateCreateChatCompletionRequest(content, options);
        return ClientResult.FromResponse(await Pipeline.ProcessMessageAsync(message, options).ConfigureAwait(false));
    }

    /// <summary>
    /// [Protocol Method] Creates a model response for the given chat conversation.
    /// </summary>
    /// <param name="content"> The content to send as the body of the request. </param>
    /// <param name="options"> The request options, which can override default behaviors of the client pipeline on a per-call basis. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="content"/> is null. </exception>
    /// <exception cref="ClientResultException"> Service returned a non-success status code. </exception>
    /// <returns> The response returned from the service. </returns>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual ClientResult CompleteChat(BinaryContent content, RequestOptions options = null)
    {
        Argument.AssertNotNull(content, nameof(content));

        using PipelineMessage message = CreateCreateChatCompletionRequest(content, options);
        return ClientResult.FromResponse(Pipeline.ProcessMessage(message, options));
    }

    // CUSTOM: Added Experimental attribute.
    [Experimental("OPENAI001")]
    public virtual ClientResult GetChatCompletion(string completionId, RequestOptions options)
    {
        Argument.AssertNotNullOrEmpty(completionId, nameof(completionId));

        using PipelineMessage message = CreateGetChatCompletionRequest(completionId, options);
        return ClientResult.FromResponse(Pipeline.ProcessMessage(message, options));
    }

    // CUSTOM: Added Experimental attribute.
    [Experimental("OPENAI001")]
    public virtual async Task<ClientResult> GetChatCompletionAsync(string completionId, RequestOptions options)
    {
        Argument.AssertNotNullOrEmpty(completionId, nameof(completionId));

        using PipelineMessage message = CreateGetChatCompletionRequest(completionId, options);
        return ClientResult.FromResponse(await Pipeline.ProcessMessageAsync(message, options).ConfigureAwait(false));
    }

    // CUSTOM: Added Experimental attribute.
    [Experimental("OPENAI001")]
    public virtual ClientResult DeleteChatCompletion(string completionId, RequestOptions options)
    {
        Argument.AssertNotNullOrEmpty(completionId, nameof(completionId));

        using PipelineMessage message = CreateDeleteChatCompletionRequest(completionId, options);
        return ClientResult.FromResponse(Pipeline.ProcessMessage(message, options));
    }

    // CUSTOM: Added Experimental attribute.
    [Experimental("OPENAI001")]
    public virtual async Task<ClientResult> DeleteChatCompletionAsync(string completionId, RequestOptions options)
    {
        Argument.AssertNotNullOrEmpty(completionId, nameof(completionId));

        using PipelineMessage message = CreateDeleteChatCompletionRequest(completionId, options);
        return ClientResult.FromResponse(await Pipeline.ProcessMessageAsync(message, options).ConfigureAwait(false));
    }
}
