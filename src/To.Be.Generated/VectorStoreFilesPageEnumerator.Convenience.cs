using System.ClientModel;
using System.ClientModel.Primitives;

#nullable enable

namespace OpenAI.VectorStores;

internal partial class VectorStoreFilesPageEnumerator : PageResultEnumerator
{
    // Note: this is the deserialization method that converts protocol to convenience
    public PageResult<VectorStoreFileAssociation> GetPageFromResult(ClientResult result)
    {
        PipelineResponse response = result.GetRawResponse();

        InternalListVectorStoreFilesResponse list = ModelReaderWriter.Read<InternalListVectorStoreFilesResponse>(response.Content)!;

        VectorStoreFilesPageToken pageToken = VectorStoreFilesPageToken.FromOptions(_vectorStoreId, _limit, _order, _after, _before, _filter);
        VectorStoreFilesPageToken? nextPageToken = pageToken.GetNextPageToken(list.HasMore, list.LastId);

        return PageResult<VectorStoreFileAssociation>.Create(list.Data, pageToken, nextPageToken, response);
    }
}