namespace OpenAI.SpecProcessor.Spec;

/// <summary> Represents a group of schemas within the same spec file that are structurally equivalent. </summary>
/// <param name="SchemaNames"> The names of all schemas in this equivalence group. </param>
/// <param name="PropertySignature"> A human-readable summary of the shared structure. </param>
public record SchemaEquivalenceGroup(
    IReadOnlyList<string> SchemaNames,
    string PropertySignature);
