namespace OpenAI.Responses;

internal static partial class InternalToolChoiceObjectTypeExtensions
{
    internal static ResponseToolChoiceKind ToResponseToolChoiceKind(this InternalToolChoiceObjectType objectType)
    {
        return objectType.ToString() switch
        {
            "file_search" => ResponseToolChoiceKind.FileSearch,
            "function" => ResponseToolChoiceKind.Function,
            "computer" => ResponseToolChoiceKind.Computer,
            "web_search" => ResponseToolChoiceKind.WebSearch,
            _ => ResponseToolChoiceKind.Unknown,
        };
    }
}