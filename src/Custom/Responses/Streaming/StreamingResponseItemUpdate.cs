using System;
using System.Collections.Generic;

namespace OpenAI.Responses;

public partial class StreamingResponseItemUpdate : StreamingResponseUpdate
{
    public ResponseItem Item
        => _outputItemAdded?.Item
        ?? _outputItemDone?.Item;

    public int ItemIndex
        => _outputItemAdded?.OutputIndex
        ?? _outputItemDone?.OutputIndex
        ?? 0;

    private readonly InternalResponsesResponseStreamEventResponseOutputItemAdded _outputItemAdded;
    private readonly InternalResponsesResponseStreamEventResponseOutputItemDone _outputItemDone;

    internal StreamingResponseItemUpdate(StreamingResponseUpdate baseUpdate)
        : base(baseUpdate.Kind)
    {
        _outputItemAdded = baseUpdate as InternalResponsesResponseStreamEventResponseOutputItemAdded;
        _outputItemDone = baseUpdate as InternalResponsesResponseStreamEventResponseOutputItemDone;
    }

    internal StreamingResponseItemUpdate()
    { }
}