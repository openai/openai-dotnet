using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Realtime;

// CUSTOM: Renamed.
[CodeGenType("RealtimeAudioFormatsAudioPcmGA")]
public partial class GARealtimePcmAudioFormat
{
    // CUSTOM: Override default value of rate parameter.
    public GARealtimePcmAudioFormat() : this(InternalRealtimeAudioFormatType.AudioPcm, default, 24000)
    {
    }
}