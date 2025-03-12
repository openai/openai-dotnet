namespace OpenAI.Responses;

public partial class StreamingResponseStatusUpdate : StreamingResponseUpdate
{
    public OpenAIResponse Response
        => _responseCreated?.Response
        ?? _responseInProgress?.Response
        ?? _responseCompleted?.Response
        ?? _responseFailed?.Response
        ?? _responseIncomplete?.Response;

    private readonly InternalResponsesResponseStreamEventResponseCreated _responseCreated;
    private readonly InternalResponsesResponseStreamEventResponseInProgress _responseInProgress;
    private readonly InternalResponsesResponseStreamEventResponseCompleted _responseCompleted;
    private readonly InternalResponsesResponseStreamEventResponseFailed _responseFailed;
    private readonly InternalResponsesResponseStreamEventResponseIncomplete _responseIncomplete;

    internal StreamingResponseStatusUpdate(StreamingResponseUpdate baseUpdate)
        : base(baseUpdate.Kind)
    {
        _responseCreated = baseUpdate as InternalResponsesResponseStreamEventResponseCreated;
        _responseInProgress = baseUpdate as InternalResponsesResponseStreamEventResponseInProgress;
        _responseCompleted = baseUpdate as InternalResponsesResponseStreamEventResponseCompleted;
        _responseFailed = baseUpdate as InternalResponsesResponseStreamEventResponseFailed;
        _responseIncomplete = baseUpdate as InternalResponsesResponseStreamEventResponseIncomplete;
    }

    internal StreamingResponseStatusUpdate()
    { }
}