using System;
using System.Collections.Generic;

namespace OpenAI.Realtime;

internal partial class InternalRealtimeNoTurnDetection : TurnDetectionOptions
{
    public InternalRealtimeNoTurnDetection()
        : this(TurnDetectionKind.Disabled, null)
    { }

    internal InternalRealtimeNoTurnDetection(TurnDetectionKind kind, IDictionary<string, BinaryData> serializedAdditionalRawData)
        : base(kind)
    { }
}
