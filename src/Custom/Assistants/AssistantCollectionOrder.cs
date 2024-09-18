using System.Diagnostics.CodeAnalysis;

namespace OpenAI.Assistants;

// CUSTOM: Renamed.
[Experimental("OPENAI001")]
[CodeGenModel("ListAssistantsRequestOrder")]
public readonly partial struct AssistantCollectionOrder
{
    // CUSTOM: Renamed.
    [CodeGenMember("Asc")]
    public static AssistantCollectionOrder Ascending { get; } = new AssistantCollectionOrder(AscendingValue);

    // CUSTOM: Renamed.
    [CodeGenMember("Desc")]
    public static AssistantCollectionOrder Descending { get; } = new AssistantCollectionOrder(DescendingValue);
}
