using System.ClientModel;
using System.Collections;
using System.Collections.Generic;

#nullable enable

namespace OpenAI;

internal abstract class PageEnumerator<T> : IEnumerator<PageResult<T>>
{
    public PageEnumerator(IEnumerator<ClientResult> resultEnumerator)
    {
        ResultEnumerator = resultEnumerator;
    }

    public IEnumerator<ClientResult> ResultEnumerator { get; }

    public abstract PageResult<T> GetPageFromResult(ClientResult result);

    public PageResult<T> Current => GetPageFromResult(ResultEnumerator.Current);

    object IEnumerator.Current => Current;

    public bool MoveNext() => ResultEnumerator.MoveNext();

    public void Reset() => ResultEnumerator.Reset();

    public void Dispose() => ResultEnumerator.Dispose();
}
