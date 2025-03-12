using System.ClientModel.Primitives;
using System.Text.Json;

namespace OpenAI.Responses;

public partial class StreamingResponseUpdate
{
    internal static StreamingResponseUpdate DeserializeUpdateWithWrappers(JsonElement element, ModelReaderWriterOptions options)
    {
        StreamingResponseUpdate directlyDeserializedUpdate
            = DeserializeStreamingResponseUpdate(element, options);
        return directlyDeserializedUpdate switch
        {
            InternalResponsesResponseStreamEventResponseContentPartAdded
                or InternalResponsesResponseStreamEventResponseOutputTextDelta
                or InternalResponsesResponseStreamEventResponseFunctionCallArgumentsDelta
                or InternalResponsesResponseStreamEventResponseRefusalDelta
                => new StreamingResponseContentPartDeltaUpdate(directlyDeserializedUpdate),
            InternalResponsesResponseStreamEventResponseOutputItemAdded
                or InternalResponsesResponseStreamEventResponseOutputItemDone
                => new StreamingResponseItemUpdate(directlyDeserializedUpdate),
            InternalResponsesResponseStreamEventResponseCreated
                or InternalResponsesResponseStreamEventResponseInProgress
                or InternalResponsesResponseStreamEventResponseCompleted
                or InternalResponsesResponseStreamEventResponseFailed
                or InternalResponsesResponseStreamEventResponseIncomplete
                => new StreamingResponseStatusUpdate(directlyDeserializedUpdate),
            InternalResponsesResponseStreamEventResponseFileSearchCallCompleted
                or InternalResponsesResponseStreamEventResponseFileSearchCallInProgress
                or InternalResponsesResponseStreamEventResponseFileSearchCallSearching
                => new StreamingResponseFileSearchCallUpdate(directlyDeserializedUpdate),
            InternalResponsesResponseStreamEventResponseWebSearchCallCompleted
                or InternalResponsesResponseStreamEventResponseWebSearchCallInProgress
                or InternalResponsesResponseStreamEventResponseWebSearchCallSearching
                => new StreamingResponseWebSearchCallUpdate(directlyDeserializedUpdate),
            _ => directlyDeserializedUpdate,
        };
    }
}