namespace OpenAI.SpecProcessor.Spec;

/// <summary> Represents a structural change to a property within a schema or operation. </summary>
public record PropertyChange
{
    /// <summary> The kind of change. </summary>
    public required ChangeType Type { get; init; }

    /// <summary> The property name. </summary>
    public required string Name { get; init; }

    /// <summary> The old type or value summary, for removals and changes. </summary>
    public string? OldType { get; init; }

    /// <summary> The new type or value summary, for additions and changes. </summary>
    public string? NewType { get; init; }

    /// <summary> Additional detail about what changed (e.g., "required → optional"). </summary>
    public string? Detail { get; init; }
}
