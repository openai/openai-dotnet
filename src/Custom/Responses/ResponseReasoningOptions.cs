namespace OpenAI.Responses;

[CodeGenType("ResponsesReasoningConfiguration")]
public partial class ResponseReasoningOptions
{
    // CUSTOM: Renamed.
    [CodeGenMember("Effort")]
    public ResponseReasoningEffortLevel? ReasoningEffortLevel { get; set; }

    // CUSTOM: Renamed.
    [CodeGenMember("Summary")]
    public ResponseReasoningSummaryVerbosity? ReasoningSummaryVerbosity { get; set; }

    // CUSTOM: Make default constructor public.
    public ResponseReasoningOptions()
    {
    }
}
