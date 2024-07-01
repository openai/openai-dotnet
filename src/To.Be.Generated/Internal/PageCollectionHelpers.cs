using System.ClientModel;
using System.Collections.Generic;

#nullable enable

namespace OpenAI.Utility;

internal class PageCollectionHelpers
{
    public static PageCollection<T> Create<T>(PageEnumerator<T> enumerator)
        => new FuncPageCollection<T>(enumerator);

    public static IEnumerable<ClientResult> CreateProtocol(IEnumerator<ClientResult> enumerator)
    {
        while (enumerator.MoveNext())
        {
            yield return enumerator.Current;
        }
    }

    private class FuncPageCollection<T> : PageCollection<T>
    {
        private readonly PageEnumerator<T> _enumerator;
        
        public FuncPageCollection(PageEnumerator<T> enumerator)
        {
            _enumerator = enumerator;
        }

        public override IEnumerator<PageResult<T>> GetEnumerator()
        {
            while (_enumerator.MoveNext())
            {
                ClientResult result = _enumerator.Current;
                yield return _enumerator.GetPageFromResult(result);
            }
        }
    }
}
