namespace OpenAI.Responses;

internal static partial class InternalToolChoiceObjectTypeExtensions
{
    internal static ResponseToolChoiceKind ToResponseToolChoiceKind(this InternalToolChoiceObjectType objectType)
    {
        return new ResponseToolChoiceKind(objectType.ToString());
    }
}