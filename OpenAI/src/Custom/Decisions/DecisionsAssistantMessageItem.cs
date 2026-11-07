namespace OpenAI.Decisions;

public partial class DecisionsAssistantMessageItem
{
    // CUSTOM: Use the content constructor to preserve both the fixed message kind and role.
    public DecisionsAssistantMessageItem() : this([])
    {
    }
}
