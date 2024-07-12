using System;
using System.Collections.Generic;
using System.Threading.Tasks;

#nullable enable

namespace OpenAI;

internal class ContinuableAsyncEnumerator<T> : IAsyncEnumerator<T>
{
    private readonly Func<Task<IAsyncEnumerator<T>?>> _getNextEnumeratorAsync;

    private IAsyncEnumerator<T>? _enumerator;
    private T? _current;
    
    public ContinuableAsyncEnumerator(Func<Task<IAsyncEnumerator<T>?>> getNextEnumeratorAsync)
    {
        _getNextEnumeratorAsync = getNextEnumeratorAsync;
    }

    public T Current => _current!;

    public async ValueTask<bool> MoveNextAsync()
    {
        if (_enumerator == null)
        {
            _enumerator = await _getNextEnumeratorAsync().ConfigureAwait(false);

            if (_enumerator == null)
            {
                return false;
            }
        }

        if (!await _enumerator.MoveNextAsync().ConfigureAwait(false))
        {
            _enumerator = await _getNextEnumeratorAsync().ConfigureAwait(false);

            if (_enumerator == null || 
                !await _enumerator.MoveNextAsync().ConfigureAwait(false))
            {
                return false;
            }
        }

        _current = _enumerator.Current;
        return true;
    }

    public async ValueTask DisposeAsync()
    {
        await DisposeAsyncCore().ConfigureAwait(false);

        GC.SuppressFinalize(this);
    }

    private async ValueTask DisposeAsyncCore()
    {
        if (_enumerator is not null)
        {
            await _enumerator.DisposeAsync().ConfigureAwait(false);
            _enumerator = null;
        }
    }
}
