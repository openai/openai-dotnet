using System.ClientModel;
using System.Collections.Generic;
using System.Threading.Tasks;

#nullable enable

namespace OpenAI;

internal abstract class PageEnumerator<T> : PageResultEnumerator,
    IAsyncEnumerator<PageResult<T>>,
    IEnumerator<PageResult<T>>
{
    public abstract PageResult<T> GetPageFromResult(ClientResult result);

    public PageResult<T> GetCurrentPage()
    {
        if (Current is null)
        {
            return GetPageFromResult(GetFirst());
        }

        return ((IEnumerator<PageResult<T>>)this).Current;
    }

    public async Task<PageResult<T>> GetCurrentPageAsync()
    {
        if (Current is null)
        {
            return GetPageFromResult(await GetFirstAsync().ConfigureAwait(false));
        }

        return ((IEnumerator<PageResult<T>>)this).Current;
    }

    PageResult<T> IEnumerator<PageResult<T>>.Current
    {
        get
        {
            if (Current is null)
            {
                return default!;
            }

            return GetPageFromResult(Current);
        }
    }

    PageResult<T> IAsyncEnumerator<PageResult<T>>.Current
    {
        get
        {
            if (Current is null)
            {
                return default!;
            }

            return GetPageFromResult(Current);
        }
    }
}
