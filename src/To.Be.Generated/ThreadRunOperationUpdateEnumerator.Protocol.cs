using System;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

#nullable enable

namespace OpenAI.Assistants;

internal partial class ThreadRunOperationUpdateEnumerator :
    IEnumerator<ClientResult>,
    IAsyncEnumerator<ClientResult>
{
    private readonly ClientPipeline _pipeline;
    private readonly Uri _endpoint;

    public ThreadRunOperationUpdateEnumerator(
        ClientPipeline pipeline,
        Uri endpoint)
    {
        _pipeline = pipeline;
        _endpoint = endpoint;
    }

    ClientResult IEnumerator<ClientResult>.Current => throw new NotImplementedException();

    object IEnumerator.Current => throw new NotImplementedException();

    bool IEnumerator.MoveNext()
    {
        throw new NotImplementedException();
    }
    void IEnumerator.Reset()
    {
        throw new NotImplementedException();
    }

    void IDisposable.Dispose()
    {
        throw new NotImplementedException();
    }

    ClientResult IAsyncEnumerator<ClientResult>.Current => throw new NotImplementedException();

    ValueTask<bool> IAsyncEnumerator<ClientResult>.MoveNextAsync()
    {
        throw new NotImplementedException();
    }

    // TODO: handle Dispose and DisposeAsync using proper patterns
    ValueTask IAsyncDisposable.DisposeAsync()
    {
        throw new NotImplementedException();
    }
}
