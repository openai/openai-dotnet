using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace OpenAI.Tests.Utility;

[Category("Smoke")]
[Parallelizable(ParallelScope.All)]
public class PlaybackCommandDocumentationTests
{
    private const string ModeVariable = "CLIENTMODEL_TEST_MODE";
    private const string DisableAutoRecordingVariable = "CLIENTMODEL_DISABLE_AUTO_RECORDING";
    private const string SkillPath = ".github/skills/running-tests/SKILL.md";

    private static readonly HashSet<string> GeneratedDocumentationDirectories =
        new(StringComparer.OrdinalIgnoreCase)
        {
            ".git",
            ".codex-validation",
            ".vs",
            "artifacts",
            "BenchmarkDotNet.Artifacts",
            "bin",
            "binaries",
            "node_modules",
            "obj",
            "TestResults",
            "vendor",
        };

    private static readonly HashSet<string> ExecutableFenceLanguages =
        new(StringComparer.OrdinalIgnoreCase)
        {
            string.Empty,
            "bash",
            "bat",
            "batch",
            "cli",
            "cmd",
            "console",
            "powershell",
            "ps1",
            "pwsh",
            "sh",
            "shell",
            "shellsession",
            "terminal",
            "zsh",
        };

    [TestCase("bash")]
    [TestCase("powershell")]
    public void PlaybackExamplesOverrideHostileInheritedSettings(string language)
    {
        string command = ExtractAgentPlaybackCommand(language);

        Assert.That(
            RunWithFakeDotnet(language, command),
            Is.EqualTo(new[] { "Playback", "true", "test OpenAI.slnx" }),
            "The actual documented command must override both unsafe inherited settings before invoking dotnet.");
    }

    [TestCase("bash")]
    [TestCase("powershell")]
    public void FakeDotnetExposesUnsafeSettingsWhenSafeguardsAreMissing(string language)
    {
        Assert.That(
            RunWithFakeDotnet(language, "dotnet test OpenAI.slnx"),
            Is.EqualTo(new[] { "Record", "false", "test OpenAI.slnx" }),
            "The regression fixture must start from genuinely unsafe inherited settings.");
    }

    [Test]
    public void AgentInstructionsScopeTestingSkillToTestRelatedWork()
    {
        string instructions = File.ReadAllText(Path.Combine(GetRepositoryRoot(), "AGENTS.md"));

        Assert.Multiple(() =>
        {
            Assert.That(instructions, Does.Contain("CLIENTMODEL_TEST_MODE=Playback"));
            Assert.That(instructions, Does.Contain("CLIENTMODEL_DISABLE_AUTO_RECORDING=true"));
            Assert.That(instructions, Does.Contain("running, writing, modifying, debugging, or validating tests"));
            Assert.That(instructions, Does.Contain(SkillPath));
            Assert.That(
                Regex.IsMatch(
                    instructions, @"\[repository testing instructions\]\([^)]+\)\s+before\s+making changes"),
                Is.False,
                "The running-tests skill must only be required for test-related work.");
        });
    }

    [Test]
    public void ContributorGuideDelegatesTestCommandsToCanonicalSkill()
    {
        string contributing = File.ReadAllText(Path.Combine(GetRepositoryRoot(), "CONTRIBUTING.md"));

        Assert.Multiple(() =>
        {
            Assert.That(contributing, Does.Contain("CLIENTMODEL_TEST_MODE=Playback"));
            Assert.That(contributing, Does.Contain("CLIENTMODEL_DISABLE_AUTO_RECORDING=true"));
            Assert.That(contributing, Does.Contain(SkillPath));
            Assert.That(
                Regex.IsMatch(contributing, @"(?m)^.*\bdotnet\s+test\b"),
                Is.False,
                "All executable test commands must have one canonical owner: the running-tests skill.");
        });
    }

    [Test]
    public void RepositoryMarkdownDelegatesExecutableTestCommandsToCanonicalSkill()
    {
        string root = GetRepositoryRoot();
        string canonicalSkill = Path.GetFullPath(Path.Join(root, SkillPath));
        StringComparison comparison = OperatingSystem.IsWindows()
            ? StringComparison.OrdinalIgnoreCase
            : StringComparison.Ordinal;
        List<string> violations = [];
        bool inspectedMutualTlsGuide = false;

        foreach (string document in EnumerateRepositoryMarkdown(root))
        {
            string absolute = Path.GetFullPath(document);

            if (string.Equals(absolute, canonicalSkill, comparison))
            {
                continue;
            }

            string relative = Path.GetRelativePath(root, absolute).Replace('\\', '/');

            if (string.Equals(relative, "examples/MutualTls/README.md", StringComparison.OrdinalIgnoreCase))
            {
                inspectedMutualTlsGuide = true;
            }

            foreach (string command in FindExecutableTestCommands(File.ReadAllText(absolute)))
            {
                violations.Add($"{relative}: {command}");
            }
        }

        Assert.Multiple(() =>
        {
            Assert.That(
                inspectedMutualTlsGuide,
                Is.True,
                "The repository-wide guard must include feature-specific documentation.");
            Assert.That(
                violations,
                Is.Empty,
                "Executable test commands must exist only in the canonical running-tests skill.");
        });
    }

