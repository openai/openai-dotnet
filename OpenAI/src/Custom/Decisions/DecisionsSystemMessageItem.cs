namespace OpenAI.Decisions;

public partial class DecisionsSystemMessageItem
{
    // CUSTOM: Use the content constructor to preserve both the fixed message kind and role.
    public DecisionsSystemMessageItem() : this([])
    {
    }
}
