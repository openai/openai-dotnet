namespace OpenAI.Responses;

public partial class StreamingResponseWebSearchCallUpdate : StreamingResponseUpdate
{
    public string OutputItemId
        => _webSearchCallCompleted?.ItemId
        ?? _webSearchCallInProgress?.ItemId
        ?? _webSearchCallSearching?.ItemId;

    public int OutputItemIndex
        => _webSearchCallCompleted?.OutputIndex
        ?? _webSearchCallInProgress?.OutputIndex
        ?? _webSearchCallSearching?.OutputIndex
        ?? 0;

    private readonly InternalResponsesResponseStreamEventResponseWebSearchCallCompleted _webSearchCallCompleted;
    private readonly InternalResponsesResponseStreamEventResponseWebSearchCallInProgress _webSearchCallInProgress;
    private readonly InternalResponsesResponseStreamEventResponseWebSearchCallSearching _webSearchCallSearching;

    internal StreamingResponseWebSearchCallUpdate(StreamingResponseUpdate baseUpdate)
        : base(baseUpdate.Kind)
    {
        _webSearchCallCompleted = baseUpdate as InternalResponsesResponseStreamEventResponseWebSearchCallCompleted;
        _webSearchCallInProgress = baseUpdate as InternalResponsesResponseStreamEventResponseWebSearchCallInProgress;
        _webSearchCallSearching = baseUpdate as InternalResponsesResponseStreamEventResponseWebSearchCallSearching;
    }

    internal StreamingResponseWebSearchCallUpdate()
    { }
}