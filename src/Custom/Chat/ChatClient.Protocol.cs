using System.ClientModel;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace OpenAI.Chat;

/// <summary> The service client for the OpenAI Chat Completions endpoint. </summary>
[CodeGenSuppress("GetChatCompletionMessagesAsync", typeof(string), typeof(string), typeof(int?), typeof(string), typeof(RequestOptions))]
[CodeGenSuppress("GetChatCompletionMessages", typeof(string), typeof(string), typeof(int?), typeof(string), typeof(RequestOptions))]
[CodeGenSuppress("GetChatCompletionsAsync", typeof(string), typeof(int?), typeof(string), typeof(IDictionary<string, string>), typeof(string), typeof(RequestOptions))]
[CodeGenSuppress("GetChatCompletions", typeof(string), typeof(int?), typeof(string), typeof(IDictionary<string, string>), typeof(string), typeof(RequestOptions))]
[CodeGenSuppress("UpdateChatCompletionAsync", typeof(string), typeof(BinaryContent), typeof(RequestOptions))]
[CodeGenSuppress("UpdateChatCompletion", typeof(string), typeof(BinaryContent), typeof(RequestOptions))]
public partial class ChatClient
{
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
