using System.ClientModel;
using System.ClientModel.Primitives;

#nullable enable

namespace OpenAI.VectorStores;

internal partial class VectorStoresPageEnumerator : PageResultEnumerator
{
    // Note: this is the deserialization method that converts protocol to convenience
    public PageResult<VectorStore> GetPageFromResult(ClientResult result)
    {
        PipelineResponse response = result.GetRawResponse();

        InternalListVectorStoresResponse list = ModelReaderWriter.Read<InternalListVectorStoresResponse>(response.Content)!;

        VectorStoresPageToken pageToken = VectorStoresPageToken.FromOptions(_limit, _order, _after, _before);
        VectorStoresPageToken? nextPageToken = pageToken.GetNextPageToken(list.HasMore, list.LastId);

        return PageResult<VectorStore>.Create(list.Data, pageToken, nextPageToken, response);
    }
}