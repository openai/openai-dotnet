namespace OpenAI.Responses;

[CodeGenType("ResponsesResponseStatus")]
public enum ResponseStatus
{
    InProgress,
    Completed,
    Incomplete,
    Failed
}
