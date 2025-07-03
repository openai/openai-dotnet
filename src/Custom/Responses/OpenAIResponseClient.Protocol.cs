using System.ClientModel;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;

namespace OpenAI.Responses;

[CodeGenSuppress("CreateResponse", typeof(string), typeof(BinaryContent), typeof(RequestOptions))]
[CodeGenSuppress("CreateResponseAsync", typeof(string), typeof(BinaryContent), typeof(RequestOptions))]
[CodeGenSuppress("GetResponse", typeof(string), typeof(string), typeof(IEnumerable<InternalIncludable>), typeof(bool?), typeof(int?), typeof(RequestOptions))]
[CodeGenSuppress("GetResponseAsync", typeof(string), typeof(string), typeof(IEnumerable<InternalIncludable>), typeof(bool?), typeof(int?), typeof(RequestOptions))]
[CodeGenSuppress("GetInputItems", typeof(string), typeof(int?), typeof(string), typeof(string), typeof(string), typeof(RequestOptions))]
[CodeGenSuppress("GetInputItemsAsync", typeof(string), typeof(int?), typeof(string), typeof(string), typeof(string), typeof(RequestOptions))]
[CodeGenSuppress("CancelResponse", typeof(string), typeof(IEnumerable<InternalIncludable>), typeof(bool?), typeof(int?), typeof(RequestOptions))]
[CodeGenSuppress("CancelResponseAsync", typeof(string), typeof(IEnumerable<InternalIncludable>), typeof(bool?), typeof(int?), typeof(RequestOptions))]
[CodeGenSuppress("GetResponse", typeof(string), typeof(IEnumerable<InternalIncludable>), typeof(bool?), typeof(int?), typeof(RequestOptions))]
[CodeGenSuppress("GetResponseAsync", typeof(string), typeof(IEnumerable<InternalIncludable>), typeof(bool?), typeof(int?), typeof(RequestOptions))]
public partial class OpenAIResponseClient
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual async Task<ClientResult> CreateResponseAsync(BinaryContent content, RequestOptions options = null)
    {
        Argument.AssertNotNull(content, nameof(content));

        using PipelineMessage message = CreateCreateResponseRequest(content, options);
        PipelineResponse protocolResponse = await Pipeline.ProcessMessageAsync(message, options).ConfigureAwait(false);
        return ClientResult.FromResponse(protocolResponse);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual ClientResult CreateResponse(BinaryContent content, RequestOptions options = null)
    {
        Argument.AssertNotNull(content, nameof(content));

        using PipelineMessage message = CreateCreateResponseRequest(content, options);
        PipelineResponse protocolResponse = Pipeline.ProcessMessage(message, options);
        return ClientResult.FromResponse(protocolResponse);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual async Task<ClientResult> GetResponseAsync(string responseId, bool? stream, int? startingAfter, RequestOptions options)
    {
        Argument.AssertNotNullOrEmpty(responseId, nameof(responseId));

        using PipelineMessage message = CreateGetResponseRequest(responseId, [], stream, startingAfter, options);

        PipelineResponse protocolResponse = await Pipeline.ProcessMessageAsync(message, options).ConfigureAwait(false);
        return ClientResult.FromResponse(protocolResponse);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual ClientResult GetResponse(string responseId, bool? stream, int? startingAfter, RequestOptions options)
    {
        Argument.AssertNotNullOrEmpty(responseId, nameof(responseId));

        using PipelineMessage message = CreateGetResponseRequest(responseId, [], stream, startingAfter, options);
        PipelineResponse protocolResponse = Pipeline.ProcessMessage(message, options);
        return ClientResult.FromResponse(protocolResponse);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual AsyncCollectionResult GetResponseInputItemsAsync(
        string responseId,
        int? limit,
        string order,
        string after,
        string before,
        RequestOptions options = null)
    {
        Argument.AssertNotNull(responseId, nameof(responseId));

        return new AsyncResponseItemCollectionResult(this, responseId, limit, order, after, before, options);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual CollectionResult GetResponseInputItems(
        string responseId,
        int? limit,
        string order,
        string after,
        string before,
        RequestOptions options = null)
    {
        Argument.AssertNotNull(responseId, nameof(responseId));

        return new ResponseItemCollectionResult(this, responseId, limit, order, after, before, options);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual async Task<ClientResult> DeleteResponseAsync(string responseId, RequestOptions options)
    {
        Argument.AssertNotNull(responseId, nameof(responseId));

        using PipelineMessage message = CreateDeleteResponseRequest(responseId, options);
        PipelineResponse protocolResponse = await Pipeline.ProcessMessageAsync(message, options).ConfigureAwait(false);
        return ClientResult.FromResponse(protocolResponse);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual ClientResult DeleteResponse(string responseId, RequestOptions options)
    {
        Argument.AssertNotNull(responseId, nameof(responseId));

        using PipelineMessage message = CreateDeleteResponseRequest(responseId, options);
        PipelineResponse protocolResponse = Pipeline.ProcessMessage(message, options);
        return ClientResult.FromResponse(protocolResponse);
    }
}