using System;
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

    private AsyncStreamingUpdateCollection? _asyncUpdates;
    private IAsyncEnumerator<StreamingUpdate>? _asyncEnumerator;

    private StreamingUpdateCollection? _updates;
    private IEnumerator<StreamingUpdate>? _enumerator;

    public StreamingRunOperationUpdateEnumerator(
        AsyncStreamingUpdateCollection updates)
    {
        _asyncUpdates = updates;
    }

    public StreamingRunOperationUpdateEnumerator(
        StreamingUpdateCollection updates)
    {
        _updates = updates;
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
        if (_updates is null)
        {
            throw new InvalidOperationException("Cannot MoveNext after starting enumerator asynchronously.");
        }

        _enumerator ??= _updates.GetEnumerator();

        bool movedNext = _enumerator.MoveNext();
        _current = _enumerator.Current;
        return movedNext;
    }

    void IEnumerator.Reset()
    {
        throw new NotSupportedException("Cannot reset streaming enumerator.");
    }

    public void Dispose()
    {
        if (_enumerator != null)
        {
            _enumerator.Dispose();
            _enumerator = null;
        }
    }

    #endregion

    #region IAsyncEnumerator<StreamingUpdate> methods

    public async ValueTask<bool> MoveNextAsync()
    {
        if (_asyncUpdates is null)
        {
            throw new InvalidOperationException("Cannot MoveNextAsync after starting enumerator synchronously.");
        }

        _asyncEnumerator ??= _asyncUpdates.GetAsyncEnumerator();

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

    public async Task ReplaceUpdateCollectionAsync(AsyncStreamingUpdateCollection updates)
    {
        if (_asyncUpdates is null)
        {
            throw new InvalidOperationException("Cannot replace null update collection.");
        }

        if (_updates is not null || _enumerator is not null)
        {
            throw new InvalidOperationException("Cannot being enumerating asynchronously after enumerating synchronously.");
        }

        if (_asyncEnumerator is not null)
        {
            await _asyncEnumerator.DisposeAsync().ConfigureAwait(false);
            _asyncEnumerator = null;
        }

        _asyncUpdates = updates;
    }

    public void ReplaceUpdateCollection(StreamingUpdateCollection updates)
    {
        if (_updates is null)
        {
            throw new InvalidOperationException("Cannot replace null update collection.");
        }

        if (_asyncUpdates is not null || _asyncEnumerator is not null)
        {
            throw new InvalidOperationException("Cannot being enumerating synchronously after enumerating asynchronously.");
        }

        _enumerator?.Dispose();
        _enumerator = null;

        _updates = updates;
    }
}
