using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace OpenAI.Realtime;

[Experimental("OPENAI002")]
public class RealtimeSessionOptions
{
    public RealtimeSessionOptions()
    {
        Headers = new ChangeTrackingDictionary<string, string>();
    }

    public Uri Endpoint { get; set; }

    public IDictionary<string, string> Headers { get; }
}