    [Test]
    public void MutualTlsPlaybackExampleOverridesHostileInheritedSettings()
    {
        string command = ExtractDocumentedCommand("### Offline mTLS Tests (Playback)", "powershell");

        Assert.That(
            RunWithFakeDotnet("powershell", command),
            Is.EqualTo(new[]
            {
                "Playback",
                "true",
                "test ./tests/OpenAI.Tests.csproj --filter TestCategory=MutualTls",
            }),
            "The feature-specific Playback command must override both unsafe inherited settings.");
    }

    [Test]
    public void MutualTlsLiveExampleIsRestrictedToAuthorizedHumans()
    {
        string section = ExtractDocumentedSection("### Live mTLS Example (Authorized Humans Only)");
        string command = ExtractDocumentedCommand(
            "### Live mTLS Example (Authorized Humans Only)", "powershell");
        string guide = File.ReadAllText(Path.Join(
            GetRepositoryRoot(), "examples", "MutualTls", "README.md"));

        Assert.Multiple(() =>
        {
            Assert.That(section, Does.Contain("explicitly authorized humans"));
            Assert.That(section, Does.Contain("approved local"));
            Assert.That(section, Does.Contain("Agents and ordinary CI must never run"));
            Assert.That(command, Does.Contain("$env:CLIENTMODEL_TEST_MODE = \"Live\""));
            Assert.That(command, Does.Contain("$env:CLIENTMODEL_DISABLE_AUTO_RECORDING = \"true\""));
            Assert.That(command, Does.Contain("NUnit.Where="));
            Assert.That(
                command,
                Does.Contain("OpenAI.Examples.MutualTlsExamples.Example01_MutualTlsAsync"));
            Assert.That(command, Does.Not.Contain("$env:OPENAI_API_KEY ="));
            Assert.That(guide, Does.Contain("explicitly authorized humans"));
            Assert.That(guide, Does.Contain("Agents and ordinary CI must never run the live example."));
            Assert.That(guide, Does.Contain("CLIENTMODEL_TEST_MODE=Playback"));
            Assert.That(guide, Does.Contain("CLIENTMODEL_DISABLE_AUTO_RECORDING=true"));
        });
    }

    private static string ExtractAgentPlaybackCommand(string language)
    {
        return ExtractDocumentedCommand("## Agent Rules", language);
    }

    private static string ExtractDocumentedSection(string heading)
    {
        string markdown = File.ReadAllText(Path.Combine(GetRepositoryRoot(), SkillPath));
        Match section = Regex.Match(
            markdown,
            @"^" + Regex.Escape(heading) + @"[ \t]*\r?\n(?<content>[\s\S]*?)(?=^#{1,3}[ \t]|\z)",
            RegexOptions.Multiline | RegexOptions.CultureInvariant);

        Assert.That(section.Success, Is.True, $"The canonical test skill must contain {heading}.");
        return section.Groups["content"].Value;
    }

    private static string ExtractDocumentedCommand(string heading, string language)
    {
        const string fence = "\u0060\u0060\u0060";
        MatchCollection commands = Regex.Matches(
            ExtractDocumentedSection(heading),
            $@"^{fence}{Regex.Escape(language)}[ \t]*\r?\n(?<command>[\s\S]*?)^{fence}[ \t]*$",
            RegexOptions.Multiline | RegexOptions.CultureInvariant);

        Assert.That(
            commands.Count,
            Is.EqualTo(1),
            $"Expected one canonical {language} command in {heading}.");

        return commands[0].Groups["command"].Value;
    }

    private static IEnumerable<string> EnumerateRepositoryMarkdown(string directory)
    {
        foreach (string file in Directory.EnumerateFiles(directory))
        {
            string extension = Path.GetExtension(file);

            if ((string.Equals(extension, ".md", StringComparison.OrdinalIgnoreCase)
                || string.Equals(extension, ".mdx", StringComparison.OrdinalIgnoreCase))
                && (File.GetAttributes(file) & FileAttributes.ReparsePoint) == 0)
            {
                yield return file;
            }
        }

        foreach (string child in Directory.EnumerateDirectories(directory))
        {
            if (GeneratedDocumentationDirectories.Contains(Path.GetFileName(child))
                || (File.GetAttributes(child) & FileAttributes.ReparsePoint) != 0)
            {
                continue;
            }

            foreach (string markdown in EnumerateRepositoryMarkdown(child))
            {
                yield return markdown;
            }
        }
    }

