namespace OpenAI.Realtime;

internal partial class InternalRealtimeAudioDisabledNoiseReduction: InputNoiseReductionOptions
{
    public InternalRealtimeAudioDisabledNoiseReduction()
        : base(InputNoiseReductionKind.Disabled)
    { }
}
