using NUnit.Framework;
using OpenAI.Responses;
using System;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

[assembly: LevelOfParallelism(8)]

namespace OpenAI.Tests;

internal static partial class TestHelpers
{
    public partial class TestScenario
    {
        public string ModelId { get; }

        public TestScenario(string modelId)
        {
            ModelId = modelId;
        }

        public virtual object CreateClient(string model, ApiKeyCredential credential, OpenAIClientOptions options)
        {
            throw new NotImplementedException();
        }

        // Only include the Responses scenario - other scenarios are defined in TestHelpers.Responses.cs
    }

    public static string GetModelForScenario(TestScenario testScenario) => testScenario.ModelId;

    public static ApiKeyCredential GetTestApiKeyCredential()
        => new(Environment.GetEnvironmentVariable("OPENAI_API_KEY"));

    public static T GetTestClient<T>(
        TestScenario scenario,
        string overrideModel = null,
        bool excludeDumpPolicy = false,
        OpenAIClientOptions options = default,
        ApiKeyCredential credential = default)
    {
        options ??= new();
        credential ??= GetTestApiKeyCredential();

        if (!excludeDumpPolicy)
        {
            options.AddPolicy(GetDumpPolicy(), PipelinePosition.BeforeTransport);
        }

        string model = overrideModel ?? GetModelForScenario(scenario);
        object clientObject = scenario.CreateClient(model, credential, options);
        return (T)clientObject;
    }

    public static async Task RetryWithExponentialBackoffAsync(Func<Task> action, int maxRetries = 5, int initialWaitMs = 750)
    {
        int waitDuration = initialWaitMs;
        int retryCount = 0;
        bool successful = false;

        while (retryCount < maxRetries && !successful)
        {
            try
            {
                await action();
                successful = true;
            }
            catch (ClientResultException ex) when (ex.Status == 404)
            {
                await Task.Delay(waitDuration);
                waitDuration *= 2;
                retryCount++;
                if (retryCount >= maxRetries)
                {
                    throw;
                }
            }
        }
    }

    private static PipelinePolicy GetDumpPolicy()
    {
        return new TestPipelinePolicy((message) =>
        {
            if (message.Request is not null && message.Response is null)
            {
                Console.WriteLine($"--- New request ---");
                IEnumerable<string> headerPairs = message?.Request?.Headers?.Select(header => $"{header.Key}={(header.Key.ToLower().Contains("auth") ? "***" : header.Value)}");
                string headers = string.Join(',', headerPairs);
                Console.WriteLine($"Headers: {headers}");
                Console.WriteLine($"{message?.Request?.Method} URI: {message?.Request?.Uri}");
                if (message.Request?.Content != null)
                {
                    string contentType = "Unknown Content Type";
                    if (message.Request.Headers?.TryGetValue("Content-Type", out contentType) == true
                        && contentType == "application/json")
                    {
                        using MemoryStream stream = new();
                        message.Request.Content.WriteTo(stream, default);
                        stream.Position = 0;
                        using StreamReader reader = new(stream);
                        string requestDump = reader.ReadToEnd();
                        stream.Position = 0;
                        requestDump = Regex.Replace(requestDump, @"""data"":[\\w\\r\\n]*""[^""]*""", @"""data"":""...""");
                        Console.WriteLine(requestDump);
                    }
                    else
                    {
                        string length = message.Request.Content.TryComputeLength(out long numberLength)
                            ? $"{numberLength} bytes"
                            : "unknown length";
                        Console.WriteLine($"<< Non-JSON content: {contentType} >> {length}");
                    }
                }
            }
            if (message.Response != null)
            {
                if (message.BufferResponse)
                {
                    Console.WriteLine("--- Begin response content ---");
                    Console.WriteLine(message.Response.Content?.ToString());
                    Console.WriteLine("--- End of response content ---");
                }
                else
                {
                    Console.WriteLine("--- Response (unbuffered, content not rendered) ---");
                }
            }
        });
    }
}
