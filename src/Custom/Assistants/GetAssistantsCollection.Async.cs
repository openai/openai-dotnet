using System;
using System.ClientModel;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace OpenAI.Assistants;

internal class AsyncGetAssistantsCollection : AsyncCollectionResult<Assistant>
{
    private readonly Func<Task<GetAssistantsPage>> _getFirstAsync;

    public AsyncGetAssistantsCollection(Func<Task<GetAssistantsPage>> getFirstPageAsync)
    {
        _getFirstAsync = getFirstPageAsync;
    }

    public override async IAsyncEnumerator<Assistant> GetAsyncEnumerator(CancellationToken cancellationToken = default)
    {
        GetAssistantsPage page = await _getFirstAsync().ConfigureAwait(false);

        while (page.HasNext)
        {
            foreach (Assistant value in page.Values)
            {
                yield return value;

                page = (GetAssistantsPage)await page.GetNextAsync().ConfigureAwait(false);
            }
        }
    }
}
