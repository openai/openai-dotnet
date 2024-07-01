using System.ClientModel;
using System.Collections;
using System.Collections.Generic;

#nullable enable

namespace OpenAI;

internal abstract class PageEnumerator<T> : IEnumerator<PageResult<T>>
{
    private readonly PageResultEnumerator _resultEnumerator;

    public PageEnumerator(PageResultEnumerator resultEnumerator)
    {
        _resultEnumerator = resultEnumerator;
    }

    public abstract PageResult<T> GetPageFromResult(ClientResult result);

    public PageResult<T> Current => GetPageFromResult(_resultEnumerator.Current);

    object IEnumerator.Current => Current;

    public bool MoveNext() => _resultEnumerator.MoveNext();

    public void Reset() => _resultEnumerator.Reset();

    public void Dispose() => _resultEnumerator.Dispose();
}
