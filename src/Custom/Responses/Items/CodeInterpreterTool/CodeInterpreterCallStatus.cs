namespace OpenAI.Responses;

// CUSTOM: Renamed and made public. Recreated as CLR enum.
[CodeGenType("CodeInterpreterToolCallItemResourceStatus")]
public enum CodeInterpreterCallStatus
{
    InProgress,
    Interpreting,
    Completed,
    Incomplete,
    Failed
}