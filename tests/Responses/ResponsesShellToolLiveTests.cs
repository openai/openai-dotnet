using NUnit.Framework;
using OpenAI.Responses;
using System;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OpenAI.Tests.Responses;

#pragma warning disable OPENAI001 // Experimental API

/// <summary>
/// End-to-end tests for the Shell tool (https://developers.openai.com/api/docs/guides/tools-shell).
///
/// These are <see cref="ExplicitAttribute">explicit</see> live tests — they hit the real OpenAI API
/// and are not part of the normal CI run. Run with:
///   $env:OPENAI_API_KEY = "..."
///   dotnet test tests --filter "FullyQualifiedName~ShellToolLiveTests" -f net10.0
/// They intentionally do NOT use the recording framework so that the API key is never persisted
/// into a session recording on disk.
/// </summary>
[Explicit("Live tests; require OPENAI_API_KEY and hit the real API.")]
[Category("Live")]
[Category("ResponsesTools")]
public class ResponsesShellToolLiveTests
{
    private const string Model = "gpt-5.5";

    private static ResponsesClient CreateClient()
    {
        string apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");
        if (string.IsNullOrWhiteSpace(apiKey))
        {
            Assert.Ignore("OPENAI_API_KEY is not set; skipping live shell tool test.");
        }
        return new ResponsesClient(new ApiKeyCredential(apiKey), new ResponsesClientOptions
        {
            NetworkTimeout = TimeSpan.FromMinutes(5),
        });
    }

    // ---------------------------------------------------------------------
    // Round-trip serialization (no network)
    // ---------------------------------------------------------------------

    [Test]
    public void ShellTool_HostedContainerAuto_RoundTrips()
    {
        ShellTool tool = ResponseTool.CreateShellTool(new ShellToolContainerAutoEnvironment
        {
            MemoryLimit = ShellToolContainerMemoryLimit.FourGigabytes,
            NetworkPolicy = new ShellToolAllowlistContainerNetworkPolicy(["pypi.org", "files.pythonhosted.org"])
            {
                DomainSecrets = { new ShellToolDomainSecret("pypi.org", "API_KEY", "redacted-test-token") },
            },
            Skills = { new ShellToolSkillReference("skill_test_id") { Version = "latest" } },
        });

        BinaryData json = ModelReaderWriter.Write(tool);
        string text = json.ToString();
        Assert.That(text, Does.Contain("\"type\":\"shell\""));
        Assert.That(text, Does.Contain("\"environment\""));
        Assert.That(text, Does.Contain("\"container_auto\""));
        Assert.That(text, Does.Contain("\"memory_limit\":\"4g\""));
        Assert.That(text, Does.Contain("\"allowlist\""));
        Assert.That(text, Does.Contain("\"pypi.org\""));
        Assert.That(text, Does.Contain("\"skill_reference\""));
        Assert.That(text, Does.Contain("\"skill_id\":\"skill_test_id\""));

        // Round-trip and verify the discriminated union resolves correctly.
        ShellTool round = ModelReaderWriter.Read<ShellTool>(json);
        Assert.That(round.Environment, Is.TypeOf<ShellToolContainerAutoEnvironment>());
        ShellToolContainerAutoEnvironment env = (ShellToolContainerAutoEnvironment)round.Environment;
        Assert.That(env.MemoryLimit, Is.EqualTo(ShellToolContainerMemoryLimit.FourGigabytes));
        Assert.That(env.NetworkPolicy, Is.TypeOf<ShellToolAllowlistContainerNetworkPolicy>());
        Assert.That(env.Skills.Single(), Is.TypeOf<ShellToolSkillReference>());
    }

    [Test]
    public void ShellTool_LocalEnvironment_RoundTrips()
    {
        ShellTool tool = ResponseTool.CreateShellTool(new ShellToolLocalEnvironment
        {
            Skills = { new ShellToolLocalSkill("custom-skill", "Test local skill.", "/skills/custom") },
        });

        BinaryData json = ModelReaderWriter.Write(tool);
        Assert.That(json.ToString(), Does.Contain("\"type\":\"shell\""));
        Assert.That(json.ToString(), Does.Contain("\"local\""));
        Assert.That(json.ToString(), Does.Contain("\"path\":\"/skills/custom\""));

        ShellTool round = ModelReaderWriter.Read<ShellTool>(json);
        Assert.That(round.Environment, Is.TypeOf<ShellToolLocalEnvironment>());
    }

