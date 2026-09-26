namespace OpenAI.SpecProcessor.Spec;

/// <summary> The kinds of `$ref` a specification can carry, as far as the split is concerned. </summary>
public enum ReferenceKind
{
    /// <summary> A local schema reference, which the splitter resolves and carries into feature files. </summary>
    LocalSchema,

    /// <summary> A local reference to a component section other than `schemas`, which the splitter does not carry. </summary>
    LocalComponent,

    /// <summary> A local reference that points outside `#/components` entirely. </summary>
    LocalOther,

    /// <summary> A reference into another document, by relative path or absolute URL. </summary>
    External
}

/// <summary>
/// Classifies `$ref` values so that supported and unsupported references are distinguished by kind
/// rather than by a single prefix test.
/// </summary>
/// <remarks>
/// The splitter builds closure over local schema references only. Everything upstream publishes
/// today is that kind, so the distinction has no effect on current output. It matters for what
/// happens when that changes: an unsupported reference must be reported as the specific thing it is,
/// since the fix for a shared parameter component is not the fix for an external document.
/// </remarks>
public static class SpecReference
{
    /// <summary> The prefix of the only reference kind the splitter resolves. </summary>
    public const string SchemaPrefix = "#/components/schemas/";

    private const string ComponentsPrefix = "#/components/";

    /// <summary> Determines what kind of reference a `$ref` value is. </summary>
    /// <param name="reference"> The raw `$ref` value. </param>
    /// <returns> The classified kind. </returns>
    public static ReferenceKind Classify(string reference)
    {
        if (string.IsNullOrWhiteSpace(reference))
        {
            return ReferenceKind.LocalOther;
        }

        if (!reference.StartsWith('#'))
        {
            return ReferenceKind.External;
        }

        if (reference.StartsWith(SchemaPrefix, StringComparison.Ordinal))
        {
            return ReferenceKind.LocalSchema;
        }

        if (reference.StartsWith(ComponentsPrefix, StringComparison.Ordinal))
        {
            return ReferenceKind.LocalComponent;
        }

        return ReferenceKind.LocalOther;
    }

    /// <summary> Gets the component section a local component reference points at, if there is one. </summary>
    /// <param name="reference"> The raw `$ref` value. </param>
    /// <returns> The section name, such as `parameters`, or null when the reference is not a local component reference. </returns>
    public static string? GetComponentSection(string reference)
    {
        if (string.IsNullOrEmpty(reference) || !reference.StartsWith(ComponentsPrefix, StringComparison.Ordinal))
        {
            return null;
        }

        var remainder = reference[ComponentsPrefix.Length..];
        var separator = remainder.IndexOf('/');

        return (separator < 0) ? remainder : remainder[..separator];
    }

    /// <summary> Builds the explanation recorded when a reference cannot be carried into a feature specification. </summary>
    /// <param name="reference"> The raw `$ref` value. </param>
    /// <returns> A message naming the kind and what it would take to support it. </returns>
    public static string DescribeUnsupported(string reference) =>
        Classify(reference) switch
        {
            ReferenceKind.LocalComponent =>
                $"Unsupported $ref: {reference} points at the '{GetComponentSection(reference)}' component section, and the split resolves only '{SchemaPrefix}'. " +
                 "The feature specification would be missing it. Extend the reference closure to cover that section before this can be processed.",

            ReferenceKind.External =>
                $"Unsupported $ref: {reference} points outside this document. A feature specification has to stand alone, so an external reference has to be inlined or vendored before it can be processed.",

            ReferenceKind.LocalOther =>
                $"Unsupported $ref: {reference} points inside the document but outside '#/components', which the split does not resolve.",

            _ => $"Unsupported $ref: {reference}"
        };
}
