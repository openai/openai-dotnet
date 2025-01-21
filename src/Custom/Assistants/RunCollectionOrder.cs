using System.Diagnostics.CodeAnalysis;

namespace OpenAI.Assistants;

// CUSTOM: Renamed.
[Experimental("OPENAI001")]
[CodeGenModel("ListRunsRequestOrder")]
public readonly partial struct RunCollectionOrder
{
    // CUSTOM: Renamed.
    [CodeGenMember("Asc")]
    public static RunCollectionOrder Ascending { get; } = new RunCollectionOrder(AscValue);

    // CUSTOM: Renamed.
    [CodeGenMember("Desc")]
    public static RunCollectionOrder Descending { get; } = new RunCollectionOrder(DescValue);
}