    [Test]
    public void ShellAction_TimeoutSerializesAsIntegerMilliseconds()
    {
        ShellAction action = new(["echo", "hello"])
        {
            Timeout = TimeSpan.FromSeconds(2.5),
            MaxOutputLength = 1024,
        };

        BinaryData json = ModelReaderWriter.Write(action);
        string text = json.ToString();

        // Verify the wire-format property name + integer-ms encoding (not seconds, not ISO-8601).
        Assert.That(text, Does.Contain("\"timeout_ms\":2500"));
        Assert.That(text, Does.Contain("\"commands\":[\"echo\",\"hello\"]"));
        Assert.That(text, Does.Contain("\"max_output_length\":1024"));

        ShellAction round = ModelReaderWriter.Read<ShellAction>(json);
        Assert.That(round.Timeout, Is.EqualTo(TimeSpan.FromMilliseconds(2500)));
        Assert.That(round.CommandParts, Is.EquivalentTo(new[] { "echo", "hello" }));
        Assert.That(round.MaxOutputLength, Is.EqualTo(1024));
    }

    [Test]
    public void ShellCallOutputContent_DiscriminatedOutcomeRoundTrips()
    {
        ShellCallOutputContent exitContent = new("stdout-text", "", new ShellCallExitOutcome(0));
        BinaryData exitJson = ModelReaderWriter.Write(exitContent);
        Assert.That(exitJson.ToString(), Does.Contain("\"type\":\"exit\""));
        Assert.That(exitJson.ToString(), Does.Contain("\"exit_code\":0"));
        ShellCallOutputContent exitRound = ModelReaderWriter.Read<ShellCallOutputContent>(exitJson);
        Assert.That(exitRound.Outcome, Is.TypeOf<ShellCallExitOutcome>());
        Assert.That(((ShellCallExitOutcome)exitRound.Outcome).ExitCode, Is.EqualTo(0));

        ShellCallOutputContent timeoutContent = new("", "stderr-text", new ShellCallTimeoutOutcome());
        BinaryData timeoutJson = ModelReaderWriter.Write(timeoutContent);
        Assert.That(timeoutJson.ToString(), Does.Contain("\"type\":\"timeout\""));
        ShellCallOutputContent timeoutRound = ModelReaderWriter.Read<ShellCallOutputContent>(timeoutJson);
        Assert.That(timeoutRound.Outcome, Is.TypeOf<ShellCallTimeoutOutcome>());
    }

    // ---------------------------------------------------------------------
    // Live calls
    // ---------------------------------------------------------------------

    /// <summary>
    /// Hosted shell — OpenAI provisions and executes against an auto-managed container.
    /// We expect at least one <see cref="ShellCallItem"/> and matching <see cref="ShellCallOutputItem"/>
    /// to come back in a single response (no client-side resolution required).
    /// </summary>
    [Test]
    public async Task HostedShellTool_ContainerAuto_ExecutesEndToEnd()
    {
        ResponsesClient client = CreateClient();

        CreateResponseOptions options = new(Model,
        [
            ResponseItem.CreateUserMessageItem(
                "Use the shell tool to run `echo OPENAI_SHELL_TEST` and report the exact output."),
        ])
        {
            Tools = { ResponseTool.CreateShellTool(new ShellToolContainerAutoEnvironment()) },
            ToolChoice = ResponseToolChoice.CreateRequiredChoice(),
        };

        ResponseResult response = await client.CreateResponseAsync(options);

        Assert.That(response.OutputItems, Is.Not.Empty);
        // Note: the request was `container_auto`, but the server provisions a container and
        // echoes it back as a `container_reference` — accept either.
        ShellToolEnvironment respondedEnvironment = response.Tools.OfType<ShellTool>().Single().Environment;
        Assert.That(
            respondedEnvironment,
            Is.TypeOf<ShellToolContainerAutoEnvironment>().Or.TypeOf<ShellToolContainerReferenceEnvironment>(),
            $"Unexpected environment type returned by server: {respondedEnvironment?.GetType().Name ?? "<null>"}");

        List<ShellCallItem> calls = response.OutputItems.OfType<ShellCallItem>().ToList();
        Assert.That(calls, Has.Count.GreaterThan(0), "Expected at least one shell_call item.");
        ShellCallItem firstCall = calls[0];
        Assert.That(firstCall.CallId, Is.Not.Null.And.Not.Empty);
        Assert.That(firstCall.Action, Is.Not.Null);
        Assert.That(firstCall.Action.CommandParts, Is.Not.Empty);
        Assert.That(string.Join(" ", firstCall.Action.CommandParts), Does.Contain("echo"));

        List<ShellCallOutputItem> outputs = response.OutputItems.OfType<ShellCallOutputItem>().ToList();
        Assert.That(outputs, Has.Count.GreaterThan(0),
            "Hosted shell should emit shell_call_output items in the same response.");
        ShellCallOutputItem firstOutput = outputs.First(o => o.CallId == firstCall.CallId);
        Assert.That(firstOutput.Output, Is.Not.Empty);
        ShellCallOutputContent stdoutChunk = firstOutput.Output[0];
        Assert.That(stdoutChunk.Stdout, Does.Contain("OPENAI_SHELL_TEST"));
        Assert.That(stdoutChunk.Outcome, Is.TypeOf<ShellCallExitOutcome>().Or.TypeOf<ShellCallTimeoutOutcome>());
    }

