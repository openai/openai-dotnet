namespace OpenAI;

[CodeGenModel("ListOrder")]
public readonly partial struct ListOrder
{
    // CUSTOM: Rename members.

    [CodeGenMember("Asc")]
    public static ListOrder OldestFirst { get; } = new ListOrder(OldestFirstValue);
    [CodeGenMember("Desc")]
    public static ListOrder NewestFirst { get; } = new ListOrder(NewestFirstValue);
}