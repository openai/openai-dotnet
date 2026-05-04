using System.Diagnostics.CodeAnalysis;

namespace OpenAI.FineTuning;

[Experimental("OPENAI001")]
public class GetCheckpointsOptions
{
    public string AfterCheckpointId { get; set; }

    public int? PageSize { get; set; }
}
