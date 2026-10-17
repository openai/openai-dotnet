namespace OpenAI.Decisions;

public partial class DecisionsMessageItem
{
    public DecisionsMessageItem() : this(DecisionsItemKind.Message, default, null, "user")
    {
    }
}
