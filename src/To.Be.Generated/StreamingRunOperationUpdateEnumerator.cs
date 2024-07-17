using System;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

#nullable enable

namespace OpenAI.Assistants;

internal partial class StreamingRunOperationUpdateEnumerator :
    IAsyncEnumerator<StreamingUpdate>,
    IEnumerator<StreamingUpdate>
{
    private StreamingUpdate? _current;
    //private bool _hasNext = true;

    private AsyncStreamingUpdateCollection? _asyncUpdates;
    private IAsyncEnumerator<StreamingUpdate>? _asyncEnumerator;

    private StreamingUpdateCollection? _updates;
    private IEnumerator<StreamingUpdate>? _enumerator;

    public StreamingRunOperationUpdateEnumerator(
        AsyncStreamingUpdateCollection updates)
    {
        _asyncUpdates = updates;
        _asyncEnumerator = updates.GetAsyncEnumerator();
    }

    public StreamingRunOperationUpdateEnumerator(
        StreamingUpdateCollection updates)
    {
        _updates = updates;
        _enumerator = updates.GetEnumerator();
    }

    // Cache this here for now
    public PipelineResponse GetRawResponse() =>
        _asyncUpdates?.GetRawResponse() ??
        _updates?.GetRawResponse() ??
        throw new InvalidOperationException("No response available.");
    
    public StreamingUpdate Current => _current!;

    #region IEnumerator<StreamingUpdate> methods

    object IEnumerator.Current => _current!;

    public bool MoveNext()
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

    #endregion

    #region IAsyncEnumerator<StreamingUpdate> methods

    public async ValueTask<bool> MoveNextAsync()
    {
        if (_asyncEnumerator is null)
        {
            throw new InvalidOperationException("Cannot MoveNextAsync after starting enumerator synchronously.");
        }

        //if (!_hasNext)
        //{
        //    _current = null;
        //    return false;
        //}

        //_hasNext = await _asyncEnumerator.MoveNextAsync().ConfigureAwait(false);
        //_current = _asyncEnumerator.Current;

        //return true;

        bool movedNext = await _asyncEnumerator.MoveNextAsync().ConfigureAwait(false);
        _current = _asyncEnumerator.Current;
        return movedNext;
    }

    public async ValueTask DisposeAsync()
    {
        // TODO: implement according to pattern

        if (_asyncEnumerator is null)
        {
            return;
        }

        await _asyncEnumerator.DisposeAsync().ConfigureAwait(false);
    }

    #endregion

    // Methods needed by LRO implementation

    public async Task ReplaceUpdateCollectionAsync(AsyncStreamingUpdateCollection updates)
    {
        if (_asyncUpdates is null || _asyncEnumerator is null)
        {
            throw new InvalidOperationException("Cannot replace null update collection.");
        }

        if (_updates is not null || _enumerator is not null)
        {
            throw new InvalidOperationException("Cannot being enumerating asynchronously after enumerating synchronously.");
        }

        await _asyncEnumerator.DisposeAsync().ConfigureAwait(false);

        _asyncUpdates = updates;
        _asyncEnumerator = updates.GetAsyncEnumerator();

        //_hasNext = await _asyncEnumerator.MoveNextAsync().ConfigureAwait(false);
        //_current = _asyncEnumerator.Current;
    }

    public void ReplaceUpdateCollection(StreamingUpdateCollection updates)
    {
        if (_updates is null || _enumerator is null)
        {
            throw new InvalidOperationException("Cannot replace null update collection.");
        }

        if (_asyncUpdates is not null || _asyncEnumerator is not null)
        {
            throw new InvalidOperationException("Cannot being enumerating synchronously after enumerating asynchronously.");
        }

        _enumerator.Dispose();

        _updates = updates;
        _enumerator = _updates.GetEnumerator();

        //_hasNext = _enumerator.MoveNext();
        //_current = _enumerator.Current;
    }
}
