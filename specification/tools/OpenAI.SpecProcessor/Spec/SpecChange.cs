namespace OpenAI.SpecProcessor.Spec;

/// <summary> Represents a single change detected between spec versions. </summary>
public record SpecChange
{
    /// <summary> The kind of change. </summary>
    public required ChangeType Type { get; init; }

    /// <summary> Categorizes the change as "path", "operation", "schema", or "property". </summary>
    public required string Category { get; init; }

    /// <summary> Dotted path to the changed element. </summary>
    public required string Path { get; init; }

    /// <summary> The previous value, or null for additions. </summary>
    public string? OldValue { get; init; }

    /// <summary> The new value, or null for removals. </summary>
    public string? NewValue { get; init; }

    /// <summary> Additional detail about the change, such as the schema type. </summary>
    public string? Detail { get; init; }

    /// <summary> Schema skeleton for newly added schemas, showing properties and their types. </summary>
    public SchemaInfo? Schema { get; init; }

    /// <summary> Structured property-level changes within a schema or operation. </summary>
    public IReadOnlyList<PropertyChange>? PropertyChanges { get; init; }

    /// <summary> The resolved request body schema for operation changes. </summary>
    public SchemaInfo? RequestSchema { get; init; }

    /// <summary> The resolved primary response schema for operation changes. </summary>
    public SchemaInfo? ResponseSchema { get; init; }

    /// <summary> The request body schema $ref name for operation changes. </summary>
    public string? RequestSchemaRef { get; init; }

    /// <summary> The response schema $ref name for operation changes. </summary>
    public string? ResponseSchemaRef { get; init; }
}
