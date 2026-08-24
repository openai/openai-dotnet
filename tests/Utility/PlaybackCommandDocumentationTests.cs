using NUnit.Framework;
using System;
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

    private static string ExtractAgentPlaybackCommand(string language)
    {
        string markdown = File.ReadAllText(Path.Combine(GetRepositoryRoot(), SkillPath));
        Match agentRules = Regex.Match(
            markdown,
            @"^## Agent Rules[ \t]*\r?\n(?<content>[\s\S]*?)(?=^## [^\r\n]+|\z)",
            RegexOptions.Multiline | RegexOptions.CultureInvariant);

        Assert.That(agentRules.Success, Is.True, "The canonical test skill must contain an Agent Rules section.");

        const string fence = "\u0060\u0060\u0060";
        MatchCollection commands = Regex.Matches(
            agentRules.Groups["content"].Value,
            $@"^{fence}{Regex.Escape(language)}[ \t]*\r?\n(?<command>[\s\S]*?)^{fence}[ \t]*$",
            RegexOptions.Multiline | RegexOptions.CultureInvariant);

        Assert.That(commands.Count, Is.EqualTo(1), $"Expected one canonical {language} Playback example.");
        return commands[0].Groups["command"].Value;
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
