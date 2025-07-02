using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace OpenAI.Realtime;

[Experimental("OPENAI002")]
[Flags]
public enum RealtimeContentModalities : int
{
    Default = 0,
    Text = 1 << 0,
    Audio = 1 << 1,
}