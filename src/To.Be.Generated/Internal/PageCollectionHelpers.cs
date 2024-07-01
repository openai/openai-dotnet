using System;
using System.ClientModel;
using System.Collections.Generic;

#nullable enable

namespace OpenAI.Utility;

internal class PageCollectionHelpers
{
    public static PageCollection<T> Create<T>(
        IEnumerator<ClientResult> enumerator,
        Func<ClientResult, PageResult<T>> getPageFromResult)
        => new FuncPageCollection<T>(enumerator, getPageFromResult);

    public static IEnumerable<ClientResult> CreateProtocol(IEnumerator<ClientResult> enumerator)
    {
        while (enumerator.MoveNext())
        {
            yield return enumerator.Current;
        }
    }

    private class FuncPageCollection<T> : PageCollection<T>
    {
        private readonly IEnumerator<ClientResult> _enumerator;
        private readonly Func<ClientResult, PageResult<T>> _getPageFromResult;

        public FuncPageCollection(
            IEnumerator<ClientResult> enumerator,
            Func<ClientResult, PageResult<T>> getPageFromResult)
        {
            _enumerator = enumerator;
            _getPageFromResult = getPageFromResult;
        }

        public override IEnumerator<PageResult<T>> GetEnumerator()
        {
            while (_enumerator.MoveNext())
            {
                yield return _getPageFromResult(_enumerator.Current);
            }
        }
    }
}
