using System;
using System.Diagnostics.CodeAnalysis;

namespace OpenAI.Realtime;

[CodeGenType("RealtimeAudioNoiseReduction")]
public partial class InputNoiseReductionOptions
{
    // CUSTOM: Rename to "Kind" and make public.
    [CodeGenMember("Type")]
    public InputNoiseReductionKind Kind { get; set; }

    public static InputNoiseReductionOptions CreateNearFieldOptions()
        => new InternalRealtimeAudioNearFieldNoiseReduction();

    public static InputNoiseReductionOptions CreateFarFieldOptions()
        => new InternalRealtimeAudioFarFieldNoiseReduction();

    public static InputNoiseReductionOptions CreateDisabledOptions()
        => new InternalRealtimeAudioDisabledNoiseReduction();
}