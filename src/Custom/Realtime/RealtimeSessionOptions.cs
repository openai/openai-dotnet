using System;
using System.Collections.Generic;

namespace OpenAI.Realtime;

public class RealtimeSessionOptions
{
    public RealtimeSessionOptions()
    {
        Headers = new ChangeTrackingDictionary<string, string>();
    }

    public Uri Endpoint { get; set; }

    public IDictionary<string, string> Headers { get; }
}
