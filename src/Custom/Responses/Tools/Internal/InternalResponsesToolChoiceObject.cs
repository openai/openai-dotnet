namespace OpenAI.Responses;

[CodeGenType("ResponsesToolChoiceObject")]
internal partial class InternalResponsesToolChoiceObject
{
    public ResponseToolChoiceKind Kind => this switch
    {
        InternalResponsesToolChoiceObjectComputer => ResponseToolChoiceKind.Computer,
        InternalResponsesToolChoiceObjectFileSearch => ResponseToolChoiceKind.FileSearch,
        InternalResponsesToolChoiceObjectFunction => ResponseToolChoiceKind.Function,
        InternalResponsesToolChoiceObjectWebSearch => ResponseToolChoiceKind.WebSearch,
        _ => ResponseToolChoiceKind.Unknown,
    };
}

