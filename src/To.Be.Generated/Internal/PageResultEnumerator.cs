using System.ClientModel;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

#nullable enable

namespace OpenAI;

// Note: implements both sync and async enumerator interfaces.
internal abstract class PageResultEnumerator : IAsyncEnumerator<ClientResult>, IEnumerator<ClientResult>
{
    private ClientResult? _current;

    protected PageResultEnumerator() { }

    public ClientResult Current => _current!;

    // Idea is that this is generated on the client
    public abstract Task<ClientResult> GetFirstAsync();

    // Idea is that this is generated on the client
    public abstract Task<ClientResult> GetNextAsync(ClientResult result);

    // Idea is that this is generated on the client
    public abstract ClientResult GetFirst();

    // Idea is that this is generated on the client
    public abstract ClientResult GetNext(ClientResult result);

    // Idea is that this is generated on the client
    public abstract bool HasNext(ClientResult result);

    #region IEnumerator<ClientResult> implementation

    object IEnumerator.Current => Current;

    public bool MoveNext()
    {
        if (_current == null)
        {
            _current = GetFirst();
        }
        else
        {
            _current = GetNext(_current);
        }

        return HasNext(_current);
    }

    public void Reset() => _current = null;

    public virtual void Dispose() { }

    #endregion

    #region IAsyncEnumerator<ClientResult> implementation

    public async ValueTask<bool> MoveNextAsync()
    {
        if (_current == null)
        {
            _current = await GetFirstAsync().ConfigureAwait(false);
        }
        else
        {
            _current = await GetNextAsync(_current).ConfigureAwait(false);
        }

        return HasNext(_current);
    }

    public virtual ValueTask DisposeAsync() => new();

    #endregion
}