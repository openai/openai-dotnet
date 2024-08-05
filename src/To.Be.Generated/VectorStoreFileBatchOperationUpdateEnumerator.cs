using System.ClientModel;
using System.ClientModel.Primitives;
using System.Collections.Generic;

#nullable enable

namespace OpenAI.VectorStores;

internal partial class VectorStoreFileBatchOperationUpdateEnumerator :
    IAsyncEnumerator<ClientResult<VectorStoreBatchFileJob>>,
    IEnumerator<ClientResult<VectorStoreBatchFileJob>>
{
    #region IEnumerator<ClientResult<VectorStoreBatchFileJob>> methods

    ClientResult<VectorStoreBatchFileJob> IEnumerator<ClientResult<VectorStoreBatchFileJob>>.Current
    {
        get
        {
            if (Current is null)
            {
                return default!;
            }

            return GetUpdateFromResult(Current);
        }
    }

    #endregion

    #region IAsyncEnumerator<ClientResult<VectorStoreBatchFileJob>> methods

    ClientResult<VectorStoreBatchFileJob> IAsyncEnumerator<ClientResult<VectorStoreBatchFileJob>>.Current
    {
        get
        {
            if (Current is null)
            {
                return default!;
            }

            return GetUpdateFromResult(Current);
        }
    }

    #endregion

    // Methods used by convenience implementation
    private ClientResult<VectorStoreBatchFileJob> GetUpdateFromResult(ClientResult result)
    {
        PipelineResponse response = result.GetRawResponse();
        VectorStoreBatchFileJob run = VectorStoreBatchFileJob.FromResponse(response);
        return ClientResult.FromValue(run, response);
    }
}
