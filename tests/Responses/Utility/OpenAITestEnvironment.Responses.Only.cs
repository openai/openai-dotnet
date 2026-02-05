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
        // RepositoryRoot resolves to the repository root directory
        // The testproxy folder is under tests/Utility/
        DevCertPath = Path.Combine(
            RepositoryRoot,
            "tests",
            "Utility",
            "testproxy",
            "dotnet-devcert.pfx");
    }
    
    public string OpenApiKey => GetRecordedVariable("OPEN-API-KEY", options => options.IsSecret("api-key"));

    public override Dictionary<string, string> ParseEnvironmentFile() => new()
        {
            { "OPEN-API-KEY", Environment.GetEnvironmentVariable("OPENAI_API_KEY") ?? "api-key" }
        };

    public override Task WaitForEnvironmentAsync()
    {
        return Task.CompletedTask;
    }
}
