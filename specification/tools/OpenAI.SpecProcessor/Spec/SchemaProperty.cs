namespace OpenAI.SpecProcessor.Spec;

/// <summary> Represents a single property within a schema skeleton. </summary>
public record SchemaProperty
{
    /// <summary> The property name. </summary>
    public required string Name { get; init; }

    /// <summary> The property type (e.g., "string", "integer", "array", "$ref"). </summary>
    public required string Type { get; init; }

    /// <summary> Whether the property is required. </summary>
    public bool IsRequired { get; init; }

    /// <summary> For arrays, the type of items in the array. </summary>
    public string? ItemType { get; init; }

    /// <summary> For $ref types, the referenced schema name. </summary>
    public string? RefTarget { get; init; }
}
