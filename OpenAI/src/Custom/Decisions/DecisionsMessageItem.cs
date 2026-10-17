namespace OpenAI.Decisions;

public partial class DecisionsMessageItem
{
    // CUSTOM: Preserve the required user role when initializing a message through its parameterless constructor.
    public DecisionsMessageItem() : this(DecisionsItemKind.Message, default, null, "user")
    {
    }
}
