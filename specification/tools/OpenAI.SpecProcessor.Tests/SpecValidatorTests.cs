using NUnit.Framework;
using OpenAI.SpecProcessor.Spec;

namespace OpenAI.SpecProcessor.Tests;

/// <summary>
/// Covers the guarantee that a feature document stands on its own, and the failure that guarantee
/// is most likely to be lost to.
/// </summary>
[TestFixture]
public class SpecValidatorTests
{
    [Test]
    public void ADanglingSchemaReferenceFailsValidation()
    {
        var spec = SpecFixture.Load("""
            openapi: "3.1.0"
            info:
              title: Test
              version: "1.0.0"
            paths:
              /widgets:
                get:
                  operationId: listWidgets
                  responses:
                    "200":
                      description: Success
                      content:
                        application/json:
                          schema:
                            $ref: "#/components/schemas/Missing"
            components:
              schemas:
                Present:
                  type: object
            """);

        var result = SpecValidator.Validate("Widgets", spec);

        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Errors, Has.Some.Contains("Dangling $ref"));
    }

    /// <remarks>
    /// The splitter builds closure over component schemas only. Upstream publishes nothing else
    /// today, so this cannot happen yet. If it ever does, the feature document would be missing the
    /// referenced component and validation would have said nothing, which is the quiet failure this
    /// covers.
    /// </remarks>
    [TestCase("parameters")]
    [TestCase("responses")]
    [TestCase("requestBodies")]
    [TestCase("headers")]
    public void AReferenceKindTheSplitterCannotCarryFailsValidation(string componentKind)
    {
        var spec = SpecFixture.Load($"""
            openapi: "3.1.0"
            info:
              title: Test
              version: "1.0.0"
            paths:
              /widgets:
                get:
                  operationId: listWidgets
                  parameters:
                    - $ref: "#/components/{componentKind}/Shared"
                  responses:
                    "200":
                      description: Success
            components:
              schemas:
                Present:
                  type: object
            """);

        var result = SpecValidator.Validate("Widgets", spec);

        Assert.That(result.IsValid, Is.False, $"A '{componentKind}' reference passed validation.");
        Assert.That(result.Errors, Has.Some.Contains("Unsupported $ref"));
        Assert.That(result.Errors, Has.Some.Contains(componentKind),
            "The error should name the component section, since supporting one is not the same work as supporting another.");
    }

    /// <remarks>
    /// A feature document has to stand alone, so a reference into another document defeats the whole
    /// point of the split. It fails for a different reason than an unsupported local component does,
    /// and the fix is different, so the two are reported separately.
    /// </remarks>
    [TestCase("common.yaml#/components/schemas/Shared")]
    [TestCase("./shared/types.yaml#/Widget")]
    [TestCase("https://example.com/spec.yaml#/components/schemas/Shared")]
    public void AnExternalReferenceFailsValidation(string reference)
    {
        var spec = SpecFixture.Load($"""
            openapi: "3.1.0"
            info:
              title: Test
              version: "1.0.0"
            paths:
              /widgets:
                get:
                  operationId: listWidgets
                  responses:
                    "200":
                      description: Success
                      content:
                        application/json:
                          schema:
                            $ref: "{reference}"
            components:
              schemas:
                Present:
                  type: object
            """);

        var result = SpecValidator.Validate("Widgets", spec);

        Assert.That(result.IsValid, Is.False, $"The external reference '{reference}' passed validation.");
        Assert.That(result.Errors, Has.Some.Contains("outside this document"));
    }

    [TestCase("#/components/schemas/Widget", ReferenceKind.LocalSchema)]
    [TestCase("#/components/parameters/PageSize", ReferenceKind.LocalComponent)]
    [TestCase("#/components/responses/NotFound", ReferenceKind.LocalComponent)]
    [TestCase("#/paths/~1widgets", ReferenceKind.LocalOther)]
    [TestCase("common.yaml#/components/schemas/Widget", ReferenceKind.External)]
    [TestCase("https://example.com/spec.yaml", ReferenceKind.External)]
    public void ReferencesAreClassifiedByKind(string reference, ReferenceKind expected)
    {
        Assert.That(SpecReference.Classify(reference), Is.EqualTo(expected));
    }

    [TestCase("#/components/parameters/PageSize", "parameters")]
    [TestCase("#/components/requestBodies/Create", "requestBodies")]
    [TestCase("#/components/schemas/Widget", "schemas")]
    public void TheComponentSectionIsReadFromALocalReference(string reference, string expected)
    {
        Assert.That(SpecReference.GetComponentSection(reference), Is.EqualTo(expected));
    }

    [Test]
    public void AFullyResolvedDocumentPassesValidation()
    {
        var (features, _) = SpecSplitter.Split(SpecCleaner.Clean(SpecFixture.Load(SpecFixture.RichSpec)));

        foreach (var result in SpecValidator.ValidateAll(features))
        {
            Assert.That(result.IsValid, Is.True, $"'{result.FeatureName}' failed: {string.Join("; ", result.Errors)}");
        }
    }
}
