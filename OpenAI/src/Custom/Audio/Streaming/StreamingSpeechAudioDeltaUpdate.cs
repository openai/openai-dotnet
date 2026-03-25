using Microsoft.TypeSpec.Generator.Customizations;
using System;

namespace OpenAI.Audio;

// CUSTOM: Renamed.
[CodeGenType("DotNetSpeechAudioDeltaEvent")]
public partial class StreamingSpeechAudioDeltaUpdate
{
    // CUSTOM: Rename.
    [CodeGenMember("Audio")]
    public BinaryData AudioBytes { get; }
}
