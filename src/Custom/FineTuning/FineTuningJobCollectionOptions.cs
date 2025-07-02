using System.Diagnostics.CodeAnalysis;

namespace OpenAI.FineTuning;

[Experimental("OPENAI001")]
public class FineTuningJobCollectionOptions
{
    public string AfterJobId { get; set; }

    public int? PageSize { get; set; }

    // TODO: Implement status as a filter for the list of jobs
    //public FineTuningStatus Status { get; set; }
}
