﻿using System;
using System.ClientModel;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace OpenAI.Assistants;

internal class AsyncAssistantCollection : AsyncCollectionResult<Assistant>
{
    private readonly Func<Task<AssistantCollectionPage>> _getFirstAsync;

    public AsyncAssistantCollection(Func<Task<AssistantCollectionPage>> getFirstPageAsync)
    {
        _getFirstAsync = getFirstPageAsync;
    }

    public override async IAsyncEnumerator<Assistant> GetAsyncEnumerator(CancellationToken cancellationToken = default)
    {
        AssistantCollectionPage page = await _getFirstAsync().ConfigureAwait(false);

        while (page.HasNext)
        {
            foreach (Assistant value in page.Values)
            {
                yield return value;
            }

            page = (AssistantCollectionPage)await page.GetNextAsync().ConfigureAwait(false);
        }
    }
}