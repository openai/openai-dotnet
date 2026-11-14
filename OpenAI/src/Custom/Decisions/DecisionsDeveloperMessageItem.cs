namespace OpenAI.Decisions;

public partial class DecisionsDeveloperMessageItem
{
    // CUSTOM: Use the content constructor to preserve both the fixed message kind and role.
    public DecisionsDeveloperMessageItem() : this([])
    {
    }
}
