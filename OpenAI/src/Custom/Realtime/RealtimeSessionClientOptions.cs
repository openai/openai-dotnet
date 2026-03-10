using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace OpenAI.Realtime;

[Experimental("OPENAI002")]
public class RealtimeSessionClientOptions
{
    private string _clientSecret;
    private string _queryString;
    private IDictionary<string, string> _headers;

    public RealtimeSessionClientOptions()
    {
        _headers = new ChangeTrackingDictionary<string, string>();
    }

    public string ClientSecret
    {
        get => _clientSecret;
        set => _clientSecret = value;
    }

    public string QueryString
    {
        get => _queryString;
        set => _queryString = value;
    }

    public IDictionary<string, string> Headers
    {
        get => _headers;
    }
}
