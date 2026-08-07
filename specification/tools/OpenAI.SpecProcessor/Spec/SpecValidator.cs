using YamlDotNet.RepresentationModel;

namespace OpenAI.SpecProcessor.Spec;

/// <summary> Validates split feature specs for completeness and correctness. </summary>
public static class SpecValidator
{
    private static void CheckForExcludedContent(SpecDocument spec, List<string> errors)
    {
        var paths = spec.Paths;
        if (paths == null)
        {
            return;
        }

        foreach (var (key, _) in paths.Children)
        {
            if (key is not YamlScalarNode scalar || scalar.Value == null)
            {
                continue;
            }

            if (FeatureAreaConfig.IsExcludedPath(scalar.Value))
            {
                errors.Add($"Excluded path still present: {scalar.Value}");
            }
        }
    }

    private static int CountMetadataFields(YamlNode node)
    {
        int count = 0;

        switch (node)
        {
            case YamlMappingNode mapping:
                foreach (var key in FeatureAreaConfig.MetadataKeysToStrip)
                {
                    if (mapping.Children.ContainsKey(new YamlScalarNode(key)))
                    {
                        count++;
                    }
                }

                foreach (var (_, value) in mapping.Children)
                {
                    count += CountMetadataFields(value);
                }
                break;

            case YamlSequenceNode sequence:
                foreach (var child in sequence.Children)
                {
                    count += CountMetadataFields(child);
                }
                break;
        }

        return count;
    }

    /// <summary> Validates a single feature spec. </summary>
    /// <param name="featureName"> The name of the feature being validated. </param>
    /// <param name="spec"> The spec document to validate. </param>
    /// <returns> The validation result containing errors, warnings, and counts. </returns>
    public static ValidationResult Validate(string featureName, SpecDocument spec)
    {
        var errors = new List<string>();
        var warnings = new List<string>();

        // Check the OpenAPI version.

        if (spec.OpenApiVersion is not "3.1.0" and not "3.2.0")
        {
            errors.Add($"Unexpected OpenAPI version: {spec.OpenApiVersion ?? "(missing)"}");
        }

        // Count paths and operations.

        int pathCount = 0;
        int opCount = 0;
        var paths = spec.Paths;
        if (paths != null)
        {
            foreach (var (_, pathValue) in paths.Children)
            {
                pathCount++;
                if (pathValue is YamlMappingNode pathItem)
                {
                    opCount += SpecDocument.GetOperations(pathItem).Count();
                }
            }
        }

        if (pathCount == 0)
        {
            errors.Add("No paths defined");
        }

        if (opCount == 0)
        {
            errors.Add("No operations found");
        }

        // Check for dangling $ref references.

        var allRefs = RefResolver.CollectAllRefs(spec.Root);
        var schemas = spec.Schemas;
        var schemaCount = 0;

        foreach (var refStr in allRefs)
        {
            // Everything upstream publishes today is a local schema reference, and the splitter only
            // builds closure over those. If that ever stops being true, the feature file is silently
            // incomplete, so an unsupported reference kind is an error rather than something to skip
            // past. The message names the kind, since what it takes to support a shared parameter
            // component is not what it takes to support an external document.

            if (SpecReference.Classify(refStr) != ReferenceKind.LocalSchema)
            {
                errors.Add(SpecReference.DescribeUnsupported(refStr));
                continue;
            }

            var schemaName = refStr[SpecReference.SchemaPrefix.Length..];
            var key = new YamlScalarNode(schemaName);

            if (schemas == null || !schemas.Children.ContainsKey(key))
            {
                errors.Add($"Dangling $ref: {refStr}");
            }
        }

        if (schemas != null)
        {
            schemaCount = schemas.Children.Count;
        }

        // Check for excluded content that shouldn't be present.

        CheckForExcludedContent(spec, errors);

        // Check for leftover metadata.

        int metadataFound = CountMetadataFields(spec.Root);
        if (metadataFound > 0)
        {
            errors.Add($"Found {metadataFound} leftover x-oaiMeta/x-oaiTypeLabel fields");
        }

        return new ValidationResult(
            featureName,
            errors.Count == 0,
            errors,
            warnings,
            pathCount,
            opCount,
            schemaCount);
    }

    /// <summary> Validates all features and reports a summary. </summary>
    /// <param name="features"> The dictionary of feature names to spec documents. </param>
    /// <returns> A list of validation results for each feature. </returns>
    public static List<ValidationResult> ValidateAll(Dictionary<string, SpecDocument> features)
    {
        var results = new List<ValidationResult>();

        foreach (var (name, spec) in features)
        {
            var result = Validate(name, spec);
            results.Add(result);
        }

        return results;
    }
}
