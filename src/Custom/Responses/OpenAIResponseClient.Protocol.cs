using System.ClientModel;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;

namespace OpenAI.Responses;

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
    public virtual async Task<ClientResult> GetResponseAsync(string responseId, bool? stream, int? startingAfter, RequestOptions options)
    {
        Argument.AssertNotNullOrEmpty(responseId, nameof(responseId));

        using PipelineMessage message = CreateGetResponseRequest(responseId, [], stream, startingAfter, options);

        PipelineResponse protocolResponse = await Pipeline.ProcessMessageAsync(message, options).ConfigureAwait(false);
        return ClientResult.FromResponse(protocolResponse);
    }

    public virtual ClientResult GetResponse(string responseId, bool? stream, int? startingAfter, RequestOptions options)
    {
        Argument.AssertNotNullOrEmpty(responseId, nameof(responseId));

        using PipelineMessage message = CreateGetResponseRequest(responseId, [], stream, startingAfter, options);
        PipelineResponse protocolResponse = Pipeline.ProcessMessage(message, options);
        return ClientResult.FromResponse(protocolResponse);
    }

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
}