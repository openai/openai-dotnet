using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace OpenAI.Audio;

[CodeGenModel("TranscriptionWord")]
[StructLayout(LayoutKind.Auto)]
public readonly partial struct TranscribedWord
{
    // CUSTOM: Remove setter.
    internal IDictionary<string, BinaryData> SerializedAdditionalRawData { get; }
}