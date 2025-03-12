namespace OpenAI.Responses;

public partial class StreamingResponseFileSearchCallUpdate : StreamingResponseUpdate
{
    public string OutputItemId
        => _fileSearchCallCompleted?.ItemId
        ?? _fileSearchCallInProgress?.ItemId
        ?? _fileSearchCallSearching?.ItemId;

    public int OutputItemIndex
        => _fileSearchCallCompleted?.OutputIndex
        ?? _fileSearchCallInProgress?.OutputIndex
        ?? _fileSearchCallSearching?.OutputIndex
        ?? 0;

    private readonly InternalResponsesResponseStreamEventResponseFileSearchCallCompleted _fileSearchCallCompleted;
    private readonly InternalResponsesResponseStreamEventResponseFileSearchCallInProgress _fileSearchCallInProgress;
    private readonly InternalResponsesResponseStreamEventResponseFileSearchCallSearching _fileSearchCallSearching;

    internal StreamingResponseFileSearchCallUpdate(StreamingResponseUpdate baseUpdate)
        : base(baseUpdate.Kind)
    {
        _fileSearchCallCompleted = baseUpdate as InternalResponsesResponseStreamEventResponseFileSearchCallCompleted;
        _fileSearchCallInProgress = baseUpdate as InternalResponsesResponseStreamEventResponseFileSearchCallInProgress;
        _fileSearchCallSearching = baseUpdate as InternalResponsesResponseStreamEventResponseFileSearchCallSearching;
    }

    internal StreamingResponseFileSearchCallUpdate()
    { }
}