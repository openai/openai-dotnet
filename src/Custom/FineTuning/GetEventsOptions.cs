using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace OpenAI.FineTuning;

[Experimental("OPENAI001")]
[CodeGenType("ListFineTuningJobEventsRequest")]
public class GetEventsOptions
{
    [CodeGenMember("JobId")]
    internal string JobId { get; set; }

    [CodeGenMember("After")]
    public string AfterEventId { get; set; }

    [CodeGenMember("Limit")]
    public int? PageSize { get; set; }
}
