using System;
using System.Collections.Generic;

namespace OpenAI.Realtime;

internal partial class InternalRealtimeAudioDisabledNoiseReduction: InputNoiseReductionOptions
{
    public InternalRealtimeAudioDisabledNoiseReduction()
        : base(InputNoiseReductionKind.Disabled)
    { }
}
