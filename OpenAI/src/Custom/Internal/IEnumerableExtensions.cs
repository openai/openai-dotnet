using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace OpenAI;

internal static class IEnumerableExtensions
{
    public static async IAsyncEnumerable<T> ToAsyncEnumerable<T>(this IEnumerable<T> enumerable, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        foreach (T item in enumerable)
        {
            await Task.Delay(0, cancellationToken).ConfigureAwait(false);
            yield return item;
        }
    }
}
