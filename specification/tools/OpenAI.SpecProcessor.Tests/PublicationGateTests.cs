using System.CommandLine;
using NUnit.Framework;
using OpenAI.SpecProcessor.Commands;

namespace OpenAI.SpecProcessor.Tests;

/// <summary>
/// Covers the contract the orchestrator relies on to decide whether a snapshot may be published.
/// </summary>
/// <remarks>
/// Publication is gated on one thing and one thing only: the processor's exit code. Every path that
/// can publish, a normal run, a forced run, a dry run, and a run with <c>--no-diff</c>, goes through
/// the same check, so a non-zero exit has to survive all of them. It used to not: validation failure
/// printed "ISSUES" and exited zero, which meant a feature document that was not self-contained
/// would be published anyway and become the baseline every future month was measured against.
///
/// These tests drive the real command rather than the pieces underneath it, because the defect lived
/// in the wiring between them.
/// </remarks>
[TestFixture]
public class PublicationGateTests
{
    private string _directory = null!;

    [SetUp]
    public void SetUp()
    {
        _directory = Path.Combine(Path.GetTempPath(), $"spec-gate-{Guid.NewGuid():N}");
        Directory.CreateDirectory(_directory);
    }

    [TearDown]
    public void TearDown()
    {
        if (Directory.Exists(_directory))
        {
            Directory.Delete(_directory, recursive: true);
        }
    }

    [Test]
    public void AValidSpecificationExitsZero()
    {
        Assert.That(Run(SpecFixture.RichSpec), Is.Zero,
            "A well-formed source has to succeed, or the gate would block every run rather than the bad ones.");
    }

    [Test]
    public void AValidSpecificationExitsZeroWithoutDiffs()
    {
        Assert.That(Run(SpecFixture.RichSpec, noDiff: true), Is.Zero);
    }

    [TestCase(false)]
    [TestCase(true)]
    public void ADanglingReferenceExitsNonZero(bool noDiff)
    {
        Assert.That(Run(DanglingReferenceSpec, noDiff), Is.Not.Zero,
            "Validation failure has to fail the run whether or not diffs were requested, since --no-diff "
            + "skips the report but not the publication decision.");
    }

    [TestCase(false)]
    [TestCase(true)]
    public void ASourceWithNoRecognizableSurfaceExitsNonZero(bool noDiff)
    {
        Assert.That(Run(EmptySpec, noDiff), Is.Not.Zero,
            "A split producing no feature areas means the source is not the specification we think it is.");
    }

    /// <remarks>
    /// The processor never writes to the committed snapshots; it writes to a staging directory the
    /// orchestrator supplies, and rotation happens afterwards only on a zero exit. This proves the
    /// first half of that promise, that a failed run leaves the directory it was pointed at as a
    /// baseline exactly as it found it. The orchestrator's half is covered by its own failure runs.
    /// </remarks>
    [Test]
    public void AFailedRunLeavesTheBaselineUntouched()
    {
        var baseline = Path.Combine(_directory, "previous");
        Directory.CreateDirectory(baseline);

        var sentinel = Path.Combine(baseline, "responses.yml");
        File.WriteAllText(sentinel, "sentinel: true\n");

        var before = File.ReadAllBytes(sentinel);
        var exitCode = Run(DanglingReferenceSpec, previousSpec: baseline);

        Assert.Multiple(() =>
        {
            Assert.That(exitCode, Is.Not.Zero);
            Assert.That(Directory.GetFiles(baseline), Has.Length.EqualTo(1));
            Assert.That(File.ReadAllBytes(sentinel), Is.EqualTo(before));
        });
    }

    private int Run(string yaml, bool noDiff = false, string? previousSpec = null)
    {
        var specPath = Path.Combine(_directory, $"source-{Guid.NewGuid():N}.yml");
        File.WriteAllText(specPath, yaml);

        var output = Path.Combine(_directory, $"output-{Guid.NewGuid():N}");
        var previous = previousSpec ?? Path.Combine(_directory, $"previous-{Guid.NewGuid():N}");

        Directory.CreateDirectory(output);
        Directory.CreateDirectory(previous);

        var arguments = new List<string>
        {
            "preprocess",
            "--new-spec", specPath,
            "--previous-spec", previous,
            "--output", output,
            "--report", output
        };

        if (noDiff)
        {
            arguments.Add("--no-diff");
        }

        var root = new RootCommand("test host");
        root.Subcommands.Add(PreprocessCommand.Create(new ProcessorSettings()));

        var original = Console.Out;

        try
        {
            Console.SetOut(TextWriter.Null);
            return root.Parse(arguments.ToArray()).Invoke();
        }
        finally
        {
            Console.SetOut(original);
        }
    }

    private const string DanglingReferenceSpec = """
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
        """;

    private const string EmptySpec = """
        openapi: "3.1.0"
        info:
          title: Test
          version: "1.0.0"
        paths: {}
        components:
          schemas: {}
        """;
}
