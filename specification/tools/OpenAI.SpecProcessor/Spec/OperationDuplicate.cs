namespace OpenAI.SpecProcessor.Spec;

/// <summary> Represents a pair of operations that have structurally identical request and response schemas. </summary>
/// <param name="OperationA"> The first operation path.method (e.g., "/responses.post"). </param>
/// <param name="OperationIdA"> The operationId of the first operation. </param>
/// <param name="OperationB"> The second operation path.method. </param>
/// <param name="OperationIdB"> The operationId of the second operation. </param>
/// <param name="SharedRequestSchema"> The shared request body schema ref, if both have the same one. </param>
/// <param name="SharedResponseSchema"> The shared response schema ref, if both have the same one. </param>
public record OperationDuplicate(
    string OperationA,
    string? OperationIdA,
    string OperationB,
    string? OperationIdB,
    string? SharedRequestSchema,
    string? SharedResponseSchema);
