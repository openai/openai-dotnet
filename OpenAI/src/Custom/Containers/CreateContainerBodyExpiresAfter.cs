namespace OpenAI.Containers;

public partial class CreateContainerBodyExpiresAfter
{
    // CUSTOM: Make public property for back compatibility.
    public string Anchor { get; } = "last_active_at";
}
