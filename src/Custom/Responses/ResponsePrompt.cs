using System.Collections.Generic;

namespace OpenAI.Responses;

[CodeGenType("ResponsesPrompt")]
public partial class ResponsePrompt
{
    [CodeGenMember("Id")]
    public string Id { get; set; }

    [CodeGenMember("Version")]
    public string Version { get; set; }

    [CodeGenMember("Variables")]
    public IDictionary<string, object> Variables { get; }

    public ResponsePrompt()
    {
        Variables = new Dictionary<string, object>();
    }
}
