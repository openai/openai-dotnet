namespace OpenAI.SpecProcessor.Spec;

/// <summary> Captures the skeleton of a schema for display in diff reports. </summary>
public record SchemaInfo
{
    /// <summary> The schema name. </summary>
    public required string Name { get; init; }

    /// <summary> The schema type (e.g., "object", "string", "integer"). </summary>
    public string? Type { get; init; }

    /// <summary> Enum values, if the schema is an enum type. </summary>
    public IReadOnlyList<string>? EnumValues { get; init; }

    /// <summary> Properties with their types, for object schemas. </summary>
    public IReadOnlyList<SchemaProperty>? Properties { get; init; }

    /// <summary> Required property names. </summary>
    public IReadOnlyList<string>? Required { get; init; }

    /// <summary> Type references for anyOf/oneOf/allOf compositions. </summary>
    public IReadOnlyList<string>? CompositionRefs { get; init; }

    /// <summary> The composition keyword used (anyOf, oneOf, allOf), if applicable. </summary>
    public string? CompositionType { get; init; }
}
