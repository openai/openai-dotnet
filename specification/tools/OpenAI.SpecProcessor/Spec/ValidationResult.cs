namespace OpenAI.SpecProcessor.Spec;

/// <summary> Represents the result of validating a single feature spec. </summary>
/// <param name="FeatureName"> The name of the validated feature. </param>
/// <param name="IsValid"> Whether the feature spec passed validation. </param>
/// <param name="Errors"> The list of validation errors found. </param>
/// <param name="Warnings"> The list of validation warnings found. </param>
/// <param name="PathCount"> The number of paths in the feature spec. </param>
/// <param name="OperationCount"> The number of operations in the feature spec. </param>
/// <param name="SchemaCount"> The number of schemas in the feature spec. </param>
public record ValidationResult(
    string FeatureName,
    bool IsValid,
    List<string> Errors,
    List<string> Warnings,
    int PathCount,
    int OperationCount,
    int SchemaCount);
