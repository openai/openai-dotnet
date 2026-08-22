namespace OpenAI.SpecProcessor.Spec;

/// <summary> Represents the aggregated diff results for a single feature area. </summary>
/// <param name="FeatureName"> The name of the feature area. </param>
/// <param name="Changes"> All individual changes detected. </param>
/// <param name="PathsAdded"> Number of new paths. </param>
/// <param name="PathsRemoved"> Number of removed paths. </param>
/// <param name="OperationsAdded"> Number of new operations. </param>
/// <param name="OperationsRemoved"> Number of removed operations. </param>
/// <param name="OperationsChanged"> Number of modified operations. </param>
/// <param name="SchemasAdded"> Number of new schemas. </param>
/// <param name="SchemasRemoved"> Number of removed schemas. </param>
/// <param name="SchemasChanged"> Number of modified schemas. </param>
/// <param name="SchemasRenamed"> Number of schemas detected as renames. </param>
/// <param name="Duplicates"> Pairs of schemas that appear to be duplicates. </param>
/// <param name="EquivalentSchemas"> Groups of structurally equivalent schemas within the spec. </param>
/// <param name="DuplicateOperations"> Pairs of operations with identical request/response schemas. </param>
/// <param name="Anomalies"> Spec anomalies detected (type mismatches, inconsistencies). </param>
public record FeatureDiff(
    string FeatureName,
    List<SpecChange> Changes,
    int PathsAdded,
    int PathsRemoved,
    int OperationsAdded,
    int OperationsRemoved,
    int OperationsChanged,
    int SchemasAdded,
    int SchemasRemoved,
    int SchemasChanged,
    int SchemasRenamed = 0,
    IReadOnlyList<SchemaDuplicate>? Duplicates = null,
    IReadOnlyList<SchemaEquivalenceGroup>? EquivalentSchemas = null,
    IReadOnlyList<OperationDuplicate>? DuplicateOperations = null,
    IReadOnlyList<SpecAnomaly>? Anomalies = null);
