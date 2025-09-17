using Microsoft.ClientModel.TestFramework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace OpenAI.Tests;

public class OpenAITestEnvironment : TestEnvironment
{
    static OpenAITestEnvironment()
    {
        DevCertPath = Path.Combine(
            RepositoryRoot,
            "Utility",
            "testproxy",
            "dotnet-devcert.pfx");
    }

    public string OpenApiKey => GetRecordedVariable("OPEN-API-KEY", options => options.IsSecret());

    public override Dictionary<string, string> ParseEnvironmentFile() => new()
        {
            { "OPEN-API-KEY", Environment.GetEnvironmentVariable("OPENAI_API_KEY") ?? string.Empty }
        };

    public override Task WaitForEnvironmentAsync()
    {
        return Task.CompletedTask;
    }
}