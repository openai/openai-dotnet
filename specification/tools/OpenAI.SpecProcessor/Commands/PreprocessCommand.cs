using System.CommandLine;
using System.Security.Cryptography;
using OpenAI.SpecProcessor.Report;
using OpenAI.SpecProcessor.Spec;

namespace OpenAI.SpecProcessor.Commands;

/// <summary> The 'preprocess' command: clean, split, validate, diff, and report. </summary>
public static class PreprocessCommand
{
    /// <summary> Creates the preprocess command with options bound to the provided settings. </summary>
    /// <param name="settings"> The processor settings loaded from configuration. </param>
    /// <returns> The configured <see cref="Command"/> instance. </returns>
    public static Command Create(ProcessorSettings settings)
    {
        var previousSpecOpt = new Option<DirectoryInfo?>("--previous-spec")
        {
            Description = "Path to directory with previous per-feature spec YAML files [default: " + settings.Defaults.PreviousSpecDirectory + "]"
        };

        var newSpecOpt = new Option<FileInfo?>("--new-spec")
        {
            Description = "Path to new raw spec file (YAML). Downloads from OpenAI GitHub if not provided."
        };

        var outputOpt = new Option<DirectoryInfo>("--output")
        {
            Description = "Directory for processed per-feature spec files",
            DefaultValueFactory = _ => new DirectoryInfo(settings.Defaults.OutputDirectory)
        };

        var reportOpt = new Option<DirectoryInfo>("--report")
        {
            Description = "Directory for the diff report",
            DefaultValueFactory = _ => new DirectoryInfo(settings.Defaults.ReportDirectory)
        };

        var noDiffOpt = new Option<bool>("--no-diff")
        {
            Description = "Skip diff and report generation (split and validate only)"
        };

        var includeDescriptionsOpt = new Option<bool>("--include-descriptions")
        {
            Description = "Include description and summary changes in the diff (excluded by default)"
        };

        var commitShaOpt = new Option<string?>("--commit-sha")
        {
            Description = "Upstream commit SHA the spec was downloaded from, recorded for provenance"
        };

        var sourceUrlOpt = new Option<string?>("--source-url")
        {
            Description = "Origin of the spec, recorded for provenance when it was downloaded elsewhere"
        };

        var cmd = new Command("preprocess", "Clean, split, validate, and diff the OpenAI REST API spec");
        cmd.Options.Add(previousSpecOpt);
        cmd.Options.Add(newSpecOpt);
        cmd.Options.Add(outputOpt);
        cmd.Options.Add(reportOpt);
        cmd.Options.Add(noDiffOpt);
        cmd.Options.Add(includeDescriptionsOpt);
        cmd.Options.Add(commitShaOpt);
        cmd.Options.Add(sourceUrlOpt);

        cmd.SetAction(parseResult =>
        {
            var previousSpec = parseResult.GetValue(previousSpecOpt)
                ?? new DirectoryInfo(settings.Defaults.PreviousSpecDirectory);
            var newSpec = parseResult.GetValue(newSpecOpt);
            var output = parseResult.GetValue(outputOpt)!;
            var report = parseResult.GetValue(reportOpt)!;
            var noDiff = parseResult.GetValue(noDiffOpt);
            var includeDescriptions = parseResult.GetValue(includeDescriptionsOpt);
            var commitSha = parseResult.GetValue(commitShaOpt);
            var sourceUrl = parseResult.GetValue(sourceUrlOpt);

            return RunAsync(settings, previousSpec, newSpec, output, report, noDiff, includeDescriptions, commitSha, sourceUrl)
                .GetAwaiter()
                .GetResult();
        });

        return cmd;
    }

