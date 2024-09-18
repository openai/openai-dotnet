using System.Diagnostics.CodeAnalysis;

namespace OpenAI.Assistants;

// CUSTOM: Renamed.
[Experimental("OPENAI001")]
[CodeGenModel("ListMessagesRequestOrder")]
public readonly partial struct MessageCollectionOrder
{
    // CUSTOM: Renamed.
    [CodeGenMember("Asc")]
    public static MessageCollectionOrder Ascending { get; } = new MessageCollectionOrder(AscendingValue);

    // CUSTOM: Renamed.
    [CodeGenMember("Desc")]
    public static MessageCollectionOrder Descending { get; } = new MessageCollectionOrder(DescendingValue);
}
