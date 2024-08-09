using System;
using System.ComponentModel;

namespace OpenAI.Assistants;

/// <summary>
/// Specifies the format that the model must output as a JSON object conforming to the provided schema.
/// </summary>
/// <remarks>
/// <para>
/// Setting to { "type": "json_schema", "json_schema": { ... } } enables JSON Schema mode, which ensures the message the model generates conforms to the provided schema.
/// </para>
/// <para>
/// Important: When using JSON Schema mode, you must also instruct the model to produce JSON in the format of the schema via a system or user message. Without this, the model may generate invalid or incomplete JSON content.
/// </para>
/// </remarks>
[CodeGenModel("AssistantJsonSchemaResponseFormat")]
internal sealed class AssistantJsonSchemaResponseFormat : AssistantResponseFormat, IEquatable<AssistantJsonSchemaResponseFormat>
{
    private const string JsonSchemaValue = "json_schema";
    private readonly string schema;

    /// <summary>
    /// Initializes a new instance of the <see cref="AssistantJsonSchemaResponseFormat"/> class with the specified JSON schema.
    /// </summary>
    /// <param name="schema">The JSON schema that the model's output must conform to.</param>
    internal AssistantJsonSchemaResponseFormat(string schema)
        : base(plainTextValue: null, objectType: JsonSchemaValue, serializedAdditionalRawData: null)
    {
        this.schema = schema ?? throw new ArgumentNullException(nameof(schema));
    }

    /// <summary>
    /// Gets the JSON schema that the model's output must conform to.
    /// </summary>
    public string Schema => this.schema;

    /// <inheritdoc />
    public static bool operator ==(AssistantJsonSchemaResponseFormat left, AssistantJsonSchemaResponseFormat right) => left.Equals(right);

    /// <inheritdoc />
    public static bool operator !=(AssistantJsonSchemaResponseFormat left, AssistantJsonSchemaResponseFormat right) => !left.Equals(right);

    /// <inheritdoc />
    [EditorBrowsable(EditorBrowsableState.Never)]
    public override bool Equals(object obj) => obj is AssistantJsonSchemaResponseFormat other && Equals(other);

    /// <inheritdoc />
    public bool Equals(AssistantJsonSchemaResponseFormat other)
        => base.Equals(other) && string.Equals(this.schema, other?.schema, StringComparison.InvariantCultureIgnoreCase);

    /// <inheritdoc />
    [EditorBrowsable(EditorBrowsableState.Never)]
    public override int GetHashCode() => $"{base.GetHashCode()}/{this.schema}".GetHashCode();

    /// <inheritdoc />
    public override string ToString() => base.ToString();
}