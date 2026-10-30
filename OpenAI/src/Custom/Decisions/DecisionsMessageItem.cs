namespace OpenAI.Decisions;

public partial class DecisionsMessageItem
{
    // CUSTOM: Preserve the required user role; ConstructorFixupVisitor otherwise passes null to the
    // serialization constructor, overwriting the generated Role property's "user" initializer.
    public DecisionsMessageItem() : this(DecisionsItemKind.Message, default, null, "user")
    {
    }
}