    /// <summary>
    /// Local shell — the model emits a <see cref="ShellCallItem"/> and the caller is responsible
    /// for executing the command and replying with a <see cref="ShellCallOutputItem"/>. We simulate
    /// the local execution and verify the follow-up response contains a final assistant message.
    /// </summary>
    [Test]
    public async Task LocalShellTool_ResolvedByCaller_EndToEnd()
    {
        ResponsesClient client = CreateClient();

        CreateResponseOptions options = new(Model,
        [
            ResponseItem.CreateDeveloperMessageItem(
                "You have access to a local shell. Use it to learn the value the user asks for."),
            ResponseItem.CreateUserMessageItem(
                "Run `echo OPENAI_LOCAL_SHELL_TOKEN` in the local shell and tell me what it prints."),
        ])
        {
            Tools = { ResponseTool.CreateShellTool(new ShellToolLocalEnvironment()) },
            ToolChoice = ResponseToolChoice.CreateRequiredChoice(),
        };

        ResponseResult first = await client.CreateResponseAsync(options);
        Assert.That(first.OutputItems, Is.Not.Empty);

        List<ShellCallItem> calls = first.OutputItems.OfType<ShellCallItem>().ToList();
        Assert.That(calls, Has.Count.GreaterThan(0),
            "Local shell tool should emit shell_call items for the caller to execute.");

        // The local environment should *not* produce server-emitted outputs.
        Assert.That(first.OutputItems.OfType<ShellCallOutputItem>(), Is.Empty,
            "Local-mode shell calls should be resolved by the client, not the server.");

        // Simulate local execution and feed outputs back via `previous_response_id`.
        CreateResponseOptions followUpOptions = new(Model, [])
        {
            PreviousResponseId = first.Id,
            Tools = { ResponseTool.CreateShellTool(new ShellToolLocalEnvironment()) },
            ToolChoice = ResponseToolChoice.CreateAutoChoice(),
        };
        foreach (ShellCallItem call in calls)
        {
            ShellCallOutputContent content = new(
                stdout: "OPENAI_LOCAL_SHELL_TOKEN\n",
                stderr: string.Empty,
                outcome: new ShellCallExitOutcome(0));
            followUpOptions.InputItems.Add(ResponseItem.CreateShellCallOutputItem(call.CallId, [content]));
        }

        ResponseResult second = await client.CreateResponseAsync(followUpOptions);
        MessageResponseItem assistantMessage = second.OutputItems.OfType<MessageResponseItem>().LastOrDefault();
        Assert.That(assistantMessage, Is.Not.Null, "Expected a final assistant message after resolving the local shell call.");
        Assert.That(assistantMessage.Content, Is.Not.Empty);
        string assistantText = assistantMessage.Content[0].Text ?? string.Empty;
        Assert.That(assistantText, Does.Contain("OPENAI_LOCAL_SHELL_TOKEN"),
            "Model should echo the local shell output in its final answer.");
    }
}
