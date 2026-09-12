using NUnit.Framework;
using OpenAI.SpecProcessor.Spec;

namespace OpenAI.SpecProcessor.Tests;

/// <summary>
/// Covers the inputs that must not be allowed to produce a published snapshot.
/// </summary>
/// <remarks>
/// The committed snapshot is the baseline every future comparison is measured against, so replacing
/// a good one with a degenerate one is worse than failing the run. These cases were found by running
/// the orchestrator against crafted sources: a document that parsed but carried no recognizable API
/// surface processed "successfully" and published an empty snapshot over the top of a real one.
/// </remarks>
[TestFixture]
public class DegenerateSourceTests
{
    [Test]
    public void ASpecificationWithNoPathsProducesNoFeatures()
    {
        var spec = SpecFixture.Load("""
            openapi: "3.1.0"
            info:
              title: Test
              version: "1.0.0"
            paths: {}
            components:
              schemas: {}
            """);

        var (features, unassigned) = SpecSplitter.Split(SpecCleaner.Clean(spec));

        Assert.Multiple(() =>
        {
            Assert.That(features, Is.Empty, "An empty document must not be mistaken for a valid split.");
            Assert.That(unassigned, Is.Empty);
        });
    }

    /// <remarks>
    /// This is what a source URL serving the wrong document looks like: a well-formed file whose
    /// paths belong to nothing the feature map knows. Every path lands in the unassigned list and no
    /// feature document is produced, which is the signature the run has to refuse to publish.
    /// </remarks>
    [Test]
    public void ADocumentOfEntirelyForeignPathsProducesNoFeatures()
    {
        var spec = SpecFixture.Load("""
            openapi: "3.1.0"
            info:
              title: Some Other API
              version: "1.0.0"
            paths:
              /invoices:
                get:
                  operationId: listInvoices
                  responses:
                    "200":
                      description: OK
              /customers:
                get:
                  operationId: listCustomers
                  responses:
                    "200":
                      description: OK
            components:
              schemas: {}
            """);

        var (features, unassigned) = SpecSplitter.Split(SpecCleaner.Clean(spec));

        Assert.Multiple(() =>
        {
            Assert.That(features, Is.Empty);
            Assert.That(unassigned.Select(entry => entry.Path), Is.EquivalentTo(new[] { "/invoices", "/customers" }));
        });
    }

    /// <remarks>
    /// Validation failing has to be a failed run rather than a warning. It used to print "ISSUES"
    /// and exit zero, which meant a feature document that was not self-contained, the single promise
    /// the split exists to make, would be published anyway.
    /// </remarks>
    [Test]
    public void ADanglingReferenceLeavesTheSnapshotUnpublishable()
    {
        var spec = SpecFixture.Load("""
            openapi: "3.1.0"
            info:
              title: Test
              version: "1.0.0"
            paths:
              /embeddings:
                post:
                  operationId: createEmbedding
                  tags:
                    - Embeddings
                  responses:
                    "200":
                      description: OK
                      content:
                        application/json:
                          schema:
                            $ref: "#/components/schemas/Missing"
            components:
              schemas:
                Present:
                  type: object
            """);

        var (features, _) = SpecSplitter.Split(SpecCleaner.Clean(spec));
        var results = SpecValidator.ValidateAll(features);

        Assert.That(features, Is.Not.Empty, "The split should succeed; it is validation that must catch this.");
        Assert.That(results.Any(result => !result.IsValid), Is.True,
            "A dangling reference has to fail validation, since that is what stops the run from publishing.");
    }
}