    private static async Task<int> RunAsync(
        ProcessorSettings settings,
        DirectoryInfo previousSpecDir,
        FileInfo? newSpecFile,
        DirectoryInfo outputDir,
        DirectoryInfo reportDir,
        bool noDiff,
        bool includeDescriptions,
        string? commitSha,
        string? sourceUrl)
    {
        // ── Step 1: Validate ─────────────────────────────────────────────

        WriteStep("Step 1: Validating Inputs");

        if (!previousSpecDir.Exists)
        {
            previousSpecDir.Create();
            WriteInfo($"Created previous spec directory: {previousSpecDir.FullName}");
        }

        if (!outputDir.Exists)
        {
            outputDir.Create();
            WriteInfo($"Created output directory: {outputDir.FullName}");
        }

        if (!reportDir.Exists)
        {
            reportDir.Create();
            WriteInfo($"Created report directory: {reportDir.FullName}");
        }

        var previousFiles = previousSpecDir.GetFiles("*.yml");

        if (previousFiles.Length == 0)
        {
            WriteWarning("No .yml files in previous spec directory. Diff will show all as 'Added'.");
        }
        else
        {
            WriteInfo($"Found {previousFiles.Length} previous spec file(s)");
        }

        WriteSuccess("Inputs validated");

        // ── Step 2: Acquire Spec ─────────────────────────────────────────
        WriteStep("Step 2: Acquiring Spec");

        string specPath;
        if (newSpecFile != null)
        {
            if (!newSpecFile.Exists)
                throw new FileNotFoundException($"Spec file not found: {newSpecFile.FullName}");

            specPath = newSpecFile.FullName;
            WriteSuccess($"Using local spec: {specPath}");
        }
        else
        {
            specPath = Path.Combine(Path.GetTempPath(), settings.Spec.RawDownloadFile);
            WriteInfo($"Downloading from: {settings.Spec.SourceUrl}");

            using var httpClient = new HttpClient();
            var content = await httpClient.GetStringAsync(settings.Spec.SourceUrl);
            await LineEndings.WriteAllTextAsync(specPath, content);

            WriteSuccess($"Downloaded: {specPath} ({new FileInfo(specPath).Length / 1024} KB)");
        }

        // ── Step 3: Clean ────────────────────────────────────────────────
        WriteStep("Step 3: Cleaning Spec");

        var repairedLineCount = 0;
        var rawSpec = SpecDocument.Load(specPath, repaired =>
        {
            repairedLineCount = repaired;
            WriteWarning($"Repaired {repaired} malformed block scalar line(s) in the source specification");
        });
        WriteInfo($"Loaded: {rawSpec.Title} v{rawSpec.SpecVersion} ({rawSpec.PathKeys.Count()} paths, {rawSpec.SchemaNames.Count()} schemas)");

        // Capture what the cleaner is about to drop. Both excluded and unassigned paths are absent
        // from every feature specification, and the two mean very different things, so the record
        // has to be able to tell them apart.

        var excludedPaths = rawSpec.PathKeys
            .Where(FeatureAreaConfig.IsExcludedPath)
            .OrderBy(p => p, StringComparer.Ordinal)
            .ToList();

        var cleanedSpec = SpecCleaner.Clean(rawSpec);

        // ── Step 4: Split ────────────────────────────────────────────────
        WriteStep("Step 4: Splitting into Feature Specs");

        var (features, unassigned) = SpecSplitter.Split(cleanedSpec);
        WriteInfo($"Split into {features.Count} feature specs");

        // A document that parses but carries nothing recognizable is not a specification worth
        // publishing, and publishing it would replace a good baseline with an empty one. This is
        // the case where a source URL quietly starts serving the wrong document: it costs nothing
        // when the input is real, and it is the difference between a failed run and a lost baseline
        // when the input is not.

        if (features.Count == 0)
        {
            WriteError("The split produced no feature specifications. The source does not look like the OpenAI REST specification, so nothing was published.");
            return 1;
        }

        if (unassigned.Count > 0)
        {
            WriteWarning($"Unassigned paths ({unassigned.Count}):");

            foreach (var p in unassigned)
            {
                WriteWarning($"  {p.Path}");
            }
        }

        // Save each feature spec to its output file.

        foreach (var (name, featureSpec) in features)
        {
            var area = FeatureAreaConfig.All.First(a => a.Name == name);
            var featurePath = Path.Combine(outputDir.FullName, area.OutputFile);
            featureSpec.Save(featurePath);
            WriteInfo($"  {area.OutputFile} ({featureSpec.PathKeys.Count()} paths, {featureSpec.SchemaNames.Count()} schemas)");
        }

        WriteSuccess($"All feature specs saved to: {outputDir.FullName}");

        // Write metadata marker alongside the spec files.

        // Prefer the origin the caller reported. A locally supplied file is only ever a staging copy
        // of something downloaded elsewhere, so its path says nothing useful about where the
        // specification actually came from.

        var newSpecSource = sourceUrl ?? newSpecFile?.FullName ?? settings.Spec.SourceUrl;

        // Label each unassigned path against the previous snapshot. A gap that was reviewed and
        // deliberately left open should stay visible without re-announcing itself as a fresh
        // regression, and one that has since been claimed is worth saying out loud too.

        var previousMeta = SpecMetadata.Load(previousSpecDir.FullName, WriteWarning);
        var reconciledUnassigned = UnassignedPath.Reconcile(unassigned, previousMeta?.UnassignedPaths);

        var newMeta = new SpecMetadata
        {
            Version = cleanedSpec.SpecVersion,
            Source = newSpecSource,
            DownloadedCommitSha = commitSha,
            SourceContentHash = ComputeFileHash(specPath),
            ProcessedAt = DateTimeOffset.UtcNow,
            FeatureCount = features.Count,
            UnassignedPaths = [.. reconciledUnassigned.Where(entry => entry.Status != UnassignedStatus.Resolved)],
            ExcludedPaths = [.. excludedPaths]
        };

        newMeta.Save(outputDir.FullName);

        // ── Step 5: Validate ─────────────────────────────────────────────
        WriteStep("Step 5: Validating Feature Specs");

        var validationResults = SpecValidator.ValidateAll(features);
        bool allValid = true;

        foreach (var result in validationResults)
        {
            if (result.IsValid)
            {
                WriteSuccess($"  {result.FeatureName}: {result.PathCount} paths, {result.OperationCount} ops, {result.SchemaCount} schemas");
            }
            else
            {
                allValid = false;
                WriteError($"  {result.FeatureName}: INVALID");

                foreach (var error in result.Errors)
                {
                    WriteError($"    - {error}");
                }
            }
        }

        if (allValid)
        {
            WriteSuccess("All feature specs are valid");
        }
        else
        {
            WriteWarning("Some specs have validation errors (see above)");
        }

        // ── Step 6: Diff ─────────────────────────────────────────────────

        var diffs = new List<FeatureDiff>();
        var specVersion = cleanedSpec.SpecVersion ?? "unknown";
        string? previousVersion = null;
        string? reportPath = null;

        if (noDiff)
        {
            WriteStep("Step 6: Skipping Diffs (--no-diff)");
            WriteStep("Step 7: Skipping Report (--no-diff)");
        }
        else
        {
            WriteStep("Step 6: Computing Diffs");

            foreach (var (name, newFeatureSpec) in features)
            {
                var area = FeatureAreaConfig.All.First(a => a.Name == name);
                var prevPath = Path.Combine(previousSpecDir.FullName, area.OutputFile);

                SpecDocument? prevSpec = null;
                if (File.Exists(prevPath))
                {
                    try
                    {
                        prevSpec = SpecDocument.Load(prevPath);
                    }
                    catch (Exception ex)
                    {
                        WriteWarning($"  Could not load previous {area.OutputFile}: {ex.Message}");
                    }
                }

            var diff = SpecDiffer.Diff(name, prevSpec, newFeatureSpec, includeDescriptions);
                diffs.Add(diff);

                if (diff.Changes.Count > 0)
                {
                    WriteInfo($"  {name}: {diff.Changes.Count} changes");
                }
                else
                {
                    WriteInfo($"  {name}: no changes");
                }
            }

            // ── Step 7: Report ───────────────────────────────────────────

            WriteStep("Step 7: Generating Diff Report");

            // Read previous metadata for provenance info in the report.

            if (previousFiles.Length > 0 && previousMeta != null)
            {
                previousVersion = previousMeta.Version;
            }
            else if (previousFiles.Length > 0)
            {
                try
                {
                    var firstPrev = SpecDocument.Load(previousFiles[0].FullName);
                    previousVersion = firstPrev.SpecVersion;
                }
                catch
                {
                    // Ignore errors reading the previous version.
                }
            }

            // Build line indexes from the saved feature spec files.

            var lineIndexes = new Dictionary<string, LineIndex>(StringComparer.Ordinal);
            var previousLineIndexes = new Dictionary<string, LineIndex>(StringComparer.Ordinal);

            foreach (var area in FeatureAreaConfig.All)
            {
                var filePath = Path.Combine(outputDir.FullName, area.OutputFile);

                if (File.Exists(filePath))
                {
                    lineIndexes[area.OutputFile] = LineIndex.Build(filePath);
                }

                var prevFilePath = Path.Combine(previousSpecDir.FullName, area.OutputFile);

                if (File.Exists(prevFilePath))
                {
                    previousLineIndexes[area.OutputFile] = LineIndex.Build(prevFilePath);
                }
            }

            var report = DiffReportWriter.Generate(diffs, specVersion, previousVersion, previousMeta, lineIndexes, features, previousLineIndexes, reconciledUnassigned, newMeta, repairedLineCount);

            reportPath = Path.Combine(reportDir.FullName, settings.Spec.DiffReportFile);
            await LineEndings.WriteAllTextAsync(reportPath, report);
            WriteSuccess($"Diff report: {reportPath}");
        }

        // ── Summary ──────────────────────────────────────────────────────
        WriteStep("Summary");
        Console.WriteLine();
        Console.WriteLine($"  Spec version:     {specVersion}");
        Console.WriteLine($"  Previous version: {previousVersion ?? "(none)"}");
        Console.WriteLine($"  Features:         {features.Count}");
        Console.WriteLine($"  Total changes:    {diffs.Sum(d => d.Changes.Count)}");
        Console.WriteLine($"  Validation:       {(allValid ? "PASS" : "ISSUES")}");
        Console.WriteLine($"  Output:           {outputDir.FullName}");

        if (reportPath != null)
        {
            Console.WriteLine($"  Report:           {reportPath}");
        }

        Console.WriteLine();

        // A feature document that failed validation is not self-contained, which is the one promise
        // the split exists to make. Publishing it would put a snapshot in the repository that the
        // next month's comparison is measured against, so the run fails and the orchestrator leaves
        // the existing snapshot in place.

        if (!allValid)
        {
            WriteWarning("Validation failed, so the processed snapshot was not fit to publish.");
            return 1;
        }

        return 0;
    }

    // ── Console helpers ──────────────────────────────────────────────────

    /// <summary> Computes the SHA-256 hash of a file as a lowercase hexadecimal string. </summary>
    /// <param name="path"> The file to hash. </param>
    /// <returns> The lowercase hexadecimal hash. </returns>
    private static string ComputeFileHash(string path)
    {
        using var stream = File.OpenRead(path);
        return Convert.ToHexStringLower(SHA256.HashData(stream));
    }

    private static void WriteStep(string msg)
    {
        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine($"── {msg} ──────────────────────────────────────────");
        Console.ResetColor();
    }

    private static void WriteInfo(string msg)
    {
        Console.WriteLine($"  → {msg}");
    }

    private static void WriteSuccess(string msg)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"  ✓ {msg}");
        Console.ResetColor();
    }

    private static void WriteWarning(string msg)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"  ⚠ {msg}");
        Console.ResetColor();
    }

    private static void WriteError(string msg)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"  ✗ {msg}");
        Console.ResetColor();
    }
}
