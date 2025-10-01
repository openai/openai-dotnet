using System;
using System.Collections.Generic;

namespace OpenAI.Realtime;

public partial class InternalRealtimeAudioDisabledNoiseReduction: InputNoiseReductionOptions
{
    public InternalRealtimeAudioDisabledNoiseReduction()
        : base(InputNoiseReductionKind.Disabled)
    { }
}
