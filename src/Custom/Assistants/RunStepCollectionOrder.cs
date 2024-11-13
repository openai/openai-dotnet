using System.Diagnostics.CodeAnalysis;

namespace OpenAI.Assistants;

// CUSTOM: Renamed.
[Experimental("OPENAI001")]
[CodeGenModel("ListRunStepsRequestOrder")]
public readonly partial struct RunStepCollectionOrder
{
    // CUSTOM: Renamed.
    [CodeGenMember("Asc")]
    public static RunStepCollectionOrder Ascending { get; } = new RunStepCollectionOrder(AscendingValue);

    // CUSTOM: Renamed.
    [CodeGenMember("Desc")]
    public static RunStepCollectionOrder Descending { get; } = new RunStepCollectionOrder(DescendingValue);
}
