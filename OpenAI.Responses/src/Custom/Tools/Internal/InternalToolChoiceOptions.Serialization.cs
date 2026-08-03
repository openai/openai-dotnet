namespace OpenAI.Responses;

internal static partial class InternalToolChoiceOptionsExtensions
{
    internal static ResponseToolChoiceKind ToResponseToolChoiceKind(this InternalToolChoiceOptions options)
    {
        return new ResponseToolChoiceKind(options.ToString());
    }
}