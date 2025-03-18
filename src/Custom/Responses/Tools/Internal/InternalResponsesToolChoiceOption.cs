namespace OpenAI.Responses;

[CodeGenType("ResponsesToolChoiceOption")]
internal readonly partial struct InternalResponsesToolChoiceOption
{
    public ResponseToolChoiceKind Kind => _value switch
    {
        AutoValue => ResponseToolChoiceKind.Auto,
        NoneValue => ResponseToolChoiceKind.None,
        RequiredValue => ResponseToolChoiceKind.Required,
        _ => ResponseToolChoiceKind.Unknown,
    };
}
