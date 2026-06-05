using NUnit.Framework;
using OpenAI.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OpenAI.Examples;

// This example uses experimental APIs which are subject to change. To use experimental APIs,
// please acknowledge their experimental status by suppressing the corresponding warning.
#pragma warning disable OPENAI001

public partial class ResponseExamples
{
    [Test]
    public async Task Example11_ShellAsync()
    {
        ResponsesClient client = new(apiKey: Environment.GetEnvironmentVariable("OPENAI_API_KEY"));

        List<ResponseItem> inputItems =
        [
            ResponseItem.CreateUserMessageItem(
                "Execute: ls -lah /mnt/data && python --version && node --version"),
        ];

        ShellTool shellTool = ResponseTool.CreateShellTool(new ShellToolContainerAutoEnvironment());

        CreateResponseOptions options = new("gpt-5.5", inputItems)
        {
            Tools = { shellTool },
        };

        ResponseResult response = await client.CreateResponseAsync(options);

        foreach (ResponseItem item in response.OutputItems)
        {
            if (item is ShellCallItem shellCall)
            {
                Console.WriteLine($"[Shell call]({shellCall.Status}) {string.Join(" ", shellCall.Action.CommandParts)}");
            }
            else if (item is ShellCallOutputItem shellOutput)
            {
                foreach (ShellCallOutputContent chunk in shellOutput.Output)
                {
                    if (!string.IsNullOrEmpty(chunk.Stdout))
                    {
                        Console.WriteLine($"[stdout] {chunk.Stdout}");
                    }
                    if (!string.IsNullOrEmpty(chunk.Stderr))
                    {
                        Console.WriteLine($"[stderr] {chunk.Stderr}");
                    }
                    if (chunk.Outcome is ShellCallExitOutcome exit)
                    {
                        Console.WriteLine($"[exit] code={exit.ExitCode}");
                    }
                    else if (chunk.Outcome is ShellCallTimeoutOutcome)
                    {
                        Console.WriteLine("[timeout]");
                    }
                }
            }
            else if (item is MessageResponseItem message)
            {
                Console.WriteLine($"[{message.Role}] {message.Content.FirstOrDefault()?.Text}");
            }
        }
    }
}

#pragma warning restore OPENAI001
