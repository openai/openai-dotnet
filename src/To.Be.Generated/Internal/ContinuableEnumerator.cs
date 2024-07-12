using System.ClientModel;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

#nullable enable

namespace OpenAI;

internal class ContinuableEnumerator<T> : IEnumerator<T>
{
    public T Current => throw new System.NotImplementedException();

    object IEnumerator.Current => throw new System.NotImplementedException();

    public void Dispose()
    {
        throw new System.NotImplementedException();
    }

    public bool MoveNext()
    {
        throw new System.NotImplementedException();
    }

    public void Reset()
    {
        throw new System.NotImplementedException();
    }
}
