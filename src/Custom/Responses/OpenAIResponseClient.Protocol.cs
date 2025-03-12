using System.ClientModel;
using System.ClientModel.Primitives;
using System.ComponentModel;
using System.Threading.Tasks;

namespace OpenAI.Responses;

[CodeGenSuppress("CreateResponse", typeof(BinaryContent), typeof(string), typeof(RequestOptions))]
[CodeGenSuppress("CreateResponseAsync", typeof(BinaryContent), typeof(string), typeof(RequestOptions))]
[CodeGenSuppress("ListInputItems", typeof(string), typeof(int?), typeof(string), typeof(string), typeof(string), typeof(RequestOptions))]
[CodeGenSuppress("ListInputItemsAsync", typeof(string), typeof(int?), typeof(string), typeof(string), typeof(string), typeof(RequestOptions))]
public partial class OpenAIResponseClient
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual async Task<ClientResult> CreateResponseAsync(BinaryContent content, RequestOptions options = null)
    {
        Argument.AssertNotNull(content, nameof(content));

        string acceptHeaderValue = options?.BufferResponse == false
            ? AcceptHeaderValue.TextEventStream.ToString()
            : AcceptHeaderValue.ApplicationJson.ToString();

        using PipelineMessage message = CreateCreateResponseRequest(content, acceptHeaderValue, options);
        PipelineResponse protocolResponse = await Pipeline.ProcessMessageAsync(message, options).ConfigureAwait(false);
        return ClientResult.FromResponse(protocolResponse);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual ClientResult CreateResponse(BinaryContent content, RequestOptions options = null)
    {
        Argument.AssertNotNull(content, nameof(content));

        string acceptHeaderValue = options?.BufferResponse == false
            ? AcceptHeaderValue.TextEventStream.ToString()
            : AcceptHeaderValue.ApplicationJson.ToString();

        using PipelineMessage message = CreateCreateResponseRequest(content, acceptHeaderValue, options);
        PipelineResponse protocolResponse = Pipeline.ProcessMessage(message, options);
        return ClientResult.FromResponse(protocolResponse);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual async Task<ClientResult> GetResponseAsync(string responseId, RequestOptions options)
    {
        Argument.AssertNotNullOrEmpty(responseId, nameof(responseId));

        using PipelineMessage message = CreateGetResponseRequest(responseId, [], options);

        PipelineResponse protocolResponse = await Pipeline.ProcessMessageAsync(message, options).ConfigureAwait(false);
        return ClientResult.FromResponse(protocolResponse);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual ClientResult GetResponse(string responseId, RequestOptions options)
    {
        Argument.AssertNotNullOrEmpty(responseId, nameof(responseId));

        using PipelineMessage message = CreateGetResponseRequest(responseId, [], options);
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