using System.ClientModel;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

#nullable enable

namespace OpenAI;

// Note: implements both sync and async enumerator interfaces.
internal abstract class PageResultEnumerator : IEnumerator<ClientResult>, 
    IAsyncEnumerator<ClientResult>
{
    private ClientResult? _current;

    public ClientResult Current => _current!;

    object IEnumerator.Current => Current;

    #region IEnumerable implementation

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

    // Idea is that this is generated on the client
    public abstract ClientResult GetFirst();

    // Idea is that this is generated on the client
    public abstract ClientResult GetNext(ClientResult result);

    // Idea is that this is generated on the client
    public abstract bool HasNext(ClientResult result);

    public virtual void Dispose() { }
    #endregion

    #region IAsyncEnumerable implementation

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

    // Idea is that this is generated on the client
    public abstract Task<ClientResult> GetFirstAsync();

    // Idea is that this is generated on the client
    public abstract Task<ClientResult> GetNextAsync(ClientResult result);

    public virtual ValueTask DisposeAsync() => new();
    #endregion
}