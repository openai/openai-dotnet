namespace OpenAI.SpecProcessor.Spec;

/// <summary> Represents an anomaly detected in the spec, such as type mismatches between related schemas. </summary>
/// <param name="Severity"> "warning" or "error" indicating the severity. </param>
/// <param name="Category"> The anomaly category (e.g., "type-mismatch", "input-output-mismatch"). </param>
/// <param name="Path"> The spec path or schema where the anomaly was found. </param>
/// <param name="Detail"> A human-readable description of the anomaly. </param>
public record SpecAnomaly(
    string Severity,
    string Category,
    string Path,
    string Detail);
