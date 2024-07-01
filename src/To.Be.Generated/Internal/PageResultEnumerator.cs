using System.ClientModel;
using System.Collections;
using System.Collections.Generic;

#nullable enable

namespace OpenAI;

internal abstract class PageResultEnumerator : IEnumerator<ClientResult>
{
    private ClientResult? _current;

    public ClientResult Current => _current!;

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

    // Idea is that this is generated on the client
    public abstract ClientResult GetFirst();

    // Idea is that this is generated on the client
    public abstract ClientResult GetNext(ClientResult result);

    // Idea is that this is generated on the client
    public abstract bool HasNext(ClientResult result);

    public virtual void Dispose() { }
}