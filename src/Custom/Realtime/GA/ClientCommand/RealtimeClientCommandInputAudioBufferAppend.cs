using Microsoft.TypeSpec.Generator.Customizations;
using System;

namespace OpenAI.Realtime;

// CUSTOM: Renamed.
[CodeGenType("RealtimeClientEventInputAudioBufferAppendGA")]
public partial class GARealtimeClientCommandInputAudioBufferAppend
{
    // CUSTOM: Renamed.
    [CodeGenMember("Audio")]
    public BinaryData AudioBytes { get; }
}