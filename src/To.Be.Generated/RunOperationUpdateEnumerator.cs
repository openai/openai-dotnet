using System.ClientModel;
using System.ClientModel.Primitives;
using System.Collections.Generic;

#nullable enable

namespace OpenAI.Assistants;

internal partial class RunOperationUpdateEnumerator :
    IAsyncEnumerator<ClientResult<ThreadRun>>,
    IEnumerator<ClientResult<ThreadRun>>
{
    #region IEnumerator<ClientResult<ThreadRun>> methods

    ClientResult<ThreadRun> IEnumerator<ClientResult<ThreadRun>>.Current
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

    #region IAsyncEnumerator<ClientResult<ThreadRun>> methods

    ClientResult<ThreadRun> IAsyncEnumerator<ClientResult<ThreadRun>>.Current
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
    private ClientResult<ThreadRun> GetUpdateFromResult(ClientResult result)
    {
        PipelineResponse response = result.GetRawResponse();
        ThreadRun run = ThreadRun.FromResponse(response);
        return ClientResult.FromValue(run, response);
    }
}
