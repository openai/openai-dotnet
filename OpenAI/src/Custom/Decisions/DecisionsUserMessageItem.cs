namespace OpenAI.Decisions;

public partial class DecisionsUserMessageItem
{
    // CUSTOM: Use the content constructor to preserve both the fixed message kind and role.
    public DecisionsUserMessageItem() : this([])
    {
    }
}