    private static IEnumerable<string> FindExecutableTestCommands(string markdown)
    {
        string openFence = null;
        bool executableFence = false;

        foreach (string sourceLine in markdown.Split('\n'))
        {
            string line = sourceLine.TrimStart();

            while (line.StartsWith(">", StringComparison.Ordinal))
            {
                line = line.Substring(1).TrimStart();
            }

            if (line.StartsWith("\u0060\u0060\u0060", StringComparison.Ordinal)
                || line.StartsWith("~~~", StringComparison.Ordinal))
            {
                string fence = line.Substring(0, 3);

                if (openFence is null)
                {
                    string[] details = line.Substring(3).Trim().Split(
                        new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                    string language = details.Length == 0 ? string.Empty : details[0];

                    openFence = fence;
                    executableFence = ExecutableFenceLanguages.Contains(language);
                }
                else if (string.Equals(openFence, fence, StringComparison.Ordinal))
                {
                    openFence = null;
                    executableFence = false;
                }

                continue;
            }

            if (openFence is not null
                && executableFence
                && !line.StartsWith("#", StringComparison.Ordinal)
                && Regex.IsMatch(line, @"\bdotnet[ \t]+test\b", RegexOptions.CultureInvariant))
            {
                yield return line.Trim();
            }
        }
    }

    private static string[] RunWithFakeDotnet(string language, string command)
    {
        string directory = Path.Combine(Path.GetTempPath(), "openai-playback-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(directory);

        try
        {
            bool useWindowsBatch = OperatingSystem.IsWindows() && language == "powershell";
            string executableName = useWindowsBatch ? "dotnet.cmd" : "dotnet";
            string fakeDotnet = Path.Combine(directory, executableName);
            string implementation = useWindowsBatch
                ? "@echo off\r\necho %CLIENTMODEL_TEST_MODE%\r\necho %CLIENTMODEL_DISABLE_AUTO_RECORDING%\r\necho %*\r\n"
                : "#!/bin/sh\nprintf '%s\\n' \"$CLIENTMODEL_TEST_MODE\" \"$CLIENTMODEL_DISABLE_AUTO_RECORDING\" \"$*\"\n";

            File.WriteAllText(fakeDotnet, implementation);

            if (!OperatingSystem.IsWindows())
            {
                File.SetUnixFileMode(
                    fakeDotnet,
                    UnixFileMode.UserRead | UnixFileMode.UserWrite | UnixFileMode.UserExecute);
            }

            ProcessStartInfo start = new(ResolveShell(language))
            {
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                WorkingDirectory = GetRepositoryRoot(),
            };

            if (language == "bash")
            {
                start.ArgumentList.Add("--noprofile");
                start.ArgumentList.Add("--norc");
                start.ArgumentList.Add("-c");
            }
            else
            {
                start.ArgumentList.Add("-NoLogo");
                start.ArgumentList.Add("-NoProfile");
                start.ArgumentList.Add("-NonInteractive");
                start.ArgumentList.Add("-Command");
            }

            start.ArgumentList.Add(command);
            start.Environment["PATH"] = directory;
            start.Environment[ModeVariable] = "Record";
            start.Environment[DisableAutoRecordingVariable] = "false";
            start.Environment.Remove("OPENAI_API_KEY");
            start.Environment.Remove("BASH_ENV");
            start.Environment.Remove("ENV");

            using Process process = Process.Start(start);
            Assert.That(process, Is.Not.Null, $"The {language} command could not be started.");

            if (!process.WaitForExit(15_000))
            {
                process.Kill(entireProcessTree: true);
                Assert.Fail($"The {language} Playback command did not finish within 15 seconds.");
            }

            string standardOutput = process.StandardOutput.ReadToEnd();
            string standardError = process.StandardError.ReadToEnd();

            Assert.That(
                process.ExitCode,
                Is.Zero,
                $"The canonical {language} command failed against the fake dotnet: {standardError}");

            return standardOutput.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
        }
        finally
        {
            Directory.Delete(directory, recursive: true);
        }
    }

    private static string ResolveShell(string language)
    {
        string[] names = language == "powershell"
            ? new[] { "pwsh", "powershell" }
            : new[] { "bash" };
        string suffix = OperatingSystem.IsWindows() ? ".exe" : string.Empty;
        string path = Environment.GetEnvironmentVariable("PATH") ?? string.Empty;

        foreach (string name in names)
        {
            foreach (string candidateDirectory in path.Split(
                Path.PathSeparator, StringSplitOptions.RemoveEmptyEntries))
            {
                string candidate = Path.Combine(candidateDirectory, name + suffix);

                if (File.Exists(candidate))
                {
                    return candidate;
                }
            }
        }

        if (language == "bash" && OperatingSystem.IsWindows())
        {
            Assert.Ignore("Bash is not a required Windows dependency; Ubuntu CI validates this example.");
        }

        Assert.Fail(
            $"A supported {language} shell ({string.Join(" or ", names)}) must be installed " +
            "to validate its Playback example.");
        return null;
    }

    private static string GetRepositoryRoot()
    {
        AssemblyMetadataAttribute source = typeof(PlaybackCommandDocumentationTests)
            .Assembly
            .GetCustomAttributes<AssemblyMetadataAttribute>()
            .Single(attribute => attribute.Key == "SourcePath");

        return Path.GetFullPath(Path.Combine(source.Value, ".."));
    }
}
