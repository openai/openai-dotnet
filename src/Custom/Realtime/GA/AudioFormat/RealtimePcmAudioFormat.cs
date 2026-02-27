using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Realtime;

// CUSTOM: Renamed.
[CodeGenType("RealtimeAudioFormatsAudioPcmGA")]
public partial class RealtimePcmAudioFormat
{
    // CUSTOM: Override default value of rate parameter.
    public RealtimePcmAudioFormat() : this(InternalRealtimeAudioFormatType.AudioPcm, default, 24000)
    {
    }
}