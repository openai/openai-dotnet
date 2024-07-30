using System;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.ComponentModel;
using System.Threading.Tasks;

namespace OpenAI.Chat;

/// <summary> The service client for the OpenAI Chat Completions endpoint. </summary>
[CodeGenSuppress("CreateChatCompletionAsync", typeof(BinaryContent), typeof(RequestOptions))]
[CodeGenSuppress("CreateChatCompletion", typeof(BinaryContent), typeof(RequestOptions))]
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
        return ClientResult.FromResponse(await _pipeline.ProcessMessageAsync(message, options).ConfigureAwait(false));
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
        return ClientResult.FromResponse(_pipeline.ProcessMessage(message, options));
    }
}
